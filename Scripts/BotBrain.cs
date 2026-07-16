using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotBrain : MonoBehaviour
{
    public BotScript botScript;
    private int numFilters = 4;
    private int filterSize = 3;
    private int numChannels = 6;
    private int actionFilters = 3; //0 = move, 1 = search, 2 = use

    private float[,,,] convWeights;
    private float[] convBiases;

    private float[,] denseWeights;
    private float[] denseBiases;

    public List<MoveMemory> moveHistory = new List<MoveMemory>();
    public List<MoveMemory> actionHistory = new List<MoveMemory>();
    [SerializeField] private int maxHistorySize = 5;
    [SerializeField] private float discountFactor = 0.7f;

    private float[] itemWeights;
    private float itemBias;
    private int IWLength = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public float CalculateTileBaseReward(MoveMemory pastMove, int width, int height) {
        //pastMove.chosenIndex is 0+
        int targetX = pastMove.chosenIndex % width;
        int targetY = pastMove.chosenIndex / width;
        float pastED = pastMove.inputs[targetX, targetY, 0];
        float pastHD = pastMove.inputs[targetX, targetY, 1];
        float pastRD = pastMove.inputs[targetX, targetY, 2];
        float pastKEP = pastMove.inputs[targetX, targetY, 3];
        float pastCH = pastMove.inputs[targetX, targetY, 4];
        float pastAirdrop = pastMove.inputs[targetX, targetY, 5];
        List<float> currentMode = pastMove.playerMode;
        float baseReward = 0f;
        float huntWeight = currentMode[1];
        float hideWeight = currentMode[2];
        float searchWeight = currentMode[0];
        if (hideWeight > searchWeight) {
            baseReward = pastHD;
        } else {
            baseReward = pastRD;
        }
        if (pastCH > 0.75f) {
            if (huntWeight > hideWeight) {
                baseReward += 1.0f;
            } else if (huntWeight < hideWeight) {
                baseReward -= 2.0f;
            }
        }
        if (pastAirdrop > 0.5f) {
            if (searchWeight > huntWeight && searchWeight > hideWeight) {
                baseReward += 3.0f;
            } else {
                baseReward += 1.0f;
            }
        }
        return baseReward;
    }
    public void ReceiveDamage(float percent, int width, int height, List<float> ED, List<float> HD, List<float> RD) {
        for (int i = moveHistory.Count - 1; i >= 0; i--) {
            int stepsAgo = (moveHistory.Count - 1) - i;

            float fadedPunishment = percent * Mathf.Pow(discountFactor, stepsAgo);

            MoveMemory pastMove = moveHistory[i];

            float baseReward = CalculateTileBaseReward(pastMove, width, height);
            float finalTarget = Mathf.Clamp(baseReward - (fadedPunishment * 2.0f), -1.0f, 1.0f);

            Backpropagate(pastMove.inputs, pastMove.chosenIndex, width, height, pastMove.playerMode, new List<int>{0}, new List<float>{finalTarget});
        }

        //moveHistory.Clear();
    }
    public void InitializeNetwork() {
        convWeights = new float[numFilters, filterSize, filterSize, numChannels];
        convBiases = new float[numFilters];
        denseWeights = new float[numFilters, actionFilters];
        denseBiases = new float[actionFilters];

        for (int a = 0; a < actionFilters; a++) {
            denseBiases[a] = UnityEngine.Random.Range(-0.1f, 0.1f);
        }
        for (int f = 0; f < numFilters; f++) {
            convBiases[f] = UnityEngine.Random.Range(-0.1f, 0.1f);
            for (int a = 0; a < actionFilters; a++) {
                denseWeights[f, a] = UnityEngine.Random.Range(-0.1f, 0.1f);
            }
            //denseWeights[f] = UnityEngine.Random.Range(-0.1f, 0.1f);
            for (int x = 0; x < filterSize; x++) {
                for (int y = 0; y < filterSize; y++) {
                    for (int c = 0; c < numChannels; c++) {
                        convWeights[f, x, y, c] = UnityEngine.Random.Range(-0.2f, 0.2f);
                    }
                }
            }
        }
        itemWeights = new float[IWLength];
        float rangeLimit = 0.6f;
        for (int i = 0; i < itemWeights.Length; i++) {
            itemWeights[i] = UnityEngine.Random.Range(-rangeLimit, rangeLimit);
        }
        itemBias = 0.1f;
        Debug.Log("Bot Brain initialized with multi-action random weights!");
    }
    public float[,,] GenerateCNNInput(int mapWidth, int mapHeight, List<float> ED, List<float> HD, List<float> RD, List<int> knownEnemyPositions, List<float> combinedHeatmap, List<int> airdrops) {
        float[,,] inputTensor = new float[mapWidth, mapHeight, numChannels]; 
        int totalTiles = mapWidth * mapHeight;
        for (int i = 0; i < totalTiles; i++) {
            int x = i % mapWidth;
            int y = i / mapWidth;
            inputTensor[x, y, 0] = ED[i];
            inputTensor[x, y, 1] = HD[i];
            inputTensor[x, y, 2] = RD[i];
            inputTensor[x, y, 3] = knownEnemyPositions.Contains(i + 1) ? 1.0f : 0.0f;
            inputTensor[x, y, 4] = combinedHeatmap[i];
            inputTensor[x, y, 5] = airdrops.Contains(i + 1) ? 1.0f : 0.0f;
            //Debug.Log("inputTensor 0: " + inputTensor[x, y, 0]);
        }
        return inputTensor;
    }
    public float[] EvaluateTilePotential(float[,,] mapInput, int targetX, int targetY, int width, int height, List<float> playerMode) {
        //Debug.Log("playerMode count should be 3 and it is " + playerMode.Count);
        int halfWindow = filterSize / 2;
        int cleanStartX = Mathf.Clamp(targetX - halfWindow, 0, width - filterSize);
        int cleanStartY = Mathf.Clamp(targetY - halfWindow, 0, height - filterSize);

        float[] filterOutputs = new float[numFilters];
        for (int f = 0; f < numFilters; f++) {
            float sum = 0f;
            for (int fx = 0; fx < filterSize; fx++) {
                for (int fy = 0; fy < filterSize; fy++) {
                    /*int sampleX = targetX + (fx - 1);
                    int sampleY = targetY + (fy - 1);
                    if (sampleX < 0 || sampleX >= width || sampleY < 0 || sampleY >= height) {
                        continue;
                    }*/
                    int sampleX = cleanStartX + fx;
                    int sampleY = cleanStartY + fy;
                    
                    for (int c = 0; c < numChannels; c++) {
                        sum += mapInput[sampleX, sampleY, c] * convWeights[f, fx, fy, c];
                    }
                    //sum += mapInput[sampleX, sampleY, 0] * convWeights[f, fx, fy, 0];
                    //sum += mapInput[sampleX, sampleY, 1] * convWeights[f, fx, fy, 1];
                    //sum += mapInput[sampleX, sampleY, 2] * convWeights[f, fx, fy, 2];
                }
            }
            sum += convBiases[f];

            //filterOutputs[f] = Mathf.Max(0f, sum);
            filterOutputs[f] = (sum > 0f) ? sum : sum * 0.01f; //\K
        }

        //float finalScore = 0f;
        float[] actionScores = new float[actionFilters];
        for (int f = 0; f < numFilters; f++) {
            float modeWeight = GetModeWeight(f, playerMode);
            float activatedOutput = filterOutputs[f] * modeWeight;
            actionScores[0] += activatedOutput * denseWeights[f, 0];
            actionScores[1] += activatedOutput * denseWeights[f, 1];
            actionScores[2] += activatedOutput * denseWeights[f, 2];

            //finalScore += (filterOutputs[f] * denseWeights[f]) * modeWeight;
        }
        //finalScore += denseBias;
        actionScores[0] += denseBiases[0];
        actionScores[1] += denseBiases[1];
        actionScores[2] += denseBiases[2];

        return actionScores;
    }
    private float GetModeWeight(int f, List<float> playerMode) {
        if (f == 0 || f == 1) {
            return playerMode[1];
        } else if (f == 2) {
            return playerMode[2];
        } else if (f == 3) {
            return playerMode[0];
        }
        return 0f;
    }
    public int PickBestMoveCNN(bool trainingModeActivated, List<bool> a, float[,,] mapInput, int width, int height, int totalTiles, List<float> playerMode) {
        int bestTileIndex = -1;
        float highestScore = 0f;
        bool firstSet = true;
        List<float> testList = new List<float>();
        for (int i = 0; i < totalTiles; i++) {
            
            int x = i % width;
            int y = i / width;

            float tileScore = EvaluateTilePotential(mapInput, x, y, width, height, playerMode)[0];
            testList.Add(tileScore);
            //Debug.Log("tileScore: " + tileScore);
            //if (a[i]) {
                if (firstSet) {
                    highestScore = tileScore;
                    bestTileIndex = i;
                    firstSet = false;
                } else {
                    if (tileScore > highestScore) {
                        highestScore = tileScore;
                        bestTileIndex = i;
                    }
                }
            //}
        }
        if (bestTileIndex != -1) {
            if (!a[bestTileIndex]) {
                bestTileIndex = botScript.FindLowestDistance(1, a, botScript.lScript.GetCoordsFromIndex(bestTileIndex + 1), null);
            }
        }
        if (bestTileIndex != -1 && trainingModeActivated) {
            float[,,] snapshotInput = (float[,,])mapInput.Clone();
            List<float> snapshotMode = new List<float>(playerMode);
            MoveMemory currentMove = new MoveMemory(snapshotInput, bestTileIndex, new List<int>{0}, snapshotMode, 0, null, null);
            moveHistory.Add(currentMove);
            if (moveHistory.Count > maxHistorySize) {
                moveHistory.RemoveAt(0);
            }
        }
        //botScript.lScript.DisplayBotTest(4, null, null, null, testList);
        return bestTileIndex;
    }
    public int PickBestActionNN(List<int> usableItems, List<int> grid) {
        int itemToUse = 0;
        float threshold = 0.0f;
        int bestItem = 0;
        float highestItemScore = 0;
        bool firstSet = true;
        float[] bestFeatures = new float[IWLength];
        int maxHealingRating = botScript.FindBestHealing(2, botScript.inventory, botScript.HP, botScript.maxHP)[0];
        int minHealingRating = botScript.FindBestHealing(3, botScript.inventory, botScript.HP, botScript.maxHP)[0];
        for (int i = 0; i < usableItems.Count; i++) {
            bool skip = false;
            int item = usableItems[i];
            int index = item - 1;
            int bomb = botScript.lScript.GetIsBomb(index);
            
            int attributeAmount = 0;
            float score = 0f;
            LocalScript LS = botScript.lScript;
            bool weapon = LS.itemIntLists[index][0].Contains(1); //AKA you need a target vector
            bool healing = LS.itemIntLists[index][0].Contains(2);
            float[] localFeatures = new float[bestFeatures.Length];
            float hpFeature = (float)botScript.HP / botScript.maxHP; //over maxHP for now
            score += hpFeature * itemWeights[0]; 
            attributeAmount++;
            localFeatures[0] = hpFeature;
            int target = botScript.FindRandomTarget(1, true);
            if (weapon) {
                if (target == -1) {
                    Debug.Log("Got an error that shouldn't happen!");
                    //skip = true;
                } else if (target == 0) {
                    skip = true;
                } else {
                    
                    int myPos = botScript.position;
                    int enPos = botScript.lastKnownPlayerPositions[target - 1];
                    Vector2 myPosV = new Vector2(LS.GRIDX[myPos - 1], LS.GRIDY[myPos - 1]);
                    Vector2 targetV = new Vector2(LS.GRIDX[enPos - 1], LS.GRIDY[enPos - 1]); //\I
                    Vector2 diff = targetV - myPosV;
                    float angleRadians = Mathf.Atan2(diff.y, diff.x);
                    float angleDeg = angleRadians * Mathf.Rad2Deg;
                    int trap = 0;
                    if (LS.itemIntLists[index][1].Contains(13)) trap = 1;
                    List<List<float>> CBS = LS.CalculateBulletStop(myPosV, Mathf.RoundToInt(LS.itemInfos[index][0][13]), angleDeg, index, botScript.playerNum, botScript.lastKnownPlayerPositions, LS.itemInfos[index][0][0], trap, myPos, LS.itemInfos[index][0][5], diff.magnitude, LS.FindElevation(grid[myPos - 1], 1) + LS.itemInfos[index][0][12], LS.GetHilltopCoe(), bomb, LS.itemInfos[index][0][1], LS.itemInfos[index][0][9]);
                    float expectedDamage = botScript.GetExpectedDamage(index);
                    float damage = 0;
                    if (bomb == 1) {//if bomb == 2 it's treated as a bullet
                        List<float> bombDamages = CBS[4];
                        foreach (float d in bombDamages) {
                            damage += d;
                        }
                    } else {
                        damage += CBS[0][2];
                    }
                    damage *= Mathf.RoundToInt(LS.itemInfos[index][0][3]);
                    float damageFeature = damage / expectedDamage;
                    score += damageFeature * itemWeights[1];
                    attributeAmount++;
                    localFeatures[1] = damageFeature;
                }
            }
            if (healing) {
                List<(int item, int UAAT, int rating)> potentials = new List<(int item, int UAAT, int rating)>();
                botScript.GetUAATRating(botScript.inventory, item, potentials, botScript.HP, botScript.maxHP);
                int rating = botScript.GetMaxRating(potentials); //when using it should find the same UAAT again
                //int maxPossibleRating = botScript.maxHealing + LS.FindHighestBookEffectAmt(botScript.botBookInventory, 2);
                //rating <= maxHealingRating

                float healingFeature = 0; 

                if (rating > 0) {
                    //healingFeature = (float)(rating - minHealingRating) / (maxHealingRating - minHealingRating);
                    if (maxHealingRating > 0) {
                        healingFeature = (float)rating / maxHealingRating;
                    } else {
                        healingFeature = 0f;
                    }
                } else if (rating < 0) {
                    //healingFeature = (float)(rating - maxHealingRating) / (maxHealingRating - minHealingRating);
                    if (minHealingRating < 0) {
                        healingFeature = -(float)rating / minHealingRating;
                    } else {
                        healingFeature = 0f;
                    }
                } else if (rating == 0) {
                    healingFeature = 0f;
                }
                
                score += healingFeature * itemWeights[2];
                attributeAmount++;
                localFeatures[2] = healingFeature;
            }
            //implement more attributes
            if (!skip) {
                score /= attributeAmount;
                score += itemBias;
                if (firstSet) {
                    highestItemScore = score;
                    bestItem = item;
                    bestFeatures = localFeatures;
                    firstSet = false;
                } else {
                    if (score > highestItemScore) {
                        highestItemScore = score;
                        bestItem = item;
                        bestFeatures = localFeatures;
                    }
                }
            }
        }
        if (firstSet) {
            return 0;
        } else if (highestItemScore >= threshold) {
            botScript.features = bestFeatures;
            return bestItem;
        } else {
            return 0;
        }
        
    }
    public void BotShootFeedback(float baselineReward, int width, int height) {
        if (actionHistory.Count == 0) return;
        float currentReward = baselineReward;
        for (int i = actionHistory.Count - 1; i >= 0; i--) {
            MoveMemory pastMemory = actionHistory[i];
            if (pastMemory.chosenItem != 0) {
                BackpropagateItemNetwork(pastMemory, currentReward);
            }
            float[,,] mapInput = pastMemory.inputs;
            int chosenIndex = pastMemory.chosenIndex;

            List<int> actionsToUpdate = pastMemory.actionIndices;
            List<float> targetScores = new List<float>();
            foreach (int act in actionsToUpdate) {
                targetScores.Add(currentReward);
            }
            Backpropagate(mapInput, chosenIndex, width, height, pastMemory.playerMode, actionsToUpdate, targetScores);
            currentReward *= discountFactor;
        }
    }
    public void AddToActionHistory(float[,,] cnnInput, int chosenIndex, List<int> actionIndices, List<float> playerMode, int item, List<int> inv, float[] PF) {
        MoveMemory MM = new MoveMemory(cnnInput, chosenIndex, actionIndices, playerMode, item, inv, PF);
        actionHistory.Add(MM);
        if (actionHistory.Count > maxHistorySize) {
            actionHistory.RemoveAt(0);
        }
    }
    public void Backpropagate(float[,,] mapInput, int chosenIndex, int width, int height, List<float> playerMode, List<int> actionsToUpdate, List<float> targetScores) {
        int chosenX = chosenIndex % width;
        int chosenY = chosenIndex / width;
        float learningRate = 0.01f;

        int halfWindow = filterSize / 2;
        int cleanStartX = Mathf.Clamp(chosenX - halfWindow, 0, width - filterSize);
        int cleanStartY = Mathf.Clamp(chosenY - halfWindow, 0, height - filterSize);

        float[] filterOutputs = new float[numFilters];
        float[] rawSums = new float[numFilters];
        for (int f = 0; f < numFilters; f++) {
            float sum = 0f;
            for (int fx = 0; fx < filterSize; fx++) {
                for (int fy = 0; fy < filterSize; fy++) {
                    /*int sampleX = chosenX + (fx - 1);
                    int sampleY = chosenY + (fy - 1);
                    if (sampleX < 0 || sampleX >= width || sampleY < 0 || sampleY >= height) continue;
                    */
                    int sampleX = cleanStartX + fx;
                    int sampleY = cleanStartY + fy;
                    for (int c = 0; c < numChannels; c++) {
                        sum += mapInput[sampleX, sampleY, c] * convWeights[f, fx, fy, c];
                    }
                    //sum += mapInput[sampleX, sampleY, 0] * convWeights[f, fx, fy, 0];
                    //sum += mapInput[sampleX, sampleY, 1] * convWeights[f, fx, fy, 1];
                    //sum += mapInput[sampleX, sampleY, 2] * convWeights[f, fx, fy, 2];
                }
            }
            sum += convBiases[f];
            rawSums[f] = sum;
            //filterOutputs[f] = Mathf.Max(0f, sum);
            filterOutputs[f] = (sum > 0f) ? sum : sum * 0.01f; //\K
        }

        /*float currentScore = 0f;
        for (int f = 0; f < numFilters; f++) {
            float modeWeight = GetModeWeight(f, playerMode);
            currentScore += (filterOutputs[f] * denseWeights[f]) * modeWeight;
        }
        currentScore += denseBias;

        float errorGradient = currentScore - targetScore;

        for (int f = 0; f < numFilters; f++) {
            float modeWeight = GetModeWeight(f, playerMode);

            float dDenseWeight = errorGradient * filterOutputs[f] * modeWeight;
            denseWeights[f] -= learningRate * dDenseWeight;
        }
        denseBias -= learningRate * errorGradient;

        for (int f = 0; f < numFilters; f++) {
            //if (rawSums[f] <= 0f) continue;
            float modeWeight = GetModeWeight(f, playerMode);
            float filterGradient = errorGradient * denseWeights[f] * modeWeight;
            if (rawSums[f] <= 0f) {
                filterGradient *= 0.01f;
            }
            convBiases[f] -= learningRate * filterGradient;

            for (int fx = 0; fx < filterSize; fx++) {
                for (int fy = 0; fy < filterSize; fy++) {
                    int sampleX = cleanStartX + fx;
                    int sampleY = cleanStartY + fy;

                    for (int c = 0; c < numChannels; c++) {
                        convWeights[f, fx, fy, c] -= learningRate * filterGradient * mapInput[sampleX, sampleY, c];
                    }
                    //convWeights[f, fx, fy, 0] -= learningRate * filterGradient * mapInput[sampleX, sampleY, 0]; //elevation
                    //convWeights[f, fx, fy, 1] -= learningRate * filterGradient * mapInput[sampleX, sampleY, 1]; //hide
                    //convWeights[f, fx, fy, 2] -= learningRate * filterGradient * mapInput[sampleX, sampleY, 2]; //rarity
                }
            }
        }*/
        float[] currentScores = new float[actionFilters];
        for (int f = 0; f < numFilters; f++) {
            float modeWeight = GetModeWeight(f, playerMode);
            float activatedOutput = filterOutputs[f] * modeWeight;
            for (int a = 0; a < actionFilters; a++) {
                currentScores[a] += activatedOutput * denseWeights[f, a];
            }
        }
        for (int a = 0; a < actionFilters; a++) {
            currentScores[a] += denseBiases[a];
        }
        float[] errorGradients = new float[actionFilters];
        for (int a = 0; a < actionFilters; a++) {
            if (actionsToUpdate.Contains(a)) {
                int listPosition = actionsToUpdate.IndexOf(a);
                errorGradients[a] = currentScores[a] - targetScores[listPosition];
            } else {
                errorGradients[a] = 0f;
            }
        }
        for (int f = 0; f < numFilters; f++) {
            float modeWeight = GetModeWeight(f, playerMode);
            for (int a = 0; a < actionFilters; a++) {
                float dDenseWeight = errorGradients[a] * filterOutputs[f] * modeWeight;
                denseWeights[f, a] -= learningRate * dDenseWeight;
            }
        }
        for (int a = 0; a < actionFilters; a++) {
            denseBiases[a] -= learningRate * errorGradients[a];
        }
        for (int f = 0; f < numFilters; f++) {
            float modeWeight = GetModeWeight(f, playerMode);
            float totalFilterGradient = 0f;
            for (int a = 0; a < actionFilters; a++) {
                totalFilterGradient += errorGradients[a] * denseWeights[f, a];
            }
            totalFilterGradient *= modeWeight;
            if (rawSums[f] <= 0f) {
                totalFilterGradient *= 0.01f;
            }
            convBiases[f] -= learningRate * totalFilterGradient;
            for (int fx = 0; fx < filterSize; fx++) {
                for (int fy = 0; fy < filterSize; fy++) {
                    int sampleX = cleanStartX + fx;
                    int sampleY = cleanStartY + fy;
                    for (int c = 0; c < numChannels; c++) {
                        convWeights[f, fx, fy, c] -= learningRate * totalFilterGradient * mapInput[sampleX, sampleY, c];
                    }
                }
            }
        }
        SaveBrainData();
        //botScript.lScript.DisplayBotTest(4, null, null, null, GetFlattenedFilterWeights(0, 1, filterSize, convWeights));
    }
    public void BackpropagateItemNetwork(MoveMemory pastMemory, float rewardValue) {
        float learningRate = 0.01f;
        if (pastMemory.chosenItem == 0) return;
        int itemIndex = pastMemory.chosenItem - 1;
        float hpFeature = pastMemory.playerFeatures[0];
        LocalScript LS = botScript.lScript;
        bool weapon = LS.itemIntLists[itemIndex][0].Contains(1);
        bool healing = LS.itemIntLists[itemIndex][0].Contains(2);
        float errorDelta = rewardValue;
        int attributeAmount = 1;
        itemWeights[0] += learningRate * errorDelta * hpFeature;
        if (weapon) {
            float damageFeature = pastMemory.playerFeatures[1];
            itemWeights[1] += learningRate * errorDelta * damageFeature;
            attributeAmount++;
        }
        if (healing) {
            float healingFeature = pastMemory.playerFeatures[2];
            itemWeights[2] += learningRate * errorDelta * healingFeature;
            attributeAmount++;
        }
        itemBias += learningRate * errorDelta;
    }
    public List<float> GetFlattenedFilterWeights(int filterIndex, int channelIndex, int fSize, float[,,,] CW) {
        List<float> flattenedWeights = new List<float>();
        for (int fx = 0; fx < fSize; fx++) {
            for (int fy = 0; fy < fSize; fy++) {
                float weight = CW[filterIndex, fx, fy, channelIndex];
                flattenedWeights.Add(weight);
            }
        }
        return flattenedWeights;
    }
    private string GetSavePath(bool trainingModeActivated) {
        if (trainingModeActivated) {
            return System.IO.Path.Combine(Application.persistentDataPath, "bot_brain.json");
            Debug.Log("Found local brain");
        } else {
            return System.IO.Path.Combine(Application.streamingAssetsPath, "bot_brain.json");
            Debug.Log("Found built brain");
        }
        
    }
    public void SaveBrainData() {
        BrainSaveData data = new BrainSaveData();
        data.denseBiases = denseBiases;
        data.convBiases = convBiases;
        data.itemWeights = this.itemWeights;
        data.itemBias = this.itemBias;

        data.denseWeightsMatrix = new List<FilterDenseWeights>();
        for (int f = 0; f < numFilters; f++) {
            FilterDenseWeights filterRow = new FilterDenseWeights();
            filterRow.actionWeights = new float[actionFilters];
            for (int a = 0; a < actionFilters; a++) {
                filterRow.actionWeights[a] = denseWeights[f, a];
            }
            data.denseWeightsMatrix.Add(filterRow);
        }
        
        data.convWeightsList = new List<float>();
        for (int f = 0; f < numFilters; f++) {
            for (int x = 0; x < filterSize; x++) {
                for (int y = 0; y < filterSize; y++) {
                    for (int c = 0; c < numChannels; c++) {
                        data.convWeightsList.Add(convWeights[f, x, y, c]);
                    }
                }
            }
        }
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(GetSavePath(true), json);
        /*BrainSaveData saveObject = new BrainSaveData();
        saveObject.denseBias = denseBias;
        for (int f = 0; f < numFilters; f++) {
            saveObject.convBiases.Add(convBiases[f]);
            saveObject.denseWeights.Add(denseWeights[f]);
        }
        for (int f = 0; f < numFilters; f++) {
            for (int x = 0; x < filterSize; x++) {
                for (int y = 0; y < filterSize; y++) {
                    for (int c = 0; c < numChannels; c++) {
                        saveObject.flattenedConvWeights.Add(convWeights[f, x, y, c]);
                    }
                }
            }
        }
        string json = JsonUtility.ToJson(saveObject, true);
        System.IO.File.WriteAllText(GetSavePath(), json);
        Debug.Log($"Brain saved successfully to: {GetSavePath()}");*/
    }
    public bool LoadBrainData(bool trainingModeActivated) {
        string path = GetSavePath(trainingModeActivated);
        if (!System.IO.File.Exists(path)) {
            Debug.Log($"No brain file found at {path}. Defaulting to random initialization.");
            return false;
        }
        string json = System.IO.File.ReadAllText(path);
        BrainSaveData data = JsonUtility.FromJson<BrainSaveData>(json);
        denseBiases = data.denseBiases;
        denseWeights = new float[numFilters, actionFilters];
        for (int f = 0; f < numFilters; f++) {
            for (int a = 0; a < actionFilters; a++) {
                denseWeights[f, a] = data.denseWeightsMatrix[f].actionWeights[a];
            }
        }
        convBiases = data.convBiases;
        /*BrainSaveData loadObject = JsonUtility.FromJson<BrainSaveData>(json);

        denseBias = loadObject.denseBias;

        convBiases = new float[numFilters];
        denseWeights = new float[numFilters, actionFilters];*/

        /*for (int f = 0; f < numFilters; f++) {
            convBiases[f] = loadObject.convBiases[f];
            denseWeights[f] = loadObject.denseWeights[f];
        }*/
        int weightsPerFilter = filterSize * filterSize;
        int savedChannels = data.convWeightsList.Count / (numFilters * weightsPerFilter);
        convWeights = new float[numFilters, filterSize, filterSize, numChannels];
        int listIndex = 0;
        for (int f = 0; f < numFilters; f++) {
            for (int x = 0; x < filterSize; x++) {
                for (int y = 0; y < filterSize; y++) {
                    for (int c = 0; c < numChannels; c++) {
                        if (c < savedChannels) {
                            convWeights[f, x, y, c] = data.convWeightsList[listIndex];
                            listIndex++;
                        } else {
                            convWeights[f, x, y, c] = UnityEngine.Random.Range(-0.5f, 0.5f);
                        }
                    }
                }
            }
        }
        this.itemWeights = data.itemWeights;
        this.itemBias = data.itemBias;
        /*int centerIndex = filterSize / 2;
        for (int fx = 0; fx < filterSize; fx++) {
            for (int fy = 0; fy < filterSize; fy++) {
                for (int c = 0; c < numChannels; c++) {
                    convWeights[0, fx, fy, c] = 0f;
                }
            }
        }
        convWeights[0, centerIndex, centerIndex, 2] = 5.0f;
        denseWeights[0] = 1.0f;
        denseBias = 0f;*/
        //Debug.Log("Frozen master brain data loaded successfully! Training lock active.");
        return true;
    }
}

[System.Serializable]
public class BrainSaveData {
    //public List<float> flattenedConvWeights = new List<float>();
    
    //public List<float> denseWeights = new List<float>();
    public float[] denseBiases;
    public List<FilterDenseWeights> denseWeightsMatrix;
    public float[] convBiases;
    public List<float> convWeightsList;

    public float[] itemWeights;
    public float itemBias;
}

[System.Serializable]
public class FilterDenseWeights {
    public float[] actionWeights;
}
public struct MoveMemory {
    public float[,,] inputs;
    public int chosenIndex;
    public List<int> actionIndices;
    public List<float> playerMode;
    public float[] playerFeatures;
    
    public int chosenItem; //1+
    public List<int> snapshotInventory; //1+
    public MoveMemory(float[,,] inputs, int chosenIndex, List<int> actionIndices, List<float> playerMode, int item, List<int> inv, float[] PF) {
        this.inputs = (float[,,])inputs.Clone();
        this.chosenIndex = chosenIndex;
        this.actionIndices = new List<int>(actionIndices);
        this.playerMode = new List<float>(playerMode);
        this.chosenItem = item;
        if (inv == null) {
            this.snapshotInventory = null;
        } else {
            this.snapshotInventory = new List<int>(inv);
        }
        if (PF == null) {
            this.playerFeatures = null;
        } else {
            this.playerFeatures = (float[])PF.Clone();
        }
        
    }
}
