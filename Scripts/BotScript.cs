using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private bool trainingModeActivated = false;
    public int playerNum; //playerNum is 1+
    public int position; //position is 1+
    [SerializeField]
    public int currentQuadrant; 
    public int targetQuadrant;
    /* 1 = topRight
       2 = topLeft
       3 = bottomLeft
       4 = bottomRight
    */
    [SerializeField]
    int timeSpentInTQ = 0;
    public int maxHP;
    public int HP;
    public float HPCalculation;
    public float[] myActions;
    public float[] myLocalDamages;
    public List<float> myBDD;
    public List<int> myTI;
    public List<int> mySI;
    public LocalScript lScript;
    public BotBrain botBrain;
    public int mode;
    /* 1 = "Searching"
       2 = "Hunting"
       3 = "Hiding"
    */
    public float armorCoefficient = 1;
    public List<int> inventory = new List<int>();
    public List<int> botBookInventory = new List<int>();
    [SerializeField]
    public List<int> myVG = new List<int>();
    public List<int> knownPlayerPositions = new List<int>(); //1+
    public List<int> lastKnownPlayerPositions = new List<int>(); //1+
    public List<List<float>> potentialMovesSinceLastSeen = new List<List<float>>();
    public List<List<bool>> potentialPlayerPositions = new List<List<bool>>();
    public List<List<int>> potentialPlayerVGs = new List<List<int>>();
    public List<List<float>> compoundPlayerVGs = new List<List<float>>();
    public List<List<int>> potentialPlayerInvs = new List<List<int>>(); //1+
    public List<int> UAVPlayerPositions = new List<int>(); //1+
    public List<bool> playerHealthsVisible = new List<bool>();
    public List<int> knownPlayerHealths = new List<int>();
    public List<int> knownPlayerMaxHealths = new List<int>();
    [SerializeField]
    List<int> botMysteryBuildings = new List<int>(); //0+
    public List<int> botItemsOnCooldown = new List<int>();
    public List<bool> botCDJA = new List<bool>();
    public List<List<List<float>>> botTraps = new List<List<List<float>>>();
    public int myFrozenTurns;
    public int botMaxWeapons = 3;
    public List<int> botEffectLengths = new List<int>();
    public List<int> botEffects = new List<int>();
    public List<int> botEffectStrengths = new List<int>();
    public List<float> botSmokes = new List<float>();
    public bool[] botIsSmoked;
    public List<int> botAirdrops = new List<int>();

    public List<float> elevationData = new List<float>();
    public List<float> hideData = new List<float>();
    public List<float> rarityData = new List<float>();
    public float dataRadius = 5;
    int maxLandRarity = 7; //manually set
    float maxLandElevation = 2; //manually set
    public int maxHealing = 100; //manually set

    public List<List<float>> allPlayerModes = new List<List<float>>();
    /* [0] is searching, [1] is hunting, [2] is hiding*/
    public List<List<float>> allPlayerDataLists = new List<List<float>>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitializeBotBrain() {
        if (!botBrain.LoadBrainData(trainingModeActivated)) {
            botBrain.InitializeNetwork();
        }
    }
    void InitializeData(List<int> mode, List<int> grid) {
        /* mode: 
        1 = elevation
        2 = hide
        3 = rarity
        */
        bool doElevation = mode.Contains(1);
        bool doHide = mode.Contains(2);
        bool doRarity = mode.Contains(3);
        if (doElevation) {
            elevationData.Clear();
        } 
        if (doHide) {
            hideData.Clear();
        }
        if (doRarity) {
            rarityData.Clear();
        }
        float maxElevationCount = 0;
        float maxHideCount = 0;
        float maxRarityCount = 0;
        for (int i = 0; i < grid.Count; i++) {
            if (Mathf.Approximately(lScript.FindElevation(grid[i], 1), maxLandElevation)) {
                maxElevationCount++;
            } else if (Mathf.Approximately(lScript.FindElevation(grid[i], 1), maxLandElevation - 1)) {
                maxElevationCount += 0.9f; //trainable
            }
            if (grid[i] == 9) { //manually set
                maxHideCount++;
            } else if (grid[i] == 5) { //manually set
                maxHideCount += 0.9f; //trainable
            }
            if (lScript.GetLandRarity(grid[i]) == maxLandRarity) { //because the bot is allowed to know how many military bases there are as are we, it's just a count and doesn't tell the bot where the bases are
                maxRarityCount++;
            } else if (BotGetLandRarity(i, grid) == maxLandRarity - 1) {
                maxRarityCount += 0.9f; //trainable
            } else if (BotGetLandRarity(i, grid) == maxLandRarity - 2) {
                maxRarityCount += 0.5f; //trainable
            }
        }
        int gridCount = grid.Count;
        float elevDensityCeiling = Mathf.Max(0.1f, maxElevationCount / gridCount);
        float hideDensityCeiling = Mathf.Max(0.1f, maxHideCount / gridCount);
        float rarityDensityCeiling = Mathf.Max(0.1f, maxRarityCount / gridCount);

        Debug.Log("elevDC: " + elevDensityCeiling);
        Debug.Log("hideDC: " + hideDensityCeiling);
        Debug.Log("rarityDC: " + rarityDensityCeiling);
        float[] totalWeights = new float[3]; //manually set
        float[] totalMaxWeights = new float[3]; //manually set
        for (int i = 0; i < grid.Count; i++) {
            System.Array.Clear(totalWeights, 0, 3); //manually set
            System.Array.Clear(totalMaxWeights, 0, 3); //manually set
            Vector2 iCoords = lScript.GetCoordsFromIndex(i + 1);
            int areaCount = 0;
            for (int j = 0; j < grid.Count; j++) {
                Vector2 jCoords = lScript.GetCoordsFromIndex(j + 1);
                float dist = lScript.FindDistance(iCoords, jCoords);
                if (dist <= dataRadius) {
                    float distWeight = 1f - (dist / dataRadius);
                    if (distWeight < 0) Debug.Log("ERROR: distWeight is negative! " + distWeight);
                    if (doElevation) {
                        float elevation = lScript.FindElevation(grid[j], 1);
                        float adjustedElevation = elevation;
                        if (Mathf.Approximately(elevation, -1)) { //manually set
                            adjustedElevation = 0; //trainable
                        } else if (Mathf.Approximately(elevation, 0)) { //manually set
                            adjustedElevation = 0.1f; //trainable
                        }
                        float weight = (adjustedElevation) / (maxLandElevation); // + 1 for water, manually set
                        //float maxWeight = 1f; //manually set
                        totalWeights[0] += distWeight * weight;
                        //totalMaxWeights[0] += distWeight; // * maxWeight; //omitted until maxWeight is no longer 1f
                        totalMaxWeights[0] += distWeight * elevDensityCeiling;
                    }
                    if (doHide) {
                        int terrain = grid[j];
                        float weight = 0;
                        if (terrain == 5) {
                            weight = 0.5f; //correlated to double tree
                        } else if (terrain == 9) {
                            weight = 1f; 
                        }
                        //incorporate smokes later
                        totalWeights[1] += distWeight * weight;
                        //totalMaxWeights[1] += distWeight; // * maxWeight; //maxWeight is 1f
                        totalMaxWeights[1] += distWeight * hideDensityCeiling;
                    }
                    if (doRarity) {
                        int rarity = BotGetLandRarity(j, grid);
                        float weight = Mathf.Pow((float)rarity / maxLandRarity, 2);
                        //float maxWeight = 1f; //manually set
                        totalWeights[2] += Mathf.Pow(distWeight, 2) * weight;
                        //totalMaxWeights[2] += distWeight; // * maxWeight; //omitted until maxWeight is no longer 1f
                        totalMaxWeights[2] += Mathf.Pow(distWeight, 2) * rarityDensityCeiling;
                    }
                    areaCount++;
                }
            }
            if (doElevation) {
                elevationData.Add(Mathf.Min(totalWeights[0] / totalMaxWeights[0], 1f));
            }
            if (doHide) {
                hideData.Add(Mathf.Min(totalWeights[1] / totalMaxWeights[1], 1f));
            }
            if (doRarity) {
                rarityData.Add(Mathf.Min(totalWeights[2] / totalMaxWeights[2], 1f));
            }
        }
        //lScript.DisplayBotTest(3, null, null, new List<List<float>> {elevationData, hideData, rarityData}, null);
    }
    
    public void InitializeDataLists(NetworkList<int> GRID) {
        List<int> grid = new List<int>();
        foreach (int g in GRID) {
            grid.Add(g);
        }
        InitializeData(new List<int>{1, 2, 3}, grid);
    }
    public void BotUseRadar(int i, int pos) {
        knownPlayerPositions[i] = pos;
        lastKnownPlayerPositions[i] = pos;
        potentialMovesSinceLastSeen[i].Clear();
        CreatePPP(i + 1);
        UpdatePlayerVG(i + 1);
    }
    public void EnemyOpenedAirdrop(int pnum, int pos) {
        knownPlayerPositions[pnum - 1] = pos;
        lastKnownPlayerPositions[pnum - 1] = pos;
        potentialMovesSinceLastSeen[pnum - 1].Clear();
        CreatePPP(pnum);
        UpdatePlayerVG(pnum);
        AdjustPlayerMode(pnum, 1, 0.2f);
    }
    public IEnumerator UpdateKnownPlayerHealths() {
        for (int i = 0; i < playerHealthsVisible.Count; i++) {
            if (i + 1 == playerNum) {
                knownPlayerHealths[i] = HP; //lowkey optional, don't use health from this list for yourself, just use the variables
                knownPlayerMaxHealths[i] = maxHP;
            } else {
                if (playerHealthsVisible[i]) {
                    knownPlayerHealths[i] = lScript.healths[i];
                    knownPlayerMaxHealths[i] = lScript.maxHealths[i];
                } else {
                    knownPlayerHealths[i] = 0;
                    knownPlayerMaxHealths[i] = 0;
                }
            }
        }
        yield return null;
    }
    public IEnumerator UpdateTargetAndQuadrant() {
        int target = FindClosestTarget(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]));
        Vector2 myPosV2 = lScript.GetCoordsFromIndex(knownPlayerPositions[target - 1]);
        int myX = Mathf.RoundToInt(myPosV2.x);
        int myY = Mathf.RoundToInt(myPosV2.y);
        timeSpentInTQ = 0;
        targetQuadrant = GetCurrentQuadrant(myX, myY);
        yield return null;
    }
    public void UpdateMyPositionInList() {
        knownPlayerPositions[playerNum - 1] = position;
        lastKnownPlayerPositions[playerNum - 1] = position;
        Vector2 myPosV2 = lScript.GetCoordsFromIndex(position);
        int myX = Mathf.RoundToInt(myPosV2.x); //1+
        int myY = Mathf.RoundToInt(myPosV2.y); //1+
        currentQuadrant = GetCurrentQuadrant(myX, myY);
    }
    int GetCurrentQuadrant(int myX, int myY) {
        int xBound = Mathf.FloorToInt(lScript.columns.Value / 2) + 1;
        int yBound = Mathf.FloorToInt(lScript.rows.Value / 2) + 1;
        int retVal = 0;
        if (myX >= xBound && myY >= yBound) {
            retVal = 1;
        } else if (myX < xBound && myY >= yBound) {
            retVal = 2;
        } else if (myX < xBound && myY < yBound) {
            retVal = 3;
        } else if (myX >= xBound && myY < yBound) {
            retVal = 4;
        }
        return retVal;
    }
    public IEnumerator UpdateMode(int type) {
        if (type == 1) {
            if (ScanInvList(inventory, 1, 0, false, 0).Count > 0) {
                if (HP < maxHP / 2) {
                    mode = 3;
                } else {
                    mode = 2;
                }
            } else {
                if (HP < maxHP / 2) {
                    mode = 3;
                } else {
                    mode = 1;
                }
            }
        } else if (type == 2) {
            //This is just after enemyAction
            if (HP < maxHP / 2) {
                mode = 3;
            } else {
                yield return StartCoroutine(UpdateMode(1));
            }
        }
        UpdateModeInList();
        yield return null;
    }
    void UpdateModeInList() {
        for (int i = 0; i < allPlayerModes[playerNum - 1].Count; i++) {
            if (i + 1 == mode) {
                allPlayerModes[playerNum - 1][i] = 1;
            } else {
                allPlayerModes[playerNum - 1][i] = 0;
            }
        }
        UpdatePlayerDataList(playerNum);
    }
    public void UpdatePlayerDataList(int pnum) {
        float searchWeight = allPlayerModes[pnum - 1][0];
        float huntWeight = allPlayerModes[pnum - 1][1];
        float hideWeight = allPlayerModes[pnum - 1][2];
        List<float> theList = allPlayerDataLists[pnum - 1];
        theList.Clear();
        for (int i = 0; i < elevationData.Count; i++) {
            theList.Add((rarityData[i] * searchWeight) + (elevationData[i] * huntWeight) + (hideData[i] * hideWeight));
        }
    }
    List<int> ScanInvList(List<int> inv, int type, int type2, bool checkCooldown, float dist) {
        List<int> retList = new List<int>();
        if (type == 1 || type == 2 || type == 4 || type == 6 || type == 7) {
            foreach (int item in inv) {
                if (item != 0) {
                    int index = item - 1;
                    bool cond1 = false;
                    if (lScript.itemIntLists[index][0].Contains(1)) {
                        if (type == 6) {
                            if (lScript.itemIntLists[index][1].Contains(13)) {
                                cond1 = true;
                            }
                        } else {
                            if (!lScript.itemIntLists[index][0].Contains(3) && !lScript.itemIntLists[index][1].Contains(13) && !lScript.itemIntLists[index][1].Contains(17)) { //the addition of the Contains(17) might be sus, watch out
                                cond1 = true;
                            }
                        }
                    }
                    if (cond1) {
                        if (type == 1) {
                            retList.Add(item);
                        } else if (type == 2 || type == 6) {
                            if (checkCooldown) {
                                if (botItemsOnCooldown[index] == 0) {
                                    retList.Add(item);
                                }
                            } else {
                                retList.Add(item);
                            }
                        } else if (type == 4) {
                            
                            if (lScript.itemInfos[index][0][8] > 0) {
                                if (checkCooldown) {
                                    if (botItemsOnCooldown[index] == 0) {
                                        retList.Add(item);
                                    }
                                } else {
                                    retList.Add(item);
                                }
                            }
                        } else if (type == 7) {
                            bool cond2 = (dist <= lScript.itemInfos[index][0][0]);
                            if (cond2) {
                                if (checkCooldown) {
                                    if (botItemsOnCooldown[index] == 0) {
                                        retList.Add(item);
                                    }
                                } else {
                                    retList.Add(item);
                                }
                            }
                        }
                    }
                } else {
                    //don't add
                }
            }
            if (type == 2 || type == 4 || type == 6 || type == 7) { //4 is ordered by damage but maybe it should be ordered by visionchange instead
                for (int i = 0; i < retList.Count - 1; i++) {
                    float currentDamage = GetExpectedDamage(retList[i] - 1);
                    float maxDamage = currentDamage;
                    int swapIndex = i;
                    for (int j = i + 1; j < retList.Count; j++) {
                        float newDamage = GetExpectedDamage(retList[j] - 1);
                        if (newDamage > maxDamage) {
                            maxDamage = newDamage;
                            swapIndex = j;
                        }
                    }
                    if (swapIndex != i) {
                        int temp = retList[i];
                        retList[i] = retList[swapIndex];
                        retList[swapIndex] = temp;
                    }
                }
            }
        } else if (type == 3) {
            foreach (int item in inv) {
                if (item != 0) {
                    int index = item - 1;
                    if (lScript.itemIntLists[index][0].Contains(2)) {
                        if (checkCooldown) {
                            if (botItemsOnCooldown[index] == 0) {
                                retList.Add(item);
                            }
                        } else {
                            retList.Add(item);
                        }
                    }
                }
            }
        } else if (type == 5) {
            foreach (int item in inv) {
                if (item != 0) {
                    int index = item - 1;
                    if (lScript.itemIntLists[index][0].Contains(3)) {
                        if (lScript.itemIntLists[index][4].Contains(type2)) {
                            if (checkCooldown) {
                                if (botItemsOnCooldown[index] == 0) {
                                    retList.Add(item);
                                }
                            } else {
                                retList.Add(item);
                            }
                        }
                    }
                }
            }
        } else if (type == 8) {
            foreach (int item in inv) {
                if (item != 0) {
                    int index = item - 1;
                    if (lScript.itemBools[index][0]) {
                        if (checkCooldown) {
                            if (botItemsOnCooldown[index] == 0) {
                                retList.Add(item);
                            }
                        } else {
                            retList.Add(item);
                        }
                    }
                }
            }
        }
        return retList;
    }
    public float GetExpectedDamage(int index) {
        return lScript.itemInfos[index][0][1] * Mathf.RoundToInt(lScript.itemInfos[index][0][3]);
    }
    bool TestBulletHit(int index, int pos, int enPos, NetworkList<int> grid) {
        Vector2 posV = new Vector2(lScript.GRIDX[pos - 1], lScript.GRIDY[pos - 1]);
        Vector2 targetV = new Vector2(lScript.GRIDX[enPos - 1], lScript.GRIDY[enPos - 1]); //\I
        Vector2 diff = targetV - posV;
        float angleRadians = Mathf.Atan2(diff.y, diff.x);
        float angleDeg = angleRadians * Mathf.Rad2Deg;
        int bomb = lScript.GetIsBomb(index);
        int trap = 0;
        if (lScript.itemIntLists[index][1].Contains(13)) trap = 1;
        List<List<float>> CBS = lScript.CalculateBulletStop(posV, Mathf.RoundToInt(lScript.itemInfos[index][0][13]), angleDeg, index, playerNum, knownPlayerPositions, lScript.itemInfos[index][0][0], trap, pos, lScript.itemInfos[index][0][5], diff.magnitude, lScript.FindElevation(grid[pos - 1], 1) + lScript.itemInfos[index][0][12], lScript.GetHilltopCoe(), bomb, lScript.itemInfos[index][0][1], lScript.itemInfos[index][0][9]);
        int endIndex = lScript.GetIndexFromWorldCoords(new Vector2(CBS[2][0], CBS[2][1]));
        float dist = lScript.FindDistance(lScript.GetCoordsFromIndex(endIndex), lScript.GetCoordsFromIndex(enPos));
        bool returnVal = false;
        if (bomb == 0) {
            if (Mathf.Approximately(dist, 0f)) {
                returnVal = true;
            }
        } else {
            if (dist < 1f || Mathf.Approximately(dist, 1f)) {
                returnVal = true;
            }
        }
        return returnVal;
    }
    int ScanInvInt(List<int> inv, int type, int search, int pos, int enPos, NetworkList<int> grid) { //pos & enPos are 1+
        int returnVal = 0;
        if (type == 1) {
            for (int i = 0; i < inv.Count; i++) {
                if (lScript.itemIntLists[inv[i] - 1][1].Contains(17)) {
                    returnVal = inv[i];
                    break;
                } else {
                    if (TestBulletHit(inv[i] - 1, pos, enPos, grid)) {
                        returnVal = inv[i];
                        break;
                    }
                }
            }
        } else if (type == 2) {
            for (int i = 0; i < inv.Count; i++) {
                if (inv[i] == search) {
                    returnVal++;
                }
            }
        }
        return returnVal;
    }
    public float myLD = 0;
    public float LD = 0;
    public int BD;
    bool seenMF = false;
    public IEnumerator TakeDamage(float[] EA, float[] EB, float[] ELDs, int[] ETI, int[] ESI, int width, int height) {
        //Debug.Log("bot TakeDamage");
        //HP -= Mathf.RoundToInt(armorCoefficient * localDamage);
        armorCoefficient = lScript.GetArmorCoefficient(inventory);
        HPCalculation = HP;
        BD = 0;
        int enemyPosition = lScript.playerPositionList[Mathf.RoundToInt(EA[0]) - 1];
        if (Mathf.RoundToInt(EA[20]) == 1) {
            enemyPosition = Mathf.RoundToInt(EA[21]);
        }
        myLD = EA[23];
        if (EA[24] != 0) {
            List<float> currentTrap = botTraps[Mathf.RoundToInt(EA[25])][Mathf.RoundToInt(EA[26])];
            yield return StartCoroutine(lScript.ExplodeBomb(Mathf.RoundToInt(EA[25]) + 1, currentTrap[3], currentTrap[2], 0, new List<int>(), Mathf.RoundToInt(EA[0]), 1, 1, false, 3, this));
            botTraps[Mathf.RoundToInt(EA[25])].RemoveAt(Mathf.RoundToInt(EA[26]));
        }
        Vector2 EV = new Vector2(lScript.GRIDX[enemyPosition - 1], lScript.GRIDY[enemyPosition - 1]);
        seenMF = false;
        for (int i = 0; i < EA[2]; i++) {
            float angleDeg = EB[i];
            GameObject bul = Instantiate(lScript.bulletPrefab, EV, Quaternion.Euler(0f, 0f, angleDeg));
            bul.GetComponent<SpriteRenderer>().enabled = false;
            //bul.GetComponent<SpriteRenderer>().sprite = lScript.bulletCostumes[Mathf.RoundToInt(EA[8])];
            //bul.transform.localScale = new Vector3(tileWidth * 5, tileHeight * 5, 1);
            int trapI = 0;
            if (i < ETI.Length) {
                trapI = ETI[i];
            }
            int smokeI = 0;
            if (i < ESI.Length) {
                smokeI = ESI[i];
            }
            StartCoroutine(lScript.BulletMove(Mathf.RoundToInt(EA[1]) - 1, bul, angleDeg, EA[6], EA[5], EA[7], enemyPosition, Mathf.RoundToInt(EA[0]), 3, EA[4], Mathf.RoundToInt(EA[9]), Mathf.RoundToInt(EA[10]), EA[11], EA[12], EA[13], EA[14], Mathf.RoundToInt(EA[15]), Mathf.RoundToInt(EA[16]), Mathf.RoundToInt(EA[17]), Mathf.RoundToInt(EA[18]), Mathf.RoundToInt(EA[19]), Mathf.RoundToInt(EA[20]), trapI, smokeI, Mathf.RoundToInt(EA[27]), false, false, this));
            //Debug.Log("bulmove2");
            yield return new WaitForSeconds(EA[3]);
        }
        while (BD != EA[2]) {
            yield return null;
        }
        if (Mathf.RoundToInt(EA[20]) == 1) { //lowkey just being lazy with grappling rn
            lastKnownPlayerPositions[Mathf.RoundToInt(EA[0]) - 1] = 0;
            potentialMovesSinceLastSeen[Mathf.RoundToInt(EA[0]) - 1].Clear();
            CreatePPP(Mathf.RoundToInt(EA[0]));
        }
        //HP = Mathf.RoundToInt(HPCalculation);
        //HP -= Mathf.RoundToInt(myLD * this.armorCoefficient);
        int hpChange = Mathf.RoundToInt(this.armorCoefficient * ELDs[playerNum - 1]);
        HP -= hpChange;
        lScript.HealthsServerRpc(playerNum, HP, maxHP, false, true, 1);
        //yield return StartCoroutine(UpdateMode(2));
        AdjustPlayerMode(Mathf.RoundToInt(EA[0]), 2, 0.1f);
        if (trainingModeActivated) {
            botBrain.ReceiveDamage(Mathf.Clamp01(hpChange / (float)maxHP), width, height, elevationData, hideData, rarityData);
            Debug.Log("Bot brain received damage " + Mathf.Clamp01(hpChange / (float)maxHP));
        }
        yield return null;
    }
    public void AdjustPlayerMode(int pnum, int mode, float amt) {
        /*float actualAmt = amt;
        if (allPlayerModes[pnum - 1][mode - 1] + amt > 1f) {
            actualAmt = 1f - allPlayerModes[pnum - 1][mode - 1];
        }*/
        int count = allPlayerModes[pnum - 1].Count;
        for (int i = 0; i < count; i++) {
            if (i + 1 == mode) {
                allPlayerModes[pnum - 1][i] += amt;
            } else {
                allPlayerModes[pnum - 1][i] -= amt / (count - 1);
            }
        }
        for (int i = 0; i < count; i++) {
            allPlayerModes[pnum - 1][i] = Mathf.Clamp01(allPlayerModes[pnum - 1][i]);
        }
        float currentSum = 0f;
        for (int i = 0; i < count; i++) {
            currentSum += allPlayerModes[pnum - 1][i];
        }
        if (Mathf.Approximately(currentSum, 0)) {
            Debug.Log("Error: currentSum is approximately 0!");
        }
        for (int i = 0; i < count; i++) {
            allPlayerModes[pnum - 1][i] /= currentSum;
        }
        UpdatePlayerDataList(pnum);
    }
    public void SeeMuzzleFlash(int pos, int pnum, int ppos) { //pos, pnum, ppos is 1+ //technically ppos is the position where the bullet started but for now the bullet always starts where the player is
        if (!seenMF) {
            if (myVG[pos - 1] != 0) {
                knownPlayerPositions[pnum - 1] = ppos;
                lastKnownPlayerPositions[pnum - 1] = ppos;
                potentialMovesSinceLastSeen[pnum - 1].Clear();
                CreatePPP(pnum);
                UpdatePlayerVG(pnum);
                seenMF = true;
            }
        }
    }
    Vector2 GetCornerCoords(int quad) {
        if (quad == 1) {
            return new Vector2(lScript.columns.Value, lScript.rows.Value);
        } else if (quad == 2) {
            return new Vector2(1, lScript.rows.Value);
        } else if (quad == 3) {
            return new Vector2(1, 1);
        } else if (quad == 4) {
            return new Vector2(lScript.columns.Value, 1);
        } else {
            return new Vector2();
        }
    }
    int FindClosestAirdrop(Vector2 myCoords, List<int> AD) {
        int index = -1;
        float minDistance = 0;
        for (int i = 0; i < AD.Count; i++) {
            if (index == -1) {
                index = i;
                minDistance = lScript.FindDistance(myCoords, lScript.GetCoordsFromIndex(AD[i]));
            } else {
                float distance = lScript.FindDistance(myCoords, lScript.GetCoordsFromIndex(AD[i]));
                if (distance < minDistance) {
                    minDistance = distance;
                    index = i;
                }
            }
        }
        return index;
    }
    List<int> GetKnownEnemyPositions(int mode) {
        List<int> choices = new List<int>();
        for (int i = 0; i < knownPlayerPositions.Count; i++) {
            if (i + 1 != playerNum) {
                if (mode == 1) {
                    if (knownPlayerPositions[i] != 0) {
                        choices.Add(i + 1);
                    } else if (lastKnownPlayerPositions[i] != 0 && (potentialMovesSinceLastSeen[i].Count == 0)) { //(potentialMovesSinceLastSeen[i].Count == 0 || potentialMovesSinceLastSeen[i] adds up to 0/the only element is 0)
                        choices.Add(i + 1);
                    }
                } else if (mode == 2) {
                    if (lastKnownPlayerPositions[i] != 0 && potentialMovesSinceLastSeen[i].Count > 0) { //(potentialMovesSinceLastSeen[i].Count > 0 || potentialMovesSinceLastSeen[i] adds up to be > 0/contains positive elements)
                        choices.Add(i + 1);
                    }
                } else if (mode == 3) {
                    choices.Add(i + 1);
                }
            }
        }
        return choices;
    }
    List<List<float>> GetEnemyHeatmaps() {
        List<List<float>> enemyHeatmaps = new List<List<float>>();
        for (int i = 0; i < allPlayerDataLists.Count; i++) {
            if (i + 1 != playerNum) {
                if (lScript.playerAliveList[i]) {
                    enemyHeatmaps.Add(allPlayerDataLists[i]);
                }
            }
        }
        return enemyHeatmaps;
    }
    List<float> GetCombinedHeatmap(List<List<float>> enemyHeatmaps) {
        List<float> combinedHeatmap = new List<float>();
        for (int i = 0; i < enemyHeatmaps[0].Count; i++) {
            float maxThreat = 0; //default value
            bool firstSet = true;
            for (int j = 0; j < enemyHeatmaps.Count; j++) {
                if (firstSet) {
                    maxThreat = enemyHeatmaps[j][i];
                    firstSet = false;
                } else {
                    if (enemyHeatmaps[j][i] > maxThreat) {
                        maxThreat = enemyHeatmaps[j][i];
                    }
                }
            }
            combinedHeatmap.Add(maxThreat);
        }
        return combinedHeatmap;
    }
    List<(int index, float moves, int LMIndex)> yetToCheck = new List<(int index, float moves, int LMIndex)>(); //index is 0+, LMIndex is 0+
    List<(int index, float moves, int LMIndex)> alreadyChecked = new List<(int index, float moves, int LMIndex)>(); //index is 0+, LMIndex is 0+
    List<bool> arrivable;
    public IEnumerator MoveSomewhere(int width, int height, NetworkList<int> g, float startMoves) {
        yield return StartCoroutine(UpdateIsSmoked(botSmokes));
        List<int> grid = new List<int>();
        arrivable = new List<bool>();
        foreach (int item in g) {
            grid.Add(item);
            arrivable.Add(false);
        }

        CreateArrivable(grid, position, arrivable, yetToCheck, alreadyChecked, new List<float>{startMoves});
        int aIndex;
        int aiMode = 1;
        if (LocalScript.gamemode == 1 || LocalScript.gamemode == 2 || LocalScript.gamemode == 3) {
            aiMode = 1;
        } else if (LocalScript.gamemode == 4) {
            if (playerNum == 1) {
                aiMode = 1;
            } else if (playerNum == 2) {
                aiMode = 2;
            }
        }
        if (aiMode == 1) {
            if (mode == 1) {
                if (botAirdrops.Count > 0) {
                    aIndex = FindLowestDistance(1, arrivable, lScript.GetCoordsFromIndex(botAirdrops[FindClosestAirdrop(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]), botAirdrops)]), null);
                } else {
                    aIndex = FindHighestRarity(arrivable, grid);
                }
            } else if (mode == 2) { 
                aIndex = HuntEnemy(arrivable, grid, usePlan);
            } else if (mode == 3) {
                aIndex = RunAway(arrivable, grid);
            } else {
                aIndex = GetRandomIndex(arrivable);
            }
            position = aIndex + 1;
            yield return StartCoroutine(BotVisionHouse(false, 0));
        } else if (aiMode == 2) {
            float[,,] cnnInput = botBrain.GenerateCNNInput(width, height, elevationData, hideData, rarityData, GetKnownEnemyPositions(1), GetCombinedHeatmap(GetEnemyHeatmaps()), botAirdrops);
            aIndex = botBrain.PickBestMoveCNN(trainingModeActivated, arrivable, cnnInput, width, height, grid.Count, allPlayerModes[playerNum - 1]);
            if (aIndex + 1 == 0) {
                Debug.Log("Why am I setting position to 0");
            }
            
            if (!arrivable[aIndex]) Debug.Log("Going somewhere that is illegal!");
            position = aIndex + 1;
            yield return StartCoroutine(BotVisionHouse(false, 0));
            if (trainingModeActivated) {
                EvalAndTrainBrain(grid, width, height, aIndex, allPlayerModes[playerNum - 1], cnnInput);
            }
        }
        //position++;

        //lScript.DisplayBotTest(1, arrivable, null);
        yield return null;
    }
    void EvalAndTrainBrain(List<int> grid, int width, int height, int aIndex, List<float> myMode, float[,,] cnnInput) {

        float targetScore = 0f;
        if (botBrain.moveHistory.Count > 0) {
            targetScore += botBrain.CalculateTileBaseReward(botBrain.moveHistory[botBrain.moveHistory.Count - 1], width, height);
        }
        if (myMode[0] > 0.5f) {
            float vagueRarity = rarityData[aIndex];
            int exactRarity = BotGetLandRarity(aIndex, grid);
            if (exactRarity == maxLandRarity) {
                targetScore += 1f;
            } else if (vagueRarity > 0.6f) {
                targetScore += 0.5f;
            } else {
                targetScore += -0.5f;
            }
        }
        botBrain.Backpropagate(cnnInput, aIndex, width, height, myMode, new List<int>{0}, new List<float>{targetScore});
    }
    void CreateArrivable(List<int> grid, int startPos, List<bool> a, List<(int index, float moves, int LMIndex)> YTC, List<(int index, float moves, int LMIndex)> AC, List<float> listMoves) {
        YTC.Clear();
        AC.Clear();
        YTC.Add((index: startPos - 1, moves: listMoves[0], LMIndex: 0)); //think if moves starts with 0, then it will be wrong
        while (YTC.Count > 0) {
            CheckNeighbors(grid, a, YTC, AC, listMoves);
        }
    }
    int HuntEnemy(List<bool> a, List<int> grid, List<int> UP) {
        int aIndex = -1;
        int target = FindRandomTarget(1, false); //target is 1+
        if (target == 0) {
            int target2 = FindRandomTarget(2, false);
            if (target2 == 0) {
                int target3 = FindRandomTarget(3, false);
                if (currentQuadrant != targetQuadrant) {
                    aIndex = FindLowestDistance(1, a, GetCornerCoords(targetQuadrant), null);
                } else {
                    if (timeSpentInTQ <= 3) { //arbitrary value
                        if (target3 == 0) {
                            aIndex = FindHighestElevation(a, grid); //you can do FindMostSeeing here based on the neural network data
                        } else {
                            aIndex = FindMostSeeing(2, a, null, allPlayerDataLists[target3 - 1]);
                        }
                    } else {
                        aIndex = FindLowestDistance(1, a, GetCornerCoords(targetQuadrant), null);
                    }
                }
            } else {
                List<int> weaponsInRange = GetWeaponsAndDetonators(1, UP, true, target, 0)[0].ConvertAll(x => (int)x);
                if (weaponsInRange.Count == 0 || lScript.itemIntLists[weaponsInRange[0] - 1][1].Contains(17)) { //\G
                    //go somewhere where you can see the most of potentialPlayerPositions
                    aIndex = FindMostSeeing(1, a, potentialPlayerPositions[target2 - 1], allPlayerDataLists[target2 - 1]);
                } else {
                    aIndex = FindLowestDistance(2, a, new Vector2(), potentialPlayerPositions[target2 - 1]);
                }
            }
        } else {
            int ppos = knownPlayerPositions[target - 1];
            if (ppos == 0) {
                ppos = lastKnownPlayerPositions[target - 1];
            }
            int aIndexAttempt = FindLowestDistance(1, a, lScript.GetCoordsFromIndex(ppos), null);
            List<int> weaponsInRange = GetWeaponsAndDetonators(2, UP, true, target, lScript.FindDistance(lScript.GetCoordsFromIndex(aIndexAttempt + 1), lScript.GetCoordsFromIndex(ppos)))[0].ConvertAll(x => (int)x);
            if (weaponsInRange.Count == 0 || lScript.itemIntLists[weaponsInRange[0] - 1][1].Contains(17)) { //\G
                //aIndex is somewhere out of view but still has enemy in view
                List<bool> arrivable2 = FindPPVGList(a, potentialPlayerVGs[target - 1]);
                if (arrivable2.Contains(true)) {
                    List<bool> arrivable3 = FindWhereICanSeeEnemy(arrivable2, ppos);
                    if (arrivable3.Contains(true)) {
                        aIndex = GetRandomIndex(arrivable3);
                    } else {
                        aIndex = GetRandomIndex(arrivable2);
                    }
                } else {
                    //find somewhere that gives the most vision
                    aIndex = aIndexAttempt;
                    Debug.Log("Nowhere to hide");
                }
            } else {
                aIndex = aIndexAttempt;
                Debug.Log("Gonna use weapon " + weaponsInRange[0]);
            }
            
        }
        return aIndex;
    }
    int FindMostSeeing(int mode, List<bool> a, List<bool> PPP, List<float> PDL) { //0+
        List<float> scores = new List<float>();
        for (int i = 0; i < a.Count; i++) {
            List<int> VL = lScript.GetVisionList(0, botSmokes, 3, this, botEffects, botEffectStrengths, inventory, i + 1); //when I move it's not gonna change my effects
            float addVal = 0;
            for (int j = 0; j < VL.Count; j++) {
                if (VL[j] != 0) {
                    if (mode == 1) {
                        if (PPP[j]) {
                            addVal += PDL[j]; //weight here
                        }
                    } else if (mode == 2) {
                        addVal += PDL[j]; //weight here
                    }
                }
            }
            scores.Add(addVal);
        }
        bool firstSet = true;
        float highest = 0;
        if (a.Count == 0) {
            Debug.Log("a Count is 0!");
        }
        for (int i = 0; i < scores.Count; i++) {
            if (a[i]) {
                if (firstSet) {
                    highest = scores[i];
                    firstSet = false;
                } else {
                    if (scores[i] > highest) {
                        highest = scores[i];
                    }
                }
            }
        }
        List<int> indexOptions = new List<int>();
        for (int i = 0; i < scores.Count; i++) {
            if (a[i]) {
                if (scores[i] == highest) {
                    indexOptions.Add(i);
                }
            }
        }
        if (indexOptions.Count > 1) {
            return indexOptions[UnityEngine.Random.Range(0, indexOptions.Count)];
        } else {
            if (indexOptions.Count > 0) {
                return indexOptions[0];
            } else {
                return -1; //error
            }
        }
    }
    List<bool> FindWhereICanSeeEnemy(List<bool> a, int pos) {
        //pos is 1+
        List<bool> retList = new List<bool>();
        for (int i = 0; i < a.Count; i++) {
            bool addVal = false;
            if (a[i]) {
                List<int> VL = lScript.GetVisionList(0, botSmokes, 3, this, botEffects, botEffectStrengths, inventory, i + 1); //when I move it's not gonna change my effects
                if (VL[pos - 1] != 0) {
                    addVal = true;
                }
            }
            
            retList.Add(addVal);
        }
        return retList;
    }
    int FindSmoked(List<bool> a, bool[] BIS, int returnMode, Vector2 coords) { //0+
        /* returnMode:
        1 = random
        2 = farthest
        */
        List<bool> a2 = new List<bool>();
        List<int> workingIndices = new List<int>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i] && BIS[i]) {
                workingIndices.Add(i);
                a2.Add(true);
            } else {
                a2.Add(false);
            }
        }
        if (returnMode == 1) {
            if (workingIndices.Count > 0) {
                return workingIndices[UnityEngine.Random.Range(0, workingIndices.Count)];
            } else {
                return -1;
            }
        } else if (returnMode == 2) {
            if (workingIndices.Count > 0) {
                return FindHighestDistance(1, a2, coords, null);
            } else {
                return -1;
            }
        }
        return -1;
    }
    List<bool> FindSmokedList(List<bool> a, bool[] BIS) {
        List<bool> retList = new List<bool>();
        for (int i = 0; i < a.Count; i++) {
            retList.Add(a[i] && BIS[i]);
        }
        return retList;
    }
    int RunAway(List<bool> a, List<int> grid) {
        int aIndex = -1;
        int target = FindRandomTarget(1, false); //target is 1+
        if (target == 0) {
            int target2 = FindRandomTarget(2, false);
            
            if (target2 == 0) {
                //this means lastKnownPlayerPositions is all 0
                if (aIndex == -1) {
                    aIndex = FindSmoked(a, botIsSmoked, 1, new Vector2());
                }
                if (aIndex == -1) {
                    aIndex = FindCave(a, grid);
                }
                if (aIndex == -1) {
                    aIndex = FindHighestDistance(1, a, lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]), null);
                }
            } else {
                //this means at least one enemy's position is somewhat known (at least 1 move since last known)
                //use compoundPlayerVGs
                List<bool> arrivable2 = FindCPVGList(a, compoundPlayerVGs[target2 - 1]);
                if (arrivable2.Contains(true)) {
                    if (aIndex == -1) {
                        List<bool> arrivable3 = FindSmokedList(arrivable2, botIsSmoked);
                        //aIndex = FindSmoked(a, botIsSmoked, 1, new Vector2());
                        if (arrivable3.Contains(true)) {
                            aIndex = FindHighestDistance(2, arrivable3, new Vector2(), potentialPlayerPositions[target2 - 1]);
                        }
                    }
                    if (aIndex == -1) {
                        List<bool> arrivable3 = FindCaveList(arrivable2, grid);
                        //aIndex = FindCave(a, grid);
                        if (arrivable3.Contains(true)) {
                            aIndex = FindHighestDistance(2, arrivable3, new Vector2(), potentialPlayerPositions[target2 - 1]);
                        }
                    }
                    if (aIndex == -1) {
                        aIndex = FindHighestDistance(2, arrivable2, new Vector2(), potentialPlayerPositions[target2 - 1]);
                    }
                } else {
                    if (aIndex == -1) {
                        List<bool> arrivable3 = FindSmokedList(a, botIsSmoked);
                        //aIndex = FindSmoked(a, botIsSmoked, 1, new Vector2());
                        if (arrivable3.Contains(true)) {
                            aIndex = FindHighestDistance(2, arrivable3, new Vector2(), potentialPlayerPositions[target2 - 1]);
                        }
                    }
                    if (aIndex == -1) {
                        List<bool> arrivable3 = FindCaveList(a, grid);
                        //aIndex = FindCave(a, grid);
                        if (arrivable3.Contains(true)) {
                            aIndex = FindHighestDistance(2, arrivable3, new Vector2(), potentialPlayerPositions[target2 - 1]);
                        }
                    }
                    if (aIndex == -1) {
                        aIndex = FindHighestDistance(2, a, new Vector2(), potentialPlayerPositions[target2 - 1]);
                    }
                }
            }
        } else {
            //this means you either know exactly where someone is or lastKnownPlayerPositions lets you infer the exact position of someone, either way you know their position
            //use potentialPlayerVGs
            int ppos = knownPlayerPositions[target - 1];
            if (ppos == 0) {
                ppos = lastKnownPlayerPositions[target - 1];
            }
            if (aIndex == -1) {
                List<bool> arrivable2 = FindSmokedList(a, botIsSmoked);
                //aIndex = FindSmoked(a, botIsSmoked, 2, lScript.GetCoordsFromIndex(ppos));
                if (arrivable2.Contains(true)) {
                    List<bool> arrivable3 = FindPPVGList(arrivable2, potentialPlayerVGs[target - 1]);
                    if (arrivable3.Contains(true)) {
                        aIndex = FindHighestDistance(1, arrivable2, lScript.GetCoordsFromIndex(ppos), null);
                    }
                }
            }
            if (aIndex == -1) {
                List<bool> arrivable2 = FindCaveList(a, grid);
                //aIndex = FindCave(a, grid);
                if (arrivable2.Contains(true)) {
                    List<bool> arrivable3 = FindPPVGList(arrivable2, potentialPlayerVGs[target - 1]);
                    if (arrivable3.Contains(true)) {
                        aIndex = FindHighestDistance(1, arrivable2, lScript.GetCoordsFromIndex(ppos), null);
                    }
                }
            }
            if (aIndex == -1) {
                List<bool> arrivable2 = FindPPVGList(a, potentialPlayerVGs[target - 1]);
                if (arrivable2.Contains(true)) {
                    aIndex = FindHighestDistance(1, arrivable2, lScript.GetCoordsFromIndex(ppos), null);
                } else {
                    aIndex = FindHighestDistance(1, a, lScript.GetCoordsFromIndex(ppos), null);
                }
            }
        }
        return aIndex;
    }
    List<bool> FindPPVGList(List<bool> a, List<int> VG) {
        List<bool> retList = new List<bool>();
        for (int i = 0; i < a.Count; i++) {
            retList.Add(a[i] && (VG[i] == 0));
        }
        return retList;
    }
    List<bool> FindCPVGList(List<bool> a, List<float> VG) {
        List<bool> retList = new List<bool>();
        bool firstSet = true;
        float lowest = 0;
        for (int i = 0; i < a.Count; i++) {
            if (firstSet) {
                lowest = VG[i];
                firstSet = false;
            } else {
                if (VG[i] < lowest) {
                    lowest = VG[i];
                }
            }
        }
        for (int i = 0; i < a.Count; i++) {
            retList.Add(a[i] && (VG[i] == lowest));
        }
        return retList;
    }
    public IEnumerator UpdateTargetQuadrant() {
        if (mode == 2) {
            int target = FindRandomTarget(1, false);
            if (target == 0) {
                if (currentQuadrant == targetQuadrant) {
                    timeSpentInTQ++;
                    if (timeSpentInTQ > 5) { //arbitrary value
                        //switch targetQuadrant
                        int newQuadrant = UnityEngine.Random.Range(1, 5);
                        while (newQuadrant == targetQuadrant) {
                            newQuadrant = UnityEngine.Random.Range(1, 5);
                        }
                        timeSpentInTQ = 0;
                        targetQuadrant = newQuadrant;
                    }
                } else {
                    timeSpentInTQ = 0;
                }
            } else {
                timeSpentInTQ = 0;
                targetQuadrant = currentQuadrant;
            }
        }
        yield return null;
    }
    int GetRandomIndex(List<bool> a) { //0+
        int returnVal = UnityEngine.Random.Range(0, a.Count);
        while (!a[returnVal]) {
            returnVal = UnityEngine.Random.Range(0, a.Count);
        }
        return returnVal;
    }
    public int FindRandomTarget(int mode, bool closest) {
        List<int> choices = GetKnownEnemyPositions(mode);
        if (closest) {
            if (choices.Count == 0) {
                return 0;
            } else if (choices.Count == 1) {
                return choices[0];
            }
            Vector2 myCoords = lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]);
            float minDistance = -1;
            bool firstSet = true;
            int minID = 0;
            List<(int num, int ID)> potentialMins = new List<(int num, int ID)>();
            for (int i = 0; i < choices.Count; i++) {
                float dist = lScript.FindDistance(myCoords, lScript.GetCoordsFromIndex(knownPlayerPositions[choices[i] - 1]));
                if (firstSet) {
                    minDistance = dist;
                    firstSet = false;
                    minID++;
                    potentialMins.Add((num: choices[i], ID: minID));
                } else {
                    if (dist == minDistance) {
                        potentialMins.Add((num: choices[i], ID: minID));
                    } else if (dist < minDistance) {
                        minDistance = dist;
                        minID++;
                        potentialMins.Add((num: choices[i], ID: minID));
                    }
                }
            }
            List<int> numOptions = new List<int>();
            for (int i = 0; i < potentialMins.Count; i++) {
                var PM = potentialMins[i];
                if (PM.ID == minID) {
                    numOptions.Add(PM.num);
                }
            }
            if (numOptions.Count > 1) {
                return numOptions[UnityEngine.Random.Range(0, numOptions.Count)];
            } else {
                if (numOptions.Count > 0) {
                    return numOptions[0];
                } else {
                    return -1; //error
                }
            }
        } else {
            if (choices.Count > 0) {
                return choices[UnityEngine.Random.Range(0, choices.Count)];
            } else {
                return 0;
            }
        }
    }
    int FindClosestTarget(Vector2 myCoords) { //returns 1+
        List<int> choices = new List<int>();
        for (int i = 0; i < knownPlayerPositions.Count; i++) {
            if (i + 1 != playerNum) {
                if (knownPlayerPositions[i] != 0) {
                    choices.Add(i + 1);
                }
            }
        }
        if (choices.Count == 0) {
            return 0;
        } else if (choices.Count == 1) {
            return choices[0];
        }
        float minDistance = -1;
        bool firstSet = true;
        int minID = 0;
        List<(int num, int ID)> potentialMins = new List<(int num, int ID)>();
        for (int i = 0; i < choices.Count; i++) {
            float dist = lScript.FindDistance(myCoords, lScript.GetCoordsFromIndex(knownPlayerPositions[choices[i] - 1]));
            if (firstSet) {
                minDistance = dist;
                firstSet = false;
                minID++;
                potentialMins.Add((num: choices[i], ID: minID));
            } else {
                if (dist == minDistance) {
                    potentialMins.Add((num: choices[i], ID: minID));
                } else if (dist < minDistance) {
                    minDistance = dist;
                    minID++;
                    potentialMins.Add((num: choices[i], ID: minID));
                }
            }
        }
        List<int> numOptions = new List<int>();
        for (int i = 0; i < potentialMins.Count; i++) {
            var PM = potentialMins[i];
            if (PM.ID == minID) {
                numOptions.Add(PM.num);
            }
        }
        if (numOptions.Count > 1) {
            return numOptions[UnityEngine.Random.Range(0, numOptions.Count)];
        } else {
            if (numOptions.Count > 0) {
                return numOptions[0];
            } else {
                return -1; //error
            }
        }
    }
    public int indexHelper; //[2]
    public float movesHelper; //[1]
    public int workedHelper; //[0]
    void CheckNeighbors(List<int> grid, List<bool> a, List<(int index, float moves, int LMIndex)> YTC, List<(int index, float moves, int LMIndex)> AC, List<float> listMoves) {
        var gridSquare = YTC[0];
        int idx = gridSquare.index;
        float mvs = gridSquare.moves;
        int lmidx = gridSquare.LMIndex;
        //Vector2 coords = lScript.GetCoordsFromIndex(idx + 1);
        CheckNeighbor(0, 1, mvs, idx + 1, lmidx, grid.Count, a, YTC, AC, listMoves);
        CheckNeighbor(0, -1, mvs, idx + 1, lmidx, grid.Count, a, YTC, AC, listMoves);
        CheckNeighbor(1, 0, mvs, idx + 1, lmidx, grid.Count, a, YTC, AC, listMoves);
        CheckNeighbor(-1, 0, mvs, idx + 1, lmidx, grid.Count, a, YTC, AC, listMoves);
        YTC.RemoveAt(0);
        AC.Add(gridSquare);
    }
    void CheckNeighbor(int x, int y, float mvs, int PP, int lmidx, int gridCount, List<bool> a, List<(int index, float moves, int LMIndex)> YTC, List<(int index, float moves, int LMIndex)> AC, List<float> listMoves) {
        /*indexHelper = -1;
        movesHelper = 0;
        workedHelper = -1;*/
        List<float> helpers = lScript.ChangePosition(x, y, 2, mvs, PP, gridCount, this);
        workedHelper = Mathf.RoundToInt(helpers[0]);
        movesHelper = helpers[1];
        indexHelper = Mathf.RoundToInt(helpers[2]);
        /*while (workedHelper == -1) {
            yield return null;
        }*/
        if (workedHelper == 1) {
            var newGridSquare = (index: indexHelper, moves: movesHelper, LMIndex: lmidx);
            if (newGridSquare.moves <= 0) {
                int lmi = newGridSquare.LMIndex;
                if (lmi + 1 > listMoves.Count - 1) {
                    a[newGridSquare.index] = true;
                } else {
                    lmi++;
                    YTC.Add((index: newGridSquare.index, moves: listMoves[lmi], LMIndex: lmi));
                }
                
            } else {
                if (YTC.Contains(newGridSquare) || AC.Contains(newGridSquare)) {
                    //don't add
                } else {
                    YTC.Add(newGridSquare);
                }
            }
        }
    }
    public int FindLowestDistance(int mode, List<bool> a, Vector2 targetCoords, List<bool> potentialLocs) { //0+
        float minDistance = -1;
        bool firstSet = true;
        int minID = 0;
        List<(int index, int ID)> potentialMins = new List<(int index, int ID)>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                float dist = 0;
                if (mode == 1) {
                    dist = lScript.FindDistance(lScript.GetCoordsFromIndex(i + 1), targetCoords);
                } else if (mode == 2) {
                    dist = FindAvgDistance(lScript.GetCoordsFromIndex(i + 1), potentialLocs);
                }
                if (firstSet) {
                    //set no matter what
                    minDistance = dist;
                    firstSet = false;
                    minID++;
                    potentialMins.Add((index: i, ID: minID));
                } else {
                    if (dist == minDistance) {
                        potentialMins.Add((index: i, ID: minID));
                    } else if (dist < minDistance) {
                        minDistance = dist;
                        minID++;
                        potentialMins.Add((index: i, ID: minID));
                    }
                }
            }
        }
        List<int> indexOptions = new List<int>();
        for (int i = 0; i < potentialMins.Count; i++) {
            var PM = potentialMins[i];
            if (PM.ID == minID) {
                indexOptions.Add(PM.index);
            }
        }
        if (indexOptions.Count > 1) {
            return indexOptions[UnityEngine.Random.Range(0, indexOptions.Count)];
        } else {
            if (indexOptions.Count > 0) {
                return indexOptions[0];
            } else {
                return -1; //error
            }
        }
    }
    float FindAvgDistance(Vector2 coords, List<bool> potentialLocs) {
        int count = 0;
        float totalDistance = 0;
        for (int i = 0; i < potentialLocs.Count; i++) {
            if (potentialLocs[i]) {
                totalDistance += lScript.FindDistance(lScript.GetCoordsFromIndex(i + 1), coords);
                count++;
            }
        }
        return totalDistance / count;
    }
    int FindHighestDistance(int mode, List<bool> a, Vector2 targetCoords, List<bool> potentialLocs) { //0+
        float maxDistance = -1;
        bool firstSet = true;
        int maxID = 0;
        List<(int index, int ID)> potentialMaxes = new List<(int index, int ID)>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                float dist = 0;
                if (mode == 1) {
                    dist = lScript.FindDistance(lScript.GetCoordsFromIndex(i + 1), targetCoords);
                } else if (mode == 2) {
                    dist = FindAvgDistance(lScript.GetCoordsFromIndex(i + 1), potentialLocs);
                }
                if (firstSet) {
                    //set no matter what
                    maxDistance = dist;
                    firstSet = false;
                    maxID++;
                    potentialMaxes.Add((index: i, ID: maxID));
                } else {
                    if (dist == maxDistance) {
                        potentialMaxes.Add((index: i, ID: maxID));
                    } else if (dist > maxDistance) {
                        maxDistance = dist;
                        maxID++;
                        potentialMaxes.Add((index: i, ID: maxID));
                    }
                }
            }
        }
        List<int> indexOptions = new List<int>();
        for (int i = 0; i < potentialMaxes.Count; i++) {
            var PM = potentialMaxes[i];
            if (PM.ID == maxID) {
                indexOptions.Add(PM.index);
            }
        }
        if (indexOptions.Count > 1) {
            return indexOptions[UnityEngine.Random.Range(0, indexOptions.Count)];
        } else {
            if (indexOptions.Count > 0) {
                return indexOptions[0];
            } else {
                return -1; //error
            }
        }
    }
    int FindHighestElevation(List<bool> a, List<int> g) { //0+
        float highestElevation = -10; //arbitrary value
        bool firstSet = true;
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                float elevation = lScript.FindElevation(g[i], 1);
                if (firstSet) {
                    highestElevation = elevation;
                    firstSet = false;
                } else {
                    if (elevation > highestElevation) {
                        highestElevation = elevation;
                    }
                }
            }
        }
        List<int> indexOptions = new List<int>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                if (lScript.FindElevation(g[i], 1) == highestElevation) {
                    indexOptions.Add(i);
                }
            }
        }
        if (indexOptions.Count > 1) {
            return indexOptions[UnityEngine.Random.Range(0, indexOptions.Count)];
        } else {
            if (indexOptions.Count > 0) {
                return indexOptions[0];
            } else {
                return -1; //error
            }
        }
    }
    int BotGetLandRarity(int i, List<int> g) {
        int rarity = 0; //arbitrary value
        if (botMysteryBuildings.Contains(i)) {
            rarity = 6; //this is the mystery building rarity rating, it's the average building rarity \E
        } else {
            rarity = lScript.GetLandRarity(g[i]);
        }
        //implement airdrops later
        return rarity;
    }
    int FindHighestRarity(List<bool> a, List<int> g) { //0+
        int highestRarity = -1;
        bool firstSet = true;
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                int rarity = BotGetLandRarity(i, g);
                if (botAirdrops.Contains(i + 1)) {
                    rarity = 9; //\D
                }
                if (firstSet) {
                    highestRarity = rarity;
                    firstSet = false;
                } else {
                    if (rarity > highestRarity) {
                        highestRarity = rarity;
                    }
                }
            }
        }
        List<int> indexOptions = new List<int>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                int rarity = BotGetLandRarity(i, g);
                if (botAirdrops.Contains(i + 1)) {
                    rarity = 9; //\D
                }
                if (rarity == highestRarity) {
                    indexOptions.Add(i);
                }
            }
        }
        if (indexOptions.Count > 1) {
            return indexOptions[UnityEngine.Random.Range(0, indexOptions.Count)];
        } else {
            if (indexOptions.Count > 0) {
                return indexOptions[0];
            } else {
                return -1; //error
            }
        }
    }
    int FindCave(List<bool> a, List<int> g) {
        List<int> indexOptions = new List<int>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                if (g[i] == 9) {
                    indexOptions.Add(i);
                }
            }
        }
        if (indexOptions.Count > 1) {
            return indexOptions[UnityEngine.Random.Range(0, indexOptions.Count)];
        } else {
            if (indexOptions.Count > 0) {
                return indexOptions[0];
            } else {
                return -1; //no caves in arrivable
            }
        }
    }
    List<bool> FindCaveList(List<bool> a, List<int> g) {
        List<bool> retList = new List<bool>();
        for (int i = 0; i < a.Count; i++) {
            retList.Add((a[i] && (g[i] == 9)));
        }
        return retList;
    }
    public void GetUAATRating(List<int> inv, int item, List<(int item, int UAAT, int rating)> potentials, int health, int maxHealth) {
        int index = item - 1;
        int maxUAAT = Mathf.Min(lScript.itemInts[index][5], ScanInvInt(inv, 2, item, 0, 0, null));
        for (int j = 1; j <= maxUAAT; j++) {
            potentials.Add((item: item, UAAT: j, GetHealingRating(health, maxHealth, item, j)));
        }
    }
    public int GetMaxRating(List<(int item, int UAAT, int rating)> potentials) {
        int maxRating = 0;
        bool firstSet = true;
        for (int i = 0; i < potentials.Count; i++) {
            int r = potentials[i].rating;
            if (firstSet) {
                maxRating = r;
                firstSet = false;
            } else {
                if (r > maxRating) {
                    maxRating = r;
                }
            }
        }
        return maxRating;
    }
    public int GetMinRating(List<(int item, int UAAT, int rating)> potentials) {
        int minRating = 0;
        bool firstSet = true;
        for (int i = 0; i < potentials.Count; i++) {
            int r = potentials[i].rating;
            if (firstSet) {
                minRating = r;
                firstSet = false;
            } else {
                if (r < minRating) {
                    minRating = r;
                }
            }
        }
        return minRating;
    }
    List<int> GetMaxHealing(List<(int item, int UAAT, int rating)> potentials) {
        int maxRating = GetMaxRating(potentials);
        List<(int item, int UAAT)> itemOptions = new List<(int item, int UAAT)>();
        for (int i = 0; i < potentials.Count; i++) {
            if (potentials[i].rating == maxRating) {
                itemOptions.Add((item: potentials[i].item, UAAT: potentials[i].UAAT));
            }
        }
        if (itemOptions.Count > 1) {
            int rand = UnityEngine.Random.Range(0, itemOptions.Count);
            return new List<int> {itemOptions[rand].item, itemOptions[rand].UAAT};
        } else if (itemOptions.Count > 0) {
            return new List<int> {itemOptions[0].item, itemOptions[0].UAAT};
        } else {
            return null;
        }
    }
    public List<int> FindBestHealing(int mode, List<int> inv, int health, int maxHealth) {
        //mode 1: returns 1+
        List<int> healingInInv = ScanInvList(inv, 3, 0, true, 0);
        List<(int item, int UAAT, int rating)> potentials = new List<(int item, int UAAT, int rating)>();
        for (int i = 0; i < healingInInv.Count; i++) {
            GetUAATRating(inv, healingInInv[i], potentials, health, maxHealth);
        }
        if (mode == 1) {
            return GetMaxHealing(potentials);
        } else if (mode == 2) {
            return new List<int>{GetMaxRating(potentials)};
        } else if (mode == 3) {
            return new List<int>{GetMinRating(potentials)};
        }
        return null;
    }
    int GetHealingRating(int health, int maxHealth, int item, int j) {
        int pseudoHP = health;
        int pseudoMHP = maxHealth;
        int index = item - 1;
        int healing = Mathf.RoundToInt(lScript.itemInfos[index][1][0]);
        healing += Mathf.RoundToInt(lScript.FindHighestBookEffectAmt(botBookInventory, 2));
        int maxHealthChange = Mathf.RoundToInt(lScript.itemInfos[index][1][2]);
        int wastedHealth = 0;
        for (int i = 0; i < j; i++) {
            pseudoMHP += maxHealthChange;
            if (pseudoHP + healing > pseudoMHP) {
                wastedHealth += pseudoHP + healing - pseudoMHP;
                pseudoHP = pseudoMHP;
            } else {
                //no wasted health!
                pseudoHP += healing;
            }
        }
        return (pseudoHP - health - wastedHealth); //you want it to be big
    }
    List<float> FindMaxDetonableDamage(int target) {
        //trap: {owner (playerNum), type, damage, blast radius, costumeIndex, fake blast radius} (all floats)
        float maxDamage = 0;
        int index1 = -1;
        int index2 = -1;
        Vector2 targetPos = lScript.GetCoordsFromIndex(knownPlayerPositions[target - 1]);
        for (int i = 0; i < botTraps.Count; i++) {
            List<List<float>> listTraps = botTraps[i];
            for (int j = 0; j < listTraps.Count; j++) {
                List<float> trap = listTraps[j];
                if (Mathf.RoundToInt(trap[0]) == playerNum) {
                    Vector2 currentPosition = lScript.GetCoordsFromIndex(i + 1);
                    float distance = lScript.FindDistance(targetPos, currentPosition);
                    float blastRadius = trap[3];
                    float targetDamage = 0;
                    if (distance <= blastRadius) {
                        if (distance > 1) {
                            targetDamage = lScript.GetBombDamage(blastRadius, distance, trap[2]);
                        } else {
                            targetDamage = trap[2];
                        }
                    } else {
                        targetDamage = 0;
                    }
                    if (targetDamage > maxDamage) {
                        maxDamage = targetDamage;
                        index1 = i;
                        index2 = j;
                    }
                }
            }
        }
        return new List<float>{maxDamage, index1, index2};
    }
    public List<int> usePlan = new List<int>(); //1+, 0 means searching (preemptive use plan)
    public void CreateUsePlan() {
        usePlan.Clear();
        //all items must not be on cooldown
        if (mode == 2) {
            //weapons
            List<int> weapons = GetWeaponsAndDetonators(1, inventory, true, 0, 0)[0].ConvertAll(x => (int)x);
            foreach (int w in weapons) {
                usePlan.Add(w);
            }
        } else if (mode == 3) {
            //concealment
            //\F
            List<int> toolOptions = ScanInvList(inventory, 5, 8, true, 0); //change location
            toolOptions.AddRange(ScanInvList(inventory, 5, 1, true, 0)); //invis
            toolOptions.AddRange(ScanInvList(inventory, 5, 3, true, 0)); //partial invis
            foreach (int t in toolOptions) {
                usePlan.Add(t);
            }
        }
        //healing
        List<int> FBH = FindBestHealing(1, inventory, HP, maxHP);
        if (FBH != null) {
            foreach (int f in FBH) {
                usePlan.Add(f);
            }
        }
        usePlan.Add(0);
    }
    public int FindClosestTargetToMe() {
        return FindClosestTarget(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]));
    }
    public float[] features;
    public IEnumerator BotSearchUse(NetworkList<int> grid, int width, int height) {
        int aiMode = 1;
        if (LocalScript.gamemode == 1 || LocalScript.gamemode == 2 || LocalScript.gamemode == 3) {
            aiMode = 1;
        } else if (LocalScript.gamemode == 4) {
            if (playerNum == 1) {
                aiMode = 1;
            } else if (playerNum == 2) {
                aiMode = 2;
            }
        }
        if (aiMode == 1) {
            bool gonnaSearch = true;
            if (mode == 1) {
                if (gonnaSearch) {
                    yield return StartCoroutine(TryToHeal(result => {
                        gonnaSearch = result;
                    }));
                }
                if (gonnaSearch) {
                    List<int> toolOptions = ScanInvList(inventory, 5, 12, true, 0); //Powerup
                    if (toolOptions.Count > 0) {
                        yield return StartCoroutine(TryToUseTool(grid, toolOptions[0], result => {
                            gonnaSearch = result;
                        }));
                    }
                }
                if (gonnaSearch) {
                    if (lScript.RandomChanceFloat(0.5f)) { //arbitrary value
                        List<int> trapOptions = ScanInvList(inventory, 6, 0, true, 0);
                        if (trapOptions.Count > 0) {
                            yield return StartCoroutine(TryToUseTool(grid, trapOptions[0], result => {
                                gonnaSearch = result;
                            })); //I know trap's technically not a tool but
                        }
                    }
                }
            } else if (mode == 2) {
                int target = FindClosestTarget(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1])); //prob replace with findrandomtarget with closest enabled and then incorporate for that
                if (target == -1) {
                    Debug.Log("Got an error that shouldn't happen!");
                } else if (target == 0) {
                    if (gonnaSearch) {
                        List<int> visionWeapons = ScanInvList(inventory, 4, 0, true, 0);
                        float myElevation = lScript.FindElevation(grid[position - 1], 1);
                        if (visionWeapons.Count > 0 && myElevation >= 1) {
                            int itemToUse = visionWeapons[0];
                            int UAAT = 1;
                            yield return StartCoroutine(TryToUseWeapon(itemToUse, target, UAAT, null, result => {
                                gonnaSearch = result;
                            }));
                        } else {
                            gonnaSearch = true;
                        }
                    }
                    if (gonnaSearch) {
                        List<int> toolOptions = ScanInvList(inventory, 5, 11, true, 0); //UAV
                        toolOptions.AddRange(ScanInvList(inventory, 5, 5, true, 0)); //radar
                        toolOptions.AddRange(ScanInvList(inventory, 5, 8, true, 0)); //change location
                        toolOptions.AddRange(ScanInvList(inventory, 5, 7, true, 0)); //enhanced movement
                        toolOptions.AddRange(ScanInvList(inventory, 5, 1, true, 0)); //invis
                        toolOptions.AddRange(ScanInvList(inventory, 5, 2, true, 0)); //enhanced vision
                        toolOptions.AddRange(ScanInvList(inventory, 5, 6, true, 0)); //see health
                        if (toolOptions.Count > 0) {
                            yield return StartCoroutine(TryToUseTool(grid, toolOptions[0], result => {
                                gonnaSearch = result;
                            }));
                        }
                    }
                    if (gonnaSearch) {
                        yield return StartCoroutine(TryToHeal(result => {
                            gonnaSearch = result;
                        }));
                    }
                } else {
                    //check if any weapon is in range
                    float distToTarget = lScript.FindDistance(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]), lScript.GetCoordsFromIndex(knownPlayerPositions[target - 1]));
                    List<List<float>> GWAD = GetWeaponsAndDetonators(1, inventory, false, target, 0);
                    List<int> weapons = GWAD[0].ConvertAll(x => (int)x);
                    Debug.Log("BSU weapons count: " + weapons.Count);
                    List<float> FMDD = GWAD[1];
                    List<int> stunTools = ScanInvList(inventory, 5, 4, true, 0);
                    int itemToUse = 0;
                    for (int w = 0; w < weapons.Count; w++) {
                        int cooldown = botItemsOnCooldown[weapons[w] - 1]; //also could loop through all of weapons but I can implement that later
                        
                        if (cooldown != 0) {
                            for (int i = 0; i < stunTools.Count; i++) {
                                if (lScript.itemInfos[stunTools[i] - 1][2][3] == cooldown) {
                                    itemToUse = stunTools[i];
                                    break;
                                }
                            }
                        } else {
                            GWAD = GetWeaponsAndDetonators(1, inventory, true, target, 0);
                            weapons = GWAD[0].ConvertAll(x => (int)x);
                            FMDD = GWAD[1];
                            itemToUse = ScanInvInt(weapons, 1, 0, knownPlayerPositions[playerNum - 1], knownPlayerPositions[target - 1], grid); //1+
                            break;
                        }
                    }
                    Debug.Log("BSU itemToUse: " + itemToUse);
                    int UAAT = 1;
                    if (itemToUse == 0) {
                        if (gonnaSearch) {
                            yield return StartCoroutine(TryToHeal(result => {
                                gonnaSearch = result;
                            }));
                        }
                    } else {
                        if (gonnaSearch) {
                            yield return StartCoroutine(TryToUseWeapon(itemToUse, target, UAAT, FMDD, result => {
                                gonnaSearch = result;
                            }));
                        }
                    }
                }
            } else if (mode == 3) {
                if (gonnaSearch) {
                    //\F
                    List<int> toolOptions = ScanInvList(inventory, 5, 8, true, 0); //change location
                    toolOptions.AddRange(ScanInvList(inventory, 5, 1, true, 0)); //invis
                    toolOptions.AddRange(ScanInvList(inventory, 5, 3, true, 0)); //partial invis
                    if (toolOptions.Count > 0) {
                        yield return StartCoroutine(TryToUseTool(grid, toolOptions[0], result => {
                            gonnaSearch = result;
                        }));
                    }
                }
                if (gonnaSearch) {  
                    yield return StartCoroutine(TryToHeal(result => {
                        gonnaSearch = result;
                    }));
                }
            }
            /*if (gonnaSearch) {
                List<int> toolOptions = ScanInvList(inventory, 5, 1); 
                toolOptions.AddRange(ScanInvList(inventory, 5, 3));
                if (toolOptions.Count > 0) {
                    yield return StartCoroutine(TryToUseTool(grid, toolOptions[0], result => {
                        gonnaSearch = result;
                    }));
                }
            }*/
            /*if (gonnaSearch) {
                List<int> toolOptions = ScanInvList(inventory, 5, 6);
                if (toolOptions.Count > 0) {
                    yield return StartCoroutine(TryToUseTool(grid, toolOptions[0], result => {
                        gonnaSearch = result;
                    }));
                }
            }*/
            if (gonnaSearch) {
                extraSearches = 0;
                extraSearches += Mathf.RoundToInt(lScript.FindHighestBookEffectAmt(botBookInventory, 3));
                yield return StartCoroutine(Search(grid));
            } else {
                yield return StartCoroutine(UpdateMode(1));
            }
        } else if (aiMode == 2) {
            float[,,] cnnInput = botBrain.GenerateCNNInput(width, height, elevationData, hideData, rarityData, GetKnownEnemyPositions(1), GetCombinedHeatmap(GetEnemyHeatmaps()), botAirdrops);
            List<int> invSnapshot = new List<int>(inventory);
            List<int> g = new List<int>();
            foreach (int i in grid) {
                g.Add(i);
            }
            features = null;
            List<float> modeSnapshot = new List<float>(allPlayerModes[playerNum - 1]);
            int itemToUse = botBrain.PickBestActionNN(ScanInvList(inventory, 8, 0, true, 0), g);
            bool gonnaSearch = true;
            if (itemToUse == 0) {
                gonnaSearch = true;
            } else {
                yield return StartCoroutine(TryToUseItem(itemToUse, grid, result => {
                    gonnaSearch = result;
                }));
                
            }
            
            if (gonnaSearch) {
                botBrain.AddToActionHistory(cnnInput, position - 1, new List<int>{1}, modeSnapshot, 0, invSnapshot, null); //should position - 1 be before or after? Let's see how it matters
                extraSearches = 0;
                extraSearches += Mathf.RoundToInt(lScript.FindHighestBookEffectAmt(botBookInventory, 3));
                yield return StartCoroutine(Search(grid));
            } else {
                botBrain.AddToActionHistory(cnnInput, position - 1, new List<int>{2}, modeSnapshot, itemToUse, invSnapshot, features); //should position - 1 be before or after? Let's see how it matters
                yield return StartCoroutine(UpdateMode(1));
            }
        }
    }
    IEnumerator TryToUseItem(int itemToUse, NetworkList<int> grid, System.Action<bool> callback) {
        //itemToUse is 1+
        List<int> itemClass = lScript.itemIntLists[itemToUse - 1][0];
        bool weapon = itemClass.Contains(1);
        bool healing = itemClass.Contains(2);
        bool tool = itemClass.Contains(3);
        bool armor = itemClass.Contains(4);
        bool gonnaSearch = true;
        int target = FindRandomTarget(1, true);
        if (weapon && !tool) {
            int UAAT = 1;
            List<float> FMDD = null; //don't forget this
            yield return StartCoroutine(TryToUseWeapon(itemToUse, target, UAAT, FMDD, result => {
                if (!result) {
                    gonnaSearch = result;
                }
            }));
        }
        if (healing) {
            
            List<(int item, int UAAT, int rating)> potentials = new List<(int item, int UAAT, int rating)>();
            GetUAATRating(inventory, itemToUse, potentials, HP, maxHP);
            List<int> GMH = GetMaxHealing(potentials);
            if (GMH == null) {
                Debug.Log("Error: GMH is null! potentials is probably empty!");
            }
            if (GMH[0] != itemToUse) {
                Debug.Log("Error: GMH[0] doesn't match with itemToUse!");
            }
            int uaat = GMH[1];
            yield return StartCoroutine(TryToUseHealing(itemToUse, uaat, result => {
                if (result) {
                    //gonnaSearch = result; //don't set gonnaSearch if it's true, only if it's false
                } else {
                    gonnaSearch = result;
                }
            }));
        }
        if (tool) {
            yield return StartCoroutine(TryToUseTool(grid, itemToUse, result => {
                if (!result) {
                    gonnaSearch = result;
                }
            }));
        }
        callback(gonnaSearch);
        yield return null;
    }
    List<List<float>> GetWeaponsAndDetonators(int mode, List<int> inv, bool checkCooldown, int target, float dist) {
        List<int> weapons = new List<int>();
        if (mode == 1) {
            weapons = ScanInvList(inv, 2, 0, checkCooldown, 0);
        } else if (mode == 2) {
            weapons = ScanInvList(inv, 7, 0, checkCooldown, dist);
        }
        List<int> detonators = ScanInvList(inv, 5, 9, checkCooldown, 0); //since it's findclosesttarget a closer target could prevent bot from using detonator on a farther one (if out of blast radius)
        List<float> FMDD = new List<float>();
        if (target != 0) {
            if (detonators.Count > 0) {
                FMDD = FindMaxDetonableDamage(target);
                //int index2 = Mathf.RoundToInt(FMDD[2]); //0+
                int i = 0;
                while (i < weapons.Count) {
                    if (FMDD[0] > GetExpectedDamage(weapons[i] - 1)) {
                        break;
                    } else {
                        i++;
                    }
                }
                if (Mathf.RoundToInt(FMDD[1]) != -1) {
                    weapons.Insert(i, detonators[0]); //detonators[0] can be looked at
                }
            }
        }
        return new List<List<float>>{weapons.ConvertAll(x => (float)x), FMDD};
    }
    IEnumerator TryToUseWeapon(int itemToUse, int target, int UAAT, List<float> FMDD, System.Action<bool> callback) {
        bool gonnaSearch = true;
        BD = 0;
        Vector2 targetPos = new Vector2();
        if (target > 0) { //this conditional could be standardized, but let's see if we really would need to
            targetPos = new Vector2(lScript.GRIDX[knownPlayerPositions[target - 1] - 1], lScript.GRIDY[knownPlayerPositions[target - 1] - 1]); //\I
            Debug.Log("targetPos set");
        } else {
            int target2 = FindRandomTarget(2, false);
            if (target2 == 0) {
                //choose random square
                int count = 0; //count squares you can't see
                for (int i = 0; i < myVG.Count; i++) {
                    if (myVG[i] == 0) {
                        count++;
                    }
                }
                int tIndex = UnityEngine.Random.Range(0, lScript.GRID.Count);
                if (count > 0) {
                    while (myVG[tIndex] != 0) { //choose a square you can't see
                        tIndex = UnityEngine.Random.Range(0, lScript.GRID.Count);
                    }
                }
                targetPos = new Vector2(lScript.GRIDX[tIndex], lScript.GRIDY[tIndex]);
            } else {
                List<bool> PPP = potentialPlayerPositions[target2 - 1];
                int count = 0; 
                for (int i = 0; i < myVG.Count; i++) {
                    if (myVG[i] == 0 && PPP[i]) {
                        count++;
                    } 
                }
                int tIndex = UnityEngine.Random.Range(0, lScript.GRID.Count);
                if (count > 0) {
                    while (!(myVG[tIndex] == 0 && PPP[tIndex])) {
                        tIndex = UnityEngine.Random.Range(0, lScript.GRID.Count);
                    }
                } else {
                    int count2 = 0;
                    for (int i = 0; i < myVG.Count; i++) {
                        if (myVG[i] == 0) {
                            count2++;
                        }
                    }
                    if (count2 > 0) {
                        while (myVG[tIndex] != 0) {
                            tIndex = UnityEngine.Random.Range(0, lScript.GRID.Count);
                        }
                    }
                }
                targetPos = new Vector2(lScript.GRIDX[tIndex], lScript.GRIDY[tIndex]);
            }
            
        }
        if (targetPos == Vector2.zero) {
            Debug.Log("targetPos is perfectly (0, 0)! (sus)");
        }
        if (inventory.Contains(itemToUse) && botItemsOnCooldown[itemToUse - 1] == 0) {
            Debug.Log("About to use " + itemToUse);
            yield return StartCoroutine(lScript.UseItem(itemToUse - 1, myActions, myBDD, myTI, mySI, 2, this, position, playerNum, targetPos, target, UAAT, FMDD, result => {
                if (result) {
                    gonnaSearch = false;
                    //Debug.Log("Used item");
                } else {
                    gonnaSearch = true;
                }
            }));
        }
        callback(gonnaSearch);
    }
    public Vector2? ChangeTarget(int prevTarget) {
        if (prevTarget == 0) {
            int retTarget = FindClosestTarget(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1])); //this will cause the closest to be a magnet instead of finding one that actually works
            if (retTarget == 0) {
                return null;
            } else {
                return new Vector2(lScript.GRIDX[knownPlayerPositions[retTarget - 1] - 1], lScript.GRIDY[knownPlayerPositions[retTarget - 1] - 1]); //\I
            }
        }
        return null;
    }
    IEnumerator TryToHeal(System.Action<bool> callback) {
        int itemToUse = 0;
        int UAAT = 0;
        bool gonnaSearch = true;
        if (HP < maxHP) {
            //Debug.Log("Trying to heal");
            List<int> FBH = FindBestHealing(1, inventory, HP, maxHP);
            if (FBH != null) {
                    
                itemToUse = FBH[0];
                UAAT = FBH[1];
                //Debug.Log("Found healing item: " + itemToUse);
            }
        }
        if (itemToUse == 0) {
            gonnaSearch = true;
        } else {
            yield return StartCoroutine(TryToUseHealing(itemToUse, UAAT, result => {
                gonnaSearch = result;
            }));
            if (!gonnaSearch) {
                yield return StartCoroutine(UpdateMode(2));
            }
        }
        callback(gonnaSearch);
    }
    IEnumerator TryToUseHealing(int itemToUse, int UAAT, System.Action<bool> callback) {
        bool gonnaSearch = true;
        BD = 0;
        if (inventory.Contains(itemToUse) && botItemsOnCooldown[itemToUse - 1] == 0) {
            yield return StartCoroutine(lScript.UseItem(itemToUse - 1, myActions, myBDD, myTI, mySI, 2, this, position, playerNum, new Vector2(), 0, UAAT, null, result => {
                if (result) {
                    gonnaSearch = false;
                    //Debug.Log("Used item");
                } else {
                    gonnaSearch = true;
                }
            }));
        }
        callback(gonnaSearch);
    }
    IEnumerator TryToUseTool(NetworkList<int> g, int itemToUse, System.Action<bool> callback) {
        bool gonnaSearch = true;
        int UAAT = 0;
        UAAT = 1;
        Vector2 targetPos = new Vector2();
        if (lScript.itemIntLists[itemToUse - 1][0].Contains(1)) {
            List<int> grid = new List<int>();
            List<bool> a = new List<bool>(); //simply those in range to save resources
            float range = lScript.itemInfos[itemToUse - 1][0][0];
            for (int i = 0; i < g.Count; i++) {
                grid.Add(g[i]);
                if (lScript.FindDistance(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]), lScript.GetCoordsFromIndex(i + 1)) <= range) {
                    a.Add(true);
                } else {
                    a.Add(false);
                }
            }
            if (lScript.itemIntLists[itemToUse - 1][4].Contains(3)) {
                targetPos = GetRandomPosWithinXOfMe(grid, lScript.itemInfos[itemToUse - 1][0][9]);
            } else if (lScript.itemIntLists[itemToUse - 1][4].Contains(8)) {
                
                int targetIndex = -1;
                if (mode == 2) {
                    int target = FindRandomTarget(1, false); //target is 1+
                    if (target == 0) {
                        //implement target2
                        int target2 = FindRandomTarget(2, false);
                        if (target2 == 0) {
                            if (currentQuadrant != targetQuadrant) {
                                targetIndex = FindLowestDistance(1, a, GetCornerCoords(targetQuadrant), null);
                            } else {
                                /*if (timeSpentInTQ <= 3) { //arbitrary value
                                    aIndex = FindHighestElevation(a, grid);
                                } else {
                                    aIndex = FindLowestDistance(a, GetCornerCoords(targetQuadrant));
                                }*/
                                targetIndex = FindHighestDistance(1, a, lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]), null);
                            }
                        } else {
                            targetIndex = FindLowestDistance(2, a, new Vector2(), potentialPlayerPositions[target2 - 1]);
                        }
                    } else {
                        int ppos = knownPlayerPositions[target - 1];
                        if (ppos == 0) {
                            ppos = lastKnownPlayerPositions[target - 1];
                        }
                        targetIndex = FindLowestDistance(1, a, lScript.GetCoordsFromIndex(ppos), null);
                    }
                } else if (mode == 3) {
                    targetIndex = RunAway(a, grid);
                }
                targetPos = new Vector2(lScript.GRIDX[targetIndex], lScript.GRIDY[targetIndex]);
            } else if (lScript.itemIntLists[itemToUse - 1][1].Contains(13)) {
                int targetIndex = -1;
                if (lScript.RandomChanceFloat(0.5f)) { //arbitrary value
                    targetIndex = FindHighestElevation(a, grid);
                } else {
                    targetIndex = FindHighestRarity(a, grid);
                }
                targetPos = new Vector2(lScript.GRIDX[targetIndex], lScript.GRIDY[targetIndex]);
            }
        }
        if (inventory.Contains(itemToUse) && botItemsOnCooldown[itemToUse - 1] == 0) {
            BD = 0;
            yield return StartCoroutine(lScript.UseItem(itemToUse - 1, myActions, myBDD, myTI, mySI, 2, this, position, playerNum, targetPos, 0, UAAT, null, result => {
                if (result) {
                    gonnaSearch = false;
                } else {
                    gonnaSearch = true;
                }
            }));
        }
        callback(gonnaSearch);
    }
    Vector2 GetRandomPosWithinXOfMe(List<int> grid, float range) { //returns in screen units
        List<int> workingIndices = new List<int>();
        List<bool> workingIndicesBinary = new List<bool>();
        for (int i = 0; i < grid.Count; i++) {
            if (lScript.FindDistance(lScript.GetCoordsFromIndex(position), lScript.GetCoordsFromIndex(i + 1)) <= range) {
                workingIndices.Add(i);
                workingIndicesBinary.Add(true);
            } else {
                workingIndicesBinary.Add(false);
            }
        }
        //int index = FindHighestDistance(1, workingIndicesBinary, lScript.GetCoordsFromIndex(position), null);
        int index = -1;
        Vector2 retV2 = new Vector2(lScript.GRIDX[position - 1], lScript.GRIDY[position - 1]);
        if (workingIndices.Count > 0) {
            index = workingIndices[UnityEngine.Random.Range(0, workingIndices.Count)];
        }
        if (index >= 0) {
            retV2 = new Vector2(lScript.GRIDX[index], lScript.GRIDY[index]);
        } else if (index == -1) {
            //Debug.Log("Throwing smoke to myself");
        }
        //return lScript.GetCoordsFromIndex(index + 1);
        //Debug.Log("Bot smoke aiming for (" + lScript.GetCoordsFromIndex(index + 1).x + ", " + lScript.GetCoordsFromIndex(index + 1).y + ")");
        return retV2;
    }
    public IEnumerator UpdateIsSmoked(List<float> s) {
        botIsSmoked = lScript.SmokeList(s);
        yield return null;
    }
    List<int> SortByRarity(List<int> ASY) { //greatest rarity at [0]
        List<int> retList = new List<int>();
        foreach (int SY in ASY) {
            int i = 0; 
            while (i < retList.Count) {
                if (lScript.itemInts[SY - 1][0] > lScript.itemInts[retList[i] - 1][0]) {
                    break;
                } else {
                    i++;
                }
            }
            retList.Insert(i, SY);
        }
        return retList;
    }
    int extraSearches;
    List<int> airdropSearchYields = new List<int>();
    public IEnumerator Search(NetworkList<int> grid) {
        int landRarity = lScript.GetLandRarity(grid[position - 1]);
        landRarity += lScript.GetLuck(botEffects, botEffectStrengths);
        int searchYield = 0;
        bool isAirdrop = botAirdrops.Contains(position);
        int ASYIndex = 0;
        if (isAirdrop) { //still gotta implement bot choosing from airdrop
            myActions[0] = playerNum;
            myActions[28] = position;
            int index = lScript.FindInIntList(botAirdrops, position);
            botAirdrops.RemoveAt(index);
            myActions[30] = botAirdrops.Count;
            landRarity = 9; //9 \D
            airdropSearchYields.Clear();
            for (int i = 0; i < 3; i++) {
                airdropSearchYields.Add(lScript.GetSearchYield(landRarity, grid[position - 1], lScript.GetViableItems(inventory, grid[position - 1], botEffects, true, airdropSearchYields, new List<int>())));
            }
            airdropSearchYields = SortByRarity(new List<int>(airdropSearchYields));
            searchYield = airdropSearchYields[ASYIndex];
            if (airdropSearchYields != null) {
                Debug.Log("airdropSearchYields is NOT null");
            }
        } else {
            searchYield = lScript.GetSearchYield(landRarity, grid[position - 1], lScript.GetViableItems(inventory, grid[position - 1], botEffects, false, null, new List<int>())); //supposed to be bot's effects and effect strengths
        }
        
        bool dontConclude = false;
        //bool keep = true;
        if (isAirdrop) {
            yield return StartCoroutine(AcceptSearch(searchYield, grid, true, result => {
                if (result) {
                    dontConclude = true;
                } else {
                    dontConclude = false;
                }
            }));
            if (airdropSearchYields == null) {
                Debug.Log("airdropSearchYields is null!");
            }
            while (dontConclude && (ASYIndex + 1 < airdropSearchYields.Count)) {
                ASYIndex++; 
                searchYield = airdropSearchYields[ASYIndex];
                yield return StartCoroutine(AcceptSearch(searchYield, grid, true, result => {
                    if (result) {
                        dontConclude = true;
                    } else {
                        dontConclude = false;
                    }
                }));
            }
            if (dontConclude) {
                dontConclude = false;
            }
        } else {
            yield return StartCoroutine(AcceptSearch(searchYield, grid, false, result => {
                if (result) {
                    dontConclude = true;
                    StartCoroutine(Search(grid));
                }
            }));
        }
        if (!dontConclude) {
            if (extraSearches != 0) {
                extraSearches--;
                if (extraSearches < 0) {
                    extraSearches = 0;
                }
                StartCoroutine(Search(grid));
            }
        }
        yield return null;
    }
    IEnumerator AcceptSearch(int searchYield, NetworkList<int> grid, bool isAirdrop, System.Action<bool> callback) {
        bool dontConclude = false;
        bool keep = true;
        /*if (isAirdrop) { //purely for testing
            keep = false;
        }*/
        if (lScript.itemIntLists[searchYield - 1][0].Contains(4)) {
            if (lScript.InventoryContainsArmor(inventory, Mathf.RoundToInt(lScript.itemInfos[searchYield - 1][3][0]))) {
                //don't forget to remove from inventory if keep == true
                keep = false;
            }
        }
        if (lScript.IsPermanentWeapon(searchYield - 1)) {
            if (lScript.CountPermWeaponsInInv(inventory) + lScript.itemInts[searchYield - 1][6] - lScript.CountNegativePermWeaponsInIntList(lScript.itemIntLists[searchYield - 1][6]) > botMaxWeapons) {
                //don't forget to choose which weapon to remove if keep == true
                keep = false;
            }
        }
        if (keep) {
            if (lScript.itemInts[searchYield - 1][6] == 0) {
                if (!lScript.itemBools[searchYield - 1][0] && lScript.itemIntLists[searchYield - 1][0].Contains(3) && lScript.itemInfos[searchYield - 1][2].Count > 0) {
                    if (lScript.itemInfos[searchYield - 1][2][0] == 1) {
                        yield return StartCoroutine(lScript.AddToEffects(searchYield - 1, botEffects, botEffectLengths, botEffectStrengths));
                    } else {
                        if (lScript.itemInfos[searchYield - 1][2][7] == 2) {
                            yield return StartCoroutine(lScript.SearchBook(2, searchYield - 1, 2, this));
                        }
                    }
                }
            }
            for (int i = 0; i < lScript.itemInts[searchYield - 1][6]; i++) {
                if (lScript.itemInts[searchYield - 1][3] == -1 || lScript.CountInIntList(inventory, searchYield) + 1 <= lScript.itemInts[searchYield - 1][3]) {
                    inventory.Add(searchYield);
                    if (!lScript.itemBools[searchYield - 1][0] && lScript.itemIntLists[searchYield - 1][0].Contains(3) && lScript.itemInfos[searchYield - 1][2].Count > 0) {
                        yield return StartCoroutine(lScript.AddToEffects(searchYield - 1, botEffects, botEffectLengths, botEffectStrengths));
                    }
                }
            }
            List<int> AAU = lScript.itemIntLists[searchYield - 1][6];
            foreach (int a in AAU) {
                if (a < 0) {
                    inventory.Remove(-a);
                }
            }
            armorCoefficient = lScript.GetArmorCoefficient(inventory);
            yield return StartCoroutine(UpdateMode(1));
            
        } else {
            /*if (!isAirdrop) {
                StartCoroutine(Search(grid));
            }*/
            dontConclude = true;
        }
        callback(dontConclude);
    }
    public IEnumerator BotVision() {
        float addVision = 0;
        if (!lScript)
            Debug.Log("lScript not assigned for vision!");
        myVG = lScript.GetVisionList(addVision, lScript.smokes, 2, this, botEffects, botEffectStrengths, inventory, position); //why we using lScript's smokes when we could use botSmokes
        Debug.Log("After botvision");
        /*yield return StartCoroutine(lScript.GetVisionList(addVision, lScript.smokes, 2, this, new List<int>{}, new List<int>{}, result =>{ 
            myVG = result;
        }));*/
        //lScript.DisplayBotTest(2, null, myVG);
        yield return null;
    }
    public IEnumerator BotVisionHouse(bool playerMoved, int pnum) {
        
        yield return StartCoroutine(BotVision());
        //Debug.Log("Bot vision house");
        BotEnemiesVisible(myVG, lScript.playerPositionList);
        if (playerMoved) {
            //lScript.ClearTestFolder();
            if (knownPlayerPositions[pnum - 1] == 0) {
                if (lastKnownPlayerPositions[pnum - 1] != 0) { //to save resources
                    potentialMovesSinceLastSeen[pnum - 1].Add(7); //guess moves here but just use 7 as a placeholder for now
                    CreatePPP(pnum);
                    if (potentialMovesSinceLastSeen[pnum - 1].Count == 1) {
                        AdjustPlayerMode(pnum, 3, 0.1f);
                    }
                }
            }
        }
        for (int i = 0; i < knownPlayerPositions.Count; i++) {
            if (i + 1 != playerNum) {
                UpdatePlayerVG(i + 1);
            }
        }
        bool mysteriesResolved = false;
        for (int i = 0; i < myVG.Count; i++) {
            if (myVG[i] == 1) {
                if (botMysteryBuildings.Contains(i)) {
                    botMysteryBuildings.Remove(i);
                    mysteriesResolved = true;
                }
            }
        }
        List<int> grid = new List<int>();
        foreach (int g in lScript.GRID) {
            grid.Add(g);
        }
        if (mysteriesResolved) {
            InitializeData(new List<int>{3}, grid);
        }
        yield return null;
    }
    void UpdatePlayerVG(int pnum) {
        Debug.Log("potentialMovesSinceLastSeen count = " + potentialMovesSinceLastSeen.Count);
        if (lastKnownPlayerPositions[pnum - 1] != 0 && potentialMovesSinceLastSeen[pnum - 1].Count == 0) { //(potentialMovesSinceLastSeen[pnum - 1].Count == 0 || potentialMovesSinceLastSeen[pnum - 1] adds up to 0/the only element is 0)
            potentialPlayerVGs[pnum - 1] = lScript.GetVisionList(0, botSmokes, 3, this, new List<int>{}, new List<int>{}, potentialPlayerInvs[pnum - 1], lastKnownPlayerPositions[pnum - 1]); //TODO: new List<int>{}s represent what the bot believes to be the player's effects and effectStrengths
            /*List<bool> testList = new List<bool>();
            foreach (int i in potentialPlayerVGs[pnum - 1]) {
                testList.Add(i != 0);
            }
            lScript.DisplayBotTest(1, testList, null);*/
        } else {
            potentialPlayerVGs[pnum - 1] = null;
        }
        if (lastKnownPlayerPositions[pnum - 1] != 0) {
            compoundPlayerVGs[pnum - 1] = GetCompoundVisionList(pnum, allPlayerDataLists[pnum - 1]);
        } else {
            compoundPlayerVGs[pnum - 1] = null;
        }
    }
    List<float> GetCompoundVisionList(int pnum, List<float> PDL) {
        List<float> retList = new List<float>();
        foreach (int item in lScript.GRID) {
            retList.Add(0);
        }
        List<bool> PPP = potentialPlayerPositions[pnum - 1];
        for (int i = 0; i < PPP.Count; i++) {
            if (PPP[i]) {
                List<int> VL = lScript.GetVisionList(0, botSmokes, 3, this, new List<int>{}, new List<int>{}, potentialPlayerInvs[pnum - 1], i + 1); //TODO: new List<int>{}s represent what the bot believes to be the player's effects and effectStrengths
                for (int j = 0; j < VL.Count; j++) {
                    if (VL[j] != 0) {
                        retList[j] += PDL[i]; //weight here
                    }
                }
            }
        }
        return retList;
    }
    void CreatePPP(int pnum) {
        List<int> grid = new List<int>();
        List<bool> a = new List<bool>();
        foreach (int item in lScript.GRID) {
            grid.Add(item);
            a.Add(false);
        }
        if (lastKnownPlayerPositions[pnum - 1] == 0) { //when last known player position is 0, the possible positions has the whole map being true
            for (int i = 0; i < a.Count; i++) {
                a[i] = true;
            }
        } else {
            if (potentialMovesSinceLastSeen[pnum - 1].Count == 0) {
                a[lastKnownPlayerPositions[pnum - 1] - 1] = true;
            } else {
                CreateArrivable(grid, lastKnownPlayerPositions[pnum - 1], a, new List<(int index, float moves, int LMIndex)>(), new List<(int index, float moves, int LMIndex)>(), potentialMovesSinceLastSeen[pnum - 1]);
            }
        }
        potentialPlayerPositions[pnum - 1] = a;
        if (!a.Contains(false)) {
            lastKnownPlayerPositions[pnum - 1] = 0;
            potentialMovesSinceLastSeen[pnum - 1].Clear();
        }
        //lScript.DisplayBotTest(1, a, null);
    }
    public void BotEnemiesVisible(List<int> MVG, List<int> PPL) {
        for (int i = 0; i < PPL.Count; i++) {
            //Debug.Log("MVG length: " + MVG.Count);
            //Debug.Log("PPL length: " + PPL.Count);
            if (i + 1 != playerNum) {
                bool canSee = false;
                bool invis = false;
                if (i + 1 == lScript.myPlayerNum) {
                    invis = lScript.myEffects.Contains(1);
                } else {
                    invis = lScript.enemyEffects[i].Contains(1);
                }
                //Debug.Log("myVG count = " + myVG.Count);
                //Debug.Log()
                if (lScript.DecideEnemyVisible(myVG, PPL[i], invis)) {
                    knownPlayerPositions[i] = PPL[i];
                    lastKnownPlayerPositions[i] = PPL[i];
                    potentialMovesSinceLastSeen[i].Clear();
                    CreatePPP(i + 1);
                } else {
                    if (UAVPlayerPositions[i] != 0) {
                        knownPlayerPositions[i] = UAVPlayerPositions[i];
                        lastKnownPlayerPositions[i] = UAVPlayerPositions[i];
                        potentialMovesSinceLastSeen[i].Clear();
                        CreatePPP(i + 1);
                    } else {
                        knownPlayerPositions[i] = 0;
                        //if (possible enemy positions covers the whole map) {
                            //lastKnownPlayerPositions[i] = 0;
                        //}
                        //when deciding to add move amounts to potentialMovesSinceLastSeen, maybe don't add if it's 0 (but think more about whether or not a fully covered map 100% will lead to another fully covered map after any amount of moves)
                        //if you don't add when it's 0 you're saying no matter what, 0 goes to 0 meaning full map coverage always goes to full map coverage
                    }
                }
                
            }
        }
    }
    public void InitializeMysteryBuildings(NetworkList<int> grid) {
        for (int i = 0; i < grid.Count; i++) {
            if (grid[i] == 6 || grid[i] == 7 || grid[i] == 8) {
                botMysteryBuildings.Add(i);
            }
        }
    }
}