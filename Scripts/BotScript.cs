using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BotScript : MonoBehaviour
{
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
    public List<float> myBDD;
    public List<int> myTI;
    public LocalScript lScript;
    public int mode;
    /* 1 = "Searching"
       2 = "Hunting"
       3 = "Hiding"
    */
    public float armorCoefficient = 1;
    public List<int> inventory = new List<int>();
    [SerializeField]
    public List<int> myVG = new List<int>();
    public List<int> knownPlayerPositions = new List<int>(); //1+
    [SerializeField]
    List<int> botMysteryBuildings = new List<int>(); //0+
    public List<int> botItemsOnCooldown = new List<int>();
    public List<bool> botCDJA = new List<bool>();
    public List<List<List<float>>> botTraps = new List<List<List<float>>>();
    public int myFrozenTurns;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateMyPositionInList() {
        knownPlayerPositions[playerNum - 1] = position;
        Vector2 myPosV2 = lScript.GetCoordsFromIndex(position);
        int myX = Mathf.RoundToInt(myPosV2.x); //1+
        int myY = Mathf.RoundToInt(myPosV2.y); //1+
        int xBound = Mathf.FloorToInt(lScript.columns.Value / 2) + 1;
        int yBound = Mathf.FloorToInt(lScript.rows.Value / 2) + 1;
        if (myX >= xBound && myY >= yBound) {
            currentQuadrant = 1;
        } else if (myX < xBound && myY >= yBound) {
            currentQuadrant = 2;
        } else if (myX < xBound && myY < yBound) {
            currentQuadrant = 3;
        } else if (myX >= xBound && myY < yBound) {
            currentQuadrant = 4;
        }
    }
    public IEnumerator UpdateMode(int type) {
        if (type == 1) {
            if (ScanInvList(inventory, 1).Count > 0) {
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
        yield return null;
    }
    List<int> ScanInvList(List<int> inv, int type) {
        List<int> retList = new List<int>();
        if (type == 1 || type == 2 || type == 4) {
            foreach (int item in inv) {
                int index = item - 1;
                if (lScript.itemIntLists[index][0].Contains(1)) {
                    if (type == 1) {
                        retList.Add(item);
                    } else if (type == 2) {
                        if (botItemsOnCooldown[index] == 0) {
                            retList.Add(item);
                        }
                    } else if (type == 4) {
                        if (botItemsOnCooldown[index] == 0) {
                            if (lScript.itemInfos[index][0][8] > 0) {
                                retList.Add(item);
                            }
                        }
                    }
                }
            }
            if (type == 2 || type == 4) {
                for (int i = 0; i < retList.Count - 1; i++) {
                    float currentDamage = lScript.itemInfos[retList[i] - 1][0][1] * Mathf.RoundToInt(lScript.itemInfos[retList[i] - 1][0][3]);
                    float maxDamage = currentDamage;
                    int swapIndex = i;
                    for (int j = i + 1; j < retList.Count; j++) {
                        float newDamage = lScript.itemInfos[retList[j] - 1][0][1] * Mathf.RoundToInt(lScript.itemInfos[retList[j] - 1][0][3]);
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
                int index = item - 1;
                if (lScript.itemIntLists[index][0].Contains(2)) {
                    if (botItemsOnCooldown[index] == 0) {
                        retList.Add(item);
                    }
                }
            }
        }
        return retList;
    }
    int ScanInvInt(List<int> inv, int type, float dist, int search) {
        int returnVal = 0;
        if (type == 1) {
            for (int i = 0; i < inv.Count; i++) {
                if (dist <= lScript.itemInfos[inv[i] - 1][0][0]) {
                    returnVal = inv[i];
                    break;
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
    public IEnumerator TakeDamage(float[] EA, float[] EB, int[] ETI) {
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
            yield return StartCoroutine(lScript.ExplodeBomb(Mathf.RoundToInt(EA[25]) + 1, currentTrap[3], currentTrap[2], Mathf.RoundToInt(EA[0]), 1, 1, false, 3, this));
            botTraps[Mathf.RoundToInt(EA[25])].RemoveAt(Mathf.RoundToInt(EA[26]));
        }
        Vector2 EV = new Vector2(lScript.GRIDX[enemyPosition - 1], lScript.GRIDY[enemyPosition - 1]);
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
            StartCoroutine(lScript.BulletMove(Mathf.RoundToInt(EA[1]) - 1, bul, angleDeg, EA[6], EA[5], EA[7], enemyPosition, Mathf.RoundToInt(EA[0]), 3, EA[4], Mathf.RoundToInt(EA[9]), Mathf.RoundToInt(EA[10]), EA[11], EA[12], EA[13], EA[14], Mathf.RoundToInt(EA[15]), Mathf.RoundToInt(EA[16]), Mathf.RoundToInt(EA[17]), Mathf.RoundToInt(EA[18]), Mathf.RoundToInt(EA[19]), Mathf.RoundToInt(EA[20]), trapI, Mathf.RoundToInt(EA[27]), false, this));
            //Debug.Log("bulmove2");
            yield return new WaitForSeconds(EA[3]);
        }
        while (BD != EA[2]) {
            yield return null;
        }
        HP = Mathf.RoundToInt(HPCalculation);
        //HP -= Mathf.RoundToInt(myLD * this.armorCoefficient);
        lScript.HealthsServerRpc(playerNum, HP, maxHP, false, true, 1);
        //yield return StartCoroutine(UpdateMode(2));
        yield return null;
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
    List<(int index, float moves)> yetToCheck = new List<(int index, float moves)>(); //index is 0+
    List<(int index, float moves)> alreadyChecked = new List<(int index, float moves)>(); //index is 0+
    List<bool> arrivable;
    public IEnumerator MoveSomewhere(NetworkList<int> g, float startMoves) {
        List<int> grid = new List<int>();
        arrivable = new List<bool>();
        foreach (int item in g) {
            grid.Add(item);
            arrivable.Add(false);
        }

        yetToCheck.Clear();
        alreadyChecked.Clear();
        yetToCheck.Add((index: position - 1, moves: startMoves));
        while (yetToCheck.Count > 0) {
            yield return StartCoroutine(CheckNeighbors(grid));
        }
        int aIndex;
        if (mode == 1) {
            aIndex = FindHighestRarity(arrivable, grid);
        } else if (mode == 2) { 
            int target = FindRandomTarget(); //target is 1+
            if (target == 0) {
                if (currentQuadrant != targetQuadrant) {
                    aIndex = FindLowestDistance(arrivable, GetCornerCoords(targetQuadrant));
                } else {
                    if (timeSpentInTQ <= 3) { //arbitrary value
                        aIndex = FindHighestElevation(arrivable, grid);
                    } else {
                        aIndex = FindLowestDistance(arrivable, GetCornerCoords(targetQuadrant));
                    }
                }
            } else {
                aIndex = FindLowestDistance(arrivable, lScript.GetCoordsFromIndex(knownPlayerPositions[target - 1]));
            }
        } else if (mode == 3) {
            int target = FindRandomTarget(); //target is 1+
            if (target == 0) {
                aIndex = FindCave(arrivable, grid);
                if (aIndex == -1) {
                    aIndex = GetRandomIndex(arrivable);
                }
            } else {
                aIndex = FindHighestDistance(arrivable, lScript.GetCoordsFromIndex(knownPlayerPositions[target - 1]));
            }
        } else {
            aIndex = GetRandomIndex(arrivable);
        }
        if (aIndex + 1 == 0) {
            Debug.Log("Why am I setting position to 0");
        }

        position = aIndex + 1;
        //position++;

        //lScript.DisplayBotTest(1, arrivable, null);
        yield return null;
    }
    public IEnumerator UpdateTargetQuadrant() {
        if (mode == 2) {
            int target = FindRandomTarget();
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
    int GetRandomIndex(List<bool> a) {
        int returnVal = UnityEngine.Random.Range(0, a.Count);
        while (!a[returnVal]) {
            returnVal = UnityEngine.Random.Range(0, a.Count);
        }
        return returnVal;
    }
    int FindRandomTarget() {
        List<int> choices = new List<int>();
        for (int i = 0; i < knownPlayerPositions.Count; i++) {
            if (i + 1 != playerNum) {
                if (knownPlayerPositions[i] != 0) {
                    choices.Add(i + 1);
                }
            }
        }
        if (choices.Count > 0) {
            return choices[UnityEngine.Random.Range(0, choices.Count)];
        } else {
            return 0;
        }
    }
    int FindClosestTarget(Vector2 myCoords) {
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
    public int indexHelper;
    public float movesHelper;
    public int workedHelper;
    IEnumerator CheckNeighbors(List<int> grid) {
        var gridSquare = yetToCheck[0];
        int idx = gridSquare.index;
        float mvs = gridSquare.moves;
        //Vector2 coords = lScript.GetCoordsFromIndex(idx + 1);
        yield return StartCoroutine(CheckNeighbor(0, 1, mvs, idx + 1, grid.Count));
        yield return StartCoroutine(CheckNeighbor(0, -1, mvs, idx + 1, grid.Count));
        yield return StartCoroutine(CheckNeighbor(1, 0, mvs, idx + 1, grid.Count));
        yield return StartCoroutine(CheckNeighbor(-1, 0, mvs, idx + 1, grid.Count));
        yetToCheck.RemoveAt(0);
        alreadyChecked.Add(gridSquare);
        yield return null;
    }
    IEnumerator CheckNeighbor(int x, int y, float mvs, int PP, int gridCount) {
        indexHelper = -1;
        movesHelper = 0;
        workedHelper = -1;
        yield return StartCoroutine(lScript.ChangePosition(x, y, 2, mvs, PP, gridCount, this));
        while (workedHelper == -1) {
            yield return null;
        }
        if (workedHelper == 1) {
            var newGridSquare = (index: indexHelper, moves: movesHelper);
            if (newGridSquare.moves <= 0) {
                arrivable[newGridSquare.index] = true;
            } else {
                if (yetToCheck.Contains(newGridSquare) || alreadyChecked.Contains(newGridSquare)) {
                    //don't add
                } else {
                    yetToCheck.Add(newGridSquare);
                }
            }
        }
    }
    int FindLowestDistance(List<bool> a, Vector2 targetCoords) {
        //returns 0+
        float minDistance = -1;
        bool firstSet = true;
        int minID = 0;
        List<(int index, int ID)> potentialMins = new List<(int index, int ID)>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                float dist = lScript.FindDistance(lScript.GetCoordsFromIndex(i + 1), targetCoords);
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
    int FindHighestDistance(List<bool> a, Vector2 targetCoords) {
        //returns 0+
        float maxDistance = -1;
        bool firstSet = true;
        int maxID = 0;
        List<(int index, int ID)> potentialMaxes = new List<(int index, int ID)>();
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                float dist = lScript.FindDistance(lScript.GetCoordsFromIndex(i + 1), targetCoords);
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
    int FindHighestElevation(List<bool> a, List<int> g) {
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
    int FindHighestRarity(List<bool> a, List<int> g) {
        int highestRarity = -1;
        bool firstSet = true;
        for (int i = 0; i < a.Count; i++) {
            if (a[i]) {
                int rarity = lScript.GetLandRarity(g[i]);
                if (botMysteryBuildings.Contains(i)) {
                    rarity = 6; //this is the mystery building rarity rating, it's the average building rarity 
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
                if (botMysteryBuildings.Contains(i)) {
                    if (6 == highestRarity) { //same 6 as average building rarity
                        indexOptions.Add(i);
                    }
                } else {
                    if (lScript.GetLandRarity(g[i]) == highestRarity) {
                        indexOptions.Add(i);
                    }
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
    List<int> FindBestHealing(List<int> inv, int health, int maxHealth) {
        //returns 1+
        List<int> healingInInv = ScanInvList(inv, 3);
        List<(int item, int UAAT, int rating)> potentials = new List<(int item, int UAAT, int rating)>();
        for (int i = 0; i < healingInInv.Count; i++) {
            int index = healingInInv[i] - 1;
            int maxUAAT = Mathf.Min(lScript.itemInts[index][5], ScanInvInt(inv, 2, 0, healingInInv[i]));
            for (int j = 1; j <= maxUAAT; j++) {
                potentials.Add((item: healingInInv[i], UAAT: j, GetHealingRating(health, maxHealth, healingInInv[i], j)));
            }
        }
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
    int GetHealingRating(int health, int maxHealth, int item, int j) {
        int pseudoHP = health;
        int pseudoMHP = maxHealth;
        int index = item - 1;
        int healing = Mathf.RoundToInt(lScript.itemInfos[index][1][0]);
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
    public IEnumerator BotSearchUse(NetworkList<int> grid) {
        bool gonnaSearch = true;
        if (mode == 1) {
            yield return StartCoroutine(TryToHeal(result => {
                gonnaSearch = result;
            }));
        } else if (mode == 2) {
            int target = FindClosestTarget(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]));
            if (target == -1) {
                Debug.Log("Got an error that shouldn't happen!");
            } else if (target == 0) {
                List<int> visionWeapons = ScanInvList(inventory, 4);
                float myElevation = lScript.FindElevation(grid[position - 1], 1);
                if (visionWeapons.Count > 0 && myElevation >= 1) {
                    int itemToUse = visionWeapons[0];
                    int UAAT = 1;
                    yield return StartCoroutine(TryToUseWeapon(itemToUse, target, UAAT, result => {
                        gonnaSearch = result;
                    }));
                } else {
                    gonnaSearch = true;
                }
                if (gonnaSearch) {
                    yield return StartCoroutine(TryToHeal(result => {
                        gonnaSearch = result;
                    }));
                }
            } else {
                //check if any weapon is in range
                float distToTarget = lScript.FindDistance(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]), lScript.GetCoordsFromIndex(knownPlayerPositions[target - 1]));
                int itemToUse = ScanInvInt(ScanInvList(inventory, 2), 1, distToTarget, 0); //1+
                int UAAT = 1;
                if (itemToUse == 0) {
                    yield return StartCoroutine(TryToHeal(result => {
                        gonnaSearch = result;
                    }));
                } else {
                    yield return StartCoroutine(TryToUseWeapon(itemToUse, target, UAAT, result => {
                        gonnaSearch = result;
                    }));
                }
            }
        } else if (mode == 3) {
            yield return StartCoroutine(TryToHeal(result => {
                gonnaSearch = result;
            }));
        }
        if (gonnaSearch) {
            yield return StartCoroutine(Search(grid));
        } else {
            yield return StartCoroutine(UpdateMode(1));
        }
    }
    IEnumerator TryToUseWeapon(int itemToUse, int target, int UAAT, System.Action<bool> callback) {
        bool gonnaSearch = true;
        BD = 0;
        Vector2 targetPos = new Vector2();
        if (target > 0) {
            targetPos = new Vector2(lScript.GRIDX[knownPlayerPositions[target - 1] - 1], lScript.GRIDY[knownPlayerPositions[target - 1] - 1]);
        } else {
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
        }
        
        yield return StartCoroutine(lScript.UseItem(itemToUse - 1, myActions, myBDD, myTI, 2, this, position, playerNum, targetPos, target, UAAT, result => {
            if (result) {
                gonnaSearch = false;
                //Debug.Log("Used item");
            } else {
                gonnaSearch = true;
            }
        }));
        callback(gonnaSearch);
    }
    public Vector2? ChangeTarget(int prevTarget) {
        if (prevTarget == 0) {
            int retTarget = FindClosestTarget(lScript.GetCoordsFromIndex(knownPlayerPositions[playerNum - 1]));
            if (retTarget == 0) {
                return null;
            } else {
                return new Vector2(lScript.GRIDX[knownPlayerPositions[retTarget - 1] - 1], lScript.GRIDY[knownPlayerPositions[retTarget - 1] - 1]);
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
            List<int> FBH = FindBestHealing(inventory, HP, maxHP);
            if (FBH != null) {
                    
                itemToUse = FBH[0];
                UAAT = FBH[1];
                //Debug.Log("Found healing item: " + itemToUse);
            }
        }
        if (itemToUse == 0) {
            gonnaSearch = true;
        } else {
            BD = 0;
            yield return StartCoroutine(lScript.UseItem(itemToUse - 1, myActions, myBDD, myTI, 2, this, position, playerNum, new Vector2(), 0, UAAT, result => {
                if (result) {
                    gonnaSearch = false;
                    //Debug.Log("Used item");
                } else {
                    gonnaSearch = true;
                }
            }));
            if (!gonnaSearch) {
                yield return StartCoroutine(UpdateMode(2));
            }
        }
        callback(gonnaSearch);
    }
    public IEnumerator Search(NetworkList<int> grid) {
        int landRarity = lScript.GetLandRarity(grid[position - 1]);
        landRarity += lScript.GetLuck(new List<int>{}, new List<int>{});
        int searchYield = lScript.GetSearchYield(landRarity, grid[position - 1], lScript.GetViableItems(inventory, grid[position - 1], new List<int>{}, false, null)); //supposed to be bot's effects and effect strengths
        bool keep = true;
        if (lScript.itemIntLists[searchYield - 1][0].Contains(4)) {
            if (lScript.InventoryContainsArmor(inventory, Mathf.RoundToInt(lScript.itemInfos[searchYield - 1][3][0]))) {
                //don't forget to remove from inventory if keep == true
                keep = false;
            }
        }
        if (keep) {
        for (int i = 0; i < lScript.itemInts[searchYield - 1][6]; i++) {
            if (lScript.itemInts[searchYield - 1][3] == -1 || lScript.CountInIntList(inventory, searchYield) + 1 <= lScript.itemInts[searchYield - 1][3]) {
                inventory.Add(searchYield);
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
            StartCoroutine(Search(grid));
        }
        yield return null;
    }
    public IEnumerator BotVision() {
        float addVision = 0;
        if (!lScript)
            Debug.Log("lScript not assigned for vision!");
        myVG = lScript.GetVisionList(addVision, lScript.smokes, 2, this, new List<int>{}, new List<int>{}); //VERY IMPORTANT (hard to notice): new List<int>{}s represent botEffects, botEffectStrengths
        
        /*yield return StartCoroutine(lScript.GetVisionList(addVision, lScript.smokes, 2, this, result =>{ 
            myVG = result;
        }));*/
        //lScript.DisplayBotTest(2, null, myVG);
        yield return null;
    }
    public IEnumerator BotVisionHouse() {
        
        yield return StartCoroutine(BotVision());
        //Debug.Log("Bot vision house");
        BotEnemiesVisible(myVG, lScript.playerPositionList);
        for (int i = 0; i < myVG.Count; i++) {
            if (myVG[i] == 1) {
                if (botMysteryBuildings.Contains(i)) {
                    botMysteryBuildings.Remove(i);
                }
            }
        }
        yield return null;
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
                if (lScript.DecideEnemyVisible(myVG, PPL[i], invis)) {
                    knownPlayerPositions[i] = PPL[i];
                } else {
                    knownPlayerPositions[i] = 0;
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