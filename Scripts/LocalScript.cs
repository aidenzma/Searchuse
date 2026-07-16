using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LocalScript : NetworkBehaviour
{
    public float mouseGSX;
    public float mouseGSY;
    public int mouseIndex;
    public int mouseX;
    public int mouseY;
    public GameObject tile;
    public GameObject tiles;
    public GameObject visions;
    public GameObject outlines;
    public float tileWidth = 0.6f;
    public float tileHeight = 0.6f;
    float tilePixelX = 18f;
    float tilePixelY = 18f;
    private bool gameStarted = false;
    public Sprite[] playerCostumes;
    public Sprite[] terrain;
    public NetworkList<int> GRID = new NetworkList<int>(); //GRID is 1+
    //public List<int> GRID;
    public List<float> GRIDX = new List<float>();
    public List<float> GRIDY = new List<float>();
    public List<float> smokes = new List<float>();
    bool[] isSmoked;
    public NetworkList<int> smokeLifespans = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<int> smokeOwners = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<int> playerPositions = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //public List<int> playerPositions;
    Vector3 bottomLeft;
    Vector3 topRight;
    float screenWidth;
    float screenHeight;
    float gameHeight;
    float gameWidth;
    float screenScaleX;
    float screenScaleY;
    public NetworkVariable<int> columns = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> rows = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> playerAmount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> TURN = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> TURNS = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<int> skippedTurns = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkList<bool> playerAliveList = new NetworkList<bool>(new List<bool>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkList<bool> botList = new NetworkList<bool>(new List<bool>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public GameObject serverOnlyPrefab;
    public int myPlayerNum = 0; //myPlayerNum is 1+
    public GameObject myPlayerObject;
    public int playersRequired = 2;
    public int playersReady = 0;
    public int myPosition; //myPosition is 1+
    public bool amSpectator = false;
    public static int gamemode = 0;
    public PlayerScript myPlayerScript;
    float limitCoefficientX;
    float limitCoefficientY;
    public List<GameObject> playerList = new List<GameObject>();
    public List<GameObject> spectatorList = new List<GameObject>();
    public List<int> playerPositionList = new List<int>();
    List<int> prevPlayerPositionList;
    public float moves;
    float FPS;
    int MBAmount = 3;
    public List<int> MB = new List<int>();
    public List<int> visionGrid = new List<int>();
    public float baseVision = 5;
    public float VISION;
    [SerializeField] Sprite visionBlack;
    public List<GameObject> visionObjects = new List<GameObject>();
    public GameObject visionTest;
    Mesh visionMesh;
    Vector2 stopPositionFOV;
    [SerializeField] GameObject visionCoverPrefab;
    GameObject visionCover;
    SpriteRenderer VCSpriterenderer;
    public GameObject searchButton;
    public GameObject searchFrame;
    public GameObject searchItem;
    public GameObject SIParent;
    public Sprite[] searchItemCostumes;
    public Sprite[] searchItemDescriptions;
    public GameObject useButton;
    public GameObject OKButton;
    public GameObject OKButton2;
    public GameObject XButton;
    public GameObject XButton2;
    public GameObject XButton3;
    public GameObject NoButton;
    public GameObject itemDescription;
    int buttonClicked = 0;
    int useItem = 0;
    bool useDisabled;
    bool searchused;
    bool searching = false;
    public Sprite[] buttons;
    public List<List<int>> itemInts = new List<List<int>>();
    public List<List<List<int>>> itemIntLists = new List<List<List<int>>>();
    public List<List<string>> itemStrings = new List<List<string>>();
    public List<List<List<float>>> itemInfos = new List<List<List<float>>>();
    public List<List<bool>> itemBools = new List<List<bool>>();
    public List<List<float>> itemFloats = new List<List<float>>();
    public List<List<List<string>>> itemStringLists = new List<List<List<string>>>();
    public List<List<string>> bookItemStrings = new List<List<string>>();
    public List<List<int>> bookItemInts = new List<List<int>>();
    public List<List<List<int>>> bookItemIntLists = new List<List<List<int>>>();
    public List<List<List<float>>> bookItemFloatLists = new List<List<List<float>>>();
    public List<List<float>> bookItemFloats = new List<List<float>>();
    public int maxHealth = 100;
    public int HEALTH; 
    float pseudoHealth;
    public List<int> healths = new List<int>();
    public List<int> maxHealths = new List<int>();
    List<int> prevHealths = new List<int>();
    List<int> prevMaxHealths = new List<int>();
    List<int> playersDeltaHealth = new List<int>();
    List<bool> enemyHealthsVisible = new List<bool>();
    public float armorCoefficient = 1;
    public List<int> INVENTORY = new List<int>(); // INVENTORY is 1+
    public List<int> bookInventory = new List<int>();
    public GameObject useOptionsScroll;
    public GameObject useOptions;
    public Transform useOptionsTF;
    public GameObject elementPrefab;
    public GameObject bulletPrefab;
    public GameObject explosionPrefab;
    public Sprite[] bulletCostumes;
    public Sprite[] explosionCostumes;
    public GameObject healthText;
    public List<GameObject> healthTexts = new List<GameObject>();
    public GameObject messageText;
    public GameObject movesText;
    public RectTransform CANVAS;
    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioClip[] sounds;
    public AudioClip[] musics;
    int actionsLength = 32;
    //public NetworkList<GameObject> playersList = new NetworkList<GameObject>();
    public List<int> myEffectLengths = new List<int>();
    public List<int> myEffects = new List<int>();
    public List<int> myEffectStrengths = new List<int>();
    List<int> prevMyEffects;
    List<int> prevMyEffectStrengths;
    public List<List<int>> enemyEffects = new List<List<int>>();
    List<List<int>> prevEnemyEffects = new List<List<int>>();
    List<List<int>> prevEnemyEffects2 = new List<List<int>>();
    int prevEnemyEffected;
    public List<int> itemsOnCooldown = new List<int>();
    public List<bool> CDJustAdded = new List<bool>();
    public GameObject SFPrefab;
    public GameObject smokeFolder = null;
    public GameObject smokePrefab;
    public GameObject flashbangPrefab;
    public Slider UAATSlider;
    GameObject UAATSliderButton;
    List<GameObject> tileObjects = new List<GameObject>();
    [SerializeField]
    List<int> mysteryBuildings = new List<int>();
    public RelayScript relay;
    public GameObject statsBook;
    bool statsBookOpen = false;
    public GameObject rollChances;
    bool rollChancesOpen = false;
    public Sprite[] rollChancesCostumes;
    public GameObject inventoryDisplayScroll;
    public GameObject inventoryItemsGLG;
    public Transform inventoryItemsTF;
    bool inventoryOpen = false;
    bool bookInventoryOpen = false;
    bool cooldownsOpen = false;
    public Sprite[] toolCostumes;
    public GameObject toolIconPF;
    public GameObject botPF;
    public List<List<List<float>>> traps = new List<List<List<float>>>();
    List<List<List<float>>> prevTraps = new List<List<List<float>>>();
    public GameObject trapPF;
    public GameObject trapsFolder;
    public Sprite[] trapCostumes;
    List<List<float>> prevListTraps = new List<List<float>>();
    List<int> prevRemovedTraps = new List<int>();
    int prevTrapPosition;
    int prevTrapPlayerNum;
    int frozenTurns = 0;
    public GameObject acceptOrReroll;
    public Sprite[] armorDefaults;
    public GameObject testIcon;
    public GameObject testFolder;
    public Sprite[] testIconCostumes;
    public GameObject UAVPF;
    public GameObject UAVFolder;
    public GameObject UAVNodePF;
    public GameObject UAVCancel;
    public GameObject AirDropPF;
    public Sprite[] airdropCostumes;
    public List<int> airdrops = new List<int>();
    List<GameObject> airdropGOs = new List<GameObject>();
    int airdropTurn = 0;
    int prevAirdropPos;
    int prevAirdropIndex;
    public List<GameObject> airdropOptions;
    public GameObject searchAirdrop;
    int airdropClicked;
    List<int> effectsWornOff = new List<int>();
    public GameObject searchBook;
    public GameObject bookItem;
    public GameObject bookItemDescription;
    public Sprite[] bookCostumes;
    public Sprite[] bookItemCostumes;
    public Sprite[] bookItemDescriptions;
    public Sprite bookItemCostumeEmpty;
    public Sprite bookItemDescriptionEmpty;
    public GameObject bookInventoryDisplay;
    public Transform bookInventoryItemsTF;
    public Sprite painCostume;
    public GameObject cooldownsDisplay;
    public Transform cooldownItemsTF;
    public GameObject cooldownViewport;
    public GameObject CWTD;
    public Transform CWTDTF;
    public GameObject turnsText;
    bool turnsTextShown = false;
    int maxWeapons = 3;
    public GameObject turnIndicator;
    public Sprite[] turnIndicatorCostumes;

    public int totalItems;

    public bool spectatorSet = false;
    public List<List<int>> spectatorVGs = new List<List<int>>();
    List<int> spectatorCombinedVG;

    public Slider volumeSlider;
    public Slider musicVolumeSlider;
    public GameObject mainMenuButton;
    public RestartGame restartGame;

    IEnumerator PlayMusic() {
        while (true) {
            yield return StartCoroutine(PlayMusicAndWait(musics[0]));
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 10f));
        }
    }
    void NetworkEnable() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }
    public void NetworkDisable() {
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkEnable();
        StartCoroutine(StartCR());
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume);
    }
    IEnumerator StartCR() {
        while (!spectatorSet) { 
            yield return null;
        }
        Debug.Log("amSpectator: " + amSpectator);
        if (!amSpectator) {
            visionMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = visionMesh;
            GetComponent<MeshRenderer>().sortingLayerName = "vision";
        }
        
        if (!amSpectator) {
            AddClickListener(searchButton, () => OnImageClicked("Search"), () => OnImageEnter("Search"), () => OnImageExit("Search"));
            AddClickListener(useButton, () => OnImageClicked("Use"), () => OnImageEnter("Use"), () => OnImageExit("Use"));
            AddClickListener(OKButton, () => OnImageClicked("OK"), () => OnImageEnter("OK"), () => OnImageExit("OK"));
            AddClickListener(OKButton2, () => OnImageClicked("OK2"), () => OnImageEnter("OK2"), () => OnImageExit("OK2"));
            AddClickListener(XButton, () => OnImageClicked("X"), () => OnImageEnter("X"), () => OnImageExit("X"));
            AddClickListener(XButton2, () => OnImageClicked("X2"), () => OnImageEnter("X2"), () => OnImageExit("X2"));
            AddClickListener(NoButton, () => OnImageClicked("No"), () => OnImageEnter("No"), () => OnImageExit("No"));
            AddClickListener(airdropOptions[0], () => OnImageClicked("Airdrop1"), () => OnImageEnter("Airdrop1"), () => OnImageExit("Airdrop1"));
            AddClickListener(airdropOptions[1], () => OnImageClicked("Airdrop2"), () => OnImageEnter("Airdrop2"), () => OnImageExit("Airdrop2"));
            AddClickListener(airdropOptions[2], () => OnImageClicked("Airdrop3"), () => OnImageEnter("Airdrop3"), () => OnImageExit("Airdrop3"));
            Debug.Log($"IsOwner: {IsOwner}, IsServer: {IsServer}, OwnerClientId: {OwnerClientId}");
            /*if (NetworkManager.Singleton.IsServer)
            {
                Debug.Log("joe");
                var obj = Instantiate(serverOnlyPrefab);
                obj.GetComponent<NetworkObject>().Spawn();
            }*/
            UAATSliderButton = UAATSlider.transform.Find("Button")?.gameObject;
            if (UAATSliderButton) {
                AddClickListener(UAATSliderButton, () => OnImageClicked("UAATOK"), () => OnImageEnter("UAATOK"), () => OnImageExit("UAATOK"));
            }
            UAATSlider.onValueChanged.AddListener(UpdateSliderText);
            AddClickListener(XButton3, () => OnImageClicked("X3"), () => OnImageEnter("X3"), () => OnImageExit("X3"));
        }
    }
    void ChangeVolume(float val) {
        audioSource.volume = val / 100;
        volumeSlider.transform.Find("SliderText").GetComponent<TextMeshProUGUI>().text = val.ToString();
    }
    void ChangeMusicVolume(float val) {
        musicSource.volume = val / 100;
        musicVolumeSlider.transform.Find("SliderText").GetComponent<TextMeshProUGUI>().text = val.ToString();
    }
    void UpdateSliderText(float val) {
        GameObject ST = UAATSlider.transform.Find("SliderText")?.gameObject;
        ST.GetComponent<TMP_Text>().text = ((int) val).ToString();
    }
    void AddClickListener(GameObject target, System.Action action, System.Action enter, System.Action exit)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { action(); });
        trigger.triggers.Add(entry);
        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((eventData) => { enter(); });
        trigger.triggers.Add(enterEntry);
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((eventData) => { exit(); });
        trigger.triggers.Add(exitEntry);
    }
    void OnImageClicked(string imageName)
    {
        if (imageName == "Search")
        {
            buttonClicked = 1;
        }
        else if (imageName == "Use")
        {
            buttonClicked = 2;
        }
        else if (imageName == "OK")
        {
            buttonClicked = 3;
        }
        else if (imageName == "OK2")
        {
            buttonClicked = 8;
        }
        else if (imageName == "X")
        {
            buttonClicked = 4;
        }
        else if (imageName == "X2") 
        {
            buttonClicked = 5;
        }
        else if (imageName == "UAATOK")
        {
            buttonClicked = 6;
        }
        else if (imageName == "X3")
        {
            buttonClicked = 7;
        }
        else if (imageName == "No")
        {
            buttonClicked = 9;
        }
        else if (imageName == "Airdrop1")
        {
            if (airdropClickable) {
                if (airdropClicked == 1) {
                    airdropClicked = 0;
                } else {
                    airdropClicked = 1;
                    StartCoroutine(PlaySound(sounds[59]));
                }
            }
        }
        else if (imageName == "Airdrop2") {
            if (airdropClickable) {
                if (airdropClicked == 2) {
                    airdropClicked = 0;
                } else {
                    airdropClicked = 2;
                    StartCoroutine(PlaySound(sounds[59]));
                }
            }
        }
        else if (imageName == "Airdrop3") {
            if (airdropClickable) {
                if (airdropClicked == 3) {
                    airdropClicked = 0;
                } else {
                    airdropClicked = 3;
                    StartCoroutine(PlaySound(sounds[59]));
                }
            }
        }
    }
    void OnImageEnter(string imageName)
    {
        GameObject buttonObject = null;
        if (imageName == "Search")
        {
            buttonObject = searchButton;
            buttonObject.GetComponent<Image>().sprite = buttons[1];
        }
        else if (imageName == "Use")
        {
            buttonObject = useButton;
            if (!useDisabled)
                buttonObject.GetComponent<Image>().sprite = buttons[3];
        }
        else if (imageName == "OK")
        {
            OKButton.GetComponent<Image>().sprite = buttons[5];
        }
        else if (imageName == "OK2") 
        {
            OKButton2.GetComponent<Image>().sprite = buttons[5];
        }
        else if (imageName == "X")
        {
            XButton.GetComponent<Image>().sprite = buttons[7];
        }
        else if (imageName == "X2")
        {
            XButton2.GetComponent<Image>().sprite = buttons[7];
        } else if (imageName == "UAATOK") {
            UAATSliderButton.GetComponent<Image>().sprite = buttons[5];
        } else if (imageName == "X3") {
            XButton3.GetComponent<Image>().sprite = buttons[7];
        }
        else if (imageName == "No") {
            if (NoButtonMode == 1) {
                NoButton.GetComponent<Image>().sprite = buttons[9];
            } else if (NoButtonMode == 2) {
                NoButton.GetComponent<Image>().sprite = buttons[11];
            }
        }
        if (imageName == "Search" || imageName == "Use")
        {
            if (!(imageName == "Use" && useDisabled))
            {
                Color color = buttonObject.GetComponent<Image>().color;
                color.a = 1;
                buttonObject.GetComponent<Image>().color = color;
            } 
        }
    }
    void OnImageExit(string imageName)
    {
        GameObject buttonObject = null;
        if (imageName == "Search")
        {
            buttonObject = searchButton;
            buttonObject.GetComponent<Image>().sprite = buttons[0];
        }
        else if (imageName == "Use")
        {
            buttonObject = useButton;
            if (!useDisabled)
                buttonObject.GetComponent<Image>().sprite = buttons[2];
        }
        else if (imageName == "OK")
        {
            OKButton.GetComponent<Image>().sprite = buttons[4];
        }
        else if (imageName == "OK2")
        {
            OKButton2.GetComponent<Image>().sprite = buttons[4];
        }
        else if (imageName == "X")
        {
            XButton.GetComponent<Image>().sprite = buttons[6];
        }
        else if (imageName == "X2") 
        {
            XButton2.GetComponent<Image>().sprite = buttons[6];
        } else if (imageName == "UAATOK") {
            UAATSliderButton.GetComponent<Image>().sprite = buttons[4];
        } else if (imageName == "X3") {
            XButton3.GetComponent<Image>().sprite = buttons[6];
        }
        else if (imageName == "No")
        {
            if (NoButtonMode == 1) {
                NoButton.GetComponent<Image>().sprite = buttons[8];
            } else if (NoButtonMode == 2) {
                NoButton.GetComponent<Image>().sprite = buttons[10];
            }
        }
        if (imageName == "Search" || imageName == "Use")
        {
            if (!(imageName == "Use" && useDisabled))
            {
                Color color = buttonObject.GetComponent<Image>().color;
                color.a = (166f / 255f);
                buttonObject.GetComponent<Image>().color = color;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        FPS = 1 / Time.deltaTime;
        //Debug.Log(FPS);
        //if (!IsOwner) return;
        if (!gameStarted) return;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseGSX = mouseWorldPos.x;
        mouseGSY = mouseWorldPos.y;
        mouseIndex = GetIndexFromWorldCoords(new Vector2(mouseWorldPos.x, mouseWorldPos.y));
        Vector2 mousePos = GetCoordsFromIndex(mouseIndex);
        mouseX = (int)mousePos.x;
        mouseY = (int)mousePos.y;
        turnIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(CANVAS.rect.width, CANVAS.rect.width * 6f / 120f);
        turnIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(CANVAS.rect.height / 2));
        //if (GNTConfirm == 2) 
            //Debug.Log("GNTConfirm = " + GNTConfirm);
        //StartCoroutine(Move());
        /*if (IsOwner)
        {
            if (!gameStarted)
            {
                gameStarted = true;
                StartCoroutine(StartGame());
            }
        }*/
    }
    void Awake()
    {
        
    }
    public void AddItem(string name, List<int> itemClass, int rarity, int addToInv, List<float> weaponInfo, List<float> healingInfo, int cooldown, List<int> specific, bool usable, int takeFromInv, int maxStack, float movesChange, List<float> toolInfo, List<int> functions, List<float> armorInfo, List<float> trapInfo, List<string> specialNotes, List<int> soundIndexes, int drawSoundIndex, List<int> bulCostIndexes, int playSoundConsecutive, int maxUseAtATime, List<int> terrainRequirements, List<float> terrainChances, List<int> addAfterUse, List<int> prerequisites, List<int> effectsExclude, List<float> formatInfo)
    {
        itemStrings.Add(new List<string> {name});
        itemIntLists.Add(new List<List<int>> {itemClass, specific, soundIndexes, bulCostIndexes, functions, terrainRequirements, addAfterUse, prerequisites, effectsExclude});
        itemInts.Add(new List<int> {rarity, cooldown, takeFromInv, maxStack, playSoundConsecutive, maxUseAtATime, addToInv, drawSoundIndex});
        itemInfos.Add(new List<List<float>> {weaponInfo, healingInfo, toolInfo, armorInfo, trapInfo, terrainChances, formatInfo});
        itemBools.Add(new List<bool> {usable});
        itemFloats.Add(new List<float> {movesChange});
        itemStringLists.Add(new List<List<string>> {specialNotes});
        itemsOnCooldown.Add(0);
        CDJustAdded.Add(false);
        //{
            /*
            Classes:
            1 = "Weapon"
            2 = "Healing"
            3 = "Tool"
            4 = "Armor"
            Specific:
            1 = "Gun"
            2 = "Bomb"
            3 = "Knife"
            4 = "Visibility"
            5 = "Vision"
            6 = "Map"
            7 = "Healing"
            8 = "Smoke"
            9 = "Stun"
            10 = "Location"
            11 = "Armor"
            12 = "Health"
            13 = "Trap"
            14 = "Mobility"
            15 = "Change location"
            16 = "Explodes but not bomb"
            17 = "Triggers trap"
            18 = "Luck"
            19 = "Powerup"
            */
            /*
            Rarity:
            0 = cannot be found
            1 = common
            2 = uncommon
            3 = rare
            4 = more rare
            5 = epic
            6 = very epic
            7 = legendary
            8 = mythic
            /*
            weaponInfo: {
            0. range, 1. damage, 2. bullet spread, 3. fire num, 4. bullet speed, 5. stop in place(0, 1), 6. homing, 
            7. suppressed(0, 1), 8. vision increase, 9. blast radius, 10. fuse time(seconds), 11. fire rate(seconds), 12. elevation increase, 13. bounce (0, 1), 14. explosion type
            }
            healingInfo: {healing, healing time, max health healing}
            toolInfo: {triggers effect(0, 1), effect length, vision increase, stun length, movesChange, stun mode, luck, powerup(0, 1, 2), book rarity}
            armorInfo: {armorSlot, armorPercentage, vision increase, xray(0, 1)}
            trapInfo: {type, damage, blast radius, costume index, fake blast radius}
            */
            /*
            Functions:
            1 = "Invisibility"
            2 = "Enhanced vision"
            3 = "Partial invisibility"
            4 = "Stun"
            5 = "Radar"
            6 = "See health"
            7 = "Enhanced movement"
            8 = "Change location"
            9 = "Trigger trap"
            10 = "Luck"
            11 = "UAV"
            12 = "Powerup"
            */
            //maxStack = -1 represents infinity
            /*
            playSoundConsecutive:
            0 = random
            else = play the first playSoundConsecutive sounds, consecutively
            */
            //formatInfo: {imageWidth, imageHeight, descWidth, descHeight}
        //}
        totalItems++;
    }
    IEnumerator AddItems()
    {
        totalItems = 0;
        AddItem("Beretta", new List<int>{1}, 3, 1, new List<float>{5, 20, 5, 1, 20, 0, 0, 0, 0, 0, 0, 0.2f, 0, 0, 0}, new List<float>{}, 0, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{2}, -1, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{46, 32, 264, 188});
        AddItem("Bandages", new List<int>{2}, 2, 1, new List<float>{}, new List<float>{20, 1, 0}, 0, new List<int>{7}, true, 1, -1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{3, 4}, -1, new List<int>{}, 2, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{23, 23, 200, 112});
        AddItem("MP5", new List<int>{1}, 4, 1, new List<float>{4, 8, 10, 3, 10, 0, 0, 0, 0, 0, 0, 0.05f, 0, 0, 0}, new List<float>{}, 0, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{5}, -1, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{87, 35, 276, 188});
        AddItem("AWP", new List<int>{1}, 8, 1, new List<float>{12, 49, 3, 1, 50, 0, 0, 0, 5, 0, 0, 1, 0, 0, 0}, new List<float>{}, 2, new List<int>{1}, true, 0, 1, -1, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{6}, 24, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{148, 32, 260, 296});
        AddItem("Grenade", new List<int>{1}, 3, 1, new List<float>{5, 70, 5, 1, 5, 1, 0, 0, 0, 5, 0.5f, 0.2f, 1, 1, 1}, new List<float>{}, 0, new List<int>{2}, true, 1, 3, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{7}, 25, new List<int>{2}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{14, 23, 284, 224});
        AddItem("Karambit", new List<int>{1}, 5, 1, new List<float>{1, 50, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0.3f, 0, 0, 0}, new List<float>{}, 1, new List<int>{3}, true, 0, 1, 1, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{10, 11, 12}, 26, new List<int>{3, 4, 5, 6}, 0, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{24, 30, 278, 260});
        AddItem("Invis Pot", new List<int>{3}, 4, 1, new List<float>{}, new List<float>{}, 2, new List<int>{4}, true, 1, -1, 0, new List<float>{1, 2, 0, 0, 0, 0, 0, 0, 0}, new List<int>{1}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{14}, -1, new List<int>{}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{22, 30, 260, 148}); //tool
        AddItem("Binoculars", new List<int>{3}, 3, 1, new List<float>{}, new List<float>{}, 5, new List<int>{5}, true, 0, 1, 0, new List<float>{1, 1, 10, 0, 0, 0, 0, 0, 0}, new List<int>{2}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{23, 11, 215, 184}); //tool
        AddItem("Health Kit", new List<int>{2}, 7, 1, new List<float>{}, new List<float>{100, 1, 0}, 3, new List<int>{7}, true, 1, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{3, 4}, -1, new List<int>{}, 2, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{40, 25, 250, 160});
        AddItem("Smoke Bomb", new List<int>{1, 3}, 2, 1, new List<float>{4, 0, 10, 1, 5, 1, 0, 0, 0, 4, 0.5f, 0.3f, 1, 1, 0}, new List<float>{}, 0, new List<int>{4, 6, 8}, true, 1, 5, 0, new List<float>{0, 3, 0, 0, 0, 0, 0, 0, 0}, new List<int>{3}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{7}, 25, new List<int>{7}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{12, 23, 252, 292}); //tool
        AddItem("Flashbang", new List<int>{1, 3}, 4, 1, new List<float>{5, 0, 5, 1, 5, 1, 0, 0, 0, 0, 0.5f, 0.3f, 1, 1, 0}, new List<float>{}, 0, new List<int>{9}, true, 1, 4, 0, new List<float>{0, 0, 0, 1, 0, 1, 0, 0, 0}, new List<int>{4}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{7}, 25, new List<int>{8}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{12, 26, 205, 220}); //tool
        AddItem("Water Bottle", new List<int>{2}, 1, 1, new List<float>{}, new List<float>{10, 1, 0}, 0, new List<int>{7}, true, 1, -1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{14, 4}, -1, new List<int>{}, 2, 5, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{9, 24, 270, 188});
        AddItem("Butterfly Knife", new List<int>{1}, 6, 1, new List<float>{1, 30, 10, 2, 1, 0, 0, 0, 0, 0, 0, 0.3f, 0, 0, 0}, new List<float>{}, 1, new List<int>{3}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{10, 11, 12}, 27, new List<int>{3, 4, 5, 6}, 0, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{15, 43, 325, 256});
        AddItem("Radar", new List<int>{3}, 1, 1, new List<float>{}, new List<float>{}, 3, new List<int>{10}, true, 1, 2, 0, new List<float>{0, 0, 0, 0, 0, 0, 0, 0, 0}, new List<int>{5}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{21}, -1, new List<int>{}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{40, 46, 256, 188}); //tool
        AddItem("Desert Eagle", new List<int>{1}, 7, 1, new List<float>{5, 49, 6, 1, 40, 0, 0, 0, 0, 0, 0, 1f, 0, 0, 0}, new List<float>{}, 2, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{22, 23}, 28, new List<int>{1}, 0, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{56, 37, 265, 268});
        AddItem("Rhino Helmet", new List<int>{4}, 5, 1, new List<float>{}, new List<float>{}, 0, new List<int>{11}, false, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{1, 0.1f, 0, 0}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 0, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{45, 42, 260, 112});
        AddItem("Vital Signs", new List<int>{3}, 1, 1, new List<float>{}, new List<float>{}, 10, new List<int>{12}, true, 1, 1, 0, new List<float>{1, 10, 0, 0, 0, 0, 0, 0, 0}, new List<int>{6}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{34}, -1, new List<int>{}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{54, 41, 276, 220}); //tool
        AddItem("Stimpak", new List<int>{2}, 5, 1, new List<float>{}, new List<float>{25, 1, 15}, 2, new List<int>{7}, true, 1, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{35, 4}, -1, new List<int>{}, 2, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{36, 33, 258, 148});
        AddItem("Kevlar Vest", new List<int>{4}, 6, 1, new List<float>{}, new List<float>{}, 0, new List<int>{11}, false, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{2, 0.2f, 0, 0}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 0, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{44, 55, 255, 112});
        AddItem("Landmine", new List<int>{1}, 3, 3, new List<float>{4, 99, 0, 1, 5, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1}, new List<float>{}, 0, new List<int>{2, 13}, true, 1, 10, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{1, 99, 0, 0, 3}, new List<string>{}, new List<int>{7, 36}, -1, new List<int>{9}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{50, 30, 245, 296});
        AddItem("Tactical Trousers", new List<int>{4}, 6, 1, new List<float>{}, new List<float>{}, 0, new List<int>{11}, false, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{3, 0.15f, 0, 0}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 0, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{57, 95, 370, 112});
        AddItem("SWAT Boots", new List<int>{4}, 5, 1, new List<float>{}, new List<float>{}, 0, new List<int>{11}, false, 0, 1, 1, new List<float>{}, new List<int>{}, new List<float>{4, 0.05f, 0, 0}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 0, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{70, 74, 245, 148});
        AddItem("M4A1", new List<int>{1}, 8, 1, new List<float>{8, 19, 5, 3, 25, 0, 0, 0, 0, 0, 0, 0.06f, 0, 0, 0}, new List<float>{}, 2, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{38}, -1, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{111, 34, 285, 224});
        AddItem("Energy Drink", new List<int>{3}, 2, 1, new List<float>{}, new List<float>{}, 2, new List<int>{14}, true, 1, -1, 0, new List<float>{1, 2, 0, 0, 10, 0, 0, 0, 0}, new List<int>{7}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{14, 39}, -1, new List<int>{}, 2, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{18, 39, 265, 184}); //tool
        AddItem("Grappling Gun", new List<int>{1, 3}, 3, 1, new List<float>{20, 0, 5, 1, 10, 1, 0, 0, 0, 0, 0, 1f, 2, 0, 0}, new List<float>{}, 1, new List<int>{15}, true, 1, 2, 0, new List<float>{0, 0, 0, 0, 0, 0, 0, 0, 0}, new List<int>{8}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{40}, -1, new List<int>{10}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{78, 33, 280, 332}); //tool
        AddItem("Energy Orb", new List<int>{1}, 4, 1, new List<float>{7, 25, 10, 1, 5, 0, 20, 0, 0, 4, 0, 0.2f, 4, 0, 2}, new List<float>{}, 0, new List<int>{2}, true, 1, 5, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{42}, -1, new List<int>{11}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{47, 35, 230, 332});
        AddItem("Snowball", new List<int>{1, 3}, 1, 1, new List<float>{3, 0, 10, 1, 5, 0, 0, 0, 0, 4, 0, 0.7f, 1, 0, 3}, new List<float>{}, 0, new List<int>{9, 16}, true, 1, 3, 0, new List<float>{0, 0, 0, 2, 0, 2, 0, 0, 0}, new List<int>{4}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{42}, -1, new List<int>{12}, 1, 1, new List<int>{4}, new List<float>{0.5f}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{37, 37, 388, 292}); //tool
        AddItem("C4", new List<int>{1}, 3, 1, new List<float>{4, 100, 5, 1, 5, 1, 0, 0, 0, 5, 0, 2f, 1, 1, 1}, new List<float>(), 0, new List<int>{2, 13}, true, 1, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{2, 100, 5, 1, 5}, new List<string>{}, new List<int>{7, 48}, -1, new List<int>{13}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{29}, new List<int>{}, new List<int>{}, new List<float>{34, 40, 272, 296});
        AddItem("Detonator", new List<int>{3}, 0, 0, new List<float>{}, new List<float>{}, 0, new List<int>{17}, true, 1, -1, 0, new List<float>{0, 0, 0, 0, 0, 0, 0, 0, 0}, new List<int>{9}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 0, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{18, 35, 344, 179}); //tool
        AddItem("MP5+", new List<int>{1}, 8, 1, new List<float>{6, 20, 8, 3, 15, 0, 0, 1, 2, 0, 0, 0.05f, 0, 0, 0}, new List<float>{}, 2, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{50}, -1, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{-3}, new List<int>{3}, new List<int>{}, new List<float>{108, 40, 283, 340});
        AddItem("Thermal Vision Goggles", new List<int>{4}, 5, 1, new List<float>{}, new List<float>{}, 0, new List<int>{11}, false, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{1, 0, 1, 1}, new List<float>{}, new List<string>{}, new List<int>{51}, -1, new List<int>{}, 1, 0, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{43, 28, 305, 201});
        AddItem("4-Leaf Clover", new List<int>{3}, 3, 0, new List<float>{}, new List<float>{}, 0, new List<int>{18}, false, 0, 0, 0, new List<float>{1, 5, 0, 0, 0, 0, 3, 0, 0}, new List<int>{10}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{52}, -1, new List<int>{}, 1, 0, new List<int>{1, 5, 6, 7, 8}, new List<float>{-1, -1, -1, -1, -1}, new List<int>{}, new List<int>{}, new List<int>{10}, new List<float>{25, 37, 536, 188}); //tool
        AddItem("UAV", new List<int>{3}, 1, 1, new List<float>{}, new List<float>{}, 3, new List<int>{10}, true, 1, 1, 0, new List<float>{1, 3, 0, 0, 0, 0, 0, 0, 0}, new List<int>{11}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{53}, -1, new List<int>{}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{79, 68, 348, 224}); //tool
        AddItem("Book", new List<int>{3}, 2, 1, new List<float>{}, new List<float>{}, 0, new List<int>{19}, true, 1, -1, 0, new List<float>{0, 0, 0, 0, 0, 0, 0, 1, 5}, new List<int>{12}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{38, 40, 244, 152}); //tool
        AddItem("Message in a Floating Bottle", new List<int>{3}, 1, 0, new List<float>{}, new List<float>{}, 0, new List<int>{19}, false, 0, 0, 0, new List<float>{0, 0, 0, 0, 0, 0, 0, 2, 1}, new List<int>{12}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 0, 0, new List<int>{2}, new List<float>{0.5f}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{39, 14, 305, 265}); //tool
        AddItem("AK-47", new List<int>{1}, 5, 1, new List<float>{6, 20, 8, 2, 30, 0, 0, 0, 0, 0, 0, 0.1f, 0, 0, 0}, new List<float>{}, 1, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{64}, -1, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{100, 30, 223, 224});
        AddItem("AK-74", new List<int>{1}, 8, 1, new List<float>{9, 19, 6, 3, 25, 0, 0, 0, 0, 0, 0, 0.06f, 0, 0, 0}, new List<float>{}, 1, new List<int>{1}, true, 0, 1, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{65}, -1, new List<int>{1}, 1, 1, new List<int>{}, new List<float>{}, new List<int>{-36}, new List<int>{36}, new List<int>{}, new List<float>{105, 29, 303, 268});
        AddItem("Meat Stick", new List<int>{2}, 4, 1, new List<float>{}, new List<float>{40, 1, 0}, 2, new List<int>{7}, true, 1, 2, 0, new List<float>{}, new List<int>{}, new List<float>{}, new List<float>{}, new List<string>{}, new List<int>{66, 4}, -1, new List<int>{}, 2, 1, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{51, 45, 215, 148});
        AddItem("Spikes", new List<int>{4}, 4, 1, new List<float>{}, new List<float>{}, 0, new List<int>{11}, false, 0, 1, 3, new List<float>{}, new List<int>{}, new List<float>{4, 0, 0, 0}, new List<float>{}, new List<string>{}, new List<int>{}, -1, new List<int>{}, 1, 0, new List<int>{}, new List<float>{}, new List<int>{}, new List<int>{}, new List<int>{}, new List<float>{56, 28, 331, 80});
        yield return null;
    }
    [ClientRpc] 
    public void StartGameClientRpc()
    {
        if (IsHost && LocalScript.gamemode == 1) {
            mainMenuButton.SetActive(false);
        }
        StartCoroutine(StartGameRpc());
    }
    //private InputAction sKeyAction;
    int COPLConfirm = 0;
    int healthInitialConfirm = 0;
    IEnumerator StartGameRpc()
    {
        StartCoroutine(PlayMusic());
        Debug.Log("Game Start");
        //yield return null;
        //yield return new WaitForSeconds(0.2f);
        GameObject[] potentialPlayers = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("Potential players length: " + potentialPlayers.Length);
        //int count = 0;
        foreach (GameObject obj in potentialPlayers)
        {
            //Debug.Log("count");
            var netObj = obj.GetComponent<NetworkObject>();
            //if (!botList[count]) {
            if (!obj.TryGetComponent<BotScript>(out _)) {
                if (netObj != null && netObj.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    myPlayerObject = obj; //netObj.transform.parent?.gameObject;
                    if (amSpectator) {
                        myPlayerObject.GetComponent<PlayerScript>().amSpectator.Value = true;
                    }
                    //Debug.Log(myPlayerObject);
                }
            }
                
            //}
            //count++;
            //Debug.Log("count: " + count);
        }
        if (IsHost) {
            Destroy(relay.joinCodeText);
        }
        if (!amSpectator) {
            while (myPlayerNum == 0)
            {
                yield return null;
            }
        }
        //Debug.Log("Run once");
        StartCoroutine(CreateMessageText("Game has started!", 200, 200, 1, true));
        if (IsClient)
        {
            Debug.Log("IsClient");
            gameStarted = true;
            
            yield return StartCoroutine(AddItems());
            yield return StartCoroutine(AddBookItems());
            //if (IsHost)
            yield return StartCoroutine(CreateOrderedPlayerList());
            
            int nonBotNum = CountNonBots();
            while (COPLConfirm != nonBotNum) {
                yield return null;
            }
            /*if (amSpectator) {
                yield return new WaitForSeconds(1);
            }*/
            Debug.Log("Done counting nonBots");
            if (!amSpectator) {
                HEALTH = maxHealth;
                UpdateHealthText(HEALTH, maxHealth);
                HealthsServerRpc(myPlayerNum, HEALTH, maxHealth, true, false, 0);
                //INVENTORY.Add(1); //beretta
                //INVENTORY.Add(2);
                //INVENTORY.Add(3); //mp5
                //INVENTORY.Add(4);
                //INVENTORY.Add(5); //gren
                //INVENTORY.Add(6);
                //INVENTORY.Add(7); //invis
                //INVENTORY.Add(8); //bino
                //INVENTORY.Add(9);
                //INVENTORY.Add(10); //smoke
                //INVENTORY.Add(11); //flashbang
                //for (int i = 0; i < 5; i++) INVENTORY.Add(12);
                //INVENTORY.Add(12);
                //INVENTORY.Add(13); //bfk
                //INVENTORY.Add(14);
                //INVENTORY.Add(15);
                //INVENTORY.Add(16); //rhino
                //INVENTORY.Add(17); //vitals
                //INVENTORY.Add(18); //stim
                //INVENTORY.Add(19);
                //INVENTORY.Add(20); //mine
                //INVENTORY.Add(21);
                //INVENTORY.Add(22);
                //INVENTORY.Add(23); //m4a1
                //INVENTORY.Add(24); //energy drink
                //INVENTORY.Add(25); //grappling
                //INVENTORY.Add(26); //energy orb
                //INVENTORY.Add(27); //snowball
                //INVENTORY.Add(28); //C4
                //INVENTORY.Add(29); //Detonator
                //INVENTORY.Add(30); //mp5+
                //INVENTORY.Add(31); //thermal
                //INVENTORY.Add(32); //clover 
                //for (int i = 0; i < 2; i++) INVENTORY.Add(33); //uav
                //for (int i = 0; i < 7; i++) INVENTORY.Add(34); //book
                //INVENTORY.Add(36);
                //INVENTORY.Add(37); //ak74
                //INVENTORY.Add(38);
                //INVENTORY.Add(39);
                //bookInventory.Add(1);
                //bookInventory.Add(2);
                //bookInventory.Add(3);
                //bookInventory.Add(4);
                //bookInventory.Add(5);
                //bookInventory.Add(6);
            }
        }
        Vector3 newScale = new Vector3();
        if (!amSpectator) {
            myPlayerScript = myPlayerObject.GetComponent<PlayerScript>();
            myPosition = UnityEngine.Random.Range(1, GRID.Count + 1);
            Debug.Log(myPosition);
            
            SetCharacterPosition(myPlayerObject, myPosition);
            UpdatePositionServerRpc(myPlayerObject.GetComponent<NetworkObject>(), myPosition, false, myPlayerNum, visionGrid.ToArray());
            StartCoroutine(myPlayerScript.InitiateCostume(1, playerCostumes[0]));
        }
        newScale = myPlayerObject.transform.localScale;
        newScale.x = tileWidth * 0.9f; // * limitCoefficientX;
        newScale.y = tileHeight * 0.9f; // * limitCoefficientY;
        myPlayerObject.transform.localScale = newScale;
        if (!amSpectator) {
            //StartCoroutine(AnimateAirdrop(UnityEngine.Random.Range(1, GRID.Count + 1)));
            Debug.Log("playerPositionList length: " + playerPositionList.Count);
            Debug.Log("number: " + (myPlayerNum - 1));
            playerPositionList[myPlayerNum - 1] = myPosition;
            yield return StartCoroutine(Vision(0, smokes, myEffects, myEffectStrengths));
        }
        if (IsServer) {
            /*int nonBotNum = CountNonBots(); //this is needed if IsServer is true but IsClient is not true, AKA running on a dedicated server
            while (COPLConfirm != nonBotNum) {
                yield return null;
            }*/
            potentialPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in potentialPlayers)
            {
                //Debug.Log("count");
                var netObj = obj.GetComponent<NetworkObject>();
                //if (!botList[count]) {
                if (obj.TryGetComponent<BotScript>(out _) && !obj.GetComponent<PlayerScript>().amSpectator.Value) {
                    //if (only bots) {
                        //Initialize bots here
                        BotScript botScript = obj.GetComponent<BotScript>();
                        botScript.botBrain = obj.GetComponent<BotBrain>();
                        botScript.botBrain.botScript = botScript;

                        botScript.maxHP = maxHealth;
                        botScript.HP = botScript.maxHP;
                        
                        botScript.position = UnityEngine.Random.Range(1, GRID.Count + 1);
                        SetCharacterPosition(obj, botScript.position);
                        botScript.mode = 1;
                        //botScript.inventory.Add(2);
                        //botScript.inventory.Add(3);
                        //botScript.inventory.Add(5);
                        //botScript.inventory.Add(10);
                        //botScript.inventory.Add(11);
                        //botScript.inventory.Add(13);
                        //botScript.inventory.Add(28);
                        //botScript.botBookInventory.Add(6);
                        PlayerScript pScript = obj.GetComponent<PlayerScript>();
                        //StartCoroutine(pScript.InitiateCostume(2));
                        botScript.playerNum = pScript.botNum;
                        // ^ should be network if not only bots
                        HealthsServerRpc(botScript.playerNum, botScript.HP, botScript.maxHP, true, false, 0);
                        botScript.lScript = this;
                        botScript.InitializeMysteryBuildings(GRID);
                        botScript.InitializeDataLists(GRID);
                        botScript.InitializeBotBrain();
                        for (int i = 0; i < playerPositionList.Count; i++) {
                            botScript.knownPlayerPositions.Add(0);
                            botScript.lastKnownPlayerPositions.Add(0);
                            botScript.potentialMovesSinceLastSeen.Add(new List<float>());
                            botScript.potentialPlayerPositions.Add(null);
                            botScript.potentialPlayerVGs.Add(null);
                            botScript.compoundPlayerVGs.Add(null);
                            botScript.potentialPlayerInvs.Add(new List<int>());
                            botScript.UAVPlayerPositions.Add(0);
                            botScript.playerHealthsVisible.Add(false);
                            botScript.knownPlayerHealths.Add(0);
                            botScript.knownPlayerMaxHealths.Add(0);
                            botScript.allPlayerModes.Add(new List<float>{1, 0, 0});
                            botScript.allPlayerDataLists.Add(new List<float>());
                            botScript.UpdatePlayerDataList(i + 1);
                            // doesn't necessarily need to be network if not only bots, because it's only needed to make decisions, and only the server needs it to have this
                        }
                        
                        botScript.UpdateMyPositionInList();
                        botScript.targetQuadrant = botScript.currentQuadrant;
                        
                        for (int i = 0; i < itemsOnCooldown.Count; i++) {
                            botScript.botItemsOnCooldown.Add(0);
                            botScript.botCDJA.Add(false);
                            // doesn't necessarily need to be network if not only bots
                        }
                        for (int i = 0; i < GRID.Count; i++) {
                            botScript.botSmokes.Add(-1);
                            botScript.botTraps.Add(new List<List<float>>());
                            // doesn't necessarily need to be network if not only bots
                        }
                        
                        
                        //Debug.Log("Supposedly bot vision house");
                        /*Vector3 newScale2 = obj.transform.localScale;
                        newScale2.x = tileWidth * 0.9f; // * limitCoefficientX;
                        newScale2.y = tileHeight * 0.9f; // * limitCoefficientY;
                        obj.transform.localScale = newScale2;*/
                        //playerPositionList[botScript.playerNum - 1] = botScript.position;
                    //} else {use network variables}
                }
            }
        }
        //yield return new WaitForSeconds(0.1f);
        for (int k = 0; k < playerList.Count; k++)
        {
            int index = k + 1;
            if (myPlayerNum != index)
            {
                GameObject enemyObject = playerList[k];
                bool enemyIsBot = botList[k];
                int enemyPosition;
                PlayerScript enemyScript = enemyObject.GetComponent<PlayerScript>();
                Debug.Log("After enemyScript");
                if (!enemyIsBot) {
                    if (enemyScript.position.Value == 0)
                    {
                        while (enemyScript.position.Value == 0)
                        {
                            yield return null;
                        }
                    }
                    enemyPosition = enemyScript.position.Value;
                } else {
                    BotScript botScript = enemyObject.GetComponent<BotScript>();
                    if (botScript.position == 0)
                    {
                        while (botScript.position == 0)
                        {
                            yield return null;
                        }
                    }
                    enemyPosition = botScript.position;
                }
                Debug.Log("Before PPLk = EP");
                playerPositionList[k] = enemyPosition;
                Debug.Log(enemyPosition);

                if (!amSpectator) {
                    yield return StartCoroutine(EnemyVisible(visionGrid, enemyObject, enemyPosition, enemyEffects[k].Contains(1)));
                }
                
                SetCharacterPosition(enemyObject, enemyPosition);
                
                //enemyObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(GRIDX[enemyPosition - 1], GRIDY[enemyPosition - 1], Camera.main.nearClipPlane));
                
                StartCoroutine(enemyScript.InitiateCostume(2, playerCostumes[1]));
                newScale = enemyObject.transform.localScale;
                newScale.x = tileWidth * 0.9f; // * limitCoefficientX;
                newScale.y = tileHeight * 0.9f; // * limitCoefficientY;
                enemyObject.transform.localScale = newScale;
            }
        }
        foreach (GameObject obj in potentialPlayers)
        {
            var netObj = obj.GetComponent<NetworkObject>();
            if (obj.TryGetComponent<BotScript>(out _) && !obj.GetComponent<PlayerScript>().amSpectator.Value) {
                BotScript botScript = obj.GetComponent<BotScript>();
                Debug.Log("Before botvisionhouse");
                yield return StartCoroutine(botScript.BotVisionHouse(false, 0));
                Debug.Log("After botvisionhouse");
            }
        }

        Debug.Log("my player ready");
        PlayerReadyServerRpc();
        if (IsServer) {
            for (int i = 0; i < playerList.Count; i++) {
                if (botList[i]) {
                    PlayerReadyServerRpc();
                }
            }
        }
        if (IsClient) {
            while (healthInitialConfirm != healths.Count) {
                yield return null;
            }
            //create enemy health displays
            Debug.Log("Healths ready!");
            float yLevel = healthText.GetComponent<RectTransform>().anchoredPosition.y;
            for (int i = 0; i < healths.Count; i++) {
                if (i + 1 != myPlayerNum) {
                    yLevel -= 50;
                    Transform healthParent = healthText.transform.parent;
                    GameObject enemyHealthText = Instantiate(healthText, healthParent);
                    int originalIndex = healthText.transform.GetSiblingIndex();
                    enemyHealthText.transform.SetSiblingIndex(originalIndex + 1); //it's gonna be all out of order with more than 2 people
                    enemyHealthText.GetComponent<RectTransform>().anchoredPosition = new Vector2(healthText.GetComponent<RectTransform>().anchoredPosition.x, yLevel);
                    enemyHealthText.name = "EnemyHealthText_" + (i + 1);
                    //enemyHealthText.GetComponent<TextMeshProUGUI>().text = "Enemy #" + (i + 1) + ": " + healths[i] + "/" + maxHealths[i];
                    healthTexts.Add(enemyHealthText);
                    enemyHealthText.SetActive(false);
                    yield return StartCoroutine(UpdateEnemyHealthText(i, healths[i], maxHealths[i]));
                    enemyHealthText.SetActive(enemyHealthsVisible[i]);
                } else {
                    healthTexts.Add(healthText);
                }
            }
            
        }
        InputAction sKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/s");
        sKeyAction.performed += ctx =>
        {
            //Debug.Log("S key pressed!");
            StartCoroutine(ToggleStatsBook(false, UnityEngine.Random.Range(0, totalItems) + 1));
        };
        sKeyAction.Enable();

        InputAction rightArrowAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/rightArrow");
        rightArrowAction.performed += ctx =>
        {
            StartCoroutine(FlipThruStatsBook(1));
            StartCoroutine(FlipThruRollChances(1));
            StartCoroutine(FlipThruBookInventory(1));
        };
        rightArrowAction.Enable();

        InputAction leftArrowAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/leftArrow");
        leftArrowAction.performed += ctx =>
        {
            StartCoroutine(FlipThruStatsBook(-1));
            StartCoroutine(FlipThruRollChances(-1));
            StartCoroutine(FlipThruBookInventory(-1));
        };
        leftArrowAction.Enable();

        if (!amSpectator) {
            InputAction iKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/i");
            iKeyAction.performed += ctx =>
            {
                //Debug.Log("I key pressed!");
                StartCoroutine(ToggleInventory());
            };
            iKeyAction.Enable();

            InputAction rKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/r");
            rKeyAction.performed += ctx =>
            {
                StartCoroutine(ReplayEnemyAction());
            };
            rKeyAction.Enable();
        }
        
        InputAction lKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/l");
        lKeyAction.performed += ctx =>
        {
            StartCoroutine(ToggleRollChances());
        };
        lKeyAction.Enable();

        InputAction bKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/b");
        bKeyAction.performed += ctx =>
        {
            StartCoroutine(ToggleBookInventory());
        };
        bKeyAction.Enable();

        if (!amSpectator) {
            InputAction cKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/c");
            cKeyAction.performed += ctx =>
            {
                StartCoroutine(ToggleCooldowns());
            };
            cKeyAction.Enable();
        }

        InputAction tKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/t");
        tKeyAction.performed += ctx =>
        {
            StartCoroutine(ToggleTurnsText());
        };
        tKeyAction.Enable();

        InputAction vKeyAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/v");
        vKeyAction.performed += ctx => 
        {
            StartCoroutine(ToggleVolume());
        };
        vKeyAction.Enable();
    }
    IEnumerator UpdateEnemyHealthText(int index, int hp, int maxHp) {
        if (index + 1 != myPlayerNum) {
            Debug.Log("healthTexts length: " + healthTexts.Count);
            if (healths.Count > 2) {
                healthTexts[index].GetComponent<TextMeshProUGUI>().text = "Enemy #" + (index + 1) + ": " + hp + "/" + maxHp;
            } else {
                healthTexts[index].GetComponent<TextMeshProUGUI>().text = "Enemy: " + hp + "/" + maxHp;
            }
            Debug.Log("Enemy health text set");
        }
        yield return null;
    }
    IEnumerator ShowEnemyHealthTexts(int mode) {
        for (int i = 0; i < enemyHealthsVisible.Count; i++) {
            if (i + 1 != myPlayerNum) {
                if (enemyHealthsVisible[i]) {
                    if (mode == 1) {
                        yield return StartCoroutine(UpdateEnemyHealthText(i, healths[i], maxHealths[i]));
                    }
                    healthTexts[i].SetActive(true);
                } else {
                    healthTexts[i].SetActive(false);
                }
            }
        }
    }
    int CountNonBots() {
        int count = 0;
        foreach (bool b in botList) {
            if (!b) {
                count++;
                Debug.Log("Player found");
            } else {
                Debug.Log("Bot found");
            }
        }
        count += spectatorList.Count; //all spectators are nonBots
        return count;
    }
    IEnumerator OpenInventory() {
        List<List<int>> ND = UsableNoDupe(INVENTORY);
        //UND -> ND
        List<int> invItems = ND[0];
        //FU2 -> invItems
        List<int> invItemsCount = ND[1];
        //FUCount -> invItemsCount
        GameObject invEmpty = inventoryDisplayScroll.transform.Find("InventoryEmpty").gameObject;
        GameObject invLabel = inventoryDisplayScroll.transform.Find("InventoryLabel").gameObject;
        //GameObject viewport = inventoryDisplayScroll.transform.Find("Viewport").gameObject;
        if (invItems.Count > 0) {
            //do stuff
            invEmpty.SetActive(false);
            invLabel.SetActive(true);
            //viewport.SetActive(true);
            for (int i = 0; i < invItems.Count; i++) {
                int index = invItems[i] - 1;
                if (itemIntLists[index][0].Contains(4)) {
                    int armorSlot = Mathf.RoundToInt(itemInfos[index][3][0]);
                    GameObject armorElement = null;
                    if (armorSlot == 1) {
                        armorElement = inventoryDisplayScroll.transform.Find("HeadArmor").gameObject;
                    } else if (armorSlot == 2) {
                        armorElement = inventoryDisplayScroll.transform.Find("BodyArmor").gameObject;
                    } else if (armorSlot == 3) {
                        armorElement = inventoryDisplayScroll.transform.Find("LegArmor").gameObject;
                    } else if (armorSlot == 4) {
                        armorElement = inventoryDisplayScroll.transform.Find("FootArmor").gameObject;
                    }

                    GameObject armorImage = armorElement.transform.Find("Image").gameObject;
                    armorImage.GetComponent<Image>().sprite = searchItemCostumes[index];
                    GameObject textObject = armorElement.transform.Find("Text (TMP)").gameObject;
                    TextMeshProUGUI elmtText = textObject.GetComponent<TextMeshProUGUI>();
                    if (elmtText != null) elmtText.text = itemStrings[index][0];
                    textObject.SetActive(true);
                    Vector2 currentAnchorMin = armorImage.GetComponent<RectTransform>().anchorMin;
                    currentAnchorMin.y = 0.5f;
                    armorImage.GetComponent<RectTransform>().anchorMin = currentAnchorMin;
                    GameObject CD = armorElement.transform.Find("Cooldown").gameObject;
                    CD.SetActive(false);
                    Button btn = armorElement.transform.Find("Hitbox").GetComponent<Button>();
                    btn.enabled = true;
                    btn.interactable = true;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        OnInventoryElementClick(index);
                    });
                    GameObject count = armorElement.transform.Find("Count").gameObject;
                    count.SetActive(false);
                } else {
                    GameObject newElement = Instantiate(elementPrefab, inventoryItemsTF);
                    newElement.name = "Button_" + i;

                    Image innerImage = newElement.transform.Find("Image").GetComponent<Image>();
                    innerImage.sprite = searchItemCostumes[index];

                    TextMeshProUGUI elmtText = newElement.GetComponentInChildren<TextMeshProUGUI>();
                    if (elmtText != null) elmtText.text = itemStrings[index][0];
                    GameObject CD = newElement.transform.Find("Cooldown").gameObject;
                    Button btn = newElement.transform.Find("Hitbox").GetComponent<Button>();
                    int cooldown = itemsOnCooldown[index];
                    if (cooldown == 0) {
                        CD.SetActive(false);
                    } else {
                        ColorBlock cb = btn.colors;
                        Color c1 = cb.normalColor;
                        Color ogc = c1;
                        c1.r = 1;
                        c1.g = 1;
                        c1.b = 1;
                        Color c2 = cb.highlightedColor;
                        Color c3 = cb.pressedColor;
                        c3.r += 1f - (ogc.r / 255f);
                        c3.g += 1f - (ogc.g / 255f);
                        c3.b += 1f - (ogc.b / 255f);
                        Color c4 = cb.selectedColor;
                        c4.r += 1f - (ogc.r / 255f);
                        c4.g += 1f - (ogc.g / 255f);
                        c4.b += 1f - (ogc.b / 255f);
                        Color c5 = cb.disabledColor;
                        c5.r = 1f;
                        c5.g = 1f;
                        c5.b = 1f;
                        cb.normalColor = c1;
                        cb.highlightedColor = c2;
                        cb.pressedColor = c3;
                        cb.selectedColor = c4;
                        cb.disabledColor = c5;
                        btn.colors = cb;
                        float alpha = c1.a;
                        Color c = newElement.GetComponent<Image>().color;
                        /*c.r = (c1.r * alpha) + ((90f / 255f) * (1f - alpha)); //same r value for cooldown in use
                        c.g = (c1.g * alpha) + ((90f / 255f) * (1f - alpha)); //same g value for cooldown in use
                        c.b = (c1.b * alpha) + ((90f / 255f) * (1f - alpha)); //same b value for cooldown in use*/
                        c.r = 90f / 255f;
                        c.g = 90f / 255f;
                        c.b = 90f / 255f;
                        newElement.GetComponent<Image>().color = c;
                        CD.SetActive(true);
                    }
                    btn.enabled = true;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        OnInventoryElementClick(index);
                    });

                    GameObject count = newElement.transform.Find("Count").gameObject;
                    if (invItemsCount[i] == 1) {
                        count.SetActive(false);
                    } else {
                        GameObject countOutline = count.transform.Find("CountOutline").gameObject;
                        GameObject countText = countOutline.transform.Find("CountText").gameObject;
                        countText.GetComponent<TextMeshProUGUI>().text = invItemsCount[i].ToString();
                        count.SetActive(true);
                    }
                }
                
            }

            
        } else {
            invEmpty.SetActive(true);
            invLabel.SetActive(false);
            //viewport.SetActive(false);
        }
        for (int i = 1; i <= 4; i++) {
            if (!InventoryContainsArmor(INVENTORY, i)) {
                //reset slot
                GameObject armorElement = null;
                if (i == 1) {
                    armorElement = inventoryDisplayScroll.transform.Find("HeadArmor").gameObject;
                } else if (i == 2) {
                    armorElement = inventoryDisplayScroll.transform.Find("BodyArmor").gameObject;
                } else if (i == 3) {
                    armorElement = inventoryDisplayScroll.transform.Find("LegArmor").gameObject;
                } else if (i == 4) {
                    armorElement = inventoryDisplayScroll.transform.Find("FootArmor").gameObject;
                }

                GameObject armorImage = armorElement.transform.Find("Image").gameObject;
                armorImage.GetComponent<Image>().sprite = armorDefaults[i - 1];
                GameObject textObject = armorElement.transform.Find("Text (TMP)").gameObject;
                TextMeshProUGUI elmtText = textObject.GetComponent<TextMeshProUGUI>();
                if (elmtText != null) elmtText.text = "";
                textObject.SetActive(false);
                Vector2 currentAnchorMin = armorImage.GetComponent<RectTransform>().anchorMin;
                currentAnchorMin.y = 0.1f;
                armorImage.GetComponent<RectTransform>().anchorMin = currentAnchorMin;
                GameObject CD = armorElement.transform.Find("Cooldown").gameObject;
                CD.SetActive(false);
                Button btn = armorElement.transform.Find("Hitbox").GetComponent<Button>();
                btn.enabled = true;
                btn.interactable = false;
                btn.onClick.RemoveAllListeners();
                GameObject count = armorElement.transform.Find("Count").gameObject;
                count.SetActive(false);
            }
        }
        inventoryDisplayScroll.SetActive(true);
        yield return null;
    }
    IEnumerator OpenBookInventory() {
        GameObject bookInvEmpty = bookInventoryDisplay.transform.Find("BookInventoryEmpty").gameObject;
        if (bookInventory.Count > 0) {
            bookInvEmpty.SetActive(false);
            for (int i = 0; i < bookInventory.Count; i++) {
                int index = bookInventory[i] - 1;
                GameObject newElement = Instantiate(elementPrefab, bookInventoryItemsTF);
                newElement.name = "Button_" + i;

                Image innerImage = newElement.transform.Find("Image").GetComponent<Image>();
                innerImage.sprite = bookItemCostumes[index];

                TextMeshProUGUI elmtText = newElement.GetComponentInChildren<TextMeshProUGUI>();
                if (elmtText != null) elmtText.text = bookItemStrings[index][0];
                GameObject CD = newElement.transform.Find("Cooldown").gameObject;
                Button btn = newElement.transform.Find("Hitbox").GetComponent<Button>();
                CD.SetActive(false);
                btn.enabled = true;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnBookInventoryElementClick(index);
                });

                GameObject count = newElement.transform.Find("Count").gameObject;
                count.SetActive(false);
            }
        } else {
            bookInvEmpty.SetActive(true);
        }
        bookInventoryPage = 1;
        yield return StartCoroutine(SetBookInventoryPage(bookInventoryPage));
        bookInventoryDisplay.SetActive(true);
        yield return null;
    }
    IEnumerator OpenCooldowns() {
        bool empty = true;
        for (int i = 0; i < itemStrings.Count; i++) {
            int cooldown = itemsOnCooldown[i];
            if (cooldown != 0) {
                empty = false;
                GameObject newElement = Instantiate(elementPrefab, cooldownItemsTF);
                newElement.name = "Button_" + i;

                Image innerImage = newElement.transform.Find("Image").GetComponent<Image>();
                innerImage.sprite = searchItemCostumes[i];

                TextMeshProUGUI elmtText = newElement.GetComponentInChildren<TextMeshProUGUI>();
                if (elmtText != null) elmtText.text = itemStrings[i][0];
                GameObject CD = newElement.transform.Find("Cooldown").gameObject;
                Button btn = newElement.transform.Find("Hitbox").GetComponent<Button>();
                if (cooldown == 0) {
                    CD.SetActive(false);
                } else {
                    ColorBlock cb = btn.colors;
                    Color c1 = cb.normalColor;
                    Color ogc = c1;
                    c1.r = 1;
                    c1.g = 1;
                    c1.b = 1;
                    Color c2 = cb.highlightedColor;
                    Color c3 = cb.pressedColor;
                    c3.r += 1f - (ogc.r / 255f);
                    c3.g += 1f - (ogc.g / 255f);
                    c3.b += 1f - (ogc.b / 255f);
                    Color c4 = cb.selectedColor;
                    c4.r += 1f - (ogc.r / 255f);
                    c4.g += 1f - (ogc.g / 255f);
                    c4.b += 1f - (ogc.b / 255f);
                    Color c5 = cb.disabledColor;
                    c5.r = 1f;
                    c5.g = 1f;
                    c5.b = 1f;
                    cb.normalColor = c1;
                    cb.highlightedColor = c2;
                    cb.pressedColor = c3;
                    cb.selectedColor = c4;
                    cb.disabledColor = c5;
                    btn.colors = cb;
                    float alpha = c1.a;
                    Color c = newElement.GetComponent<Image>().color;
                    /*c.r = (c1.r * alpha) + ((90f / 255f) * (1f - alpha)); //same r value for cooldown in use
                    c.g = (c1.g * alpha) + ((90f / 255f) * (1f - alpha)); //same g value for cooldown in use
                    c.b = (c1.b * alpha) + ((90f / 255f) * (1f - alpha)); //same b value for cooldown in use*/
                    c.r = 90f / 255f;
                    c.g = 90f / 255f;
                    c.b = 90f / 255f;
                    newElement.GetComponent<Image>().color = c;
                    CD.SetActive(true);
                }
                btn.enabled = true;
                btn.onClick.RemoveAllListeners();
                int index = i;
                btn.onClick.AddListener(() => {
                    OnInventoryElementClick(index);
                });

                GameObject count = newElement.transform.Find("Count").gameObject;
                if (cooldown == 0) {
                    count.SetActive(false);
                } else {
                    GameObject countOutline = count.transform.Find("CountOutline").gameObject;
                    GameObject countText = countOutline.transform.Find("CountText").gameObject;
                    countText.GetComponent<TextMeshProUGUI>().text = cooldown.ToString();
                    count.transform.Find("CountBackdrop").GetComponent<Image>().color = CD.transform.Find("CooldownBackdrop").GetComponent<Image>().color;
                    count.SetActive(true);
                }
            }
        }
        if (empty) {
            cooldownsDisplay.transform.Find("CooldownsEmpty").gameObject.SetActive(true);
            cooldownViewport.SetActive(false);
        } else {
            cooldownsDisplay.transform.Find("CooldownsEmpty").gameObject.SetActive(false);
            cooldownViewport.SetActive(true);
        }
        cooldownsDisplay.SetActive(true);
        yield return null;
    }
    public bool InventoryContainsArmor(List<int> inv, int armorSlot) {
        foreach (int item in inv) {
            int index = item - 1;
            if (itemIntLists[index][0].Contains(4)) {
                if (Mathf.RoundToInt(itemInfos[index][3][0]) == armorSlot) {
                    return true;
                }
            }
        }
        return false;
    }
    public float GetArmorCoefficient(List<int> inv) {
        float returnVal = 1;
        foreach (int item in inv) {
            int index = item - 1;
            if (itemIntLists[index][0].Contains(4)) {
                returnVal -= itemInfos[index][3][1];
            }
        }
        return returnVal;
    }
    IEnumerator CloseInventory() {
        inventoryDisplayScroll.SetActive(false);
        foreach (Transform child in inventoryItemsTF) {
            GameObject.Destroy(child.gameObject);
        }
        yield return null;
    }
    IEnumerator CloseBookInventory() {
        bookInventoryDisplay.SetActive(false);
        foreach (Transform child in bookInventoryItemsTF) {
            GameObject.Destroy(child.gameObject);
        }
        yield return null;
    }
    IEnumerator CloseCooldowns() {
        cooldownsDisplay.SetActive(false);
        foreach (Transform child in cooldownItemsTF) {
            GameObject.Destroy(child.gameObject);
        }
        yield return null;
    }
    IEnumerator ToggleInventory() {
        if (!useOptionsScroll.activeSelf) {
            if (inventoryOpen) {
                StartCoroutine(CloseInventory());
                inventoryOpen = false;
            } else {
                StartCoroutine(OpenInventory());
                inventoryOpen = true;
                if (bookInventoryOpen) {
                    yield return StartCoroutine(ToggleBookInventory());
                }
                if (cooldownsOpen) {
                    yield return StartCoroutine(ToggleCooldowns());
                }
            }
        }
        
        yield return null;
    }
    IEnumerator ToggleBookInventory() {
        if (bookInventoryOpen) {
            StartCoroutine(CloseBookInventory());
            bookInventoryOpen = false;
        } else {
            StartCoroutine(OpenBookInventory());
            bookInventoryOpen = true;
            if (inventoryOpen) {
                yield return StartCoroutine(ToggleInventory());
            }
            if (cooldownsOpen) {
                yield return StartCoroutine(ToggleCooldowns());
            }
        }
        yield return null;
    }
    IEnumerator ToggleCooldowns() {
        if (cooldownsOpen) {
            StartCoroutine(CloseCooldowns());
            cooldownsOpen = false;
        } else {
            StartCoroutine(OpenCooldowns());
            cooldownsOpen = true;
            if (inventoryOpen) {
                yield return StartCoroutine(ToggleInventory());
            }
            if (bookInventoryOpen) {
                yield return StartCoroutine(ToggleBookInventory());
            }
        }
        yield return null;
    }
    IEnumerator ToggleTurnsText() {
        if (turnsTextShown) {
            HideTurnsText();
            turnsTextShown = false;
        } else {
            ShowTurnsText(TURNS.Value);
            turnsTextShown = true;
        }
        yield return null;
    }
    bool volumeShown = false;
    IEnumerator ToggleVolume() {
        if (volumeShown) {
            volumeSlider.gameObject.SetActive(false);
            musicVolumeSlider.gameObject.SetActive(false);
            volumeShown = false;
        } else {
            volumeSlider.gameObject.SetActive(true);
            musicVolumeSlider.gameObject.SetActive(true);
            volumeShown = true;
        }
        yield return null;
    }
    public int statsBookPage = 0;
    IEnumerator FlipThruStatsBook(int f) {
        if (statsBookOpen) {
            statsBookPage += f;
            if (statsBookPage > totalItems) {
                statsBookPage = 1;
            }
            if (statsBookPage < 1) {
                statsBookPage = totalItems;
            }
            yield return StartCoroutine(SetStatsBookPage(statsBookPage));
        }
    }
    int rollChancesPage = 0;
    IEnumerator FlipThruRollChances(int f) {
        if (rollChancesOpen) {
            rollChancesPage += f;
            if (rollChancesPage > rollChancesCostumes.Length) {
                rollChancesPage = 1;
            }
            if (rollChancesPage < 1) {
                rollChancesPage = rollChancesCostumes.Length;
            }
            yield return StartCoroutine(SetRollChancesPage(rollChancesPage));
        }
    }
    int bookInventoryPage = 0;
    IEnumerator FlipThruBookInventory(int f) {
        if (bookInventoryOpen) {
            int initBIP = bookInventoryPage;
            bookInventoryPage += f;
            if (bookInventoryPage > bookItemCostumes.Length) {
                bookInventoryPage = 1;
            }
            if (bookInventoryPage < 1) {
                bookInventoryPage = bookItemCostumes.Length;
            }
            bookInventoryDisplay.transform.Find("BIParent").Find("BookItem").gameObject.SetActive(false);
            bookInventoryDisplay.transform.Find("BIDParent").Find("BookItemDescription").gameObject.SetActive(false);
            bool initBIEActive = bookInventoryDisplay.transform.Find("BookInventoryEmpty").gameObject.activeSelf;
            bookInventoryDisplay.transform.Find("BookInventoryEmpty").gameObject.SetActive(false);
            bookInventoryItemsTF.gameObject.SetActive(false);
            yield return StartCoroutine(FlipBook(bookInventoryDisplay.GetComponent<Image>(), bookInventoryPage - initBIP, 3));
            yield return StartCoroutine(SetBookInventoryPage(bookInventoryPage));
            bookInventoryDisplay.transform.Find("BIParent").Find("BookItem").gameObject.SetActive(true);
            bookInventoryDisplay.transform.Find("BIDParent").Find("BookItemDescription").gameObject.SetActive(true);
            bookInventoryDisplay.transform.Find("BookInventoryEmpty").gameObject.SetActive(initBIEActive);
            bookInventoryItemsTF.gameObject.SetActive(true);
        }
    }
    public GameObject statsBookItem;
    public GameObject statsBookDescription;
    float itemCoe = 4f;
    float descCoe = 0.75f;
    IEnumerator SetStatsBookPage(int c) {
        //statsBookItem.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Min(itemInfos[c - 1][6][0] * SBICoe, 300), Mathf.Min(itemInfos[c - 1][6][1] * SBICoe, 175));
        SetFormatSize(1, statsBookItem, c - 1, itemCoe, 300, 175, 1);
        statsBookItem.GetComponent<Image>().sprite = searchItemCostumes[c - 1];
        //statsBookDescription.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Min(itemInfos[c - 1][6][2] * SBDCoe, 300), Mathf.Min(itemInfos[c - 1][6][3] * SBDCoe, 200));
        SetFormatSize(1, statsBookDescription, c - 1, descCoe, 300, 200, 2);
        statsBookDescription.GetComponent<Image>().sprite = searchItemDescriptions[c - 1];
        yield return null;
    }
    void SetFormatSize(int mode, GameObject obj, int index, float coe, float constraintX, float constraintY, int mode2) {
        float x = 0;
        float y = 0;
        if (mode == 1) {
            if (mode2 == 1) {
                x = itemInfos[index][6][0];
                y = itemInfos[index][6][1];
            } else if (mode2 == 2) {
                x = itemInfos[index][6][2];
                y = itemInfos[index][6][3];
            }
        } else if (mode == 2) {
            if (index == -1) { //manually set for "You've mastered all skills!"
                if (mode2 == 1) {
                    x = 18;
                    y = 17;
                } else if (mode2 == 2) {
                    x = 360;
                    y = 80;
                }
            } else {
                if (mode2 == 1) {
                    x = bookItemFloatLists[index][0][0];
                    y = bookItemFloatLists[index][0][1];
                } else if (mode2 == 2) {
                    x = bookItemFloatLists[index][0][2];
                    y = bookItemFloatLists[index][0][3];
                }
            }
        }
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Min(x * coe, constraintX), Mathf.Min(y * coe, constraintY));
    }
    IEnumerator SetRollChancesPage(int c) {
        rollChances.transform.Find("LuckDescription").GetComponent<Image>().sprite = rollChancesCostumes[c - 1];
        yield return null;
    }
    IEnumerator SetBookInventoryPage(int c) {
        GameObject BI = bookInventoryDisplay.transform.Find("BIParent").Find("BookItem").gameObject;
        GameObject BID = bookInventoryDisplay.transform.Find("BIDParent").Find("BookItemDescription").gameObject;
        SetFormatSizeBook(BI, BID, c - 1);
        BI.GetComponent<Image>().sprite = bookItemCostumes[c - 1];
        BID.GetComponent<Image>().sprite = bookItemDescriptions[c - 1];
        yield return null;
    }
    IEnumerator ToggleStatsBook(bool openNoMatterWhat, int startCostume) {
        if (openNoMatterWhat) {
            statsBookPage = startCostume;
            StartCoroutine(SetStatsBookPage(startCostume));
            statsBook.SetActive(true);
            statsBookOpen = true;
        } else {
            if (statsBookOpen) {
                statsBook.SetActive(false);
                statsBookOpen = false;
            } else {
                statsBookPage = startCostume;
                StartCoroutine(SetStatsBookPage(startCostume));
                statsBook.SetActive(true);
                statsBookOpen = true;
                if (rollChancesOpen) {
                    yield return StartCoroutine(ToggleRollChances());
                }
            }
        }
        yield return null;
    }
    IEnumerator ToggleRollChances() {
        if (rollChancesOpen) {
            rollChances.SetActive(false);
            rollChancesOpen = false;
        } else {
            rollChancesPage = 1;
            StartCoroutine(SetRollChancesPage(1));
            rollChances.SetActive(true);
            rollChancesOpen = true;
            if (statsBookOpen) {
                yield return StartCoroutine(ToggleStatsBook(false, 0));
            }
        }
    }
    IEnumerator OpenAirdrop(int pos, int index, int mode) {
        GameObject airdrop = null;
        if (mode == 1) {
            airdrop = airdropGOs[index];
        } else if (mode == 2) {
            airdrop = Instantiate(AirDropPF, new Vector2(GRIDX[pos - 1], GRIDY[pos - 1]), Quaternion.identity);
            airdrop.transform.localScale = new Vector2(myPlayerObject.transform.localScale.x, myPlayerObject.transform.localScale.y); //general tile size
            airdrop.GetComponent<SpriteRenderer>().sprite = airdropCostumes[4];
            yield return new WaitForSeconds(0.2f);
        }
        StartCoroutine(PlaySound(sounds[58]));
        airdrop.GetComponent<SpriteRenderer>().sprite = airdropCostumes[5];
        float totalTime = 1.0f;
        float timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            yield return StartCoroutine(SetGhost(airdrop, 1f - progress));
        }
        Destroy(airdrop);
        yield return null;
    }
    IEnumerator AnimateAirdrop(int pos, int index) {
        StartCoroutine(CreateMessageText("Airdrop inbound!", 300, 300, 1, true));
        StartCoroutine(PlaySound(sounds[57]));
        GameObject airdrop = Instantiate(AirDropPF, new Vector2(GRIDX[pos - 1], GRIDY[pos - 1] + tileHeight), Quaternion.identity);
        if (airdropGOs[index] != null) {
            Destroy(airdropGOs[index]);
        }
        airdropGOs[index] = airdrop;
        airdrop.transform.localScale = new Vector2(myPlayerObject.transform.localScale.x, myPlayerObject.transform.localScale.y); //general tile size
        SpriteRenderer airdropSR = airdrop.GetComponent<SpriteRenderer>();
        int currentCostume = 0;
        airdropSR.sprite = airdropCostumes[currentCostume];

        /*int repeats = 10;
        for (int i = 0; i < repeats; i++) {
            Vector2 currentV2 = airdrop.transform.position;
            currentV2.y -= tileHeight / repeats;
            airdrop.transform.position = currentV2;
            yield return null;
        }*/
        float totalTime = 0.25f;
        float timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            Vector2 currentV2 = airdrop.transform.position;
            currentV2.y = GRIDY[pos - 1] + (tileHeight * (1f - progress));
            airdrop.transform.position = currentV2;
            yield return null;
        }
        for (int i = 0; i < 4; i++) {
            yield return new WaitForSeconds(0.2f);
            currentCostume++;
            airdropSR.sprite = airdropCostumes[currentCostume];
        }
        yield return null;
    }
    public void SetCharacterPosition(GameObject character, int pos)
    {
        Debug.Log(pos);
        character.transform.position = new Vector3(GRIDX[pos - 1], GRIDY[pos - 1], Camera.main.nearClipPlane);
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc()
    {
        Debug.Log("player ready");
        playersReady += 1;
        if (playersReady == playerAmount.Value)
        {
            if (IsServer)
            {
                //StartCoroutine(CreateOrderedPlayerList());
                TURN.Value = UnityEngine.Random.Range(1, playerList.Count + 1);
                StartCoroutine(FirstTurn());
                Debug.Log("First turn!");
            }
        }
    }
    IEnumerator FirstTurn() {
        yield return StartCoroutine(ClearActions(1, null));
        NewTurnClientRpc(0, TURN.Value, actions, new float[0], localDamages, new int[0], new int[0], 0, 0, false, TURNS.Value);
        //Debug.Log("First turn");
    }
    [ClientRpc]
    public void NewTurnClientRpc(int prevPlayer, int newTurn, float[] a, float[] BDD, float[] LDs, int[] TI, int[] SI, int hp, int maxHp, bool skipped, int turns)
    {
        StartCoroutine(NewTurn(prevPlayer, newTurn, a, BDD, LDs, TI, SI, hp, maxHp, skipped, turns));
    }
    int extraSearches;
    IEnumerator SearchUse()
    {
        yield return StartCoroutine(ClearActions(1, null));
        searching = false;
        searchButton.GetComponent<Image>().sprite = buttons[0];
        useButton.GetComponent<Image>().sprite = buttons[2];
        Color color = searchButton.GetComponent<Image>().color;
        color.a = (166f / 255f);
        searchButton.GetComponent<Image>().color = color;
        color = useButton.GetComponent<Image>().color;
        color = SetBrightness(color, 1);
        color.a = (166f / 255f);
        useButton.GetComponent<Image>().color = color;
        searchButton.SetActive(true);
        useButton.SetActive(true);
        buttonClicked = 0;
        if (!useDisabled) {
            while (buttonClicked == 0)
            {
                yield return null;
            }
        } else {
            while (buttonClicked == 0 || buttonClicked == 2) 
            {
                yield return null;
            }
        }
        if (replayable3 != 0) {
            replayable3 = 0;
            Debug.Log("replayable3 is false");
        }
        searchButton.SetActive(false);
        useButton.SetActive(false);
        if (buttonClicked == 1)
        {
            extraSearches = 0;
            extraSearches += Mathf.RoundToInt(FindHighestBookEffectAmt(bookInventory, 3));
            rejectedItems.Clear();
            yield return StartCoroutine(Search());
            //searchused = true;
        } else if (buttonClicked == 2) {
            StartCoroutine(UseHelper());
            /*if (Use()) {
                searchused = true;
                Debug.Log("searchused = true");
            }*/
        }
    }
    int FindFirstArmorInInv(List<int> inv, int armorSlot) {
        foreach (int item in inv) {
            int index = item - 1;
            if (itemIntLists[index][0].Contains(4)) {
                if (Mathf.RoundToInt(itemInfos[index][3][0]) == armorSlot) {
                    return item;
                }
            }
        }
        return 0;
    }
    List<int> rejectedItems = new List<int>();
    bool airdropClickable = false;
    int searchLoops = 35;
    IEnumerator Search() {
        bool isAirdrop = false;
        int searchYield = 0;
        bool dontConclude = false;
        List<int> airdropSearchYields = new List<int>();
        if (airdrops.Contains(myPosition)) {
            searching = true;
            isAirdrop = true;
            actions[0] = myPlayerNum;
            actions[28] = myPosition;
            int index = FindInIntList(airdrops, myPosition);
            yield return StartCoroutine(OpenAirdrop(myPosition, index, 1));
            airdrops.RemoveAt(index);
            airdropGOs.RemoveAt(index);
            actions[30] = airdrops.Count;
            if (index < prevAirdropIndex) {
                prevAirdropIndex--;
            }
            airdropClicked = 0;
            for (int i = 1; i <= 3; i++) {
                GameObject SIParent = airdropOptions[i - 1].transform.Find("SIParent").gameObject;
                GameObject SI = SIParent.transform.Find("SearchItem").gameObject;
                SIParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                //SI.GetComponent<RectTransform>().anchoredPosition = new Vector2(SI.GetComponent<RectTransform>().anchoredPosition.x, -190f);
                RectTransform AORT = airdropOptions[i - 1].GetComponent<RectTransform>();
                AORT.sizeDelta = new Vector2(AORT.sizeDelta.x, 500f);
            }
            airdropOptions[0].SetActive(false);
            airdropOptions[1].SetActive(false);
            airdropOptions[2].SetActive(false);
            airdropClickable = false;
            searchAirdrop.SetActive(true);
            
            List<int> viableItems = GetViableItems(INVENTORY, GRID[myPosition - 1], myEffects, true, airdropSearchYields, new List<int>());
            for (int i = 0; i < 3; i++) {
                viableItems = GetViableItems(INVENTORY, GRID[myPosition - 1], myEffects, true, airdropSearchYields, new List<int>());
                int airdropSearchYield = GetSearchYield(9, GRID[myPosition - 1], viableItems); //9 \D
                airdropSearchYields.Add(airdropSearchYield);
                GameObject airdropOption = airdropOptions[i];
                GameObject SIParent = airdropOption.transform.Find("SIParent").gameObject;
                GameObject SI = SIParent.transform.Find("SearchItem").gameObject;
                GameObject IDParent = airdropOption.transform.Find("IDParent").gameObject;
                GameObject ID = IDParent.transform.Find("ItemDescription").gameObject;
                //SI.GetComponent<RectTransform>().localScale = new Vector3(190, 190, 1);
                //SI.GetComponent<RectTransform>().anchoredPosition = new Vector2(SI.GetComponent<RectTransform>().anchoredPosition.x, -275f);
                SI.SetActive(true);
                ID.SetActive(false);
                airdropOption.SetActive(true);
                float slow = 0.01f;
                for (int j = 0; j < searchLoops; j++)
                {
                    if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) {
                        break;
                    }
                    int fakeSearchYieldIndex = viableItems[UnityEngine.Random.Range(0, viableItems.Count)];
                    SetFormatSize(1, SI, fakeSearchYieldIndex, itemCoe * (190f / 300f), 190, 190, 1); //coe on coe based on the normal size
                    SI.GetComponent<Image>().sprite = searchItemCostumes[fakeSearchYieldIndex];
                    StartCoroutine(PlaySound(sounds[30]));
                    // random costume
                    if (j > 15)
                    {
                        slow *= 1.25f;
                    }
                    yield return new WaitForSeconds(slow);
                }
                SetFormatSize(1, SI, airdropSearchYield - 1, itemCoe * (190f / 300f), 190, 190, 1); //coe on coe based on the normal size
                SI.GetComponent<Image>().sprite = searchItemCostumes[airdropSearchYield - 1];
                StartCoroutine(PlaySound(sounds[30]));
                slow *= 1.25f;
                if (!(Keyboard.current != null && Keyboard.current.spaceKey.isPressed)) {
                    yield return new WaitForSeconds(slow);
                }
                SetFormatSize(1, SI, airdropSearchYield - 1, itemCoe * (150f / 250f), 150, 150, 1); //coe on coe based on the normal size
                //SI.GetComponent<RectTransform>().localScale = new Vector3(150, 150, 1);
                //SI.GetComponent<RectTransform>().anchoredPosition = new Vector2(SI.GetComponent<RectTransform>().anchoredPosition.x, -190f);
                SIParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                IDParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.2f);
                SetFormatSize(1, ID, airdropSearchYield - 1, descCoe * (180f / 300f), 180, 180, 2); //coe on coe based on the normal size
                ID.GetComponent<Image>().sprite = searchItemDescriptions[airdropSearchYield - 1];
                ID.SetActive(true);
                StartCoroutine(PlaySound(sounds[31]));
            }
            yield return StartCoroutine(ChooseFromAirdrop(airdropSearchYields, result => {
                if (result) {
                    dontConclude = true;
                }
            }));
            while (dontConclude) {
                yield return StartCoroutine(ChooseFromAirdrop(airdropSearchYields, result => {
                    if (result) {
                        dontConclude = true;
                    } else {
                        dontConclude = false;
                    }
                }));
            }
            //searchused = true;
        } else {
            searching = true;
            //searchItem.GetComponent<RectTransform>().localScale = new Vector3(200, 200, 1);
            searchItem.GetComponent<Image>().GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            //searchItem.GetComponent<Image>().GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            SIParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            /*searchItem.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            searchItem.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);*/
            //searchItem.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
            searchFrame.SetActive(true);
            searchItem.SetActive(true);
            float slow = 0.01f;
            List<int> viableItems = GetViableItems(INVENTORY, GRID[myPosition - 1], myEffects, false, null, rejectedItems);
            int landRarity = GetLandRarity(GRID[myPosition - 1]);
            landRarity += GetLuck(myEffects, myEffectStrengths);
            searchYield = GetSearchYield(landRarity, GRID[myPosition - 1], viableItems);//UnityEngine.Random.Range(0, searchItemCostumes.Length) + 1; (better to use totalItems than searchItemCostumes.Length) //1+
            
            for (int i = 0; i < searchLoops; i++)
            {
                if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) {
                    break;
                }
                int fakeSearchYieldIndex = viableItems[UnityEngine.Random.Range(0, viableItems.Count)];
                SetFormatSize(1, searchItem, fakeSearchYieldIndex, itemCoe, 300, 175, 1);
                searchItem.GetComponent<Image>().sprite = searchItemCostumes[fakeSearchYieldIndex];
                StartCoroutine(PlaySound(sounds[30]));
                // random costume
                if (i > 15)
                {
                    slow *= 1.25f;
                }
                yield return new WaitForSeconds(slow);
            }
            SetFormatSize(1, searchItem, searchYield - 1, itemCoe, 300, 175, 1);
            searchItem.GetComponent<Image>().sprite = searchItemCostumes[searchYield - 1];
            StartCoroutine(PlaySound(sounds[30]));
            slow *= 1.25f;
            if (!(Keyboard.current != null && Keyboard.current.spaceKey.isPressed)) {
                yield return new WaitForSeconds(slow);
            }
            SetFormatSize(1, itemDescription, searchYield - 1, descCoe, 300, 200, 2);
            itemDescription.GetComponent<Image>().sprite = searchItemDescriptions[searchYield - 1];
            itemDescription.SetActive(true);
            StartCoroutine(PlaySound(sounds[31]));
            buttonClicked = 0;
            OKButton.GetComponent<Image>().sprite = buttons[4];
            XButton.GetComponent<Image>().sprite = buttons[6];
            OKButton.SetActive(true);
            XButton.SetActive(true);
            
            RectTransform frameRect = searchFrame.GetComponent<RectTransform>();
            RectTransform imageRect = searchItem.GetComponent<Image>().GetComponent<RectTransform>();
            /*float frameHeightWorld = 499f * searchFrame.GetComponent<RectTransform>().localScale.y;
            float imageHeightWorld = imageRect.rect.height * searchItem.GetComponent<Image>().transform.localScale.y;
            float maxY = (frameHeightWorld - imageHeightWorld) / 2f;
            Vector3 newLocalPosition = searchItem.GetComponent<Image>().transform.localPosition;
            newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, -maxY, maxY);
            searchItem.GetComponent<Image>().transform.localPosition = newLocalPosition;*/
            float frameHeight = frameRect.rect.height;
            float imageHeight = imageRect.rect.height;
            float maxY = (frameHeight - imageHeight) / 2f;
            Debug.Log(maxY);
            Vector2 anchoredPos = imageRect.anchoredPosition;
            anchoredPos.y = Mathf.Clamp(100f, -maxY, maxY);
            //searchItem.GetComponent<Image>().GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            //searchItem.GetComponent<Image>().GetComponent<RectTransform>().pivot = new Vector2(0.5f, -0.1f);
            SIParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, -0.1f);
            SetFormatSize(1, searchItem, searchYield - 1, itemCoe, 250, 150, 1);
            //searchItem.GetComponent<RectTransform>().localScale = new Vector3(150f, 150f, 1);
            while (buttonClicked == 0)
            {
                yield return null;
            }
            searchFrame.SetActive(false);
            searchItem.SetActive(false);
            itemDescription.SetActive(false);
            OKButton.SetActive(false);
            XButton.SetActive(false);
        
            if (buttonClicked == 3) {
                yield return StartCoroutine(AcceptSearch(searchYield, false, result => {
                    if (result) {
                        dontConclude = true;
                        StartCoroutine(Search());
                    }
                }));
            }
        }
        //Debug.Log(buttonClicked);
        if (!dontConclude) {
            searching = false;
            /*searchItem.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.75f);
            searchItem.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.75f);*/
            //searchItem.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
            if (extraSearches == 0) {
                searchused = true;
            } else {
                extraSearches--;
                if (extraSearches < 0) {
                    extraSearches = 0;
                }
                //rejectedItems.Clear() //only if you want the second search to repeat rejected items
                StartCoroutine(Search());
            }
        }
    }
    public bool IsPermanentWeapon(int index) {
        return (itemIntLists[index][0].Contains(1) && itemInts[index][2] == 0);
    }
    public int CountPermWeaponsInInv(List<int> inv) {
        int count = 0;
        foreach (int item in inv) {
            if (IsPermanentWeapon(item - 1)) {
                count++;
            }
        }
        return count;
    }
    IEnumerator ChooseFromAirdrop(List<int> airdropSearchYields, System.Action<bool> callback) {
        airdropOptions[0].SetActive(true);
        airdropOptions[1].SetActive(true);
        airdropOptions[2].SetActive(true);
        searchAirdrop.SetActive(true);
        airdropClicked = 0;
        airdropClickable = true;
        buttonClicked = 0;
        OKButton.GetComponent<Image>().sprite = buttons[4];
        XButton.GetComponent<Image>().sprite = buttons[6];
        //OKButton.SetActive(true);
        //XButton.SetActive(true);
        while (buttonClicked == 0) {
            for (int i = 1; i <= 3; i++) {
                RectTransform AORT = airdropOptions[i - 1].GetComponent<RectTransform>();
                if (airdropClicked == i) {
                    AORT.sizeDelta = new Vector2(AORT.sizeDelta.x, 600f);
                    GameObject SIParent = airdropOptions[i - 1].transform.Find("SIParent").gameObject;
                    GameObject SI = SIParent.transform.Find("SearchItem").gameObject;
                    SIParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.1f);
                    GameObject IDParent = airdropOptions[i - 1].transform.Find("IDParent").gameObject;
                    IDParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.5f);
                    //SI.GetComponent<RectTransform>().anchoredPosition = new Vector2(SI.GetComponent<RectTransform>().anchoredPosition.x, -225f);
                } else {
                    GameObject SIParent = airdropOptions[i - 1].transform.Find("SIParent").gameObject;
                    GameObject SI = SIParent.transform.Find("SearchItem").gameObject;
                    SIParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                    //SI.GetComponent<RectTransform>().anchoredPosition = new Vector2(SI.GetComponent<RectTransform>().anchoredPosition.x, -190f);
                    GameObject IDParent = airdropOptions[i - 1].transform.Find("IDParent").gameObject;
                    IDParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.2f);
                    AORT.sizeDelta = new Vector2(AORT.sizeDelta.x, 500f);
                }
            }
            if (airdropClicked != 0) {
                if (!OKButton.activeSelf) {
                    OKButton.SetActive(true);
                }
                if (!XButton.activeSelf) {
                    XButton.SetActive(true);
                }
            } else if (airdropClicked == 0) {
                if (OKButton.activeSelf) {
                    OKButton.SetActive(false);
                }
                if (XButton.activeSelf) {
                    XButton.SetActive(false);
                }
            }
            yield return null;
        }
        airdropClickable = false;
        searchAirdrop.SetActive(false);
        OKButton.SetActive(false);
        XButton.SetActive(false);
        bool dontConclude = false;
        if (buttonClicked == 3) {
            yield return StartCoroutine(AcceptSearch(airdropSearchYields[airdropClicked - 1], true, result => {
                if (result) {
                    dontConclude = true;
                }
            }));
        }
        callback(dontConclude);
    }
    public int CountNegativePermWeaponsInIntList(List<int> list) {
        int count = 0;
        foreach (int i in list) {
            if (i < 0) {
                int index = Math.Abs(i) - 1;
                if (IsPermanentWeapon(index)) {
                    count++;
                }
            }
        }
        return count;
    }
    IEnumerator MaxWeaponsReached(int index, bool isAirdrop, System.Action<bool> callback) {
        bool keep = true;
        OKButton2.GetComponent<Image>().sprite = buttons[4];
        GameObject AORText = acceptOrReroll.transform.Find("Text").gameObject;
        AORText.GetComponent<TMP_Text>().text = "You have the max amount of weapons! Would you like to switch?";
        if (isAirdrop) {
            AORText.GetComponent<TMP_Text>().text += " (Back to airdrop if no)";
            NoButtonMode = 2;
            NoButton.GetComponent<Image>().sprite = buttons[10];
        } else {
            AORText.GetComponent<TMP_Text>().text += " (Reroll if no)";
            NoButtonMode = 1;
            NoButton.GetComponent<Image>().sprite = buttons[8];
        }
        GameObject element = acceptOrReroll.transform.Find("Element").gameObject;
        Image innerImage = element.transform.Find("Image").GetComponent<Image>();
        innerImage.sprite = searchItemCostumes[index];

        TextMeshProUGUI elmtText = element.GetComponentInChildren<TextMeshProUGUI>();
        if (elmtText != null) elmtText.text = itemStrings[index][0];
        Button btn = element.transform.Find("Hitbox").GetComponent<Button>();
        
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => {
            OnInventoryElementClick(index);
        });
        btn.enabled = true;
        acceptOrReroll.SetActive(true);
        buttonClicked = 0;
        while (buttonClicked == 0)
        {
            yield return null;
        }
        acceptOrReroll.SetActive(false);
        if (buttonClicked == 8) {
            keep = true;
            yield return StartCoroutine(ChooseWeaponToDrop(INVENTORY));
            if (CWTDCLicked == 0) {
                yield return StartCoroutine(MaxWeaponsReached(index, isAirdrop, result => {
                    keep = result;
                }));
            } else {
                INVENTORY.Remove(CWTDCLicked);
            }
            //INVENTORY.Remove(FindFirstArmorInInv(INVENTORY, Mathf.RoundToInt(itemInfos[searchYield - 1][3][0])));
        } else if (buttonClicked == 9) {
            keep = false;
        }
        callback(keep);
    }
    int NoButtonMode = 0;
    IEnumerator AcceptSearch(int searchYield, bool isAirdrop, System.Action<bool> callback) {
        bool dontConclude = false;
        bool keep = true;
        if (itemIntLists[searchYield - 1][0].Contains(4)) {
            if (InventoryContainsArmor(INVENTORY, Mathf.RoundToInt(itemInfos[searchYield - 1][3][0]))) {
                OKButton2.GetComponent<Image>().sprite = buttons[4];
                
                GameObject AORText = acceptOrReroll.transform.Find("Text").gameObject;
                AORText.GetComponent<TMP_Text>().text = "You're already wearing " + itemStrings[FindFirstArmorInInv(INVENTORY, Mathf.RoundToInt(itemInfos[searchYield - 1][3][0])) - 1][0] + ", would you like to switch?";
                if (isAirdrop) {
                    AORText.GetComponent<TMP_Text>().text += " (Back to airdrop if no)";
                    NoButtonMode = 2;
                    NoButton.GetComponent<Image>().sprite = buttons[10];
                } else {
                    AORText.GetComponent<TMP_Text>().text += " (Reroll if no)";
                    NoButtonMode = 1;
                    NoButton.GetComponent<Image>().sprite = buttons[8];
                }
                GameObject element = acceptOrReroll.transform.Find("Element").gameObject;
                Image innerImage = element.transform.Find("Image").GetComponent<Image>();
                innerImage.sprite = searchItemCostumes[searchYield - 1];

                TextMeshProUGUI elmtText = element.GetComponentInChildren<TextMeshProUGUI>();
                if (elmtText != null) elmtText.text = itemStrings[searchYield - 1][0];
                Button btn = element.transform.Find("Hitbox").GetComponent<Button>();
                
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnInventoryElementClick(searchYield - 1);
                });
                btn.enabled = true;
                acceptOrReroll.SetActive(true);
                buttonClicked = 0;
                while (buttonClicked == 0)
                {
                    yield return null;
                }
                if (buttonClicked == 8) {
                    keep = true;
                    INVENTORY.Remove(FindFirstArmorInInv(INVENTORY, Mathf.RoundToInt(itemInfos[searchYield - 1][3][0])));
                } else if (buttonClicked == 9) {
                    keep = false;
                }
                acceptOrReroll.SetActive(false);
            }
        }
        if (IsPermanentWeapon(searchYield - 1)) {
            if (CountPermWeaponsInInv(INVENTORY) + itemInts[searchYield - 1][6] - CountNegativePermWeaponsInIntList(itemIntLists[searchYield - 1][6]) > maxWeapons) {
                yield return StartCoroutine(MaxWeaponsReached(searchYield - 1, isAirdrop, result => {
                    keep = result;
                }));
            }
        }
        if (keep) {
            if (itemInts[searchYield - 1][6] == 0) {
                if (!itemBools[searchYield - 1][0] && itemIntLists[searchYield - 1][2].Count > 0) {
                    StartCoroutine(ItemSound(searchYield - 1));
                }
                if (!itemBools[searchYield - 1][0] && itemIntLists[searchYield - 1][0].Contains(3) && itemInfos[searchYield - 1][2].Count > 0) {
                    if (itemInfos[searchYield - 1][2][0] == 1) {
                        yield return StartCoroutine(AddToEffects(searchYield - 1, myEffects, myEffectLengths, myEffectStrengths));
                    } else {
                        if (itemInfos[searchYield - 1][2][7] == 2) {
                            yield return StartCoroutine(SearchBook(2, searchYield - 1, 1, null));
                        }
                    }
                }
            }
            for (int i = 0; i < itemInts[searchYield - 1][6]; i++) {
                if (itemInts[searchYield - 1][3] == -1 || CountInIntList(INVENTORY, searchYield) + 1 <= itemInts[searchYield - 1][3]) {
                    INVENTORY.Add(searchYield);
                    if (!itemBools[searchYield - 1][0] && itemIntLists[searchYield - 1][2].Count > 0) {
                        StartCoroutine(ItemSound(searchYield - 1));
                    }
                    if (itemInts[searchYield - 1][6] > 0) {
                        StartCoroutine(PlaySound(sounds[29]));
                    }
                    if (!itemBools[searchYield - 1][0] && itemIntLists[searchYield - 1][0].Contains(3) && itemInfos[searchYield - 1][2].Count > 0) {
                        yield return StartCoroutine(AddToEffects(searchYield - 1, myEffects, myEffectLengths, myEffectStrengths));
                    }
                    
                    if (i < itemInts[searchYield - 1][6] - 1) {
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            List<int> AAU = itemIntLists[searchYield - 1][6];
            foreach (int a in AAU) {
                if (a < 0) {
                    INVENTORY.Remove(-a);
                }
            }
            armorCoefficient = GetArmorCoefficient(INVENTORY);
            if (inventoryOpen) {
                foreach (Transform child in inventoryItemsTF) {
                    GameObject.Destroy(child.gameObject);
                }
                StartCoroutine(OpenInventory());
            }
        } else {
            if (!isAirdrop) {
                rejectedItems.Add(searchYield);
            }
            dontConclude = true;
        }
        callback(dontConclude);
    }
    int CWTDCLicked = 0;
    IEnumerator ChooseWeaponToDrop(List<int> inv) {
        foreach (Transform child in CWTDTF) {
            GameObject.Destroy(child.gameObject);
        }
        CWTDCLicked = 0;
        for (int i = 0; i < inv.Count; i++) {
            int index = inv[i] - 1;
            if (IsPermanentWeapon(index)) {
                GameObject newElement = Instantiate(elementPrefab, CWTDTF);
                newElement.name = "Button_" + i;

                Image innerImage = newElement.transform.Find("Image").GetComponent<Image>();
                innerImage.sprite = searchItemCostumes[index];

                TextMeshProUGUI elmtText = newElement.GetComponentInChildren<TextMeshProUGUI>();
                if (elmtText != null) elmtText.text = itemStrings[index][0];
                GameObject CD = newElement.transform.Find("Cooldown").gameObject;
                Button btn = newElement.transform.Find("Hitbox").GetComponent<Button>();
                CD.SetActive(false);
                btn.enabled = true;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    OnCWTDClick(index);
                });

                GameObject count = newElement.transform.Find("Count").gameObject;
                count.SetActive(false);
            }
        }
        CWTD.SetActive(true);
        buttonClicked = 0;
        XButton2.GetComponent<Image>().sprite = buttons[6];
        Color color = XButton2.GetComponent<Image>().color;
        color.a = (166f / 255f);
        XButton2.GetComponent<Image>().color = color;
        XButton2.SetActive(true);
        while (CWTDCLicked == 0 && buttonClicked == 0) {
            yield return null;
        }
        CWTD.SetActive(false);
        XButton2.SetActive(false);
        yield return null;
    }
    public int GetLuck(List<int> effects, List<int> effectStrengths) {
        int returnVal = 0;
        for (int i = 0; i < effects.Count; i++) {
            if (effects[i] == 10) {
                returnVal += effectStrengths[i];
            }
        }
        return returnVal;
    }
    public int GetSearchYield(int landRarity, int terrain, List<int> viableItems) { //returns 1+
        Debug.Log("land rarity: " + landRarity);
        //List<int> viableItems = GetViableItems(inv, t, fx);
        List<int> weightedRarityList = GetWRL(landRarity);
        int selectedRarity = weightedRarityList[UnityEngine.Random.Range(0, weightedRarityList.Count)];
        List<int> itemsWithRarity = GetItemsWithRarity(viableItems, selectedRarity);
        while (itemsWithRarity.Count == 0) {
            selectedRarity = weightedRarityList[UnityEngine.Random.Range(0, weightedRarityList.Count)];
            itemsWithRarity = GetItemsWithRarity(viableItems, selectedRarity);
        }
        /*if (t == 5)
            return 32;*/
        for (int i = 0; i < viableItems.Count; i++) {
            int index = viableItems[i];
            List<int> terrainRequirements = itemIntLists[index][5];
            List<float> terrainChances = itemInfos[index][5];
            for (int j = 0; j < terrainChances.Count; j++) {
                if (terrainChances[j] != -1) {
                    if (terrain == terrainRequirements[j]) {
                        if (RandomChanceFloat(terrainChances[j])) {
                            return index + 1;
                        }
                    }
                }
            }
        }
        return itemsWithRarity[UnityEngine.Random.Range(0, itemsWithRarity.Count)] + 1;
    }
    public int GetBookSearchYield(int bookRarity, List<int> viableBookItems) { //returns 1+
        List<int> weightedRarityList = GetWRL(bookRarity);
        int selectedRarity = weightedRarityList[UnityEngine.Random.Range(0, weightedRarityList.Count)];
        List<int> bookItemsWithRarity = GetBookItemsWithRarity(viableBookItems, selectedRarity);
        while (bookItemsWithRarity.Count == 0) {
            selectedRarity = weightedRarityList[UnityEngine.Random.Range(0, weightedRarityList.Count)];
            bookItemsWithRarity = GetBookItemsWithRarity(viableBookItems, selectedRarity);
        }
        return bookItemsWithRarity[UnityEngine.Random.Range(0, bookItemsWithRarity.Count)] + 1;
    }
    List<int> GetItemsWithRarity(List<int> VI, int SR) { //returns 0+
        List<int> retList = new List<int>();
        foreach (int item in VI) {
            int rarity = itemInts[item][0];
            if (rarity == SR) {
                retList.Add(item);
            }
        }
        return retList;
    }
    List<int> GetBookItemsWithRarity(List<int> VBI, int SR) { //returns 0+
        List<int> retList = new List<int>();
        foreach (int item in VBI) {
            int rarity = bookItemInts[item][0];
            if (rarity == SR) {
                retList.Add(item);
            }
        }
        return retList;
    }
    List<int> GetWRL(int LR) {
        List<int> retList = new List<int>();
        List<int> weightedList = new List<int>();
        int maxRarity = 8; //Must change to accommodate new rarities
        for (int i = 1; i <= maxRarity; i++) { 
            int repeats = maxRarity + 1 - Math.Abs(LR - i);
            if (LR == 0 && i == 1) {
                repeats += maxRarity + 1;
            }
            if (repeats < 0) {
                repeats = 0;
            }
            if (i <= LR) {
                repeats += maxRarity + 1 - Math.Abs(LR - i);
                //repeats = repeats * repeats;
            }
            repeats = repeats * repeats;
            weightedList.Add(repeats);
            for (int j = 0; j < repeats; j++) {
                retList.Add(i);
            }
        }
        int sum = 0;
        foreach (int r in weightedList) {
            sum += r;
        }
        /*Debug.Log("Common: " + ((float) weightedList[0] / sum).ToString("P1"));
        Debug.Log("Uncommon: " + ((float) weightedList[1] / sum).ToString("P1"));
        Debug.Log("Rare: " + ((float) weightedList[2] / sum).ToString("P1"));
        Debug.Log("More Rare: " + ((float) weightedList[3] / sum).ToString("P1"));
        Debug.Log("Epic: " + ((float) weightedList[4] / sum).ToString("P1"));
        Debug.Log("Very Epic: " + ((float) weightedList[5] / sum).ToString("P1"));
        Debug.Log("Legendary: " + ((float) weightedList[6] / sum).ToString("P1"));
        Debug.Log("Mythic: " + ((float) weightedList[7] / sum).ToString("P1"));*/
        return retList;
    }
    public List<int> GetViableItems(List<int> inv, int terrain, List<int> fx, bool airdrop, List<int> airdropSearchYields, List<int> RI) { //returns 0+
        List<int> retList = new List<int>();
        for (int i = 0; i < itemStrings.Count; i++) {
            bool addable = true;
            if (itemInts[i][6] == 0) {
                //retList.Add(i);
                //addable still true
            } else { //assuming itemInts[i][6] >= 1
                int maxStack = itemInts[i][3];
                if (maxStack == -1) {
                    //retList.Add(i);
                    //addable still true
                } else {
                    int countInInv = CountInIntList(inv, i + 1);
                    if (countInInv < maxStack) {
                        //retList.Add(i);
                        //addable still true
                    } else {
                        addable = false;
                    }
                }
            }
            if (itemInts[i][0] <= 0) {
                addable = false;
            }
            List<int> terrainReqs = itemIntLists[i][5];
            if (!terrainReqs.Contains(terrain)) {
                bool mapContainsOne = false;
                foreach (int t in terrainReqs) {
                    if (GRID.Contains(t)) {
                        mapContainsOne = true;
                    }
                }
                if (mapContainsOne) {
                    addable = false; 
                }
            }
            List<int> prereqs = itemIntLists[i][7];
            foreach (int p in prereqs) {
                if (!inv.Contains(p)) {
                    addable = false;
                }
            }
            List<int> effectsExclude = itemIntLists[i][8];
            foreach (int e in effectsExclude) {
                if (fx.Contains(e)) {
                    addable = false;
                }
            }
            if (airdrop) {
                if (airdropSearchYields.Contains(i + 1)) {
                    addable = false;
                }
            }
            if (TURNS.Value <= 4) {
                if (itemIntLists[i][0].Contains(1)) {
                    if (itemInfos[i][0][1] * itemInfos[i][0][3] > 25) {
                        addable = false;
                    }
                }
            }
            if (RI.Contains(i + 1)) {
                addable = false;
            }
            if (addable) {
                retList.Add(i);
            }
        }
        return retList;
    }
    public List<int> GetViableBookItems(List<int> bookInv) { //returns 0+
        List<int> retList = new List<int>();
        for (int i = 0; i < bookItemStrings.Count; i++) {
            bool addable = true;
            if (bookInv.Contains(i + 1)) {
                addable = false;
            }
            foreach (int prereq in bookItemIntLists[i][0]) {
                if (!bookInv.Contains(prereq)) {
                    addable = false;
                }
            }

            if (addable) {
                retList.Add(i);
            }
        }
        return retList;
    }
    public int GetLandRarity(int t) {
        if (t == 1)
            return 1;
        else if (t == 2)
            return 0;
        else if (t == 3) 
            return 1;
        else if (t == 4)
            return 0;
        else if (t == 5)
            return 2;
        else if (t == 6)
            return 5;
        else if (t == 7)
            return 6;
        else if (t == 8)
            return 7;
        else if (t == 9)
            return 0;
        return 0;
    }
    IEnumerator UseHelper() {
        yield return StartCoroutine(Use(result => {
            if (result) {
                searchused = true;
                //Debug.Log("searchused = true");
            }
        }));
    }
    List<List<int>> UsableNoDupe(List<int> FU) {
        List<int> noDupe = new List<int>(FU);
        List<int> noDupeCount = new List<int>();
        for (int i = 0; i < noDupe.Count; i++) {
            int amount = CountInIntList(noDupe, noDupe[i]);
            noDupeCount.Add(amount);
            for (int j = i + 1; j < noDupe.Count; j++) {
                if (noDupe[i] == noDupe[j]) {
                    noDupe.RemoveAt(j);
                    j--;
                }
            }
        }
        return new List<List<int>> {noDupe, noDupeCount};
    }
    bool goBackToUse;
    IEnumerator Use(System.Action<bool> callback) {
        bool success = false;
        List<int> FU = FindUsable(INVENTORY);
        List<List<int>> UND = UsableNoDupe(FU);
        List<int> FU2 = UND[0];
        List<int> FUCount = UND[1];
        if (FU2.Count > 0) {
            //do stuff
            foreach (Transform child in useOptionsTF) {
                GameObject.Destroy(child.gameObject);
            }
            for (int i = 0; i < FU2.Count; i++) {
                int index = FU2[i] - 1;
                GameObject newElement = Instantiate(elementPrefab, useOptionsTF);
                newElement.name = "Button_" + i;

                Image innerImage = newElement.transform.Find("Image").GetComponent<Image>();
                innerImage.sprite = searchItemCostumes[index];

                TextMeshProUGUI elmtText = newElement.GetComponentInChildren<TextMeshProUGUI>();
                if (elmtText != null) elmtText.text = itemStrings[index][0];
                GameObject CD = newElement.transform.Find("Cooldown").gameObject;
                Button btn = newElement.transform.Find("Hitbox").GetComponent<Button>();
                if (itemsOnCooldown[index] == 0) {
                    btn.enabled = true;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        OnElementClick(index);
                    });
                    CD.SetActive(false);
                } else {
                    Color c = newElement.GetComponent<Image>().color;
                    c.r = 90f / 255f;
                    c.g = 90f / 255f;
                    c.b = 90f / 255f;
                    newElement.GetComponent<Image>().color = c;
                    btn.enabled = false;
                    CD.SetActive(true);
                }
                

                GameObject count = newElement.transform.Find("Count").gameObject;
                if (FUCount[i] == 1) {
                    count.SetActive(false);
                } else {
                    GameObject countOutline = count.transform.Find("CountOutline").gameObject;
                    GameObject countText = countOutline.transform.Find("CountText").gameObject;
                    countText.GetComponent<TextMeshProUGUI>().text = FUCount[i].ToString();
                    count.SetActive(true);
                }
            }
            if (inventoryOpen) {
                StartCoroutine(CloseInventory());
                inventoryOpen = false;
            }
            useOptionsScroll.SetActive(true);
            buttonClicked = 0;
            XButton2.GetComponent<Image>().sprite = buttons[6];
            Color color = XButton2.GetComponent<Image>().color;
            color.a = (166f / 255f);
            XButton2.GetComponent<Image>().color = color;
            XButton2.SetActive(true);
            while (!(buttonClicked == 5 || buttonClicked == 6))
                yield return null;
            useOptionsScroll.SetActive(false);
            XButton2.SetActive(false);
            if (buttonClicked == 5) {
                StartCoroutine(SearchUse());
                success = false;
            } else if (buttonClicked == 6) {
                goBackToUse = false;
                yield return StartCoroutine(UseItem(useItem, actions, bulDirDeg, trapIndexes, smokeIndexes, 1, null, myPosition, myPlayerNum, new Vector2(), 0, 0, null, result => {
                    if (result) {
                        success = true;
                        //Debug.Log("success!");
                    } else {
                        if (goBackToUse) {
                            StartCoroutine(UseHelper());
                        } else {
                            StartCoroutine(SearchUse());
                        }
                        success = false;
                    }
                }));
            }
            
            //success = true;
        } else {
            useDisabled = true;
            StartCoroutine(CreateMessageText("You have no usable items!", 200, 200, 1, false));
            StartCoroutine(ShakeUse());
            StartCoroutine(FlashUse());
            StartCoroutine(SearchUse());
            success = false;
        }
        //Debug.Log("Success2!");
        callback(success);
        yield return null;
    }
    /*bool Use() {
        bool success = false;
        if (FindUsable(INVENTORY) > 0) {
            //do stuff
            success = true;
        } else {
            useDisabled = true;
            StartCoroutine(ShakeUse());
            StartCoroutine(FlashUse());
            StartCoroutine(SearchUse());
            success = false;
        }
        return success;
    }*/
    float[] actions;
    [SerializeField]
    float[] localDamages;
    /* 
    0. playerNum (1+)
    1. useItem (1+)
    2. fire num
    3. fire rate
    4. damage
    5. range
    6. speed
    7. elevation
    */
    int CTOTGrid(List<List<List<float>>> trapsList, int type, int owner) {
        int count = 0;
        foreach (List<List<float>> t in trapsList) {
            bool alrAdded = false;
            foreach (List<float> l in t) {
                if (Mathf.RoundToInt(l[1]) == type && Mathf.RoundToInt(l[0]) == owner) {
                    if (!alrAdded) {
                        count++;
                        alrAdded = true;
                    }
                }
            }
        }
        return count;
    }
    int CountTrapsOfType(List<List<List<float>>> trapsList, int type, int owner) {
        int count = 0;
        foreach (List<List<float>> t in trapsList) {
            foreach (List<float> l in t) {
                if (Mathf.RoundToInt(l[1]) == type && Mathf.RoundToInt(l[0]) == owner) {
                    count++;
                }
            }
        }
        return count;
    }
    List<int> FindFirstTrapOfType(List<List<List<float>>> trapsList, int type, int owner) {
        List<int> retList = new List<int>();
        for (int i = 0; i < trapsList.Count; i++) {
            List<List<float>> listTraps = trapsList[i];
            int j = FFTOTIndex(listTraps, type, owner);
            if (j != -1) {
                retList.Add(i);
                retList.Add(j);
                return retList;
            }
        }
        return null;
    }
    int FFTOTIndex(List<List<float>> listTraps, int type, int owner) {
        for (int i = 0; i < listTraps.Count; i++) {
            List<float> l = listTraps[i];
            if (Mathf.RoundToInt(l[0]) == owner && Mathf.RoundToInt(l[1]) == type) {
                return i;
            }
        }
        return -1; 
    }
    public int GetIsBomb(int index) {
        int bomb = 0;
        if (itemIntLists[index][1].Contains(2)) bomb = 1;
        if (itemIntLists[index][1].Contains(16)) bomb = 2;
        return bomb;
    }
    List<float> bulDirDeg = new List<float>();
    List<int>  trapIndexes = new List<int>();
    List<int> smokeIndexes = new List<int>();
    List<int> bulStops = new List<int>();
    List<int> bombStops = new List<int>();
    float localDamage;
    public IEnumerator UseItem(int index, float[] a, List<float> b, List<int> ti, List<int> si, int mode, BotScript BS, int myP, int myPnum, Vector2 targetPos, int target, int botUAAT, List<float> FMDD, System.Action<bool> callback) {
        /* mode 1 is player, mode 2 is bot*/
        a[0] = myPnum;
        a[1] = index + 1;
        bool stillPlaySound = true;
        if (mode == 2) {
            stillPlaySound = false;
        }
        bool success = false;
        int useAtATime = 1;
        if (mode == 1) {
            if (itemInts[index][7] >= 0) {
                StartCoroutine(PlaySound(sounds[itemInts[index][7]]));
            }
        }
        bombStops = new List<int>();
        int triggerTrap = 0;
        if (itemIntLists[index][1].Contains(17)) triggerTrap = 1;
        if (mode == 1) {
            if (triggerTrap == 1) {
                int countTriggerables = CTOTGrid(traps, 2, myPlayerNum);
                List<int> foundTriggerable = null;
                if (countTriggerables == 0) {
                    StartCoroutine(CreateMessageText("You have no traps to trigger, how'd you even get this detonator?", 300, 200, 1, false));
                    //success is false (or at least not true) (meaning you don't set it to true)
                } else if (countTriggerables == 1) {
                    foundTriggerable = FindFirstTrapOfType(traps, 2, myPlayerNum);
                } else {
                    StartCoroutine(CreateMessageText("Choose which trap you want to trigger!", 300, 200, 2, false));
                    while (!(Mouse.current.leftButton.wasPressedThisFrame && mouseIndex != 0 && FFTOTIndex(traps[mouseIndex - 1], 2, myPlayerNum) != -1))
                        yield return null;
                    foundTriggerable = new List<int>();
                    foundTriggerable.Add(mouseIndex - 1);
                    foundTriggerable.Add(FFTOTIndex(traps[mouseIndex - 1], 2, myPlayerNum));
                }
                if (foundTriggerable == null) {
                    Debug.Log("foundTriggerable is null!");
                } else {
                    List<float> currentTrap = traps[foundTriggerable[0]][foundTriggerable[1]];
                    yield return StartCoroutine(PlaySoundAndWait(sounds[49]));
                    yield return StartCoroutine(ExplodeBomb(foundTriggerable[0] + 1, currentTrap[3], currentTrap[2], 0, new List<int>(), myPlayerNum, 1, 1, false, 1, null));
                    traps[foundTriggerable[0]].RemoveAt(foundTriggerable[1]);
                    yield return StartCoroutine(RenderTraps(traps));
                    a[25] = foundTriggerable[0]; //0+
                    a[26] = foundTriggerable[1]; //0+
                    success = true;
                }
            }
        } else if (mode == 2) {
            if (triggerTrap == 1) {
                int index1 = Mathf.RoundToInt(FMDD[1]);
                int index2 = Mathf.RoundToInt(FMDD[2]);
                BS.botTraps[index1].RemoveAt(index2);
                a[25] = index1; //0+
                a[26] = index2; //0+
                success = true;
            }
        }
        if (itemIntLists[index][0].Contains(1)) {
            stillPlaySound = false;
            //Debug.Log("Gun used");
            Vector2 targetV = new Vector2(0, 0);
            if (mode == 2) {
                targetV = targetPos;
            }
            if (mode == 1) {
                yield return StartCoroutine(VisionHouse(itemInfos[index][0][8], smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
            } else if (mode == 2) {
                yield return StartCoroutine(BS.BotVisionHouse(false, 0));
                /*Vector2? potentialNewTarget = BS.ChangeTarget(target);
                if (potentialNewTarget != null) {
                    targetV = potentialNewTarget.Value;
                }*/
            }
            
            /*for (int i = 0; i < playerList.Count; i++)
            {
                if (i + 1 != myPlayerNum)
                {
                    StartCoroutine(EnemyVisible(playerList[i], playerPositionList[i], enemyEffects[i].Contains(1)));
                }
            }*/
            int bomb = GetIsBomb(index);
            int knife = 0;
            if (itemIntLists[index][1].Contains(3)) knife = 1;
            int smoke = 0;
            if (itemIntLists[index][1].Contains(8)) smoke = 1;
            int stunMode = 0;
            if (itemIntLists[index][1].Contains(9)) stunMode = Mathf.RoundToInt(itemInfos[index][2][5]);
            int trap = 0;
            if (itemIntLists[index][1].Contains(13)) trap = 1;
            int changeLoc = 0;
            if (itemIntLists[index][1].Contains(15)) changeLoc = 1;
            int suppressed = 0;
            if (Mathf.RoundToInt(itemInfos[index][0][7]) == 1) suppressed = 1;
            if (mode == 1) {
                if (itemIntLists[index][1].Contains(1)) {
                    StartCoroutine(CreateMessageText("Click where you'd like to shoot", 300, 200, 2, false));
                }
                if (bomb == 1 || bomb == 2 || smoke == 1 || stunMode != 0) {
                    StartCoroutine(CreateMessageText("Click where you'd like to throw", 300, 200, 2, false));
                }
                if (knife == 1) {
                    StartCoroutine(CreateMessageText("Click where you'd like to swing", 300, 200, 2, false));
                }
                //Debug.Log("Click where you'd like to ...");
            }
            //Debug.Log("Mode: " + mode);
            if (mode == 1) {
                while (!(Mouse.current.leftButton.wasPressedThisFrame && mouseIndex != 0))
                    yield return null;
            }
            //float targetX = mouseGSX;
            //float targetY = mouseGSY;
            
            if (mode == 1) {
                targetV = new Vector2(mouseGSX, mouseGSY);
            }
            Vector2 myV = new Vector2(GRIDX[myP - 1], GRIDY[myP - 1]);
            Vector2 diff = targetV - myV;
            float angleRadians = Mathf.Atan2(diff.y, diff.x);
            float myElevation = FindElevation(GRID[myP - 1], 1);
            bulStops = new List<int>();
            //bombStops = new List<int>();
            bulDone = 0;
            
            int ogMode = 1;
            if (mode == 2) {
                ogMode = 4;
            }
            int hitSound = 1;
            if (index == 5) hitSound = 13;
            if (index == 12) hitSound = -1;
            if (mode == 1) {
                localDamage = 0;
            } else {
                BS.LD = 0;
            }
            
            for (int i = 0; i < itemInfos[index][0][3]; i++) {
                float angleDeg = angleRadians * Mathf.Rad2Deg + (UnityEngine.Random.Range(-1f, 1f) * itemInfos[index][0][2]);
                b.Add(angleDeg);
                GameObject bul = Instantiate(bulletPrefab, myV, Quaternion.Euler(0f, 0f, angleDeg));
                if (mode == 1) {
                    bul.GetComponent<SpriteRenderer>().sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][0])];
                } else {
                    bul.GetComponent<SpriteRenderer>().enabled = false;
                }
                
                bul.transform.localScale = new Vector3(tileWidth * 5, tileHeight * 5, 1);
                StartCoroutine(BulletMove(index, bul, angleDeg, itemInfos[index][0][4], itemInfos[index][0][0], myElevation + itemInfos[index][0][12], myP, myPnum, ogMode, itemInfos[index][0][1], Mathf.RoundToInt(itemInfos[index][0][13]), bomb, itemInfos[index][0][10], itemInfos[index][0][9], itemInfos[index][0][5], diff.magnitude, knife, hitSound, smoke, stunMode, trap, changeLoc, 0, 0, suppressed, false, false, BS));
                //Debug.Log("bulmove");
                if (mode == 1) {
                    StartCoroutine(ItemSound(index));
                    StartCoroutine(ItemCostume(index, bul, i));
                }
                yield return new WaitForSeconds(itemInfos[index][0][11]);
            }
            a[2] = itemInfos[index][0][3];
            a[3] = itemInfos[index][0][11];
            a[4] = itemInfos[index][0][1];
            a[5] = itemInfos[index][0][0];
            a[6] = itemInfos[index][0][4];
            a[7] = myElevation + itemInfos[index][0][12];
            a[8] = itemIntLists[index][3][0];
            a[9] = itemInfos[index][0][13];
            a[10] = bomb;
            a[11] = itemInfos[index][0][10];
            a[12] = itemInfos[index][0][9];
            a[13] = itemInfos[index][0][5];
            a[14] = diff.magnitude;
            a[15] = knife;
            a[16] = hitSound;
            a[17] = smoke;
            a[18] = stunMode;
            a[19] = trap;
            a[20] = changeLoc;
            a[21] = myP;
            //a[22] is myPosition (updated)
            a[23] = localDamage;
            if (mode == 2) {
                a[23] = BS.LD;
                Debug.Log("a[23] mode is 2");
            }
            Debug.Log("a[23] = " + a[23]);
            //a[24] is triggerTrap
            //a[25 and 26] are related to triggerTrap
            a[27] = suppressed;
            //a[28] is for airdrop
            //a[29] is playerNum no matter what
            //a[30] is for number of airdrops
            a[31] = smoke;
            if (mode == 1) {
                while (bulDone != itemInfos[index][0][3]) {
                    yield return null;
                }
            } else if (mode == 2) {
                while (BS.BD != itemInfos[index][0][3]) {
                    //Debug.Log("Waiting for BS.BD");
                    //Debug.Log("BS.BD = " + BS.BD + ", firenum = " + itemInfos[index][0][3]);
                    yield return null;
                }
            }
            if (mode == 1) {
                if (localDamages[myPlayerNum - 1] != 0) {
                    Debug.Log("ERROR: I damaged myself for " + localDamages[myPlayerNum - 1] + " damage");
                }
            } else if (mode == 2) {
                if (BS.myLocalDamages[BS.playerNum - 1] != 0) {
                    Debug.Log("ERROR: Bot damaged itself for " + BS.myLocalDamages[BS.playerNum - 1] + " damage");
                }
            }
            a[22] = myPosition;
            if (mode == 2) {
                a[22] = BS.position;
                Debug.Log("BSposition: " + BS.position);
            }
            if (mode == 1) {
                if (stunMode == 0 && bomb == 0 && smoke == 0 && changeLoc == 0) {
                    if (bulStops.Contains(3)) {
                        int shotsHit = CountInIntList(bulStops, 3);
                        String msg;
                        if (itemInfos[index][0][3] == 1) {
                            msg = "You hit " + shotsHit + " out of " + itemInfos[index][0][3] + " shot!";
                        } else {
                            msg = "You hit " + shotsHit + " out of " + itemInfos[index][0][3] + " shots!";
                        }
                        StartCoroutine(CreateMessageText(msg, 0, -100, 1, false));
                    }
                    else {
                        StartCoroutine(CreateMessageText("You missed...", 0, -100, 1, false));
                    }
                } 
                if (bomb == 1 && triggerTrap == 0) {
                    if (trap == 0) {
                        yield return StartCoroutine(BombText());
                    }
                }
                if (smoke == 1) {
                    StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
                }
                if (trap == 1) {
                    StartCoroutine(CreateMessageText("Trap has been planted", 200, 100, 1, false));
                    //StartCoroutine(PlaySound(sounds[37]));
                }
            } else if (mode == 2) {
                //improve to be more precise
                if (bulStops.Contains(3)) {
                    int shotsHit = CountInIntList(bulStops, 3);
                    BS.botBrain.BotShootFeedback(shotsHit / itemInfos[index][0][3], columns.Value, rows.Value);
                }
                else {
                    BS.botBrain.BotShootFeedback(-0.25f, columns.Value, rows.Value);
                }
            }
            success = true;
        }
        a[24] = triggerTrap;
        
        if (triggerTrap != 0) {
            if (!itemIntLists[index][1].Contains(13)) {
                if (success) {
                    if (mode == 1) {
                        yield return StartCoroutine(BombText());
                    } else if (mode == 2) {
                        if (bombStops.Count > 0) {
                            int bombsHit = bombStops.Count;
                            BS.botBrain.BotShootFeedback((float)bombsHit / (playerPositionList.Count - 1), columns.Value, rows.Value); //amount of enemies, might be different if on teams
                        } else {
                            BS.botBrain.BotShootFeedback(-0.25f, columns.Value, rows.Value);
                        }
                    }
                }
            }
        }
        if (itemIntLists[index][0].Contains(2)) {
            int healing = Mathf.RoundToInt(itemInfos[index][1][0]);
            if (mode == 1) {
                healing += Mathf.RoundToInt(FindHighestBookEffectAmt(bookInventory, 2));
            } else if (mode == 2) {
                healing += Mathf.RoundToInt(FindHighestBookEffectAmt(BS.botBookInventory, 2));
            }
            int maxHealthChange = Mathf.RoundToInt(itemInfos[index][1][2]);
            if (mode == 1) {
                bool success2 = true;
                if (!(itemIntLists[index][0].Contains(1) || itemIntLists[index][0].Contains(3))) { //keep filling as more classes appear
                    if (maxHealthChange == 0) {
                        if (Mathf.RoundToInt(itemInfos[index][1][1]) == 1) {
                            if (HEALTH >= maxHealth) {
                                StartCoroutine(CreateMessageText("You're already at max health!", 200, 150, 1, false));
                                success2 = false;
                                success = false;
                            }
                        }
                    }
                }
                if (success2) {
                    int maxUAAT = itemInts[index][5];
                    int countInInv = CountInIntList(INVENTORY, index + 1);
                    //maxUAAT = min(itemInts[index][5], countInInv)
                    if (itemInts[index][5] == -1) {
                        maxUAAT = countInInv;
                    } else {
                        if (itemInts[index][5] < countInInv)
                            maxUAAT = itemInts[index][5];
                        else
                            maxUAAT = countInInv;
                    }
                    if (maxHealthChange == 0) {
                        int maxFit = (int) Math.Ceiling(((double) maxHealth - (double) HEALTH) / (double) healing);
                        //maxUAAT = min(maxUAAT, maxFit)
                        if (maxUAAT < maxFit) 
                            maxUAAT = maxUAAT;
                        else 
                            maxUAAT = maxFit;
                    }
                    bool success3 = true;
                    if (maxUAAT > 1) {
                        //you can choose [1, maxUAAT]
                        buttonClicked = 0;
                        UAATSlider.minValue = 1;
                        UAATSlider.maxValue = maxUAAT;
                        UAATSlider.wholeNumbers = true;
                        UAATSlider.value = UAATSlider.minValue;
                        UpdateSliderText(UAATSlider.value);
                        UAATSlider.enabled = true;
                        UAATSlider.gameObject.SetActive(true);
                        XButton3.GetComponent<Image>().sprite = buttons[6];
                        Color color = XButton3.GetComponent<Image>().color;
                        color.a = (166f / 255f);
                        XButton3.GetComponent<Image>().color = color;
                        XButton3.SetActive(true);
                        while (buttonClicked == 0) {
                            yield return null;
                        }
                        UAATSlider.enabled = false;
                        UAATSlider.gameObject.SetActive(false);
                        XButton3.SetActive(false);
                        if (buttonClicked == 6) {
                            useAtATime = Mathf.RoundToInt(UAATSlider.value);
                            //Debug.Log("useAtATime = " + useAtATime);
                        } else if (buttonClicked == 7) {
                            //exit
                            goBackToUse = true;
                            success3 = false; 
                        }
                    }
                    
                    if (success3) {
                        for (int i = 0; i < useAtATime; i++) {
                            maxHealth += maxHealthChange;
                            if (HEALTH + healing > maxHealth)
                                HEALTH = maxHealth;
                            else
                                HEALTH += healing;
                            UpdateHealthText(HEALTH, maxHealth);
                            StartCoroutine(CreateMessageText("You are healed", 200, 150, 1, false));
                            StartCoroutine(ItemSound(index)); 
                            if (useAtATime > 1) {
                                yield return new WaitForSeconds(1f);
                            }
                        }
                        HealthsServerRpc(myPlayerNum, HEALTH, maxHealth, false, true, 2);
                        success = true;
                    }
                }
            } else if (mode == 2) {
                for (int i = 0; i < botUAAT; i++) {
                    BS.maxHP += maxHealthChange;
                    if (BS.HP + healing > BS.maxHP)
                        BS.HP = BS.maxHP;
                    else
                        BS.HP += healing;
                }
                HealthsServerRpc(BS.playerNum, BS.HP, BS.maxHP, false, true, 2);
                success = true;
            }
            /*if (success)
                StartCoroutine(ItemSound(index));*/
        }
        if (itemIntLists[index][0].Contains(3)) {
            if (triggerTrap == 0) {
                if (mode == 1) {
                    if (itemInfos[index][2][0] == 1) {
                        yield return StartCoroutine(AddToEffects(index, myEffects, myEffectLengths, myEffectStrengths));
                        yield return StartCoroutine(VitalsStatus(myPlayerNum, myEffects, 1, 1, enemyHealthsVisible, null));
                        //EffectsServerRpc(myPlayerNum, myEffects.ToArray());
                        yield return StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
                    }
                    if (itemIntLists[index][1].Contains(10)) {
                        if (itemIntLists[index][4].Contains(5)) {
                            stillPlaySound = false;
                            yield return StartCoroutine(UseRadar(index, 1, myPlayerNum, null));
                        }
                        if (itemIntLists[index][4].Contains(11)) {
                            StartCoroutine(StartUAV(1));
                            //StartCoroutine(ShowUAV(myEffects));
                        }
                    }
                    if (itemInfos[index][2][7] == 1) {
                        yield return StartCoroutine(SearchBook(1, index, 1, null));
                    }
                    
                    success = true;
                    if (stillPlaySound) {
                        StartCoroutine(ItemSound(index));
                    }
                } else if (mode == 2) {
                    if (itemInfos[index][2][0] == 1) {
                        yield return StartCoroutine(AddToEffects(index, BS.botEffects, BS.botEffectLengths, BS.botEffectStrengths));
                        //vitals
                        yield return StartCoroutine(VitalsStatus(BS.playerNum, BS.botEffects, 1, 2, BS.playerHealthsVisible, BS));
                    }
                    if (itemIntLists[index][1].Contains(10)) {
                        if (itemIntLists[index][4].Contains(5)) {
                            yield return StartCoroutine(UseRadar(index, 2, BS.playerNum, BS));
                            //Debug.Log("bot use radar");
                        }
                    }
                    if (itemInfos[index][2][7] == 1) {
                        yield return StartCoroutine(SearchBook(1, index, 2, BS));
                    }
                    success = true;
                }
            }
        }
        if (success) {//this part is sus
            if (mode == 1) {
                itemsOnCooldown[index] += itemInts[index][1];
                CDJustAdded[index] = true;
                yield return StartCoroutine(RemoveItems(INVENTORY, index, useAtATime));
                List<int> AAU = itemIntLists[index][6];
                foreach (int i in AAU) {
                    if (i > 0) {
                        INVENTORY.Add(i);
                    }
                }
                if (inventoryOpen) {
                    foreach (Transform child in inventoryItemsTF) {
                        GameObject.Destroy(child.gameObject);
                    }
                    StartCoroutine(OpenInventory());
                }
                if (cooldownsOpen) {
                    foreach (Transform child in cooldownItemsTF) {
                        GameObject.Destroy(child.gameObject);
                    }
                    StartCoroutine(OpenCooldowns());
                }
            } else if (mode == 2) {
                //do cooldown
                BS.botItemsOnCooldown[index] += itemInts[index][1];
                BS.botCDJA[index] = true;
                yield return StartCoroutine(RemoveItems(BS.inventory, index, useAtATime));
                List<int> AAU = itemIntLists[index][6];
                foreach (int i in AAU) {
                    if (i > 0) {
                        BS.inventory.Add(i);
                    }
                }
            }
        }
        callback(success);
        yield return null;
    }

    public void AddBookItem(string name, int rarity, int bookItemClass, float effectAmt, List<int> prerequisites, int level, List<float> formatInfo) {
        bookItemStrings.Add(new List<string>{name});
        bookItemInts.Add(new List<int>{rarity, bookItemClass, level});
        bookItemFloats.Add(new List<float>{effectAmt});
        bookItemIntLists.Add(new List<List<int>>{prerequisites});
        bookItemFloatLists.Add(new List<List<float>>{formatInfo});
        /* classes:
        1 = movement increase
        2 = healing increase
        3 = searching increase
        */
        /*
        Rarity:
        0 = cannot be found
        1 = common
        2 = uncommon
        3 = rare
        4 = more rare
        5 = epic
        6 = very epic
        7 = legendary
        8 = mythic
        */
    }
    IEnumerator AddBookItems() { //I think you could shift around the orders, but just update the prerequisites
        AddBookItem("Hiker", 1, 1, 1, new List<int>{}, 1, new List<float>{16, 20, 155, 112});
        AddBookItem("Sprinter", 2, 1, 2, new List<int>{1}, 2, new List<float>{26, 24, 240, 112});
        AddBookItem("Distance Runner", 3, 1, 3, new List<int>{2}, 3, new List<float>{18, 27, 215, 157});
        AddBookItem("Healer", 2, 2, 5, new List<int>{}, 1, new List<float>{36, 14, 180, 116});
        AddBookItem("Medic", 3, 2, 15, new List<int>{4}, 2, new List<float>{40, 38, 172, 116});
        AddBookItem("Searcher", 7, 3, 1, new List<int>{}, 1, new List<float>{40, 40, 260, 160});
        yield return null;
    }
    void SetFormatSizeBook(GameObject obj1, GameObject obj2, int index) {
        SetFormatSize(2, obj1, index, itemCoe * (200f / 250f), 200, 125, 1); //coe on coe based on the normal size
        SetFormatSize(2, obj2, index, descCoe, 300, 175, 2); //coe on coe based on the normal size, it's descCoe * (300f / 300f) but just make it 1
    }
    public IEnumerator SearchBook(int type, int index, int mode, BotScript BS) {
        if (mode == 1) {
            if (type == 1) {
                List<int> viableBookItems = GetViableBookItems(bookInventory);
                int bookSearchYield = 0;
                if (viableBookItems.Count == 0) {
                    bookSearchYield = 0;
                } else {
                    bookSearchYield = GetBookSearchYield(Mathf.RoundToInt(itemInfos[index][2][8]), viableBookItems);//UnityEngine.Random.Range(0, bookItemCostumes.Length) + 1;
                }
                StartCoroutine(PlaySound(sounds[60]));
                searchBook.GetComponent<Image>().sprite = bookCostumes[0];
                RectTransform SBRT = searchBook.GetComponent<RectTransform>();
                SBRT.sizeDelta = new Vector2(900, 900);
                SBRT.pivot = new Vector2(SBRT.pivot.x, 0);
                SBRT.anchoredPosition = new Vector2(0, -275);
                GameObject BSIParent = searchBook.transform.Find("BSIParent").gameObject;
                RectTransform BSIPRT = BSIParent.GetComponent<RectTransform>();
                BSIPRT.anchorMin = new Vector2(0.72f, 0.3f);
                BSIPRT.anchorMax = new Vector2(0.72f, 0.3f);
                //BSIParent.transform.Find("BookSearchItem").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                GameObject BIDParent = searchBook.transform.Find("BIDParent").gameObject;
                RectTransform BIDPRT = BIDParent.GetComponent<RectTransform>();
                BIDPRT.anchorMin = new Vector2(0.72f, 0.3f);
                BIDPRT.anchorMax = new Vector2(0.72f, 0.3f);
                //BIDParent.transform.Find("BookItemDescription").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                searchBook.SetActive(true);
                //yield return StartCoroutine(FlipBook(searchLoops));
                float slow = 0.01f;
                for (int i = 0; i < searchLoops; i++)
                {
                    if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) {
                        break;
                    }
                    yield return StartCoroutine(FlipBook(searchBook.GetComponent<Image>(), 1, 0));
                    if (i > 15)
                    {
                        slow *= 1.25f;
                    }
                    yield return new WaitForSeconds(slow);
                }
                yield return StartCoroutine(FlipBook(searchBook.GetComponent<Image>(), 1, 0));
                if (bookSearchYield == 0) {
                    SetFormatSizeBook(bookItem, bookItemDescription, -1);
                    bookItem.GetComponent<Image>().sprite = bookItemCostumeEmpty;
                    bookItemDescription.GetComponent<Image>().sprite = bookItemDescriptionEmpty;
                } else {
                    SetFormatSizeBook(bookItem, bookItemDescription, bookSearchYield - 1);
                    bookItem.GetComponent<Image>().sprite = bookItemCostumes[bookSearchYield - 1];
                    bookItemDescription.GetComponent<Image>().sprite = bookItemDescriptions[bookSearchYield - 1];
                }
                bookItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                bookItemDescription.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                bookItem.SetActive(true);
                bookItemDescription.SetActive(true);
                slow *= 1.25f;
                if (!(Keyboard.current != null && Keyboard.current.spaceKey.isPressed)) {
                    yield return new WaitForSeconds(slow);
                }
                buttonClicked = 0;
                OKButton.GetComponent<Image>().sprite = buttons[4];
                XButton.GetComponent<Image>().sprite = buttons[6];
                OKButton.SetActive(true);
                XButton.SetActive(true);
                while (buttonClicked == 0)
                {
                    yield return null;
                }
                searchBook.SetActive(false);
                bookItem.SetActive(false);
                bookItemDescription.SetActive(false);
                OKButton.SetActive(false);
                XButton.SetActive(false);
                if (bookSearchYield != 0) {
                    if (buttonClicked == 3) {
                        StartCoroutine(PlaySound(sounds[61]));
                        StartCoroutine(PlaySound(sounds[62]));
                        bookInventory.Add(bookSearchYield);
                        if (bookInventoryOpen) {
                            foreach (Transform child in bookInventoryItemsTF) {
                                GameObject.Destroy(child.gameObject);
                            }
                            StartCoroutine(OpenBookInventory());
                        }
                    }
                }
            } else if (type == 2) {
                List<int> viableBookItems = GetViableBookItems(bookInventory);
                int bookSearchYield = 0;
                if (viableBookItems.Count == 0) {
                    bookSearchYield = 0;
                } else {
                    bookSearchYield = GetBookSearchYield(Mathf.RoundToInt(itemInfos[index][2][8]), viableBookItems);//UnityEngine.Random.Range(0, bookItemCostumes.Length) + 1;
                }
                StartCoroutine(PlaySound(sounds[63]));
                searchBook.GetComponent<Image>().sprite = bookCostumes[6];
                RectTransform SBRT = searchBook.GetComponent<RectTransform>();
                SBRT.sizeDelta = new Vector2(410, 170);
                SBRT.pivot = new Vector2(SBRT.pivot.x, 0.5f);
                SBRT.anchoredPosition = new Vector2(SBRT.anchoredPosition.x, 0);
                GameObject BSIParent = searchBook.transform.Find("BSIParent").gameObject;
                RectTransform BSIPRT = BSIParent.GetComponent<RectTransform>();
                BSIPRT.anchorMin = new Vector2(0.5f, 0.5f);
                BSIPRT.anchorMax = new Vector2(0.5f, 0.5f);
                //BSIParent.transform.Find("BookSearchItem").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                GameObject BIDParent = searchBook.transform.Find("BIDParent").gameObject;
                RectTransform BIDPRT = BIDParent.GetComponent<RectTransform>();
                BIDPRT.anchorMin = new Vector2(0.5f, 0.5f);
                BIDPRT.anchorMax = new Vector2(0.5f, 0.5f);
                //BIDParent.transform.Find("BookItemDescription").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                searchBook.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                searchBook.GetComponent<Image>().sprite = bookCostumes[7];
                SBRT.sizeDelta = new Vector2(410, 280);
                SBRT.anchoredPosition = new Vector2(SBRT.anchoredPosition.x, 0);
                yield return new WaitForSeconds(0.2f);
                searchBook.GetComponent<Image>().sprite = bookCostumes[8];
                SBRT.sizeDelta = new Vector2(405, 450);
                SBRT.anchoredPosition = new Vector2(SBRT.anchoredPosition.x, 0);
                if (bookSearchYield == 0) {
                    SetFormatSizeBook(bookItem, bookItemDescription, -1);
                    bookItem.GetComponent<Image>().sprite = bookItemCostumeEmpty;
                    bookItemDescription.GetComponent<Image>().sprite = bookItemDescriptionEmpty;
                } else {
                    SetFormatSizeBook(bookItem, bookItemDescription, bookSearchYield - 1);
                    bookItem.GetComponent<Image>().sprite = bookItemCostumes[bookSearchYield - 1];
                    bookItemDescription.GetComponent<Image>().sprite = bookItemDescriptions[bookSearchYield - 1];
                }
                bookItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                bookItemDescription.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                bookItem.SetActive(true);
                bookItemDescription.SetActive(true);
                buttonClicked = 0;
                OKButton.GetComponent<Image>().sprite = buttons[4];
                XButton.GetComponent<Image>().sprite = buttons[6];
                OKButton.SetActive(true);
                XButton.SetActive(true);
                while (buttonClicked == 0)
                {
                    yield return null;
                }
                searchBook.SetActive(false);
                bookItem.SetActive(false);
                bookItemDescription.SetActive(false);
                OKButton.SetActive(false);
                XButton.SetActive(false);
                if (bookSearchYield != 0) {
                    if (buttonClicked == 3) {
                        StartCoroutine(PlaySound(sounds[61]));
                        StartCoroutine(PlaySound(sounds[62]));
                        bookInventory.Add(bookSearchYield);
                        if (bookInventoryOpen) {
                            foreach (Transform child in bookInventoryItemsTF) {
                                GameObject.Destroy(child.gameObject);
                            }
                            StartCoroutine(OpenBookInventory());
                        }
                    }
                }
            }
        } else if (mode == 2) {
            List<int> viableBookItems = GetViableBookItems(BS.botBookInventory);
            int bookSearchYield = 0;
            if (viableBookItems.Count == 0) {
                bookSearchYield = 0;
            } else {
                bookSearchYield = GetBookSearchYield(Mathf.RoundToInt(itemInfos[index][2][8]), viableBookItems);//UnityEngine.Random.Range(0, bookItemCostumes.Length) + 1;
            }
            if (bookSearchYield != 0) {
                BS.botBookInventory.Add(bookSearchYield);
            }
        }
        yield return null;
    }
    IEnumerator FlipBook(Image book, int amt, int helper) {
        for (int i = 0; i < Math.Abs(amt); i++) {
            if (amt == 0) {
            } else if (amt > 0) {
                book.sprite = bookCostumes[1 + helper];
                yield return null;
                book.sprite = bookCostumes[2 + helper];
                yield return null;
                book.sprite = bookCostumes[0 + helper];
                yield return null;
            } else if (amt < 0) {
                book.sprite = bookCostumes[2 + helper];
                yield return null;
                book.sprite = bookCostumes[1 + helper];
                yield return null;
                book.sprite = bookCostumes[0 + helper];
                yield return null;
            }
        }
        yield return null;
    }
    [SerializeField]
    List<bool> UAVFadingOut;
    [SerializeField]
    List<GameObject> UAVNodeObjects = new List<GameObject>();
    int nodesFaded = 0;
    int showUAVNum = 0;
    IEnumerator ShowUAV(List<int> e, float[] a, int mode, int pnum, BotScript BS) {
        int currentSUNum = -1;
        if (mode == 1) {
            showUAVNum++;
            currentSUNum = showUAVNum;
        }
        List<int> PPL = new List<int>(playerPositionList);
        if (mode == 1) {

            bool deleted = false;
            int j = 0;
            nodesFaded = 0;
            /*foreach (Transform child in UAVFolder.transform) {
                //if (Count > 0) {
                //    UAVFadingOut.RemoveAt(0); //might be off if the gameObject traversal order is different than the order we created the gameObjects in
                //}
                Debug.Log("j = " + j);
                //UAVFadingOut[j] = true;
                j++;
                StartCoroutine(FadeUAVNode(child.gameObject, 1, 0, -1));
                deleted = true;
            }*/
            for (int i = 0; i < UAVNodeObjects.Count; i++) {
                UAVFadingOut[j] = true;
                j++;
                StartCoroutine(FadeUAVNode(UAVNodeObjects[i], 1, 0, -1));
                deleted = true;
            }
            while (nodesFaded != j) {
                yield return null;
            }
            UAVNodeObjects.Clear();
            if (deleted || e.Contains(11)) {
                StartCoroutine(PlaySound(sounds[56]));
            }
            if (UAVCancel.activeSelf) {
                float startA = UAVCancel.GetComponent<Image>().color.a;
                yield return StartCoroutine(FadeUAVNode(UAVCancel, 3, 0, -1));
                UAVCancel.SetActive(false);
                Color colour = UAVCancel.GetComponent<Image>().color;
                colour.a = startA;
                UAVCancel.GetComponent<Image>().color = colour;
            }
        } else if (mode == 2) {
            for (int i = 0; i < BS.UAVPlayerPositions.Count; i++) {
                BS.UAVPlayerPositions[i] = 0;
            }
        }
        bool cancelUAV = false;
        int UFOIndex = 0;
        bool initUFO = (UAVFadingOut.Count == 0);
        if (e.Contains(11)) {
            for (int i = 0; i < PPL.Count; i++) {
                if (i + 1 != pnum) {
                    int pos = PPL[i];
                    if (i + 1 == Mathf.RoundToInt(a[0])) {
                        Debug.Log("Matching player");
                        if (Mathf.RoundToInt(a[20]) == 1) {
                            pos = Mathf.RoundToInt(a[21]);
                            Debug.Log("pos: " + pos);
                        }
                    }
                    bool cont1 = enemyEffects[i].Contains(1);
                    if (mode == 2) {
                        if (i + 1 == myPlayerNum) { //yes it's myPlayerNum
                            cont1 = myEffects.Contains(1);
                        }
                    }
                    if (GRID[pos - 1] == 9 || isSmoked[pos - 1] || cont1) {
                        cancelUAV = true;
                        if (mode == 2) {
                            //BS.UAVPlayerPositions[i] = 0;
                        }
                    } else {
                        if (mode == 1) {
                            if (currentSUNum == showUAVNum) {
                                if (initUFO) {
                                    UAVFadingOut.Add(false);
                                } else {
                                    UAVFadingOut[UFOIndex] = false;
                                }
                                GameObject node = Instantiate(UAVNodePF, UAVFolder.transform);
                                UAVNodeObjects.Add(node);
                                StartCoroutine(FadeUAVNode(node, 2, pos, UFOIndex));
                                UFOIndex++;
                            }
                        } else if (mode == 2) {
                            BS.UAVPlayerPositions[i] = pos;
                        }
                    }
                }
            }
        } else {
            if (mode == 1) {
                UAVFadingOut.Clear();
            }
        }
        if (mode == 2) {
            yield return StartCoroutine(BS.BotVisionHouse(false, 0));
        }
        if (mode == 1) {
            if (cancelUAV) {
                UAVCancel.SetActive(true);
                Debug.Log("cancelUAV");
                yield return StartCoroutine(FadeUAVNode(UAVCancel, 4, 0, -1));
            }
        } else if (mode == 2) {
            if (cancelUAV) {
                //tell bot enemy is using invis or in smoke or cave or something
            }
        }
        yield return null;
    }
    bool fadingUAVNode = false;
    IEnumerator FadeUAVNode(GameObject node, int mode, int pos, int UFOIndex) { //pos is 1+
        if (mode == 3 || mode == 4) {
            while (fadingUAVNode) {
                yield return null;
            }
            fadingUAVNode = true;
        }
        
        float startA = 0;
        if (mode == 1 || mode == 2) {
            startA = node.GetComponent<SpriteRenderer>().color.a;
        } else if (mode == 3 || mode == 4) {
            startA = node.GetComponent<Image>().color.a;
        }
        if (mode == 2) {
            node.transform.position = new Vector2(GRIDX[pos - 1], GRIDY[pos - 1]);
            node.transform.localScale = new Vector2(myPlayerObject.transform.localScale.x, myPlayerObject.transform.localScale.y); //general tile size
            Color colour = node.GetComponent<SpriteRenderer>().color;
            colour.a = 0;
            node.GetComponent<SpriteRenderer>().color = colour;
        }
        if (mode == 4) {
            Color colour = node.GetComponent<Image>().color;
            colour.a = 0;
            node.GetComponent<Image>().color = colour;
        }
        //int repeats = 60;
        float totalTime = 2.0f;
        float timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            if (mode == 2 && UAVFadingOut[UFOIndex]) {
                break;
            } else {
                Color colour = new Color();
                if (mode == 1 || mode == 2) {
                    colour = node.GetComponent<SpriteRenderer>().color;
                } else if (mode == 3 || mode == 4) {
                    colour = node.GetComponent<Image>().color;
                }
                if (mode == 1 || mode == 3) {
                    colour.a = startA * (1f - progress);
                } else if (mode == 2 || mode == 4) {
                    colour.a = startA * progress;
                }
                if (mode == 1 || mode == 2) {
                    node.GetComponent<SpriteRenderer>().color = colour;
                } else if (mode == 3 || mode == 4) {
                    node.GetComponent<Image>().color = colour;
                }
                //yield return new WaitForSeconds(0.02f);
                yield return null;
            }
        }
        if (mode == 1) {
            Destroy(node);
            nodesFaded++;
        }
        if (mode == 3 || mode == 4) {
            fadingUAVNode = false;
        }
        yield return null;
    }
    IEnumerator StartUAV(int mode) {
        if (mode == 1) {

        } else if (mode == 2) {
            StartCoroutine(PlaySound(sounds[55]));
        }
        StartCoroutine(PlaySound(sounds[54]));
        float startY = GRIDY[0];
        GameObject uav = Instantiate(UAVPF, new Vector2((GRIDX[0] + GRIDX[columns.Value - 1]) / 2, startY), Quaternion.identity);
        uav.transform.localScale = new Vector2(myPlayerObject.transform.localScale.x, myPlayerObject.transform.localScale.y); //general tile size
        float targetY = GRIDY[GRIDY.Count - 1];
        int repeats = 200;
        float changeY = (targetY - startY);
        float startA = uav.GetComponent<SpriteRenderer>().color.a;
        Color colour = uav.GetComponent<SpriteRenderer>().color;
        colour.a = 0;
        uav.GetComponent<SpriteRenderer>().color = colour;
        float totalTime = 2.0f;
        float timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            yield return StartCoroutine(SetGhost(uav, progress * startA));
            uav.transform.position = new Vector2(uav.transform.position.x, startY + (changeY * progress / 2));
        }
        timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            yield return StartCoroutine(SetGhost(uav, (1f - progress) * startA));
            uav.transform.position = new Vector2(uav.transform.position.x, startY + (changeY * (progress + 1f) / 2));
        }
        /*for (int i = 0; i < repeats / 2; i++) {
            uav.transform.position = new Vector2(uav.transform.position.x, uav.transform.position.y + (changeY / repeats));
            colour = uav.GetComponent<SpriteRenderer>().color;
            colour.a += (startA - 0) / (repeats / 2);
            uav.GetComponent<SpriteRenderer>().color = colour;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < repeats / 2; i++) {
            uav.transform.position = new Vector2(uav.transform.position.x, uav.transform.position.y + (changeY / repeats));
            colour = uav.GetComponent<SpriteRenderer>().color;
            colour.a += (0 - startA) / (repeats / 2);
            uav.GetComponent<SpriteRenderer>().color = colour;
            yield return new WaitForSeconds(0.02f);
        }*/
        Destroy(uav);
        yield return null;
    }
    public IEnumerator AddToEffects(int index, List<int> ME, List<int> MEL, List<int> MES) {
        for (int i = 0; i < itemIntLists[index][4].Count; i++) {
            MEL.Add(Mathf.RoundToInt(itemInfos[index][2][1]) + 1);
            ME.Add(itemIntLists[index][4][i]);

            if (itemIntLists[index][4][i] == 2) 
                MES.Add(Mathf.RoundToInt(itemInfos[index][2][2]));
            else if (itemIntLists[index][4][i] == 7)
                MES.Add(Mathf.RoundToInt(itemInfos[index][2][4]));
            else if (itemIntLists[index][4][i] == 10) {
                MES.Add(Mathf.RoundToInt(itemInfos[index][2][6]));
            } else {
                MES.Add(0);
            }
        }
        yield return null;
    }
    IEnumerator BombText() {
        if (bombStops.Count > 0) {
            int bombsHit = bombStops.Count;
            String msg;
            if (bombsHit == 1) {
                msg = "You hit " + bombsHit + " enemy!";
            } else {
                msg = "You hit " + bombsHit + " enemies!";
            }
            StartCoroutine(CreateMessageText(msg, 0, -100, 1, false));
        } else {
            StartCoroutine(CreateMessageText("You missed... with a bomb...", 0, -100, 1, false));
        }
        yield return null;
    }
    IEnumerator VitalsStatus(int pnum, List<int> e, int mode, int botMode, List<bool> EHV, BotScript BS) {
        if (e.Contains(6)) {
            for (int i = 0; i < EHV.Count; i++) {
                if (i + 1 != pnum) {
                    EHV[i] = true;
                }
            }
        } else {
            for (int i = 0; i < EHV.Count; i++) {
                EHV[i] = false;
            }
        }
        if (botMode == 1) {
            yield return StartCoroutine(ShowEnemyHealthTexts(mode));
        } else if (botMode == 2) {
            yield return StartCoroutine(BS.UpdateKnownPlayerHealths());
        }
    }
    IEnumerator ShowPain(Vector2 hitPos) {
        Vector2 myPos = new Vector2(GRIDX[playerPositionList[myPlayerNum - 1] - 1], GRIDY[playerPositionList[myPlayerNum - 1] - 1]);
        Vector2 direction = hitPos - myPos;
        float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject pain = Instantiate(toolIconPF, new Vector2(GRIDX[myPosition - 1], GRIDY[myPosition - 1]), Quaternion.Euler(0f, 0f, angleDeg));
        pain.GetComponent<SpriteRenderer>().sprite = painCostume;
        pain.transform.localScale = new Vector3(tileWidth * 10, tileHeight * 10, 1);
        while (!(Mouse.current.leftButton.wasPressedThisFrame && mouseIndex != 0 || (Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed) || Keyboard.current.rKey.isPressed)) {
            yield return null;
        }
        float totalTime = 0.5f;
        float timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            yield return StartCoroutine(SetGhost(pain, 1f - progress));
        }
        Destroy(pain);
        yield return null;
    }
    IEnumerator UseRadar(int index, int mode, int pnum, BotScript BS) {
        for (int i = 0; i < playerPositionList.Count; i++) {
            if (i + 1 != pnum) {
                if (mode == 1) {
                    Vector2 myPos = GetCoordsFromIndex(playerPositionList[myPlayerNum - 1]);
                    Vector2 enemyPos = GetCoordsFromIndex(playerPositionList[i]);
                    Vector2 direction = enemyPos - myPos;
                    float angleDeg = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    yield return new WaitForSeconds(0.2f);
                    GameObject rad = Instantiate(toolIconPF, new Vector2(GRIDX[myPosition - 1], GRIDY[myPosition - 1]), Quaternion.Euler(0f, 0f, angleDeg));
                    rad.GetComponent<SpriteRenderer>().sprite = toolCostumes[0];
                    rad.transform.localScale = new Vector3(tileWidth * 10, tileHeight * 10, 1);
                    for (int k = 0; k < 2; k++) {
                        yield return StartCoroutine(SetGhost(rad, 1));
                        StartCoroutine(PlaySound(sounds[itemIntLists[index][2][0]]));
                        yield return new WaitForSeconds(0.5f);
                        float totalTime = 1.5f;
                        float timer = 0f;
                        while (timer < totalTime) {
                            timer += Time.deltaTime;
                            float progress = Mathf.Clamp01(timer / totalTime);
                            yield return StartCoroutine(SetGhost(rad, 1f - progress));
                        }
                        yield return StartCoroutine(SetGhost(rad, 0));
                        yield return new WaitForSeconds(0.75f);
                        
                    }
                    int dist = Mathf.RoundToInt(FindDistance(myPos, enemyPos));
                    if (playerPositionList.Count > 2) {
                        if (dist == 1) {
                            StartCoroutine(CreateMessageText("An enemy is about " + dist + " block away!", 300, 200, 1, false));
                        } else {
                            StartCoroutine(CreateMessageText("An enemy is about " + dist + " blocks away!", 300, 200, 1, false));
                        }
                    } else {
                        if (dist == 1) {
                            StartCoroutine(CreateMessageText("The enemy is about " + dist + " block away!", 300, 200, 1, false));
                        } else {
                            StartCoroutine(CreateMessageText("The enemy is about " + dist + " blocks away!", 300, 200, 1, false));
                        }
                    }
                    yield return new WaitForSeconds(0.5f);
                } else if (mode == 2) {
                    BS.BotUseRadar(i, playerPositionList[i]);
                }
                //Debug.Log("dist: " + dist);
            }
        }
        if (mode == 2) {
            yield return StartCoroutine(BS.UpdateTargetAndQuadrant());
        }

        yield return null;
    }
    IEnumerator SetGhost(GameObject g, float gh) {
        Color colour = g.GetComponent<SpriteRenderer>().color;
        colour.a = gh;
        g.GetComponent<SpriteRenderer>().color = colour;
        yield return null;
    }
    IEnumerator SetGhostImage(GameObject g, float gh) {
        Color colour = g.GetComponent<Image>().color;
        colour.a = gh;
        g.GetComponent<Image>().color = colour;
        yield return null;
    }
    IEnumerator ItemSound(int index) {
        if (itemIntLists[index][2].Count > 0) {
            int PSC = itemInts[index][4];
            if (PSC == 0) {
                //random sound
                AudioClip clip = sounds[itemIntLists[index][2][UnityEngine.Random.Range(0, itemIntLists[index][2].Count)]];
                StartCoroutine(PlaySound(clip));
                yield return new WaitForSeconds(clip.length);
            } else {
                for (int i = 0; i < PSC; i++) {
                    AudioClip clip = sounds[itemIntLists[index][2][i]];
                    StartCoroutine(PlaySound(clip));
                    yield return new WaitForSeconds(clip.length);
                }
            }
        }
    }
    IEnumerator ItemCostume(int index, GameObject bul, int i) {
        SpriteRenderer BSR = bul.GetComponent<SpriteRenderer>();
        if (index == 5) {
            BSR.sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][0])];
            yield return new WaitForSeconds(0.2f);
            BSR.sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][1])];
        } else if (index == 12) {
            if (i % 2 == 0) {
                BSR.sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][0])];
                yield return new WaitForSeconds(0.2f);
                BSR.sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][1])];
            } else {
                BSR.sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][2])];
                yield return new WaitForSeconds(0.2f);
                BSR.sprite = bulletCostumes[Mathf.RoundToInt(itemIntLists[index][3][3])];
            }
        }
    }
    IEnumerator PlaySound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
        yield return null;
    }
    IEnumerator PlaySoundAndWait(AudioClip clip) {
        StartCoroutine(PlaySound(clip));
        yield return new WaitForSeconds(clip.length);
    }
    IEnumerator PlayMusicAndWait(AudioClip clip) {
        musicSource.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
    }
    IEnumerator RemoveItems(List<int> inv, int index, int uaat) {
        for (int h = 0; h < uaat; h++) {
            for (int i = 0; i < itemInts[index][2]; i++) {
                yield return StartCoroutine(TryRemoveFromInv(inv, index));
            }
        }
        yield return null;
    }
    IEnumerator TryRemoveFromInv(List<int> inv, int index) {
        for (int i = 0; i < inv.Count; i++) {
            if (inv[i] == index + 1) {
                inv.RemoveAt(i);
                break;
            }
        }
        yield return null;
    }
    public int CountInIntList(List<int> list, int target) {
        int count = 0;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == target)
                count++;
        }
        return count; 
    }
    void RenderLine(Vector2 start, Vector2 end) {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;
    }
    int FindClosestTarget(Vector2 myCoords, int pNum) {
        List<int> choices = new List<int>();
        for (int i = 0; i < playerPositionList.Count; i++) {
            if (i + 1 != pNum) {
                choices.Add(i + 1);
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
            float dist = FindDistance(myCoords, GetCoordsFromIndex(playerPositionList[choices[i] - 1]));
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
    public float GetHilltopCoe() {
        return 0.5f;
    }
    public IEnumerator BulletMove(int itemIndex, GameObject bul, float angleDeg, float speed, float range, float startingElevation, int position, int playerNum, int mode, float damage, int bounce, int bomb, float fuseTime, float blastRadius, float stopInPlace, float distanceClicked, int knife, int hitSound, int smoke, int stunMode, int trap, int changeLoc, int trapIndex, int smokeIndex, int suppressed, bool replay, bool LR6Smoke, BotScript BS) {
        /*mode:
        1 = player useitem
        2 = player replay
        3 = bot replay
        4 = bot useitem
        */
        int stop = 0;
        float distanceTraveled = 0;
        //lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        Vector2 bulStart = new Vector2(bul.transform.position.x, bul.transform.position.y);
        float hilltop = 0;
        float hilltopCoefficient = GetHilltopCoe();
        if (mode == 2) {
            Debug.Log("player replay");
        }
        Color c = bul.GetComponent<SpriteRenderer>().color;
        c.a = 0f;
        bul.GetComponent<SpriteRenderer>().color = c;
        List<int> PPL = null; 
        if (mode == 1 || mode == 2 || mode == 3 || mode == 4) PPL = playerPositionList;
        List<List<float>> CBS = CalculateBulletStop(bulStart, bounce, angleDeg, itemIndex, playerNum, PPL, range, trap, position, stopInPlace, distanceClicked, startingElevation, hilltopCoefficient, bomb, damage, blastRadius);
        List<float> CBS1 = CBS[0];
        List<int> MPPI = null;
        if (CBS[1] != null) {
            MPPI = CBS[1].ConvertAll(x => Mathf.RoundToInt(x));
        }
        if (mode == 3) {
            if (suppressed == 0) {
                int endIndex = GetIndexFromWorldCoords(new Vector2(CBS[2][0], CBS[2][1]));
                if (endIndex == 0) {
                    endIndex = BackBulFromEdge(new Vector2(CBS[2][0], CBS[2][1]), angleDeg);
                }
                BS.SeeMuzzleFlash(endIndex, playerNum, position);
            }
        }
        stop = Mathf.RoundToInt(CBS1[0]);

        if (mode == 1 || mode == 2) {
            bul.transform.position = bulStart;
            float goalDistance = CBS1[1];
            while (distanceTraveled < goalDistance) {
                for (int i = 0; i < speed; i++) {
                    if (distanceTraveled < goalDistance) {
                        bul.transform.position += (Vector3)(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * 1 * Time.deltaTime); //this may cause lagginess to change outcomes, best to implement a math calculation first
                        float DT = Time.deltaTime;
                        distanceTraveled += DT;
                        
                        int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y)); //index is 1+
                        /*if (mode == 4) {
                            Debug.Log("index is " + index);
                        }*/
                        float homing = itemInfos[itemIndex][0][6];
                        if (homing != 0) {
                            int closest = FindClosestTarget(GetCoordsFromIndex(index), playerNum);
                            //Debug.Log("closest: " + closest);
                            if (closest > 0) {
                                Vector2 diff = new Vector2(GRIDX[playerPositionList[closest - 1] - 1], GRIDY[playerPositionList[closest - 1] - 1]) - new Vector2(bul.transform.position.x, bul.transform.position.y);
                                float targetAngleDeg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                                angleDeg += Mathf.DeltaAngle(angleDeg, targetAngleDeg) * homing * Time.deltaTime / 40;
                            }
                            /*if (distanceTraveled >= 2 * range * tileWidth) {
                                if (stop == 0) {
                                    stop = 1;
                                }
                            }*/
                        }
                        if (mode != 3 && mode != 4) {
                            if (amSpectator || (index > 0 && visionGrid[index - 1] != 0 && !(trap != 0 && mode == 2) && !(suppressed == 1 && mode == 2))) {
                                Color colour = bul.GetComponent<SpriteRenderer>().color;
                                colour.a = 1f;
                                bul.GetComponent<SpriteRenderer>().color = colour;
                                //visionObjects[index - 1].GetComponent<SpriteMask>().enabled = false;
                            } else {
                                Color colour = bul.GetComponent<SpriteRenderer>().color;
                                colour.a = 0f;
                                bul.GetComponent<SpriteRenderer>().color = colour;
                                //visionObjects[index - 1].GetComponent<SpriteMask>().enabled = true;
                            }
                            if (changeLoc == 1) {
                                lineRenderer.enabled = true;
                                RenderLine(bulStart, new Vector2(bul.transform.position.x, bul.transform.position.y));
                            }
                        } /*else if (mode == 4) {
                            Color colour = bul.GetComponent<SpriteRenderer>().color;
                            colour.a = 1f;
                            bul.GetComponent<SpriteRenderer>().color = colour;
                        }*/
                        
                        /*if (index == 0) {
                            //stop = 4;
                            if (bounce == 1) {
                                yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                                Debug.Log("BB1");
                                break;
                            }
                        } else {
                            //Debug.Log(index);
                            //List<int> matchingPP = new List<int>();
                            /*float bulElevation = FindElevation(GRID[index - 1], 2);
                            Vector2 myPositionVector2 = GetCoordsFromIndex(position);
                            Vector2 myPositionVector2WS = new Vector2(GRIDX[position - 1], GRIDY[position - 1]);
                            List<int> matchingPPI = new List<int>();
                            //Debug.Log(playerPositionList.Count);
                            for (int j = 0; j < playerPositionList.Count; j++) {
                                if (playerPositionList[j] == index && j != playerNum - 1) {
                                    matchingPPI.Add(j + 1);
                                }
                            }
                            //if ((FindDistance(myPositionVector2, GetCoordsFromIndex(index)) > range) || (stopInPlace == 1 && (distanceTraveled >= distanceClicked))) {
                            if ((FindDistance(myPositionVector2WS, new Vector2(bul.transform.position.x, bul.transform.position.y)) > range * tileWidth) || (stopInPlace == 1 && (distanceTraveled >= distanceClicked))) { //this means tileWidth must always equal tileHeight
                                if (bulElevation > startingElevation) {
                                    hilltop += DT;
                                    if (hilltop >= tileWidth * hilltopCoefficient) {
                                        //stop = 2;
                                        if (bounce == 1) {
                                            yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                                            Debug.Log("BB2");
                                            break;
                                        }
                                    }
                                }
                                else {
                                    /*if (mode == 4) {
                                        Debug.Log("stop is 1, index is " + index);
                                        Debug.Log("out of range is " + (FindDistance(myPositionVector2WS, new Vector2(bul.transform.position.x, bul.transform.position.y)) > range * tileWidth));
                                        Debug.Log("range * TW = " + (range * tileWidth));
                                        Debug.Log("Distance = " + FindDistance(myPositionVector2WS, new Vector2(bul.transform.position.x, bul.transform.position.y)));
                                        Debug.Log("stopped in place is " + (stopInPlace == 1 && (distanceTraveled >= distanceClicked)));
                                    }

                                    //stop = 1;

                                    //break;
                                }
                            } else if (matchingPPI.Count > 0) {
                                //stop = 3;
                                
                                
                            } else if (index != position && bulElevation > startingElevation) {
                                hilltop += DT;
                                Debug.Log("TW * HTC = " + tileWidth * hilltopCoefficient);
                                if (hilltop >= tileWidth * hilltopCoefficient) {
                                    //stop = 2;
                                    if (bounce == 1) {
                                        yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                                        Debug.Log("BB4");
                                        break;
                                    }
                                }
                            }*/
                            /*if (matchingPPI.Count > 0) {
                                stop = 3;
                                if (bounce == 0) {
                                    StartCoroutine(PlaySound(sounds[1]));
                                    if (mode == 2) {
                                        if (matchingPPI.Contains(myPlayerNum)) {
                                            HEALTH -= Mathf.RoundToInt(armorCoefficient * damage);
                                            yield return StartCoroutine(CheckHealth());
                                            UpdateHealthText();
                                        }
                                    }
                                } else {
                                    yield return StartCoroutine(BulletBounce(bul, dir, playerNum, mode));
                                    break;
                                }
                                
                            } else if (bulElevation > startingElevation) {
                                stop = 2;
                                if (bounce == 1) {
                                    yield return StartCoroutine(BulletBounce(bul, dir, playerNum, mode));
                                    break;
                                }
                            } else if ((FindDistance(myPositionVector2, GetCoordsFromIndex(index)) > range) || (stopInPlace == 1 && (distanceTraveled >= distanceClicked))) {
                                stop = 1;
                                //break;
                            }
                        }*/
                    }
                }
                //yield return new WaitForSeconds(0.2f);
                /*
                stop = 1: out of range
                stop = 2: stopped by wall
                stop = 3: hit enemy
                stop = 4: went off map
                OLD: 
                    priority: 4, 1, 3, 2
                    goal: 2, 1 (trivial)
                    1, 3 (important)
                    3, 2 (important)
                NEW:
                    priority: 4, 3, 2, 1
                */
                yield return null;
            }
        }
        bul.transform.position = new Vector2(CBS[2][0], CBS[2][1]);

        if (MPPI != null) {
            if (MPPI.Count > 0) {
                if (bounce == 0) {
                    if (bomb == 0) {
                        if (mode != 3 && mode != 4) {
                            if (changeLoc == 0) {
                                if (hitSound == -1) {
                                    StartCoroutine(PlayHitSound(itemIndex));
                                } else {
                                    StartCoroutine(PlaySound(sounds[hitSound]));
                                }
                            }
                        }
                        if (mode == 1 || mode == 4) {
                            foreach (int p in MPPI) {
                                if (mode == 1) {
                                    localDamages[p - 1] += damage;
                                    Debug.Log("changed my localDamages");
                                } else if (mode == 4) {
                                    BS.myLocalDamages[p - 1] += damage;
                                }
                            }
                            //localDamage += damage;
                            //Debug.Log("localDamage = " + localDamage);
                        }
                        if (mode == 2) {
                            if (MPPI.Contains(myPlayerNum)) {
                                if (!replay) {
                                    healthCalculation -= armorCoefficient * damage;
                                    //yield return StartCoroutine(CheckHealth());
                                    UpdateHealthText(Mathf.RoundToInt(healthCalculation), maxHealth);
                                } else {
                                    pseudoHealth -= armorCoefficient * damage;
                                    UpdateHealthText(Mathf.RoundToInt(pseudoHealth), maxHealth);
                                }
                                
                            }
                        } 
                        if (mode == 3) {
                            if (MPPI.Contains(BS.playerNum)) {
                                BS.HPCalculation -= BS.armorCoefficient * damage;
                                //yield return StartCoroutine(CheckHealth());
                                //UpdateHealthText(Mathf.RoundToInt(healthCalculation), maxHealth);
                            }
                        }
                        if (mode == 4) {
                            BS.LD += damage;
                        }
                    }
                    if (bomb == 0 || (bomb != 0 && bounce == 0)) {
                        if (mode == 2) {
                            if (MPPI.Contains(myPlayerNum)) {
                                if (suppressed != 1) {
                                    if (damage > 0) {
                                        StartCoroutine(ShowPain(new Vector2(bul.transform.position.x, bul.transform.position.y)));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (stop == 2 || stop == 3 || stop == 4) {
            if (bounce != 0) {
                yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                Debug.Log("BB3");
            }
        }
        /*if (mode == 4) {
            Debug.Log("stop is " + stop);
        }*/
        if (mode != 3 && mode != 4) {
            bulStops.Add(stop);
        }
        if (stop == 3 || stop == 4) {
            
        }
        bool bulFade = (stunMode == 0 && smoke == 0 && bomb == 0 && knife == 0 && trap == 0 && changeLoc == 0 && (stop == 1 || stop == 2 || stop == 4) && mode != 3 && mode != 4);
        if (bulFade) {
            bul.GetComponent<SpriteRenderer>().sprite = bulletCostumes[0];
            bul.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            StartCoroutine(PlaySound(sounds[0]));
            if (!(suppressed == 1 && mode == 2)) {
                StartCoroutine(BulletFade(bul, speed, angleDeg, stop));
            } else {
                Destroy(bul);
            }
        }
        if (bomb == 1 || bomb == 2) { //\H
            Debug.Log("bomb");
            int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
            
            if (trap == 0) {
                Debug.Log("not trap");
                yield return new WaitForSeconds(fuseTime);
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                if (index != 0) {
                    yield return StartCoroutine(ExplodeBomb(index, blastRadius, damage, bounce, MPPI, playerNum, Mathf.RoundToInt(itemInfos[itemIndex][0][14]), bomb, replay, mode, BS));
                }
            } else if (trap == 1) {
                if (mode == 2 || mode == 3) {
                    index = trapIndex;
                    Debug.Log("TrapIndex is " + trapIndex);
                }
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                List<float> trapInfo = itemInfos[itemIndex][4];
                List<float> newTrap = new List<float>{playerNum, trapInfo[0], trapInfo[1], trapInfo[2], trapInfo[3], trapInfo[4]};
                if (mode == 1) {
                    trapIndexes.Add(index);
                } else if (mode == 4) {
                    BS.myTI.Add(index);
                }
                if (mode == 1 || mode == 2) { //trap: {owner (playerNum), type, damage, blast radius, costumeIndex, fake blast radius}
                    if (!replay) {
                        traps[index - 1].Add(newTrap);
                        Debug.Log("Trap added, trap info: " + newTrap[0] + ", " + newTrap[1] + ", at position " + index);
                    }
                    List<int> soundIndexes = itemIntLists[itemIndex][2];
                    StartCoroutine(PlaySound(sounds[soundIndexes[soundIndexes.Count - 1]]));
                    yield return StartCoroutine(RenderTraps(traps));
                } else if (mode == 3 || mode == 4) {
                    BS.botTraps[index - 1].Add(newTrap);
                    Debug.Log("Bot trap added, trap info: " + newTrap[0] + ", " + newTrap[1] + ", at position " + index);
                }
            }
        } else if (smoke == 1) {
            Debug.Log("smoke");
            if (mode != 3 && mode != 4) {
                yield return new WaitForSeconds(fuseTime);
                int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
                if (mode == 2) {
                    //Debug.Log("Player bot smoke bullet index is " + index);
                    index = smokeIndex;
                    //Debug.Log("SmokeIndex is " + smokeIndex);
                }
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                if (index != 0) {
                    if (mode == 1) {
                        smokeIndexes.Add(index);
                    }
                    
                    if (mode == 1 || (mode == 2 && !replay)) {
                        smokes[index - 1] = blastRadius;
                    }
                    if (mode == 1) {
                        /*smokeLifespans[index - 1] = Mathf.RoundToInt(itemInfos[itemIndex][2][1]);
                        smokeOwners[index - 1] = myPlayerNum;*/
                        //SmokeListsServerRpc(index - 1, Mathf.RoundToInt(itemInfos[itemIndex][2][1]), myPlayerNum);
                        SLSServerRpc(index - 1, Mathf.RoundToInt(itemInfos[itemIndex][2][1]) + 1);
                        SOServerRpc(index - 1, myPlayerNum);
                    }
                    StartCoroutine(PlaySound(sounds[15]));
                    List<float> currentSmokes = new List<float>(smokes);
                    if (mode == 2 && replay) {
                        if (LR6Smoke) {
                            currentSmokes = new List<float>(prevSmokes2);
                        }
                    }
                    yield return StartCoroutine(UpdateIsSmoked(currentSmokes));
                }
            } else if (mode == 3 || mode == 4) {
                int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
                if (mode == 3) {
                    index = smokeIndex;
                    Debug.Log("SmokeIndex is " + smokeIndex);
                }
                if (index != 0) {
                    if (mode == 4) {
                        BS.mySI.Add(index);
                    }
                    //Debug.Log("Start Bot smoke");
                    if (mode == 4 || mode == 3) {
                        BS.botSmokes[index - 1] = blastRadius;
                    }
                    if (mode == 4) {
                        Debug.Log("Bot smoke index is " + index);
                        SLSServerRpc(index - 1, Mathf.RoundToInt(itemInfos[itemIndex][2][1]) + 1);
                        SOServerRpc(index - 1, BS.playerNum);
                    }
                    yield return StartCoroutine(BS.UpdateIsSmoked(BS.botSmokes));
                    //Debug.Log("End Bot smoke");
                }
            }
            /*if (mode == 4) {
                yield return new WaitForSeconds(fuseTime);
                int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                if (index != 0) {
                    smokes[index - 1] = blastRadius;
                    SLSServerRpc(index - 1, Mathf.RoundToInt(itemInfos[itemIndex][2][1]) + 1);
                    SOServerRpc(index - 1, myPnum);
                    yield return StartCoroutine(UpdateIsSmoked(smokes));
                }
            }*/
        }
        if (stunMode == 1) {
            Debug.Log("stun");
            int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
            if (mode != 3 && mode != 4) {
                yield return new WaitForSeconds(fuseTime);
                
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                StartCoroutine(PlaySound(sounds[16]));
                if (mode == 1) {
                    yield return new WaitForSeconds(0.25f);
                }
                if (mode == 2) {
                    if (index > 0 && visionGrid[index - 1] == 1) {
                        if (!replay) {
                            yield return StartCoroutine(UpdateSkippedTurnsAndWait(myPlayerNum, Mathf.RoundToInt(itemInfos[itemIndex][2][3])));
                        }
                        //Debug.Log("USTAW1");
                        StartCoroutine(Flashbang());
                        StartCoroutine(CreateMessageText("You got stunned!", 0, -100, 1, false));
                        if (!replay) {
                            YSTEServerRpc(playerNum);
                        }
                        
                    }
                }
            }
            if (mode == 3) {
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                if (index > 0 && BS.myVG[index - 1] == 1) {
                    yield return StartCoroutine(UpdateSkippedTurnsAndWait(BS.playerNum, Mathf.RoundToInt(itemInfos[itemIndex][2][3])));
                    Debug.Log("USTAW: player " + BS.playerNum);
                    Debug.Log("Bot USTAW");
                    YSTEServerRpc(playerNum);
                }
            }
        } else if (stunMode == 2) {
            int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
            if (mode != 3 && mode != 4) {
                yield return new WaitForSeconds(fuseTime);
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                if (mode == 2) {
                    if (index > 0 && index == myPosition) {
                        if (RandomChance(2)) {
                            if (!replay) {
                                yield return StartCoroutine(UpdateSkippedTurnsAndWait(myPlayerNum, Mathf.RoundToInt(itemInfos[itemIndex][2][3])));
                            }
                            //Debug.Log("USTAW1");
                            
                            StartCoroutine(CreateMessageText("You got stunned!", 0, -100, 1, false));
                            if (!replay) {
                                YSTEServerRpc(playerNum);
                                if (frozenTurns == 0) {
                                StartCoroutine(PlaySound(sounds[45]));
                                playerList[myPlayerNum - 1].GetComponent<SpriteRenderer>().sprite = playerCostumes[2];
                                changedFrozen = true;
                                FreezeServerRpc(myPlayerNum, 1);
                                }
                                frozenTurns += Mathf.RoundToInt(itemInfos[itemIndex][2][3]);
                            }
                        }
                    }
                }
            }
            if (mode == 3) {
                //Destroy(bul);
                bul.GetComponent<SpriteRenderer>().enabled = false;
                if (index > 0 && index == BS.position) {
                    if (RandomChance(2)) {
                        yield return StartCoroutine(UpdateSkippedTurnsAndWait(BS.playerNum, Mathf.RoundToInt(itemInfos[itemIndex][2][3])));
                        Debug.Log("USTAW: player " + BS.playerNum);
                        YSTEServerRpc(playerNum);
                        if (BS.myFrozenTurns == 0) {
                            FreezeServerRpc(BS.playerNum, 1);
                        }
                        BS.myFrozenTurns += Mathf.RoundToInt(itemInfos[itemIndex][2][3]);
                    }
                }
            }
        }
        if (changeLoc == 1) {
            Debug.Log("changeLoc");
            int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
            if (mode != 3 && mode != 4) {
                StartCoroutine(PlaySound(sounds[41]));
            }
            if (index == 0) {
                index = BackBulFromEdge(new Vector2(bul.transform.position.x, bul.transform.position.y), angleDeg);
            }
            if (mode == 1) {
                yield return StartCoroutine(Grapple(myPlayerObject, myPosition, index, new Vector2(bul.transform.position.x, bul.transform.position.y)));
                myPosition = index;
                yield return StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
                VCSpriterenderer.enabled = false;
                //UpdatePositionServerRpc(myPlayerObject.GetComponent<NetworkObject>(), myPosition, true);
                playerPositionList[myPlayerNum - 1] = myPosition;
                SetCharacterPosition(myPlayerObject, myPosition);
                yield return StartCoroutine(SteppingOnTrap(1, 2, traps[myPosition - 1], myPosition, null));
            } else if (mode == 4) {
                BS.position = index;
                Debug.Log("bot was pulled to " + index + " AKA " + BS.position);
                yield return StartCoroutine(BS.BotVisionHouse(false, 0));
                BS.UpdateMyPositionInList();
                yield return StartCoroutine(SteppingOnTrap(3, 2, BS.botTraps[BS.position - 1], BS.position, BS));
            }
            lineRenderer.enabled = false;
            //Destroy(bul);
            bul.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (!bulFade) {
            Destroy(bul);
        }
        if (mode == 3 || mode == 4) {
            Debug.Log("BS.BD++");
            BS.BD++;
        } else {
            bulDone++;
        }
    }
    int BackBulFromEdge(Vector2 bulCoords, float angleDeg) {
        Vector2 BC = new Vector2(bulCoords.x, bulCoords.y);
        int index = GetIndexFromWorldCoords(BC);
        while (index == 0) {
            BC += new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * -1 * tileWidth / 5; //\J
            index = GetIndexFromWorldCoords(BC);
        }
        return index;
    }
    public List<List<float>> CalculateBulletStop(Vector2 startPos, int bounce, float angleDeg, int itemIndex, int playerNum, List<int> PPL, float range, int trap, int position, float stopInPlace, float distanceClicked, float startingElevation, float hilltopCoefficient, int bomb, float damage, float blastRadius) {
        Vector2 bulPos = new Vector2(startPos.x, startPos.y);
        int stop = 0;
        float distanceUnit = tileWidth / 5; //\J
        float distanceTraveled = 0;
        float hilltop = 0;
        float bulDamage = 0;
        List<int> MPPI = null;
        while (stop == 0) {
            //for (int i = 0; i < speed; i++) {
            if (stop == 0) {
                bulPos += new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * 1 * distanceUnit; //this may cause lagginess to change outcomes, best to implement a math calculation first
                float DT = distanceUnit;
                distanceTraveled += DT;
                
                int index = GetIndexFromWorldCoords(bulPos); //index is 1+
                float homing = itemInfos[itemIndex][0][6];
                if (homing != 0) {
                    int closest = FindClosestTarget(GetCoordsFromIndex(index), playerNum);
                    if (closest > 0) {
                        Vector2 diff = new Vector2(GRIDX[playerPositionList[closest - 1] - 1], GRIDY[playerPositionList[closest - 1] - 1]) - bulPos;
                        float targetAngleDeg = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                        angleDeg += Mathf.DeltaAngle(angleDeg, targetAngleDeg) * homing * distanceUnit / 40;
                    }
                    if (distanceTraveled >= 2 * range * tileWidth) {
                        if (stop == 0) {
                            stop = 1;
                        }
                    }
                }
                /*if (mode != 3 && mode != 4) {
                    if (index > 0 && visionGrid[index - 1] != 0 && !(trap != 0 && mode == 2) && !(suppressed == 1 && mode == 2)) {
                        Color colour = bul.GetComponent<SpriteRenderer>().color;
                        colour.a = 1f;
                        bul.GetComponent<SpriteRenderer>().color = colour;
                    } else {
                        Color colour = bul.GetComponent<SpriteRenderer>().color;
                        colour.a = 0f;
                        bul.GetComponent<SpriteRenderer>().color = colour;
                    }
                    if (changeLoc == 1) {
                        lineRenderer.enabled = true;
                        RenderLine(bulStart, new Vector2(bul.transform.position.x, bul.transform.position.y));
                    }
                }*/
                
                if (index == 0) {
                    stop = 4;
                    /*if (bounce == 1) {
                        yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                        Debug.Log("BB1");
                        break;
                    }*/
                } else {
                    float bulElevation = FindElevation(GRID[index - 1], 2);
                    Vector2 myPositionVector2 = GetCoordsFromIndex(position);
                    Vector2 myPositionVector2WS = new Vector2(GRIDX[position - 1], GRIDY[position - 1]);
                    List<int> matchingPPI = new List<int>();
                    for (int j = 0; j < playerPositionList.Count; j++) {
                        if (playerPositionList[j] == index && j != playerNum - 1) {
                            matchingPPI.Add(j + 1);
                        }
                    }
                    if ((FindDistance(myPositionVector2WS, bulPos) > range * tileWidth) || (stopInPlace == 1 && (distanceTraveled >= distanceClicked))) { //this means tileWidth must always equal tileHeight
                        if (bulElevation > startingElevation) {
                            hilltop += DT;
                            if (hilltop >= tileWidth * hilltopCoefficient) {
                                stop = 2;
                                /*if (bounce == 1) {
                                    yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                                    Debug.Log("BB2");
                                    break;
                                }*/
                            }
                        }
                        else {
                            stop = 1;
                        }
                    } else if (matchingPPI.Count > 0) {
                        stop = 3;
                        MPPI = new List<int>(matchingPPI);
                        if (bounce == 0) {
                            bulDamage += damage;
                        }
                        /*if (bounce == 0) {
                            if (bomb == 0) {
                                if (mode != 3 && mode != 4) {
                                    if (changeLoc == 0) {
                                        if (hitSound == -1) {
                                            StartCoroutine(PlayHitSound(itemIndex));
                                        } else {
                                            StartCoroutine(PlaySound(sounds[hitSound]));
                                        }
                                    }
                                }
                                if (mode == 1 || mode == 4) {
                                    foreach (int p in matchingPPI) {
                                        if (mode == 1) {
                                            localDamages[p - 1] += damage;
                                        } else if (mode == 4) {
                                            BS.myLocalDamages[p - 1] += damage;
                                        }
                                    }
                                }
                                if (mode == 2) {
                                    if (matchingPPI.Contains(myPlayerNum)) {
                                        if (!replay) {
                                            healthCalculation -= armorCoefficient * damage;
                                            UpdateHealthText(Mathf.RoundToInt(healthCalculation), maxHealth);
                                        } else {
                                            pseudoHealth -= armorCoefficient * damage;
                                            UpdateHealthText(Mathf.RoundToInt(pseudoHealth), maxHealth);
                                        }
                                        if (suppressed != 1) {
                                            if (damage > 0) {
                                                StartCoroutine(ShowPain(new Vector2(bul.transform.position.x, bul.transform.position.y)));
                                            }
                                        }
                                    }
                                } 
                                if (mode == 3) {
                                    if (matchingPPI.Contains(BS.playerNum)) {
                                        BS.HPCalculation -= BS.armorCoefficient * damage;
                                    }
                                }
                                if (mode == 4) {
                                    BS.LD += damage;
                                }
                            }
                        }*/ /*else {
                            yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                            Debug.Log("BB3");
                            break;
                        }*/
                        
                    } else if (index != position && bulElevation > startingElevation) {
                        hilltop += DT;
                        Debug.Log("TW * HTC = " + tileWidth * hilltopCoefficient);
                        if (hilltop >= tileWidth * hilltopCoefficient) {
                            stop = 2;
                            /*if (bounce == 1) {
                                yield return StartCoroutine(BulletBounce(bul, angleDeg, playerNum, mode, trap));
                                Debug.Log("BB4");
                                break;
                            }*/
                        }
                    }
                }
            }
            //}
            /*
            stop = 1: out of range
            stop = 2: stopped by wall
            stop = 3: hit enemy
            stop = 4: went off map
            OLD: 
                priority: 4, 1, 3, 2
                goal: 2, 1 (trivial)
                1, 3 (important)
                3, 2 (important)
            NEW:
                priority: 4, 3, 2, 1
            */
        }
        Vector2 finalPos = bulPos;
        List<int> bombHits = new List<int>();
        List<float> bombDamages = new List<float>();
        if (bomb == 1 || bomb == 2) { //\H
            int index = GetIndexFromWorldCoords(bulPos);
            if (trap == 0) {
                if (index != 0) {
                    int[] binaryList = BombBinary(index, blastRadius);
                    if (bomb == 1) {
                        float[] damageList = BombDamage(index, blastRadius, damage);
                        //Debug.Log("damageList length: " + damageList.Length);
                        for (int i = 0; i < binaryList.Length; i++) {
                            if (binaryList[i] == 1) {
                                for (int j = 0; j < PPL.Count; j++) {
                                    if (PPL[j] != 0) {
                                        if (i == PPL[j] - 1 && j != playerNum - 1) {
                                            bombHits.Add(j + 1);
                                            bombDamages.Add(damageList[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        List<float> MPPIFloat = null;
        if (MPPI != null) {
            MPPIFloat = MPPI.ConvertAll(x => (float)x);
        }
        return new List<List<float>>{new List<float>{stop, distanceTraveled, bulDamage}, MPPIFloat, new List<float>{finalPos.x, finalPos.y}, bombHits.ConvertAll(x => (float)x), bombDamages};
    }
    IEnumerator BulletFade(GameObject bul, float speed, float angleDeg, int stop) {
        float slowedSpeed = speed * 0.5f;
        float coe = 1;
        if (stop == 2) {
            coe = -1;
        }
        float ghost = 1f;
        for (int i = 0; i < 255; i++) {
            bul.transform.position += (Vector3)(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * coe * slowedSpeed * 0.5f * Time.deltaTime);
            bul.transform.position += new Vector3(0f, slowedSpeed * 0.001f, 0f);
            int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
            slowedSpeed *= 0.96f;
            ghost -= (1f / 255f);
            Color colour = bul.GetComponent<SpriteRenderer>().color;
            if (index > 0 && visionGrid[index - 1] != 0) {
                colour.a = ghost;
            } else {
                colour.a = 0f;
            }
            bul.GetComponent<SpriteRenderer>().color = colour;
            yield return null;
        }
        Destroy(bul);
    }
    public IEnumerator ExplodeBomb(int index, float blastRadius, float damage, int bounce, List<int> MPPI, int playerNum, int bombType, int bomb, bool replay, int mode, BotScript BS) {
        int[] binaryList = BombBinary(index, blastRadius);
        if (bomb == 1) {
            float[] damageList = BombDamage(index, blastRadius, damage);
            //Debug.Log("damageList length: " + damageList.Length);
            for (int i = 0; i < binaryList.Length; i++) {
                if (binaryList[i] == 1) {
                    for (int j = 0; j < playerPositionList.Count; j++) {
                        if (i == playerPositionList[j] - 1 && j != playerNum - 1) {
                            bombStops.Add(j + 1);
                            if (mode == 1) {
                                localDamages[j] += damageList[i];
                                //Debug.Log("Player " + (j + 1) + " took " + damageList[i] + " bomb damage");
                            } else if (mode == 4) {
                                BS.myLocalDamages[j] += damageList[i];
                            }
                        }
                    }
                }
            }
            if (mode == 2) {
                if (!amSpectator) {
                    if (binaryList[myPosition - 1] == 1) {
                        if (!replay) {
                            healthCalculation -= armorCoefficient * damageList[myPosition - 1];
                            myLocalDamage += damageList[myPosition - 1];
                            //yield return StartCoroutine(CheckHealth());
                            UpdateHealthText(Mathf.RoundToInt(healthCalculation), maxHealth);
                        } else {
                            pseudoHealth -= armorCoefficient * damageList[myPosition - 1];
                            UpdateHealthText(Mathf.RoundToInt(pseudoHealth), maxHealth);
                        }
                        if ((bounce == 0 && MPPI.Contains(myPlayerNum))) {
                        } else {
                            StartCoroutine(ShowPain(new Vector2(GRIDX[index - 1], GRIDY[index - 1])));
                        }
                        //Debug.Log("Blown up");
                    }
                }
            }
            if (mode == 3) {
                if (binaryList[BS.position - 1] == 1) {
                    BS.HPCalculation -= BS.armorCoefficient * damageList[BS.position - 1];
                    BS.myLD += damageList[BS.position - 1];
                }
            }
        }
        if (mode != 3 && mode != 4) {
            bool overlap = false;
            for (int i = 0; i < visionGrid.Count; i++) {
                if (visionGrid[i] != 0 && binaryList[i] == 1) {
                    overlap = true;
                }
            }
            Debug.Log("Explode");
            StartCoroutine(Explosion(index, blastRadius, overlap, bombType));
        }
        yield return null;
    }
    [ServerRpc(RequireOwnership = false)]
    void FreezeServerRpc(int pnum, int mode) {
        FreezeClientRpc(pnum, mode);
    }
    [ClientRpc]
    void FreezeClientRpc(int pnum, int mode) {
        if (mode == 1) {
            if (pnum != myPlayerNum) {
                SpriteRenderer SR = playerList[pnum - 1].GetComponent<SpriteRenderer>();
                StartCoroutine(PlaySound(sounds[45]));
                SR.sprite = playerCostumes[2];
            }
        } else if (mode == 2) {
            SpriteRenderer SR = playerList[pnum - 1].GetComponent<SpriteRenderer>();
            StartCoroutine(Explosion(playerPositionList[pnum - 1], 3, visionGrid[playerPositionList[pnum - 1] - 1] != 0, 4));
            if (pnum == myPlayerNum) {
                SR.sprite = playerCostumes[0];
            } else {
                SR.sprite = playerCostumes[1];
            }

        }
    }
    public LineRenderer lineRenderer;
    IEnumerator Grapple(GameObject playerObj, int prevPos, int newPos, Vector2 bulPos) {
        int repeats = 0;
        float startX = GRIDX[prevPos - 1];
        float startY = GRIDY[prevPos - 1];
        float targetX = GRIDX[newPos - 1];
        float targetY = GRIDY[newPos - 1];
        repeats = Mathf.RoundToInt(FindDistance(GetCoordsFromIndex(prevPos), GetCoordsFromIndex(newPos)) * 2);
        
        float xChange = (targetX - startX) / repeats;
        float yChange = (targetY - startY) / repeats;
        float xIteration = startX;
        float yIteration = startY;
        
        for (int i = 0; i < visionGrid.Count; i++)
        {
            SpriteRenderer VSpriteRenderer = visionObjects[i].GetComponent<SpriteRenderer>();
            Color color = VSpriteRenderer.color;
            color.a = 0;
            VSpriteRenderer.color = color;
            visionObjects[i].GetComponent<SpriteMask>().enabled = false;
        }
        VCSpriterenderer.enabled = true;
        RenderLine(new Vector2(xIteration, yIteration), bulPos);
        for (int i = 0; i < repeats; i++) {
            //add
            xIteration += xChange;
            yIteration += yChange;
            playerObj.transform.position = new Vector3(xIteration, yIteration, Camera.main.nearClipPlane);
            RenderLine(new Vector2(xIteration, yIteration), bulPos);
            int index = GetIndexFromWorldCoords(new Vector2(xIteration, yIteration));
            float myElevation = FindElevation(GRID[index - 1], 1);
            float myVision = FindVision(GRID[index - 1], INVENTORY, myEffects, myEffectStrengths);
            StartCoroutine(CreateFOVRaycast(new Vector3(xIteration, yIteration, Camera.main.nearClipPlane), myElevation, myVision, smokes));

            yield return null;
        }
        
        yield return null;
    }
    IEnumerator RenderTraps(List<List<List<float>>> t) {
        foreach (Transform child in trapsFolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < t.Count; i++) {
            List<List<float>> listTraps = t[i];
            for (int j = 0; j < listTraps.Count; j++) {
                List<float> currentTrap = listTraps[j];
                if (currentTrap[0] == myPlayerNum) {
                    GameObject newTrap = Instantiate(trapPF, new Vector3(GRIDX[i], GRIDY[i], 0), Quaternion.identity);
                    newTrap.transform.SetParent(trapsFolder.transform, true);
                    newTrap.GetComponent<SpriteRenderer>().sprite = trapCostumes[Mathf.RoundToInt(currentTrap[4])];
                    //set scale
                    newTrap.transform.localScale = new Vector3(tileWidth, tileHeight, 1);
                }
            }
        }
        yield return null;
    }
    IEnumerator PlayHitSound(int index) {
        int soundIndex = 0;
        Debug.Log("Butterfly");
        if (index == 12) {
            if (RandomChance(2)) {
                soundIndex = 19;
            } else {
                soundIndex = 20;
            }
        }
        StartCoroutine(PlaySound(sounds[soundIndex]));
        yield return null;
    }
    IEnumerator Flashbang() {
        GameObject FB = Instantiate(flashbangPrefab, CANVAS);
        Animator anim = FB.GetComponent<Animator>();
        if (RandomChance(2)) {
            StartCoroutine(PlaySound(sounds[17]));
        } else {
            StartCoroutine(PlaySound(sounds[18]));
        }
        yield return StartCoroutine(PlayAnimationAndWait(anim, "Flashbang"));
        Destroy(FB);
        yield return null;
    }
    IEnumerator PlayAnimationAndWait(Animator anim, string animation) {
        anim.Play(animation);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animation));
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
    [ServerRpc(RequireOwnership = false)]
    void YSTEServerRpc(int pnum) {
        YSTEClientRpc(pnum);
    }
    [ClientRpc]
    void YSTEClientRpc(int pnum) {
        if (pnum == myPlayerNum) {
            StartCoroutine(CreateMessageText("You stunned the enemy!", 0, -100, 1, false));
        }
    }
    bool USTAWConfirm = false;
    IEnumerator UpdateSkippedTurnsAndWait(int pnum, int stunLength) {
        USTAWConfirm = false;
        USTServerRpc(pnum, stunLength, myPlayerNum);
        while (!USTAWConfirm) {
            yield return null;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void USTServerRpc(int pnum, int stunLength, int sender) {
        skippedTurns[pnum - 1] += stunLength;
        if (skippedTurns[pnum - 1] < 0) {
            skippedTurns[pnum - 1] = 0;
        }
        if (!botList[pnum - 1]) {
            ConfirmUSTClientRpc(pnum);
        } else {
            ConfirmUSTClientRpc(sender);
        }
    }
    [ClientRpc]
    void ConfirmUSTClientRpc(int pnum) {
        if (pnum == myPlayerNum) {
            USTAWConfirm = true;
        }
    }
    /*[ServerRpc(RequireOwnership = false)]
    void SmokeListsServerRpc(int i, int life, int pnum) {
        smokeLifespans[i] = life;
        smokeOwners[i] = pnum;
    }*/
    [ServerRpc(RequireOwnership = false)]
    void SLSServerRpc(int i, int life) {
        smokeLifespans[i] = life;
    }
    [ServerRpc(RequireOwnership = false)]
    void SOServerRpc(int i, int pnum) {
        smokeOwners[i] = pnum;
    }
    IEnumerator BulletBounce(GameObject bul, float angleDeg, int playerNum, int mode, int trap) {
        bool hearAndSee = false;
        if (mode == 1 || mode == 2) {
            hearAndSee = true;
        }
        if (hearAndSee) {
           StartCoroutine(PlaySound(sounds[8])); 
        }
        for (int k = 0; k < 4; k++) {
            float coe = 0.1f;
            bul.transform.position += (Vector3)(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * -1 * tileWidth * coe); //bul.transform.position += (Vector3)(dir * -1 * tileWidth * 20 * Time.deltaTime);
            int index = GetIndexFromWorldCoords(new Vector2(bul.transform.position.x, bul.transform.position.y));
            if (!hearAndSee) {
                    Color colour = bul.GetComponent<SpriteRenderer>().color;
                    colour.a = 0f;
                    bul.GetComponent<SpriteRenderer>().color = colour;
            } else {
                if (index != 0 && visionGrid[index - 1] == 0 || (trap != 0 && mode == 2)) {
                    Color colour = bul.GetComponent<SpriteRenderer>().color;
                    colour.a = 0f;
                    bul.GetComponent<SpriteRenderer>().color = colour;
                } else {
                    Color colour = bul.GetComponent<SpriteRenderer>().color;
                    colour.a = 1f;
                    bul.GetComponent<SpriteRenderer>().color = colour;
                }
            }
            
            List<int> matchingPPI = new List<int>();
            for (int j = 0; j < playerPositionList.Count; j++) {
                if (playerPositionList[j] == index && j != playerNum - 1) {
                    matchingPPI.Add(j + 1);
                }
            }
            yield return new WaitForSeconds(0.05f);
            if (index == 0) {
                bul.transform.position += (Vector3)(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * -1 * tileWidth * coe); //bul.transform.position += (Vector3)(dir * -1 * tileWidth * 20 * Time.deltaTime);
                if (hearAndSee) {
                    StartCoroutine(PlaySound(sounds[8]));
                }
                yield return new WaitForSeconds(0.05f);
            } else {
                if (mode == 1) {
                    if (index == myPosition) {
                        bul.transform.position += (Vector3)(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * 1.5f * tileWidth * coe); //bul.transform.position += (Vector3)(dir * 1.5f * tileWidth * 20 * Time.deltaTime);
                        if (hearAndSee) {
                            StartCoroutine(PlaySound(sounds[8]));
                        }
                        yield return new WaitForSeconds(0.05f);
                    }
                } else if (mode == 2) {
                    if (index == playerPositionList[playerNum - 1]) {
                        bul.transform.position += (Vector3)(new Vector2(Mathf.Cos(angleDeg * Mathf.Deg2Rad), Mathf.Sin(angleDeg * Mathf.Deg2Rad)) * 1.5f * tileWidth * coe); //bul.transform.position += (Vector3)(dir * 1.5f * tileWidth * 20 * Time.deltaTime);
                        Debug.Log("bounce off enemy");
                        if (hearAndSee) {
                            StartCoroutine(PlaySound(sounds[8]));
                        }
                        yield return new WaitForSeconds(0.05f);
                    }
                }
            }
        }
    }
    IEnumerator Explosion(int index, float blastRadius, bool overlap, int type) {
        Debug.Log("Explosion type: " + type);
        GameObject explode = Instantiate(explosionPrefab, new Vector2(GRIDX[index - 1], GRIDY[index - 1]), Quaternion.Euler(0f, 0f, 0f));
        explode.transform.localScale = new Vector3(tileWidth * 5, tileHeight * 5, 1);
        if (amSpectator || overlap) {
            Color color = explode.GetComponent<SpriteRenderer>().color;
            color.a = (200f / 255f);
            explode.GetComponent<SpriteRenderer>().color = color;
        } else {
            Color color = explode.GetComponent<SpriteRenderer>().color;
            color.a = 0f;
            explode.GetComponent<SpriteRenderer>().color = color;
        }
        if (type == 1) {
            StartCoroutine(PlaySound(sounds[9]));
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[0];
            Vector3 newScale = explode.transform.localScale;
            newScale.x = 2 * tileWidth * blastRadius * 6f / 5f;
            newScale.y = 2 * tileHeight * blastRadius * 6f / 5f;
            explode.transform.localScale = newScale;
            yield return new WaitForSeconds(0.1f);
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[1];
            yield return new WaitForSeconds(0.1f);
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[2];
            yield return new WaitForSeconds(0.1f);
            Destroy(explode);
        } else if (type == 2 || type == 3) {
            if (type == 2) {
                StartCoroutine(PlaySound(sounds[43]));
                explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[3];
            } else if (type == 3) {
                StartCoroutine(PlaySound(sounds[44]));
                explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[4];
            }
            Vector3 newScale = explode.transform.localScale;
            newScale.x = 2 * tileWidth * blastRadius * 6f / 5f;
            newScale.y = 2 * tileHeight * blastRadius * 6f / 5f;
            explode.transform.localScale = newScale;
            for (int i = 0; i < 20; i++) {
                newScale = explode.transform.localScale;
                newScale.x += tileWidth * 0.25f;
                newScale.y += tileHeight * 0.25f;
                explode.transform.localScale = newScale;
                Color color = explode.GetComponent<SpriteRenderer>().color;
                color.a -= (10f / 255f);
                explode.GetComponent<SpriteRenderer>().color = color;
                yield return null;
            }
            Destroy(explode);
        } else if (type == 4) {
            StartCoroutine(PlaySound(sounds[46]));
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[5];
            Vector3 newScale = explode.transform.localScale;
            newScale.x = myPlayerObject.transform.localScale.x; //general tile size
            newScale.y = myPlayerObject.transform.localScale.y;
            explode.transform.localScale = newScale;
            yield return new WaitForSeconds(0.1f);
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[6];
            yield return new WaitForSeconds(0.1f);
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[7];
            yield return new WaitForSeconds(0.1f);
            explode.GetComponent<SpriteRenderer>().sprite = explosionCostumes[8];
            yield return new WaitForSeconds(0.1f);
            Destroy(explode);
        }
        yield return null;
    }
    int[] BombBinary(int index, float blastRadius) {
        int[] damages = new int[GRID.Count];
        Vector2 originalPosition = GetCoordsFromIndex(index);
        for (int i = 0; i < GRID.Count; i++) {
            Vector2 currentPosition = GetCoordsFromIndex(i + 1);
            float distance = FindDistance(originalPosition, currentPosition);
            if (distance <= blastRadius) {
                damages[i] = 1;
            } else {
                damages[i] = 0; 
            }
        }
        return damages;
    }
    float[] BombDamage(int index, float blastRadius, float damage) { //don't necessarily use damage is 0 as an indicator of out of bomb radius
        float[] damages = new float[GRID.Count];
        Vector2 originalPosition = GetCoordsFromIndex(index);
        for (int i = 0; i < GRID.Count; i++) {
            Vector2 currentPosition = GetCoordsFromIndex(i + 1);
            float distance = FindDistance(originalPosition, currentPosition);
            if (distance <= blastRadius) {
                if (distance > 1) {
                    damages[i] = GetBombDamage(blastRadius, distance, damage);
                } else {
                    damages[i] = damage;
                }
                
            } else {
                damages[i] = 0; 
            }
        }
        return damages;
    }
    public float GetBombDamage(float blastRadius, float distance, float damage) {
        return (((blastRadius - (distance - 1)) / blastRadius) * damage);
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeAliveStateServerRpc(int num, bool newState, ServerRpcParams rpcParams = default) {
        ulong senderId = rpcParams.Receive.SenderClientId;
        playerAliveList[num - 1] = newState;
        var target = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { senderId }
            }
        };
        ConfirmCheckHealthClientRpc(true, true, target);
    }
    [ClientRpc]
    void ConfirmCheckHealthClientRpc(bool result, bool changed, ClientRpcParams rpcParams = default) {
        changeHealthConfirm = changed;
        checkHealthConfirm = result;
    }
    bool checkHealthConfirm;
    bool changeHealthConfirm;
    IEnumerator CheckHealth(int mode, BotScript BS) {
        if (mode == 1) {
            if (HEALTH <= 0) {
                checkHealthConfirm = false;
                ChangeAliveStateServerRpc(myPlayerNum, false);
                while (!checkHealthConfirm) {
                    yield return null;
                }
            }
        } else if (mode == 2) {
            if (BS.HP <= 0) {
                checkHealthConfirm = false;
                ChangeAliveStateServerRpc(BS.playerNum, false);
                while (!checkHealthConfirm) {
                    yield return null;
                }
            }
        }
        yield return null;
    }
    void OnElementClick(int index) {
        useItem = index;
        buttonClicked = 6;
    }
    void OnInventoryElementClick(int index) {
        StartCoroutine(ToggleStatsBook(true, index + 1));
    }
    void OnBookInventoryElementClick(int index) {
        if (((index + 1) - bookInventoryPage) != 0) {
            StartCoroutine(FlipThruBookInventory((index + 1) - bookInventoryPage));
        }
    }
    void OnCWTDClick(int index) {
        CWTDCLicked = index + 1;
    }
    IEnumerator ShakeUse() {
        float shake = 0.1f; 
        float angle = 0;
        float OGXMin = useButton.GetComponent<RectTransform>().anchorMin.x;
        float OGYMin = useButton.GetComponent<RectTransform>().anchorMin.y;
        float OGXMax = useButton.GetComponent<RectTransform>().anchorMax.x;
        float OGYMax = useButton.GetComponent<RectTransform>().anchorMax.y;
        for (int i = 0; i < 400; i++) {
            if (!searching) {
                useButton.GetComponent<RectTransform>().anchorMin = new Vector2(OGXMin - (shake * (Mathf.Sin(angle))), OGYMin);
                useButton.GetComponent<RectTransform>().anchorMax = new Vector2(OGXMax - (shake * (Mathf.Sin(angle))), OGYMax);
                shake *= 0.99f;
                angle += 0.1f;
                yield return null;
            } else {
                break;
            }
        }
        useButton.GetComponent<RectTransform>().anchorMin = new Vector2(OGXMin, OGYMin);
        useButton.GetComponent<RectTransform>().anchorMax = new Vector2(OGXMax, OGYMax);
    }
    IEnumerator FlashUse() {
        for (int i = 0; i < 2; i++) {
            useButton.GetComponent<Image>().sprite = buttons[2];
            if (searching) 
                break;
            yield return new WaitForSeconds(0.2f);
            if (searching) 
                break;
            useButton.GetComponent<Image>().sprite = buttons[3];
            if (searching) 
                break;
            yield return new WaitForSeconds(0.2f);
            if (searching) 
                break;
        }
        useButton.GetComponent<Image>().sprite = buttons[2];
        if (!searching) {
            Color color = useButton.GetComponent<Image>().color;
            color = SetBrightness(color, 0.75f);
            color.a = (166f / 255f);
            useButton.GetComponent<Image>().color = color;
            //Debug.Log("Use dimmed");
        }
    }
    List<int> FindUsable(List<int> inv) {
        List<int> usables = new List<int>();
        for (int i = 0; i < inv.Count; i++) {
            if (itemBools[inv[i] - 1][0] == true) 
                usables.Add(inv[i]);
        }
        return usables;
    }
    Color SetBrightness(Color c, float newBrightness) {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);
        v = newBrightness;
        return Color.HSVToRGB(h, s, v);
    }
    [ServerRpc(RequireOwnership = false)]
    void RequestEffectsServerRpc(int num) {
        if (botList[num - 1]) {
            BotScript botScript = playerList[num - 1].GetComponent<BotScript>();
            EffectsServerRpc(num, botScript.botEffects.ToArray(), botScript.position, 2, myPlayerNum);
        } else {
            RequestEffectsClientRpc(num);
        }
    }
    [ClientRpc]
    void RequestEffectsClientRpc(int num) {
        if (num == myPlayerNum) {
            EffectsServerRpc(myPlayerNum, myEffects.ToArray(), myPosition, 1, myPlayerNum);
        }
    }
    int CountTrues(List<bool> l) {
        int count = 0;
        foreach (bool b in l) {
            if (b) {
                count++;
            }
        }
        return count;
    }
    bool changedFrozen = false; //currently this is only tied with replayable, add more in the future if necessary
    bool replayable = false;
    public bool replayable2 = false;
    int replayable3 = 0;
    bool replayable4 = false;
    bool replayable5 = false;
    bool replayable6 = false;
    bool R6Smoke = false;
    bool replaying = false;
    bool replaying2 = false;
    bool replaying3 = false;
    bool replaying4 = false;
    bool replaying5 = false;
    bool replaying6 = false;
    IEnumerator ReplayEnemyAction() {
        //Debug.Log("Replayable: " + replayable);
        bool compoundBool = replayable2 && (CountTrues(enemyHealthsVisible) >= 1); //compoundBool = replayable2 && at least one enemy health is showing
        //Note to self: enemyHealthsVisible[myPlayerNum - 1] will always be false
        bool boring = false;
        bool initReplaying = replaying;
        bool initReplayable = replayable;
        bool localReplaying2 = false;
        bool localReplaying3 = false;
        bool localReplaying4 = false;
        bool localReplaying6 = false;
        bool LR6Smoke = false;
        int initReplayable3 = replayable3;
        bool initReplayable5 = replayable5;
        bool initReplayable6 = replayable6;
        bool initR6Smoke = R6Smoke;
        bool initReplaying5 = replaying5;
        bool initReplaying6 = replaying6;
        bool ewo2 = false;
        bool smokeThrown = false;
        bool enemyActioned = false;
        List<int> PDH = new List<int>(playersDeltaHealth);
        List<int> EWO = new List<int>(effectsWornOff);
        if (initReplayable6 && !initReplaying6 && !initReplaying) {
            if (EWO.Contains(2)) {
                replaying6 = true;
                localReplaying6 = true;
                ewo2 = true;
                yield return StartCoroutine(VisionHouse(0, smokes, prevEnemyEffects2, 0, 0, prevPlayerPositionList, prevMyEffects, prevMyEffectStrengths)); //prevEnemyEffects is for enemy using and prevEnemyEffects2 is for enemy wearing off
            }
            if (EWO.Contains(6)) {
                replaying6 = true;
                localReplaying6 = true;
                yield return StartCoroutine(VitalsStatus(myPlayerNum, prevMyEffects, 2, 1, enemyHealthsVisible, null)); //the list int 6 thing is lowkey a placeholder/life hack
                
            }
            if (initR6Smoke) {
                replaying6 = true;
                localReplaying6 = true;
                LR6Smoke = true;
                if (EAMemory != null && EAMemory[0] != 0 && EAMemory[0] != myPlayerNum) { //\C
                    if (Mathf.RoundToInt(EAMemory[31]) == 1) {
                        smokeThrown = true;
                    }
                }
                if (!smokeThrown) {
                    int enemyPosition = 0;
                    int EA20 = 0;
                    int EA0 = 0;
                    if (EAMemory != null && EAMemory[0] != 0 && EAMemory[0] != myPlayerNum) { //\C
                        enemyPosition = playerPositionList[Mathf.RoundToInt(EAMemory[0]) - 1];
                        EA20 = Mathf.RoundToInt(EAMemory[20]);
                        EA0 = Mathf.RoundToInt(EAMemory[0]);
                        if (Mathf.RoundToInt(EAMemory[20]) == 1) {
                            enemyPosition = Mathf.RoundToInt(EAMemory[21]);
                        }
                    }
                    yield return StartCoroutine(ShowPrevSmokes(ewo2, localReplaying4, EA20, EA0, prevSmokes2, enemyPosition));
                    yield return StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, prevSmokes2));
                }
            }
            if (localReplaying6) {
                yield return new WaitForSeconds(0.2f);
            }
        }
        if (initReplayable5 && !initReplaying5) {
            replaying5 = true;
            yield return StartCoroutine(AnimateAirdrop(prevAirdropPos, prevAirdropIndex));
            replaying5 = false;
        }
        
        if (compoundBool) {
            if (!replaying2) {
                replaying2 = true;
                localReplaying2 = true;
                foreach (int player in PDH) {
                    if (player != myPlayerNum) {
                        if (enemyHealthsVisible[player - 1]) {
                            StartCoroutine(UpdateEnemyHealthText(player - 1, prevHealths[player - 1], prevMaxHealths[player - 1]));
                        }
                    }
                }
            }
        }
        Debug.Log("initR3: " + initReplayable3);
        if (initReplayable3 == 1 || initReplayable3 == 2 || initReplayable3 == 3 || initReplayable3 == 4) {
            if (!replaying3) {
                replaying3 = true;
                localReplaying3 = true;
                List<List<List<float>>> trapsTemp = CloneLLLF(traps);
                trapsTemp[prevTrapPosition - 1] = prevListTraps;
                yield return StartCoroutine(RenderTraps(trapsTemp));
                if (initReplayable3 == 1 || initReplayable3 == 3) {
                    UpdateHealthText(Mathf.RoundToInt(pseudoHealth), maxHealth);
                }
            }
        }
        Debug.Log("replayable4: " + replayable4);
        if (replayable4 && !replaying4) {  //change replayable4 to initReplayable4?
            //if (EAMemory[0] == 0) { //temporary solution
                replaying4 = true;
                localReplaying4 = true;
                Debug.Log("prevEnemyEffected: " + prevEnemyEffected);
                Debug.Log("EAMEMORY[21]: " + EAMemory[21]);
                int enPos = playerPositionList[prevEnemyEffected - 1];
                if (EAMemory[20] == 1) {
                    enPos = Mathf.RoundToInt(EAMemory[21]);
                }
                Debug.Log("Invis: " + prevEnemyEffects2[prevEnemyEffected - 1].Contains(1));
                StartCoroutine(EnemyVisible(visionGrid, playerList[prevEnemyEffected - 1], enPos, prevEnemyEffects2[prevEnemyEffected - 1].Contains(1))); //use prevVisionGrid
                Debug.Log("hide enemy");
                //yield return new WaitForSeconds(1);
                boring = true;
                yield return new WaitForSeconds(0.2f); //\A
            //}
        }
        bool initCF = changedFrozen;
        int initFT = frozenTurns;
        if (initCF) {
            if (initFT > 0) {
                playerList[myPlayerNum - 1].GetComponent<SpriteRenderer>().sprite = playerCostumes[0];
            }
        }
        if (replayable && !replaying) {
            replaying = true;
            Debug.Log("Replaying");
            Debug.Log("EAMem[0] = " + EAMemory[0]);
            if (EAMemory != null && EAMemory[0] != 0 && EAMemory[0] != myPlayerNum) { //\C
            //if (EAMemory != null && EAMemory[0] != myPlayerNum) {
                Debug.Log("REPLAYING");
                Debug.Log("LR4: " + localReplaying4);
                if (Mathf.RoundToInt(EAMemory[31]) == 1) {
                    smokeThrown = true;
                }
                yield return StartCoroutine(EnemyAction(EAMemory, EBMemory, null, TIMemory, SIMemory, true, localReplaying4, ewo2, smokeThrown, LR6Smoke, result => {
                    boring = result;
                    /*if (!boring) {
                        Debug.Log("Boring1");
                    }*/
                }));
                enemyActioned = true;
            } else {
                if (localReplaying2 || localReplaying3 || localReplaying4) { //\B
                    yield return new WaitForSeconds(0.2f); //\A
                }
                boring = true;
            }
            replaying = false;
            if (boring) {
                if (localReplaying2 || localReplaying3 || localReplaying4) { //\B
                    yield return new WaitForSeconds(0.3f); //calculated carefully based on the contents of EnemyAction
                }
            }
        } else {
            if (localReplaying2 || localReplaying3 || localReplaying4) {
                yield return new WaitForSeconds(0.5f);
            }
        }
        if (localReplaying6) {
            yield return new WaitForSeconds(0.2f);
            if (EWO.Contains(2)) {
                yield return StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
            }
            if (EWO.Contains(6)) {
                yield return StartCoroutine(VitalsStatus(myPlayerNum, myEffects, 1, 1, enemyHealthsVisible, null));
            }
            if (LR6Smoke) {
                //yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(UpdateIsSmoked(smokes));
                yield return StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
                Debug.Log("LR6Smoke");
                //if (!enemyActioned) {
                yield return StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
                //}
            }
            replaying6 = false;
        }
        Debug.Log("boring: " + boring);
        if (initCF) {
            if (initFT > 0) {
                StartCoroutine(PlaySound(sounds[45]));
                playerList[myPlayerNum - 1].GetComponent<SpriteRenderer>().sprite = playerCostumes[2];
            }
        }
        if (localReplaying3) {
            //Debug.Log("Stepping on trap mode 2");
            if (initReplayable3 == 1 || initReplayable3 == 3) {
                yield return StartCoroutine(SteppingOnTrap(2, 1, prevListTraps, prevTrapPosition, null));
            } else if (initReplayable3 == 2 || initReplayable3 == 4) {
                yield return StartCoroutine(TrapClient(2, 1, prevTrapPlayerNum, prevTrapPosition, prevRemovedTraps.ToArray(), prevListTraps));
                //if (prevRemovedTraps.Count > 0)
                StartCoroutine(RenderTraps(traps));

            }
            replaying3 = false;
        }
        if (localReplaying2) {
            foreach (int player in PDH) {
                if (player != myPlayerNum) {
                    if (enemyHealthsVisible[player - 1]) {
                        StartCoroutine(UpdateEnemyHealthText(player - 1, healths[player - 1], maxHealths[player - 1]));
                    }
                }
            }
            replaying2 = false;
        } 
        if (localReplaying4) {
            StartCoroutine(EnemyVisible(visionGrid, playerList[prevEnemyEffected - 1], playerPositionList[prevEnemyEffected - 1], enemyEffects[prevEnemyEffected - 1].Contains(1)));
            Debug.Log("show enemy");
            replaying4 = false;
        }
        if (!compoundBool && initReplayable3 == 0) {
            if (initReplayable && !initReplaying && !localReplaying4) {
                if (boring) {
                    StartCoroutine(NothingInteresting());
                }
            }
        }
        yield return null;
    }
    IEnumerator ShowPrevSmokes(bool ewo2, bool LR4, int EA20, int EA0, List<float> currentPrevSmokes, int enemyPosition) {
        yield return StartCoroutine(UpdateIsSmoked(currentPrevSmokes));
        if (!ewo2) {
            if (LR4) {
                if (EA20 == 1) {
                    yield return StartCoroutine(VisionHouse(0, currentPrevSmokes, prevEnemyEffects2, EA0, enemyPosition, playerPositionList, myEffects, myEffectStrengths));
                } else {
                    yield return StartCoroutine(VisionHouse(0, currentPrevSmokes, prevEnemyEffects2, 0, 0, playerPositionList, myEffects, myEffectStrengths));
                }
            } else {
                if (EA20 == 1) {
                    yield return StartCoroutine(VisionHouse(0, currentPrevSmokes, enemyEffects, EA0, enemyPosition, playerPositionList, myEffects, myEffectStrengths));
                } else {
                    yield return StartCoroutine(VisionHouse(0, currentPrevSmokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
                }
            }
        }
    }
    float myLocalDamage = 0;
    float[] EAMemory;
    float[] EBMemory;
    int[] TIMemory;
    int[] SIMemory;
    int prevPseudoHealth;
    List<float> prevSmokes;
    List<float> prevSmokes2;
    int bulDone = 0;
    float healthCalculation;
    IEnumerator EnemyAction(float[] EA, float[] EB, float[] ELDs, int[] ETI, int[] ESI, bool replay, bool LR4, bool ewo2, bool smokeThrown, bool LR6Smoke, System.Action<bool> callback) {
        bool nothingInteresting = true;
        if (Mathf.RoundToInt(EA[28]) == 0) {
            int enemyPosition = playerPositionList[Mathf.RoundToInt(EA[0]) - 1];
            if (Mathf.RoundToInt(EA[20]) == 1) {
                enemyPosition = Mathf.RoundToInt(EA[21]);
            }
            if (!replay) {
                prevEnemyEffects = enemyEffects.Select(inner => new List<int>(inner)).ToList();
                RequestEffectsServerRpc(Mathf.RoundToInt(EA[0]));
            } else {
                if (!LR4) {
                    bool same = (prevEnemyEffects[Mathf.RoundToInt(EA[0]) - 1].Contains(1) == enemyEffects[Mathf.RoundToInt(EA[0]) - 1].Contains(1));
                    if (!same) {
                        bool enemyInVision;
                        if (visionGrid[enemyPosition - 1] == 0) {
                            enemyInVision = false;
                        } else {
                            enemyInVision = true;
                        }
                        if (enemyInVision) {
                            StartCoroutine(EnemyVisible(visionGrid, playerList[Mathf.RoundToInt(EA[0]) - 1], playerPositionList[Mathf.RoundToInt(EA[0]) - 1], prevEnemyEffects[Mathf.RoundToInt(EA[0]) - 1].Contains(1)));
                            yield return new WaitForSeconds(0.5f);
                            //Debug.Log("invis used");
                            nothingInteresting = false;
                            Debug.Log("NI1");
                            Debug.Log("something interesting");
                            StartCoroutine(EnemyVisible(visionGrid, playerList[Mathf.RoundToInt(EA[0]) - 1], playerPositionList[Mathf.RoundToInt(EA[0]) - 1], enemyEffects[Mathf.RoundToInt(EA[0]) - 1].Contains(1)));
                        }
                    }
                }
            }
            if (replay) {
                prevPseudoHealth = Mathf.RoundToInt(pseudoHealth);
                UpdateHealthText(Mathf.RoundToInt(pseudoHealth), maxHealth);
                
                if (Mathf.RoundToInt(EA[20]) == 1) {
                    /*if (LR4) {
                        StartCoroutine(EnemyVisible(playerList[Mathf.RoundToInt(EA[0]) - 1], enemyPosition, prevEnemyEffects2[prevEnemyEffected - 1].Contains(1))); //use prevVisionGrid
                    } else {
                        StartCoroutine(EnemyVisible(playerList[Mathf.RoundToInt(EA[0]) - 1], enemyPosition, enemyEffects[Mathf.RoundToInt(EA[0]) - 1].Contains(1)));
                    }*/
                    //SetCharacterPosition(playerList[Mathf.RoundToInt(EA[0]) - 1], enemyPosition);
                    //PositionEnemyClientRpc(Mathf.RoundToInt(EA[0]), enemyPosition);
                } else {
                    yield return new WaitForSeconds(0.2f); //A
                }
            } else {
                pseudoHealth = HEALTH;
                healthCalculation = HEALTH;
                armorCoefficient = GetArmorCoefficient(INVENTORY); //be wary if bullets can break armor or sm
            }
            if (!replay) {
                prevSmokes = new List<float>(smokes);
                myLocalDamage = EA[23];
            } else {
                List<float> currentPrevSmokes = new List<float>(smokes);
                if (smokeThrown) {
                    currentPrevSmokes = new List<float>(prevSmokes);
                }
                yield return StartCoroutine(ShowPrevSmokes(ewo2, LR4, Mathf.RoundToInt(EA[20]), Mathf.RoundToInt(EA[0]), currentPrevSmokes, enemyPosition)); //not just for smokes btw
                if (Mathf.RoundToInt(EA[20]) == 1) { //changeLoc is only possible if you're doing EnemyAction
                    //technically should wait till enemy is appropriately hidden
                    SetCharacterPosition(playerList[Mathf.RoundToInt(EA[0]) - 1], enemyPosition);
                    yield return new WaitForSeconds(0.2f); //\A
                }
                if (smokeThrown) {
                    StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, currentPrevSmokes));
                }
            }
            Debug.Log("myLocalDamage = " + myLocalDamage);
            //Debug.Log("Enemy shooting");
            //Debug.Log(EA[2]);
            if (EA[24] != 0) {
                List<float> currentTrap = null;
                if (!replay) {
                    currentTrap = traps[Mathf.RoundToInt(EA[25])][Mathf.RoundToInt(EA[26])];
                } else {
                    yield return StartCoroutine(RenderTraps(prevTraps));
                    currentTrap = prevTraps[Mathf.RoundToInt(EA[25])][Mathf.RoundToInt(EA[26])];
                }
                yield return StartCoroutine(PlaySoundAndWait(sounds[49]));
                yield return StartCoroutine(ExplodeBomb(Mathf.RoundToInt(EA[25]) + 1, currentTrap[3], currentTrap[2], 0, new List<int>(), Mathf.RoundToInt(EA[0]), 1, 1, replay, 2, null));
                if (!replay) {
                    prevTraps = CloneLLLF(traps);
                    traps[Mathf.RoundToInt(EA[25])].RemoveAt(Mathf.RoundToInt(EA[26]));
                }
                yield return StartCoroutine(RenderTraps(traps));
                nothingInteresting = false;
                Debug.Log("NI2");
            }
            Vector2 EV = new Vector2(GRIDX[enemyPosition - 1], GRIDY[enemyPosition - 1]);
            bulDone = 0;
            bulStops = new List<int>();
            bombStops = new List<int>();
            if (EA[2] > 0) {
                nothingInteresting = false;
                Debug.Log("NI3");
            }
            for (int i = 0; i < EA[2]; i++) {
                float angleDeg = EB[i];
                GameObject bul = Instantiate(bulletPrefab, EV, Quaternion.Euler(0f, 0f, angleDeg));
                bul.GetComponent<SpriteRenderer>().sprite = bulletCostumes[Mathf.RoundToInt(EA[8])];
                bul.transform.localScale = new Vector3(tileWidth * 5, tileHeight * 5, 1);
                int trapI = 0;
                if (i < ETI.Length) {
                    trapI = ETI[i];
                }
                Debug.Log("Before smokeI");
                int smokeI = 0;
                if (i < ESI.Length) {
                    smokeI = ESI[i];
                    Debug.Log("smokeI = " + smokeI);
                }
                StartCoroutine(BulletMove(Mathf.RoundToInt(EA[1]) - 1, bul, angleDeg, EA[6], EA[5], EA[7], enemyPosition, Mathf.RoundToInt(EA[0]), 2, EA[4], Mathf.RoundToInt(EA[9]), Mathf.RoundToInt(EA[10]), EA[11], EA[12], EA[13], EA[14], Mathf.RoundToInt(EA[15]), Mathf.RoundToInt(EA[16]), Mathf.RoundToInt(EA[17]), Mathf.RoundToInt(EA[18]), Mathf.RoundToInt(EA[19]), Mathf.RoundToInt(EA[20]), trapI, smokeI, Mathf.RoundToInt(EA[27]), replay, LR6Smoke, null));
                //Debug.Log("bulmove2");
                StartCoroutine(ItemSound(Mathf.RoundToInt(EA[1]) - 1));
                StartCoroutine(ItemCostume(Mathf.RoundToInt(EA[1]) - 1, bul, i));
                yield return new WaitForSeconds(EA[3]);
            }
            while (bulDone != EA[2]) {
                yield return null;
            }
            if (itemIntLists[Mathf.RoundToInt(EA[1]) - 1][4].Contains(11)) {
                StartCoroutine(CreateMessageText("The enemy is tracking you!", 300, 200, 1, false));
                if (replay) {
                    nothingInteresting = false;
                    Debug.Log("NI4");
                    yield return StartCoroutine(StartUAV(2));
                } else {
                    yield return StartCoroutine(StartUAV(2));
                }
                
            }
            if (Mathf.RoundToInt(EA[20]) == 1) {
                if (!replay) {
                    playerPositionList[Mathf.RoundToInt(EA[0]) - 1] = Mathf.RoundToInt(EA[22]);
                }
                if (!ewo2) {
                    List<float> currentSmokes = new List<float>(smokes);
                    if (LR6Smoke) {
                        currentSmokes = new List<float>(prevSmokes2);
                    }
                    yield return StartCoroutine(UpdateIsSmoked(currentSmokes));
                    if (!amSpectator) {
                        yield return StartCoroutine(VisionHouse(0, currentSmokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths)); //notice how this might happen twice
                    }
                }
                SetCharacterPosition(playerList[Mathf.RoundToInt(EA[0]) - 1], playerPositionList[Mathf.RoundToInt(EA[0]) - 1]);
                //yield return new WaitForSeconds(2);
                //PositionEnemyClientRpc(Mathf.RoundToInt(EA[0]), playerPositionList[Mathf.RoundToInt(EA[0]) - 1]);
                //Debug.Log("PECR");
            }
            if (replay) {
                pseudoHealth = prevPseudoHealth;
                UpdateHealthText(HEALTH, maxHealth);
                List<float> currentSmokes = new List<float>(smokes);
                if (LR6Smoke) {
                    currentSmokes = new List<float>(prevSmokes2);
                }
                if (smokeThrown) {
                    if (Mathf.RoundToInt(EA[20]) != 1) {
                        if (!ewo2) {
                            yield return StartCoroutine(UpdateIsSmoked(currentSmokes));
                            StartCoroutine(VisionHouse(0, currentSmokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths)); //notice how this might happen twice
                            //yield return new WaitForSeconds(0.2f);
                        }
                    }
                }
            }
            if (!amSpectator) {
                if (!replay) {
                    //HEALTH = Mathf.RoundToInt(healthCalculation);
                    Debug.Log("myLocalDamage = " + myLocalDamage);
                    //HEALTH -= Mathf.RoundToInt(myLocalDamage * armorCoefficient);
                    HEALTH -= Mathf.RoundToInt(armorCoefficient * ELDs[myPlayerNum - 1]);
                    //Debug.Log("health is " + HEALTH);
                    UpdateHealthText(HEALTH, maxHealth);
                    HealthsServerRpc(myPlayerNum, HEALTH, maxHealth, false, true, 1);
                    yield return StartCoroutine(CheckHealth(1, null));
                }
            
                if (LR6Smoke) {
                    StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, prevSmokes2));
                } else {
                    StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
                }
            }
            
            //Now all the health is checked and server has updated it
            if (EA[17] == 0 && EA[18] == 0 && EA[20] == 0) {
                if (EA[10] == 0) {
                    if (bulStops.Contains(3)) //what if it hits someone else you gotta look into that
                        StartCoroutine(CreateMessageText("You got hit!", 0, -100, 1, false));
                    else {
                        if (bulStops.Count > 0)
                            StartCoroutine(CreateMessageText("Enemy shot but missed you", 0, -100, 1, false));
                    }
                } else if (EA[10] == 1) {
                    if (EA[19] == 0) {
                        if (bombStops.Contains(myPlayerNum))
                            StartCoroutine(CreateMessageText("You got hit!", 0, -100, 1, false));
                        else {
                            if (bombStops.Count > 0)
                                StartCoroutine(CreateMessageText("Enemy hit someone else, but not you", 0, -100, 1, false));
                            else
                                StartCoroutine(CreateMessageText("Enemy tried and failed to hit anyone", 0, -100, 1, false));
                        }
                    }
                }
            }
            if (EA[18] != 0) {
                StartCoroutine(CreateMessageText("Enemy tried to stun you!", 200, 100, 1, false));
            }
            if (EA[19] == 1) {
                StartCoroutine(CreateMessageText("Enemy planted a trap!", 200, 100, 1, false));
            }
            if (EA[20] != 0) {
                StartCoroutine(CreateMessageText("The enemy has grappled", 300, 200, 1, false));
            }
            if (!replay) {
                StartCoroutine(CheckYouDied());
            }
        } else {
            //Debug.Log("Open airdrop");
            StartCoroutine(CreateMessageText("The enemy opened an airdrop!", 250, 250, 1, true));
            if (replay) {
                yield return StartCoroutine(OpenAirdrop(Mathf.RoundToInt(EA[28]), -1, 2));
                nothingInteresting = false;
            } else { //joe
                int index = FindInIntList(airdrops, Mathf.RoundToInt(EA[28]));
                yield return StartCoroutine(OpenAirdrop(Mathf.RoundToInt(EA[28]), index, 1));
                airdrops.RemoveAt(index);
                airdropGOs.RemoveAt(index);
                if (index < prevAirdropIndex) {
                    prevAirdropIndex--;
                }
            }
        }
        if (replay && nothingInteresting) {
            callback(true);
        } else {
            callback(false);
        }
    }
    public int FindInIntList(List<int> l, int target) {
        for (int i = 0; i < l.Count; i++) {
            if (l[i] == target) {
                return i;
            }
        }
        return -1;
    }
    IEnumerator CheckYouDied() {
        if (!amSpectator) {
            if (playerAliveList[myPlayerNum - 1] == false) {
                StartCoroutine(CreateMessageText("You died!", 0, 0, 4, false));
                StartCoroutine(PlaySound(sounds[33]));
                mainMenuButton.SetActive(true);
            }
            Debug.Log("Checking death, alive: " + playerAliveList[myPlayerNum - 1]);
        }
        yield return null;
    }
    IEnumerator NothingInteresting() {
        yield return StartCoroutine(CreateMessageText("Enemy did nothing interesting", 200, 200, 1, false));
    }
    IEnumerator ClearActions(int mode, BotScript bScript) {
        if (mode == 1) {
            actions = new float[actionsLength];
            for (int i = 0; i < actionsLength; i++) {
                actions[i] = 0;
            }
            localDamages = new float[playerList.Count];
            for (int i = 0; i < localDamages.Length; i++) {
                localDamages[i] = 0;
            }
            Debug.Log("localDamages length = " + localDamages.Length);
            bulDirDeg.Clear();
            trapIndexes.Clear();
            smokeIndexes.Clear();
        } else if (mode == 2) {
            bScript.myActions = new float[actionsLength];
            for (int i = 0; i < actionsLength; i++) {
                bScript.myActions[i] = 0;
            }
            bScript.myLocalDamages = new float[playerList.Count];
            for (int i = 0; i < bScript.myLocalDamages.Length; i++) {
                bScript.myLocalDamages[i] = 0;
            }
            bScript.myBDD.Clear();
            bScript.myTI.Clear();
            bScript.mySI.Clear();
        }
        yield return null;
    }
    IEnumerator ChangeEffectLengths(int mode, List<int> fx, List<int> fxs, List<int> fxl, BotScript BS) {
        if (mode == 1) {
            effectsWornOff.Clear();
            prevMyEffects = new List<int>(myEffects);
            prevMyEffectStrengths = new List<int>(myEffectStrengths);
            prevPlayerPositionList = new List<int>(playerPositionList);
        }
        for (int i = 0; i < fxl.Count; i++) {
            fxl[i]--;
        }
        for (int i = fxl.Count - 1; i >= 0; i--) {
            if (fxl[i] <= 0) {
                if (mode == 1) {
                    if (fx[i] == 2) {
                        effectsWornOff.Add(2);
                    }
                    if (fx[i] == 6) {
                        effectsWornOff.Add(6);
                    }
                }
                fxl.RemoveAt(i);
                fx.RemoveAt(i);
                fxs.RemoveAt(i);
            }
        }
        if (mode == 1) {
            yield return StartCoroutine(VitalsStatus(myPlayerNum, myEffects, 1, 1, enemyHealthsVisible, null));

            if (effectsWornOff.Count > 0) {
                replayable6 = true;
            }
        } else if (mode == 2) {
            yield return StartCoroutine(VitalsStatus(BS.playerNum, BS.botEffects, 1, 2, BS.playerHealthsVisible, BS));
        }
        yield return null;
    }
    int ESRAWConfirm = 0;
    IEnumerator ESRAndWait(int pnum, int[] e, int pos, int mode, int transmitter) {
        ESRAWConfirm = 0;
        EffectsServerRpc(pnum, e, pos, mode, transmitter);
        Debug.Log("Before ESRAW waiting");
        while (ESRAWConfirm < playerList.Count - 1) {
            yield return null;
        }
        Debug.Log("ESRAW done waiting");
    }
    [ServerRpc(RequireOwnership = false)]
    void EffectsServerRpc(int pnum, int[] e, int pos, int mode, int transmitter) {
        EffectsClientRpc(pnum, e, pos, mode, transmitter);
        for (int i = 0; i < botList.Count; i++) {
            if (botList[i]) {
                ConfirmESRClientRpc(pnum, mode, transmitter);
            }
        }
        for (int i = 0; i < botList.Count; i++) {
            if (botList[i]) {
                StartCoroutine(playerList[i].GetComponent<BotScript>().BotVisionHouse(false, 0));
            }
        }
    }
    [ClientRpc]
    void EffectsClientRpc(int n, int[] e, int pos, int mode, int transmitter) {
        if (n != myPlayerNum) {
            //Set before list here
            prevEnemyEffects2 = CloneLLI(enemyEffects);
            Debug.Log("prevEnemyEffects2 set");
            prevEnemyEffected = n;
            enemyEffects[n - 1] = e.ToList();
            bool same = (prevEnemyEffects2[n - 1].Contains(1) == enemyEffects[n - 1].Contains(1));
            if (!same) {
                bool enemyInVision;
                if (visionGrid[pos - 1] == 0) {
                    enemyInVision = false;
                } else {
                    enemyInVision = true;
                }
                if (enemyInVision) {
                    if (prevEnemyEffects2[n - 1].Contains(1) && !enemyEffects[n - 1].Contains(1)) { //temporary solution
                        replayable4 = true;
                    }
                    Debug.Log("replayable4 is true");
                }
            }
            
            StartCoroutine(PlayerObjectEffects(n, enemyEffects[n - 1], smokes));
            NTCCEConfirm = true;
            ConfirmESRServerRpc(n, mode, transmitter);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ConfirmESRServerRpc(int n, int mode, int transmitter) {
        ConfirmESRClientRpc(n, mode, transmitter);
    }
    [ClientRpc]
    void ConfirmESRClientRpc(int n, int mode, int transmitter) {
        if (mode == 1) {
            if (n == myPlayerNum) {
                ESRAWConfirm++;
            }
        } else if (mode == 2) {
            if (transmitter == myPlayerNum) {
                ESRAWConfirm++;
            }
        }
    }
    bool CELConfirm = false;
    [ServerRpc(RequireOwnership = false)]
    void CELServerRpc() {
        CELClientRpc();
    }
    [ClientRpc]
    void CELClientRpc() {
        CELConfirm = true;
    }
    int FindMaxEffectLength(int effect, List<int> e, List<int> el) {
        int returnVal = -1;
        for (int i = 0; i < e.Count; i++) {
            if (e[i] == effect) {
                int length = el[i];
                if (length > returnVal) {
                    returnVal = length;
                }
            }
        }
        return returnVal;
    }
    [ServerRpc(RequireOwnership = false)]
    public void HealthsServerRpc(int pnum, int hp, int maxHp, bool initialization, bool changed, int mode) {
        /* mode:
        0 = initialization
        1 = beginning of turn
        2 = end of turn
        3 = end of moving
        */
        /*Debug.Log("Max effect length: " + FindMaxEffectLength(6, myEffects, myEffectLengths));
        if (mode == 2 && FindMaxEffectLength(6, myEffects, myEffectLengths) == 1) { //about to wear out
            Debug.Log("About to wear out!");
        } else {
            
        }*/
        HealthsClientRpc(pnum, hp, maxHp, initialization, changed, mode);
        for (int i = 0; i < botList.Count; i++) {
            if (botList[i]) {
                StartCoroutine(playerList[i].GetComponent<BotScript>().UpdateKnownPlayerHealths());
            }
        }
    }
    [ClientRpc]
    void HealthsClientRpc(int pnum, int hp, int maxHp, bool initialization, bool changed, int mode) {
        HealthsHouse(pnum, hp, maxHp, changed, mode);
        if (initialization) {
            healthInitialConfirm++;
        }
    }
    void HealthsHouse(int pnum, int hp, int maxHp, bool changed, int mode) {
        bool show = !(mode == 2 && FindMaxEffectLength(6, myEffects, myEffectLengths) == 1);
        if (FindMaxEffectLength(6, myEffects, myEffectLengths) <= 0) {
            show = false;
        }
        if (show) {
            if (changed) {
                //set replayable2 to true, create backup lists
                if (pnum != myPlayerNum) {
                    replayable2 = true;
                    prevHealths[pnum - 1] = healths[pnum - 1];
                    prevMaxHealths[pnum - 1] = maxHealths[pnum - 1];
                    playersDeltaHealth.Add(pnum);
                }
            }
        }
        healths[pnum - 1] = hp;
        maxHealths[pnum - 1] = maxHp;
        if (show) {
            if (pnum != myPlayerNum) {
                //Debug.Log("Healths Client RPC");
                Debug.Log("healths length: " + healths.Count);
                Debug.Log("maxHealths length: " + maxHealths.Count);
                Debug.Log("mode: " + mode);
                Debug.Log("MaxEffectLength: " + FindMaxEffectLength(6, myEffects, myEffectLengths));
                StartCoroutine(UpdateEnemyHealthText(pnum - 1, healths[pnum - 1], maxHealths[pnum - 1]));
            }
        }
    }
    IEnumerator PlayerObjectEffects(int num, List<int> e, List<float> s) {
        if (num == myPlayerNum) {
            SpriteRenderer mySpriteRenderer = myPlayerObject.GetComponent<SpriteRenderer>();
            Color color = mySpriteRenderer.color;
            if (e.Contains(1))
            {
                color.a = 0.2f; //\L
            }
            else
            {
                isSmoked = SmokeList(s);
                if (isSmoked[myPosition - 1]) {
                    color.a = 0.25f;
                } else {
                    color.a = 1;
                }
            }
            mySpriteRenderer.color = color;
        } else {
            if (!amSpectator) {
                yield return StartCoroutine(EnemyVisible(visionGrid, playerList[num - 1], playerPositionList[num - 1], e.Contains(1)));
            }
        }
    } 
    int[] ChangeSmokeLifes(int mode, BotScript BS, int pnum) {
        List<float> tempSmokes = new List<float>(smokes);
        List<int> killed = new List<int>();
        for (int i = 0; i < smokeOwners.Count; i++) {
            if (smokeOwners[i] == pnum) {
                int SLS = smokeLifespans[i];
                SLSServerRpc(i, smokeLifespans[i] - 1); //might have to wait
                if (SLS - 1 <= 0) {
                    SLSServerRpc(i, 0);
                    SOServerRpc(i, 0);
                    /*smokeOwners[i] = 0;
                    smokeLifespans[i] = 0;*/
                    if (mode == 1) {
                        smokes[i] = -1;
                    } else if (mode == 2) {
                        BS.botSmokes[i] = -1;
                    }
                    killed.Add(i);
                }
            }
        }
        if (mode == 1) {
            if (killed.Count > 0) {
                prevSmokes2 = tempSmokes;
                //Debug.Log("prevSmokes2 is set");
                replayable6 = true;
                R6Smoke = true;
                StartCoroutine(UpdateIsSmoked(smokes));
            }
        } else if (mode == 2) {
            if (killed.Count > 0) {
                StartCoroutine(BS.UpdateIsSmoked(BS.botSmokes));
            }
        }
        return killed.ToArray();
    }
    [ServerRpc(RequireOwnership = false)]
    void SmokesServerRpc(int pnum, int[] s) {
        SmokesClientRpc(pnum, s);
        for (int i = 0; i < botList.Count; i++) {
            if (botList[i]) {
                BotScript botScript = playerList[i].GetComponent<BotScript>();
                if (pnum != botScript.playerNum) {
                    for (int j = 0; j < s.Length; j++) {
                        botScript.botSmokes[s[j]] = -1;
                    }
                    //yield return StartCoroutine(botScript.UpdateIsSmoked(botScript.botSmokes));
                    botScript.botIsSmoked = SmokeList(botScript.botSmokes); //must be the same as the coroutine
                    StartCoroutine(botScript.BotVisionHouse(false, 0));
                }
            }
        }
    }
    [ClientRpc]
    void SmokesClientRpc(int pnum, int[] s) {
        if (pnum != myPlayerNum) {
            //Debug.Log("Smokes client rpc, s length: " + s.Length);
            if (s.Length > 0) {
                prevSmokes2 = new List<float>(smokes);
                //Debug.Log("prevSmokes2 is set");
                replayable6 = true;
                R6Smoke = true;
            }
            for (int i = 0; i < s.Length; i++) {
                smokes[s[i]] = -1;
                Debug.Log("pos " + (s[i] + 1) + " is now -1");
            }
            StartCoroutine(UISPOEVH());
        }
    }
    IEnumerator UISPOEVH() {
        yield return StartCoroutine(UpdateIsSmoked(smokes));
        if (!amSpectator) {
            StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
            StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
        }
    }
    IEnumerator VisionHouse(float addVision, List<float> s, List<List<int>> EE, int specialEnemy, int SEPos, List<int> PPL, List<int> fx, List<int> fxs) {
        if (!amSpectator) {
            yield return StartCoroutine(Vision(addVision, s, fx, fxs)); 
            for (int i = 0; i < playerList.Count; i++)
            {
                if (i + 1 != myPlayerNum)
                {
                    int pos = PPL[i];
                    if (specialEnemy != 0) {
                        pos = SEPos;
                        //should yield return EnemyVisible but whatever, I haven't seen any bugs yet
                    }
                    StartCoroutine(EnemyVisible(visionGrid, playerList[i], pos, EE[i].Contains(1)));
                }
            }
            for (int i = 0; i < visionGrid.Count; i++) {
                if (visionGrid[i] != 0) {
                    if (mysteryBuildings.Contains(i)) {
                        tileObjects[i].GetComponent<SpriteRenderer>().sprite = terrain[GRID[i]];
                        mysteryBuildings.Remove(i);
                    }
                }
            }
        }
    }
    IEnumerator ChangeCooldowns(List<int> IOC, int mode, BotScript BS) {
        for (int i = 0; i < IOC.Count; i++) {
            if (IOC[i] > 0) {
                bool change = false;
                if (mode == 1) {
                    if (CDJustAdded[i]) {
                        CDJustAdded[i] = false;
                    } else {
                        change = true;
                    }
                } else if (mode == 2) {
                    if (BS.botCDJA[i]) {
                        BS.botCDJA[i] = false;
                    } else {
                        change = true;
                    }
                }
                if (change) {
                    IOC[i] -= 1;
                    if (IOC[i] < 0) {
                        IOC[i] = 0;
                    }
                }
            } 
        }
        if (mode == 1) {
            if (inventoryOpen) {
                foreach (Transform child in inventoryItemsTF) {
                    GameObject.Destroy(child.gameObject);
                }
                StartCoroutine(OpenInventory());
            }
        }
        yield return null;
    }
    [ServerRpc(RequireOwnership = false)]
    void R5ServerRpc(int n) {
        R5ClientRpc(n);
        foreach (bool bot in botList) {
            if (bot) {
                ConfirmR5ClientRpc(n);
            }
        }
    }
    [ClientRpc]
    void R5ClientRpc(int n) {
        if (replayable5) {
            replayable5 = false;
        }
        ConfirmR5ServerRpc(n);
    }
    [ServerRpc(RequireOwnership = false)]
    void ConfirmR5ServerRpc(int n) {
        ConfirmR5ClientRpc(n);
    }
    [ClientRpc]
    void ConfirmR5ClientRpc(int n) {
        if (n == myPlayerNum) {
            R5Confirm++;
        }
    }
    int R5Confirm = 0;
    IEnumerator R5AndWait() {
        R5Confirm = 0;
        R5ServerRpc(myPlayerNum);
        while (R5Confirm < playerList.Count) {
            yield return null;
        }
        yield return null;
    }
    int GNTConfirm = 0;
    [ServerRpc(RequireOwnership = false)]
    void GNTServerRpc(int newTurn) {
        GNTClientRpc(newTurn);
    }
    [ClientRpc]
    void GNTClientRpc(int newTurn) {
        if (newTurn == myPlayerNum) {
            GNTConfirm++;
        }
    }
    bool NTCCEConfirm = false;
    IEnumerator NewTurn(int prevPlayer, int newTurn, float[] a, float[] BDD, float[] LDs, int[] TI, int[] SI, int hp, int maxHp, bool skipped, int turns) {
        UpdateTurnsText(turns);
        if (newTurn == myPlayerNum) {
            //if (skippedTurns[myPlayerNum - 1] == 0) {
                if (a[0] != myPlayerNum && a != null) {
                    EAMemory = (float[]) a.Clone();
                    EBMemory = (float[]) BDD.Clone();
                    TIMemory = (int[]) TI.Clone();
                    SIMemory = (int[]) SI.Clone();
                }
            //}
        } else {
            //if (skippedTurns[newTurn - 1] == 0) {
                if (a[0] != myPlayerNum && a != null) {
                    EAMemory = (float[]) a.Clone();
                    EBMemory = (float[]) BDD.Clone();
                    TIMemory = (int[]) TI.Clone();
                    SIMemory = (int[]) SI.Clone();
                }
            //}
        }
        if (replayable3 != 0) {
            if (replayable3 != 3 && replayable3 != 4) {
                replayable3 = 0;
                Debug.Log("replayable3 is false");
            }
        }
        if (prevPlayer != 0) {
            HealthsHouse(prevPlayer, hp, maxHp, false, 2);
        }
        if (newTurn == myPlayerNum) {
            //Debug.Log("Before R5");
            yield return R5AndWait();
            //Debug.Log("After R5");
            //AirdropServerRpc();
            int adnum = airdrops.Count;
            if (a[28] != 0) {
                adnum = Mathf.RoundToInt(a[30]);
            }
            yield return StartCoroutine(AirdropAndWait(myPlayerNum, adnum, 1, myPlayerNum)); //don't forget airdrop for bot
        }
        
        yield return StartCoroutine(GeneralNewTurn(a, BDD, LDs, TI, SI, skipped));
        //prevSmokes is smokes
        GNTServerRpc(newTurn);
        
        if (IsServer) {
            if (botList[newTurn - 1]) {
                yield return R5AndWait();
                int adnum = playerList[newTurn - 1].GetComponent<BotScript>().botAirdrops.Count;
                if (Mathf.RoundToInt(a[28]) != 0) { //changed from unrounded a[28], should be no problem
                    adnum = Mathf.RoundToInt(a[30]);
                }
                yield return StartCoroutine(AirdropAndWait(newTurn, adnum, 2, myPlayerNum));
                //Debug.Log("After bot R5");
            }
            for (int i = 0; i < botList.Count; i++) {
                if (botList[i]) {
                    if (!skipped) {
                        BotScript botScript = playerList[i].GetComponent<BotScript>();
                        if (Mathf.RoundToInt(a[0]) != botScript.playerNum) {
                            if (Mathf.RoundToInt(a[28]) != 0) {//joe
                                int index = FindInIntList(botScript.botAirdrops, Mathf.RoundToInt(a[28]));
                                botScript.botAirdrops.RemoveAt(index);
                            }
                        }
                        yield return StartCoroutine(botScript.BotVisionHouse(false, 0)); //new
                        if (Mathf.RoundToInt(a[28]) == 0) {
                            if (a[0] != botScript.playerNum) {
                                if (a[0] != 0) {
                                    //Debug.Log("LS bot TakeDamage");
                                    yield return StartCoroutine(botScript.TakeDamage(a, BDD, LDs, TI, SI, columns.Value, rows.Value));
                                }
                            }
                            yield return StartCoroutine(CheckHealth(2, botScript));
                        } else {
                            botScript.EnemyOpenedAirdrop(Mathf.RoundToInt(a[0]), Mathf.RoundToInt(a[28]));
                        }
                        if (Mathf.RoundToInt(a[29]) != botScript.playerNum && Mathf.RoundToInt(a[29]) != 0) { 
                            if (Mathf.RoundToInt(a[0]) == 0) {
                                botScript.AdjustPlayerMode(Mathf.RoundToInt(a[29]), 1, 0.1f);
                            }
                        }
                        //maybe do a BotVisionHouse here
                    }
                    GNTServerRpc(newTurn);
                    Debug.Log("Bot GNT");
                }
            }
            if (botList[newTurn - 1]) {
                BotScript botScript = playerList[newTurn - 1].GetComponent<BotScript>();
                yield return StartCoroutine(ChangeCooldowns(botScript.botItemsOnCooldown, 2, botScript));
                //Debug.Log("Bot ESRAW");
                yield return StartCoroutine(ChangeEffectLengths(2, botScript.botEffects, botScript.botEffectStrengths, botScript.botEffectLengths, botScript));
                yield return StartCoroutine(ESRAndWait(botScript.playerNum, botScript.botEffects.ToArray(), botScript.position, 2, myPlayerNum)); //new int[] is a placeholder for botEffects
                
                //Debug.Log("Bot ESRAW done");
                int[] smokesKilled = ChangeSmokeLifes(2, botScript, botScript.playerNum);
                if (smokesKilled.Length > 0) {
                    Debug.Log("smokes killed length: " + smokesKilled.Length);
                    SmokesServerRpc(botScript.playerNum, smokesKilled);
                }
                CELServerRpc();
                yield return StartCoroutine(botScript.BotVisionHouse(false, 0));
            }
            /*for (int i = 0; i < botList.Count; i++) {
                if (botList[i]) {
                    BotScript botScript = playerList[i].GetComponent<BotScript>();
                    yield return StartCoroutine(ShowUAV(botScript.botEffects, a, 2, botScript.playerNum, botScript));
                    Debug.Log("Bot UAV");
                }
            }
            if (botList[newTurn - 1]) {
                BotScript botScript = playerList[newTurn - 1].GetComponent<BotScript>();
                if (skippedTurns[newTurn - 1] > 0) {
                    yield return StartCoroutine(UpdateSkippedTurnsAndWait(newTurn, -1));
                    if (botScript.myFrozenTurns > 0) {
                        botScript.myFrozenTurns -= 1;
                        if (botScript.myFrozenTurns < 0) {
                            botScript.myFrozenTurns = 0;
                        }
                        if (botScript.myFrozenTurns == 0) {
                            FreezeServerRpc(botScript.playerNum, 2);
                        }
                    }
                    //Debug.Log("USTAW2");
                    TurnDoneServerRpc(newTurn, a, BDD, TI, SI, botScript.HP, botScript.maxHP, true); //passes in its own health
                } else {
                    StartCoroutine(NewBotTurn(newTurn, a, BDD));
                }
            }*/
        }
        if (newTurn == myPlayerNum) {
            //Debug.Log("GNTInit = " + GNTConfirm);
            while (GNTConfirm < playerList.Count) {
                yield return null;
            }
            //Debug.Log("Done waiting for GNT");
            GNTConfirm = 0;
        }
        if (newTurn == myPlayerNum) {
            
            yield return StartCoroutine(ChangeCooldowns(itemsOnCooldown, 1, null));
            if (cooldownsOpen) {
                foreach (Transform child in cooldownItemsTF) {
                    GameObject.Destroy(child.gameObject);
                }
                StartCoroutine(OpenCooldowns());
            }
            //check skippedTurns
            yield return StartCoroutine(ChangeEffectLengths(1, myEffects, myEffectStrengths, myEffectLengths, null));
            yield return StartCoroutine(ESRAndWait(myPlayerNum, myEffects.ToArray(), myPosition, 1, myPlayerNum));
            
            Debug.Log("My turn");
            int[] smokesKilled = ChangeSmokeLifes(1, null, myPlayerNum);
            if (smokesKilled.Length > 0) {
                SmokesServerRpc(myPlayerNum, smokesKilled);
            }
            CELServerRpc();
            yield return StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
            StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
        }
        //Debug.Log("Before NTCCE");
        if (newTurn != myPlayerNum) {
            while (!NTCCEConfirm) {
                yield return null;
            }
        }
        NTCCEConfirm = false;
        //Debug.Log("After NTCCE");
        while (!CELConfirm) {
            yield return null;
        }
        CELConfirm = false;

        

        yield return StartCoroutine(ShowUAV(myEffects, a, 1, myPlayerNum, null));
        if (IsServer) { //very sus move, could be the cause of many problems
            for (int i = 0; i < botList.Count; i++) {
                if (botList[i]) {
                    BotScript botScript = playerList[i].GetComponent<BotScript>();
                    yield return StartCoroutine(ShowUAV(botScript.botEffects, a, 2, botScript.playerNum, botScript));
                    Debug.Log("Bot UAV");
                }
            }
            if (botList[newTurn - 1]) {
                BotScript botScript = playerList[newTurn - 1].GetComponent<BotScript>();
                if (skippedTurns[newTurn - 1] > 0) {
                    yield return StartCoroutine(UpdateSkippedTurnsAndWait(newTurn, -1));
                    if (botScript.myFrozenTurns > 0) {
                        botScript.myFrozenTurns -= 1;
                        if (botScript.myFrozenTurns < 0) {
                            botScript.myFrozenTurns = 0;
                        }
                        if (botScript.myFrozenTurns == 0) {
                            FreezeServerRpc(botScript.playerNum, 2);
                        }
                    }
                    //Debug.Log("USTAW2");
                    TurnDoneServerRpc(newTurn, a, BDD, LDs, TI, SI, botScript.HP, botScript.maxHP, true); //passes in its own health
                } else {
                    StartCoroutine(NewBotTurn(newTurn, a, BDD));
                }
            }
        }
        if (newTurn == myPlayerNum) {
            
            //*RESOLVED* should technically wait for EffectsServerRpc to finish, but hard to implement
            
            //if (skippedTurns[myPlayerNum - 1] > 0 || (myPlayerNum == 2 && TURNS.Value > 1)) {
            if (skippedTurns[myPlayerNum - 1] > 0) {
                yield return StartCoroutine(UpdateSkippedTurnsAndWait(myPlayerNum, -1));
                if (frozenTurns > 0) {
                    frozenTurns -= 1;
                    if (frozenTurns < 0) {
                        frozenTurns = 0;
                    }
                    if (frozenTurns == 0) {
                        FreezeServerRpc(myPlayerNum, 2);
                    }
                }
                
                //Debug.Log("USTAW2");
                TurnDoneServerRpc(myPlayerNum, a, BDD, LDs, TI, SI, HEALTH, maxHealth, true);
            } else {
                StartCoroutine(MyNewTurn());
            }
        }
    }
    IEnumerator NewBotTurn(int newTurn, float[] a, float[] BDD) {
        GameObject botObject = playerList[newTurn - 1];
        BotScript bScript = botObject.GetComponent<BotScript>();
        
        //yield return StartCoroutine(bScript.BotVisionHouse());
        if (playerAliveList[bScript.playerNum - 1]) {
            IMovedServerRpc(newTurn);
            yield return StartCoroutine(bScript.UpdateMode(2));
            Debug.Log("Mode before use plan = " + bScript.mode);
            bScript.CreateUsePlan();
            yield return StartCoroutine(bScript.MoveSomewhere(columns.Value, rows.Value, GRID, 7 + ChangeMoves(bScript.inventory) + ChangeMovesEffects(bScript.botEffects, bScript.botEffectStrengths) + FindHighestBookEffectAmt(bScript.botBookInventory, 1))); //placeholder for now
            //yield return new WaitForSeconds(1f);
            bScript.UpdateMyPositionInList();
            yield return StartCoroutine(SteppingOnTrap(3, 1, bScript.botTraps[bScript.position - 1], bScript.position, bScript));
            changeHealthConfirm = false;
            List<bool> prevPAL = ConvertNetworkBoolList(playerAliveList);
            yield return StartCoroutine(CheckHealth(2, bScript));
            if (changeHealthConfirm) {
                while (CountTrues(prevPAL) == CountTrues(ConvertNetworkBoolList(playerAliveList))) {
                    yield return null;
                }
            }
            //move
            UpdatePositionServerRpc(botObject.GetComponent<NetworkObject>(), bScript.position, true, bScript.playerNum, bScript.myVG.ToArray()); //pretty sure this causes the bot to change vision
            //yield return StartCoroutine(bScript.BotVisionHouse(false, 0));

            yield return StartCoroutine(bScript.UpdateTargetQuadrant());
            //yield return new WaitForSeconds(0.5f);
            if (playerAliveList[bScript.playerNum - 1]) {
                Debug.Log("Mode before search use = " + bScript.mode);
                yield return StartCoroutine(bScript.BotSearchUse(GRID, columns.Value, rows.Value));
            }
        }
        bScript.myActions[29] = bScript.playerNum;
        TurnDoneServerRpc(newTurn, bScript.myActions, bScript.myBDD.ToArray(), bScript.myLocalDamages, bScript.myTI.ToArray(), bScript.mySI.ToArray(), bScript.HP, bScript.maxHP, false);
        //Debug.Log("New bot turn");
        yield return null;
    }
    IEnumerator GeneralNewTurn(float[] enemyActions, float[] enemyBDD, float[] enemyLDs, int[] enemyTI, int[] enemySI, bool skipped) {
        if (!amSpectator) {
            StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
        }
        yield return StartCoroutine(ClearActions(1, null));
        if (IsServer) {
            for (int i = 0; i < playerList.Count; i++) {
                if (botList[i]) {
                    GameObject botObject = playerList[i];
                    BotScript bScript = botObject.GetComponent<BotScript>();
                    yield return StartCoroutine(ClearActions(2, bScript));
                }
            }
        }
        if (enemyActions[0] != myPlayerNum) {
            if (enemyActions != null) {
                if (enemyActions[1] != 0 || enemyActions[28] != 0) {
                    if (!skipped) {
                        yield return StartCoroutine(EnemyAction(enemyActions, enemyBDD, enemyLDs, enemyTI, enemySI, false, false, false, false, false, result => {}));
                    }
                }
            }
            
        }
        if (enemyActions[29] != myPlayerNum) {
            replayable = true;
            Debug.Log("replayable is true");
        }
        yield return null;
    }
    void ShowMovesText(float m) {
        UpdateMovesText(m);
        movesText.SetActive(true);
    }
    void UpdateMovesText(float m) {
        movesText.GetComponent<TextMeshProUGUI>().text = "Moves: " + m;
    }
    void HideMovesText() {
        movesText.SetActive(false);
    }
    void ShowTurnsText(int t) {
        UpdateTurnsText(t);
        turnsText.SetActive(true);
    }
    void UpdateTurnsText(int t) {
        turnsText.GetComponent<TextMeshProUGUI>().text = "Turns: " + t;
    }
    void HideTurnsText() {
        turnsText.SetActive(false);
    }
    IEnumerator ChangeTurnsIndicator(int turn) {
        float setGhost = 0.15f;
        float totalTime = 1.0f;
        float timer = 0f;
        if (turnIndicator.GetComponent<Image>().enabled) {
            totalTime = 1.0f;
            timer = 0f;
            while (timer < totalTime) {
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / totalTime);
                yield return StartCoroutine(SetGhostImage(turnIndicator, (1f - progress) * setGhost));
            }
            yield return StartCoroutine(SetGhostImage(turnIndicator, 0));
        } else {
            yield return StartCoroutine(SetGhostImage(turnIndicator, 0));
            turnIndicator.GetComponent<Image>().enabled = true;
        }
        turnIndicator.GetComponent<Image>().sprite = turnIndicatorCostumes[turn - 1];
        totalTime = 1.0f;
        timer = 0f;
        while (timer < totalTime) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalTime);
            yield return StartCoroutine(SetGhostImage(turnIndicator, progress * setGhost));
        }
        yield return StartCoroutine(SetGhostImage(turnIndicator, setGhost));
    }
    IEnumerator MyNewTurn()
    {
        ReturnToGrid = false;
        if (playerAliveList[myPlayerNum - 1] == true) {
            StartCoroutine(CreateMessageText("It's your turn", 100, 0, 1, false));
            StartCoroutine(ChangeTurnsIndicator(1));
            IMovedServerRpc(myPlayerNum);
            moves = 7;
            moves += ChangeMoves(INVENTORY);
            moves += ChangeMovesEffects(myEffects, myEffectStrengths);
            moves += FindHighestBookEffectAmt(bookInventory, 1); 
            //yield return StartCoroutine(ChangeMoves());
            ShowMovesText(moves);
            StartCoroutine(RTG());
            StartCoroutine(RTGCheck());
            while (moves > 0)
            {
                yield return StartCoroutine(Move());
            }
            while (moving) {
                yield return null;
            }
            //yield return new WaitForSeconds(0.2f);
            VCSpriterenderer.enabled = false;
            StartCoroutine(PlayerObjectEffects(myPlayerNum, myEffects, smokes));
            yield return StartCoroutine(VisionHouse(0, smokes, enemyEffects, 0, 0, playerPositionList, myEffects, myEffectStrengths));
            UpdatePositionServerRpc(myPlayerObject.GetComponent<NetworkObject>(), myPosition, true, myPlayerNum, visionGrid.ToArray());
            playerPositionList[myPlayerNum - 1] = myPosition;
            HideMovesText();
            yield return StartCoroutine(SteppingOnTrap(1, 1, traps[myPosition - 1], myPosition, null));
            Debug.Log("My health: " + HEALTH);
            changeHealthConfirm = false;
            List<bool> prevPAL = ConvertNetworkBoolList(playerAliveList);
            yield return StartCoroutine(CheckHealth(1, null));
            if (changeHealthConfirm) {
                while (CountTrues(prevPAL) == CountTrues(ConvertNetworkBoolList(playerAliveList))) {
                    yield return null;
                }
            }
            yield return StartCoroutine(CheckYouDied());
            if (playerAliveList[myPlayerNum - 1]) {
                useDisabled = false;
                searchused = false;
                StartCoroutine(SearchUse());
                while (!searchused)
                    yield return null;
            }
            //Debug.Log("searchused is " + searchused);
        }
        //Debug.Log("Turn done");
        actions[29] = myPlayerNum;
        TurnDoneServerRpc(myPlayerNum, actions, bulDirDeg.ToArray(), localDamages, trapIndexes.ToArray(), smokeIndexes.ToArray(), HEALTH, maxHealth, false);
    }
    List<bool> ConvertNetworkBoolList(NetworkList<bool> l) {
        List<bool> retList = new List<bool>();
        foreach (bool b in l) {
            retList.Add(b);
        }
        return retList;
    }
    bool PAL;
    /*bool GetPAL(int index) {

    }
    [ServerRpc(RequireOwnership = false)]
    bool GetPALServerRpc(int index) {
        return playerAliveList[index];
    }*/
    List<List<float>> CloneLLF(List<List<float>> og) {
        List<List<float>> retList = new List<List<float>>();
        foreach (List<float> l in og) {
            retList.Add(new List<float>(l));
        }
        return retList;
    }
    List<List<List<float>>> CloneLLLF(List<List<List<float>>> og) {
        List<List<List<float>>> retList = new List<List<List<float>>>();
        foreach (List<List<float>> l in og) {
            retList.Add(CloneLLF(l));
        }
        return retList;
    }
    List<List<int>> CloneLLI(List<List<int>> og) {
        List<List<int>> retList = new List<List<int>>();
        foreach (List<int> l in og) {
            retList.Add(new List<int>(l));
        }
        return retList;
    }
    IEnumerator SteppingOnTrap(int mode, int mode2, List<List<float>> listTraps, int pos, BotScript BS) {
        List<int> removedTraps = new List<int>(); //removedTraps is the same as triggeredTraps
        bool doneDamage = false;
        /*for (int i = 0; i < listTraps.Count; i++) {
            List<float> currentTrap = listTraps[i];
            if (currentTrap[0] != myPlayerNum) {
                if (currentTrap[1] == 1) {
                    doneDamage = true;
                }
            }
        }*/
        /*if (mode == 2) {
            if (doneDamage) {
                UpdateHealthText(Mathf.RoundToInt(pseudoHealth), maxHealth);
            }
        }*/
        
        if (mode == 1)
            healthCalculation = HEALTH;
        if (mode == 3)
            BS.HPCalculation = BS.HP;
        int pnum = myPlayerNum;
        if (mode == 3) {
            pnum = BS.playerNum;
        }
        for (int i = 0; i < listTraps.Count; i++) {
            List<float> currentTrap = listTraps[i];
            if (currentTrap[0] != pnum) {
                if (currentTrap[1] == 1) {
                    doneDamage = true;
                    if (mode == 1) {
                        healthCalculation -= armorCoefficient * currentTrap[2];
                    }
                    if (mode == 3)
                        BS.HPCalculation -= BS.armorCoefficient * currentTrap[2];
                    if (mode != 3) {
                        StartCoroutine(ExplodeTrap(pos, currentTrap));
                        StartCoroutine(ShowPain(new Vector2(GRIDX[pos - 1], GRIDY[pos - 1])));
                    }
                    removedTraps.Add(i);
                }
                Debug.Log("Stepping on trap!");
            }
        }
        Debug.Log("removedTraps#: " + removedTraps.Count);
        if (doneDamage) {
            if (mode == 1) {
                //prevHealth = HEALTH
                pseudoHealth = HEALTH;
                HEALTH = Mathf.RoundToInt(healthCalculation);
                UpdateHealthText(HEALTH, maxHealth);
                if (replayable3 == 0) {
                    if (mode2 == 1) {
                        replayable3 = 1;
                    } else if (mode2 == 2) {
                        replayable3 = 3;
                        Debug.Log("replayable3 is 3");
                    }
                    
                    Debug.Log("replayable3 is true");
                }
                prevListTraps = CloneLLF(listTraps);
                prevRemovedTraps = new List<int>(removedTraps);
                prevTrapPosition = pos;
                prevTrapPlayerNum = myPlayerNum;
            } else if (mode == 2) {
                UpdateHealthText(HEALTH, maxHealth);
            } else if (mode == 3) {
                BS.HP = Mathf.RoundToInt(BS.HPCalculation);
            }
        }
        
        if (removedTraps.Count > 0) {
            if (mode == 1) {
                for (int i = removedTraps.Count - 1; i >= 0; i--) {
                    traps[pos - 1].RemoveAt(removedTraps[i]);
                }
            } else if (mode == 3) {
                for (int i = removedTraps.Count - 1; i >= 0; i--) {
                    BS.botTraps[pos - 1].RemoveAt(removedTraps[i]);
                }
            }
            if (mode != 3) {
                if (removedTraps.Count == 1) {
                    StartCoroutine(CreateMessageText("You stepped on a trap!", 300, 200, 1, false));
                } else {
                    StartCoroutine(CreateMessageText("You stepped on " + removedTraps.Count + " traps!", 300, 200, 1, false));
                }
                yield return StartCoroutine(RenderTraps(traps));
            }
            if (mode == 1) {
                HealthsServerRpc(myPlayerNum, HEALTH, maxHealth, false, true, 3);
                TrapServerRpc(myPlayerNum, pos, removedTraps.ToArray(), mode2);
            } else if (mode == 3) {
                HealthsServerRpc(BS.playerNum, BS.HP, BS.maxHP, false, true, 3);
                TrapServerRpc(BS.playerNum, pos, removedTraps.ToArray(), mode2);
            }
        }
        yield return null;
    }
    IEnumerator ExplodeTrap(int pos, List<float> currentTrap) {
        //blow up
        float FBR = currentTrap[5];
        int[] binaryList = BombBinary(pos, FBR);
        bool overlap = false;
        for (int j = 0; j < visionGrid.Count; j++) {
            if (visionGrid[j] != 0 && binaryList[j] == 1) {
                overlap = true;
            }
        }
        StartCoroutine(Explosion(pos, FBR, overlap, 1));
        yield return null;
    }
    [ServerRpc(RequireOwnership = false)]
    void TrapServerRpc(int pnum, int pos, int[] removedTraps, int mode2) {
        TrapClientRpc(1, mode2, pnum, pos, removedTraps);
        for (int i = 0; i < botList.Count; i++) {
            if (botList[i]) {
                BotScript bScript = playerList[i].GetComponent<BotScript>();
                if (removedTraps.Length > 0) {
                    for (int j = removedTraps.Length - 1; j >= 0; j--) {
                        bScript.botTraps[pos - 1].RemoveAt(removedTraps[j]);
                    }
                }
            }
        }
    }
    [ClientRpc]
    void TrapClientRpc(int mode, int mode2, int pnum, int pos, int[] removedTraps) {
        if (pnum != myPlayerNum) {
            List<List<float>> listTraps = CloneLLF(traps[pos - 1]);
            StartCoroutine(TrapClient(mode, mode2, pnum, pos, removedTraps, listTraps));
            if (removedTraps.Length > 0) {
                if (mode == 1) {
                    for (int i = removedTraps.Length - 1; i >= 0; i--) {
                        traps[pos - 1].RemoveAt(removedTraps[i]);
                    }
                }
                StartCoroutine(RenderTraps(traps));
            }
            //remove and rerender
        }
    }
    IEnumerator TrapClient(int mode, int mode2, int pnum, int pos, int[] removedTraps, List<List<float>> listTraps) {
        if (mode == 1) {
            if (replayable3 == 0) {
                if (mode2 == 1) {
                    replayable3 = 2;
                } else if (mode2 == 2) {
                    replayable3 = 4;
                    Debug.Log("replayable3 is 4");
                }
                
                //Debug.Log("replayable3 is true");
            }
            prevListTraps = CloneLLF(listTraps);
            prevRemovedTraps = new List<int>(removedTraps.ToList());
            prevTrapPosition = pos;
            prevTrapPlayerNum = pnum;
        }
        int countMyTraps = 0;
        for (int i = 0; i < removedTraps.Length; i++) {
            List<float> currentTrap = listTraps[removedTraps[i]];
            if (currentTrap[0] == myPlayerNum) {
                countMyTraps++;
            }
            StartCoroutine(ExplodeTrap(pos, currentTrap));
        }
        if (countMyTraps == 1) {
            StartCoroutine(CreateMessageText("The enemy stepped on one of your traps!", 300, 200, 1, false));
        } else if (countMyTraps > 1) {
            StartCoroutine(CreateMessageText("The enemy stepped on " + countMyTraps + " of your traps!", 300, 200, 1, false));
        } else if (countMyTraps == 0) {
            if (removedTraps.Length == 1) {
                StartCoroutine(CreateMessageText("The enemy stepped on someone else's trap!", 300, 200, 1, false));
            } else if (removedTraps.Length > 1) {
                StartCoroutine(CreateMessageText("The enemy stepped on " + removedTraps.Length + " traps belonging to others!", 300, 200, 1, false));
            }
        }
        yield return null;
    }
    float ChangeMoves(List<int> inv) {
        float returnVal = 0;
        for (int i = 0; i < inv.Count; i++) {
            int itemIndex = inv[i] - 1;
            returnVal += itemFloats[itemIndex][0];
        }
        return returnVal;
    }
    float ChangeMovesEffects(List<int> e, List<int> eS) {
        float returnVal = 0;
        for (int i = 0; i < e.Count; i++) {
            if (e[i] == 7) {
                returnVal += eS[i];
            }
        }
        return returnVal;
    }
    public float FindHighestBookEffectAmt(List<int> bookInv, int bookItemClass) {
        int highestLevel = 0;
        int trackedItem = 0;
        foreach (int item in bookInv) {
            int index = item - 1;
            if (bookItemInts[index][1] == bookItemClass) {
                if (bookItemInts[index][2] > highestLevel) {
                    highestLevel = bookItemInts[index][2];
                    trackedItem = item;
                }
            }
        }
        if (trackedItem == 0) {
            return 0; //if none is in bookInv
        } else {
            return bookItemFloats[trackedItem - 1][0];
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void IMovedServerRpc(int num) {
        EnemyMovedClientRpc(num);
    }
    [ClientRpc] 
    public void EnemyMovedClientRpc(int num) {
        if (num != myPlayerNum) {
            if (playerPositionList.Count > 2) {
                StartCoroutine(CreateMessageText("It's enemy " + num + "'s turn", 100, 0, 1, false));
            } else {
                StartCoroutine(CreateMessageText("It's the enemy's turn", 100, 0, 1, false));
            }
            StartCoroutine(ChangeTurnsIndicator(2));
        }
    }
    IEnumerator EnemyVisible(List<int> VG, GameObject enemyObject, int enemyPosition, bool invis)
    {
        SpriteRenderer ESpriteRenderer = enemyObject.GetComponent<SpriteRenderer>();
        Color color = ESpriteRenderer.color;
        if (DecideEnemyVisible(VG, enemyPosition, invis)) {
            color.a = 1;
        } else {
            if (amSpectator) {
                if (VG[enemyPosition - 1] != 0)
                {
                    color.a = 0.2f; //\L
                } else {
                    color.a = 0;
                }
            } else {
                color.a = 0;
            }
        }
        ESpriteRenderer.color = color;
        yield return null;
    }
    public bool DecideEnemyVisible(List<int> VG, int enemyPosition, bool invis) {
        if (VG[enemyPosition - 1] == 0)
        {
            return false;
        }
        else
        {
            if (invis)
                return false;
            else
                return true;
        }
    }
    void ShowVisionGreys(List<int> VG) {
        for (int k = 0; k < VG.Count; k++)
        {
            SpriteRenderer VSpriteRenderer = visionObjects[k].GetComponent<SpriteRenderer>();
            Color color = VSpriteRenderer.color;
            if (VG[k] == 0)
            {
                color.a = 0.8f;
                visionObjects[k].GetComponent<SpriteMask>().enabled = true;
            }
            else
            {
                color.a = 0;
                visionObjects[k].GetComponent<SpriteMask>().enabled = false;
            }
            VSpriteRenderer.color = color;
        }
    }
    IEnumerator Vision(float addVision, List<float> s, List<int> fx, List<int> fxs)
    {  
        visionGrid = GetVisionList(addVision, s, 1, null, fx, fxs, INVENTORY, myPosition);
        /*yield return StartCoroutine(GetVisionList(addVision, s, 1, null, fx, fxs, result =>{ 
            visionGrid = result;
        }));*/
        ShowVisionGreys(visionGrid);
        yield return null;
        //yield return new WaitForSeconds(5f);
        /*for (int ij = 0; ij < testList.Count; ij++)
        {
            Destroy(testList[ij]);
        }
        testList = new List<GameObject>();*/
    }
    float AddArmorVisions(List<int> inv) {
        float v = 0;
        foreach (int item in inv) {
            int index = item - 1;
            if (itemIntLists[index][0].Contains(4)) {
                v += itemInfos[index][3][2];
            }
        }
        return v;
    }
    int ScanInvXray(List<int> inv) {
        foreach (int item in inv) {
            int index = item - 1;
            if (itemIntLists[index][0].Contains(4)) {
                if (Mathf.RoundToInt(itemInfos[index][3][3]) == 1) {
                    return 1;
                }
            }
        }
        return 0;
    }
    //public IEnumerator GetVisionList(float addVision, List<float> s, int mode, BotScript BS, List<int> fx, List<int> fxs, System.Action<List<int>> callback)
    public List<int> GetVisionList(float addVision, List<float> s, int mode, BotScript BS, List<int> fx, List<int> fxs, List<int> inv, int pos) 
    {
        float v = 0;
        v = FindVision(GRID[pos - 1], inv, fx, fxs);
        v += addVision;
        if (mode == 1) {
            VISION = v;
        }
        int xray = 0;
        xray = ScanInvXray(inv);
        //Debug.Log("v: " + v);
        List<int> retList = new List<int>();
        //myPosition is 1+
        Vector2 myPositionVector2 = GetCoordsFromIndex(pos);
        for (int i = 0; i < GRID.Count; i++)
        {
            Vector2 specificVector2 = GetCoordsFromIndex(i + 1);
            float visionDistance = FindDistance(myPositionVector2, specificVector2);
            if (visionDistance <= v)
            {
                retList.Add(1);
                //Debug.Log("1 added");
            }
            else
            {
                retList.Add(0);
            }
        }
        Vector2 myPositionInWorldSpace = new Vector2(GRIDX[pos - 1], GRIDY[pos - 1]);
        //Debug.Log(visionGrid.Count);
        isSmoked = SmokeList(s);
        //ClearTestFolder(); //if testing, of course
        for (int j = 0; j < retList.Count; j++)
        {
            if (retList[j] == 1)
            {
                Vector2 targetPositionInWorldSpace = new Vector2(GRIDX[j], GRIDY[j]);
                Vector2 direction = targetPositionInWorldSpace - myPositionInWorldSpace;
                float angleRadians = Mathf.Atan2(direction.y, direction.x);
                float distance = FindDistance(myPositionVector2, GetCoordsFromIndex(j + 1));
                float myElevation = FindElevation(GRID[pos - 1], 1);
                retList[j] = CheckVisionObstacles(myPositionInWorldSpace, angleRadians, distance, myElevation, j, 1, retList, pos - 1, xray);
                /*if (retList[j] == 0) {
                    Debug.Log("Turned to 0");
                }*/
                /*yield return StartCoroutine(CheckVisionObstacles(myPositionInWorldSpace, angleRadians, distance, myElevation, j, 1, retList, myP - 1, xray, result => {
                    retList[j] = result;
                }));*/
            }
        }
        //callback(retList);
        return retList;
    }
    //IEnumerator CheckVisionObstacles(Vector3 startPosition, float angleRadians, float distance, float myElevation, int j, int mode, List<int> VG, int myIndex, int xray, System.Action<int> callback)
    int CheckVisionObstacles(Vector3 startPosition, float angleRadians, float distance, float myElevation, int j, int mode, List<int> VG, int myIndex, int xray) //j, myIndex are 0+
    {
        //List<GameObject> testList = new List<GameObject>();
        int visualTest = 0;
        /*if (Keyboard.current.spaceKey.isPressed) {
            visualTest = 2;
        }*/
        GameObject testObject = null;
        if (visualTest == 1 || visualTest == 2) {
            testObject = Instantiate(testIcon, new Vector2(GRIDX[j], GRIDY[j]), Quaternion.identity, testFolder.transform);
            if (visualTest == 2) {
                //yield return new WaitForSeconds(0.1f);
            }
            if (visualTest != 2) {
                Debug.Log("New test object");
                //yield return new WaitForSeconds(1f);
            }
        }
        int replaceVal;
        if (mode == 1)
        {
            replaceVal = VG[j]; //basically replaceVal = 1
        }
        else
        {
            replaceVal = 1;
        }
        float treeCoefficient = 2;
        if (myElevation == 1)
        {
            treeCoefficient = 3.5f;
        }
        else if (myElevation == 2)
        {
            treeCoefficient = 5;
        }
        //treeCoefficient -= 0.1f; //justified for floating point error
        float hilltopCoefficient = 0.5f;
        //bool alrInSmoke = isSmoked[myPosition - 1];
        bool alrInSmoke = isSmoked[myIndex];
        //bool alrInSmoke = false;
        float smokeCoefficient = 0;
        if (alrInSmoke)
            smokeCoefficient = 1;
        float smoke = -1;
        int stopIndex = 0;
        float tree = 0;
        float hilltop = 0;
        int iterationIndex = 0; //iterationIndex is 1+
        float distanceStop = -1;
        float distanceTraveled = 0;
        bool stoppedXray = false;
        if (mode == 2) {
            Vector2 moveDirection = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            Vector2 positionIteration = startPosition;
            //GameObject visionTestClone = Instantiate(visionTest, positionIteration, Quaternion.identity);
            //testList.Add(visionTestClone);
            
            float distanceUnit = 0.01f; //tileWidth / 2;
            int distanceDivide = 100;
            if (mode == 1) distanceDivide = 100;
            
            //float repeats = distance * tileWidth / distanceUnit;
            if (mode == 2)
            {
                distance /= tileWidth;
            }
            float repeats = distance * tileWidth * distanceDivide; 
            //Debug.Log("repeats: " + repeats);
            
            iterationIndex = GetIndexFromWorldCoords(positionIteration);
            bool alreadyMet = false;
            //while (true)
            //for (int l = 1; l <= Math.Ceiling(repeats); l++)
            //while (repeats >= 1)
            /*if (mode == 1) {
            while (iterationIndex != 0)
            {
                positionIteration += moveDirection / distanceDivide;
                distanceTraveled += moveDirection.magnitude / distanceDivide;
                iterationIndex = GetIndexFromWorldCoords(positionIteration);
                if (iterationIndex == j + 1) {
                    if (!alreadyMet) alreadyMet = true;
                } else {
                    if (alreadyMet) {
                        if (tree >= tileWidth * treeCoefficient * distanceDivide || hilltop >= tileWidth * hilltopCoefficient * distanceDivide || smoke >= tileWidth * smokeCoefficient * distanceDivide)
                        {
                            if (mode == 1)
                            {
                                if (stopIndex == 0)
                                {
                                    stopIndex = iterationIndex;
                                    //break;
                                }
                            }
                            else if (mode == 2)
                            {
                                if (distanceStop == -1)
                                {
                                    distanceStop = distanceTraveled;
                                    stopPositionFOV = positionIteration;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                /*if (iterationIndex == 0)
                {
                    /*Debug.Log("glitch");
                    Debug.Log(j + 1);
                    Debug.Log(positionIteration.x);
                    Debug.Log(positionIteration.y);
                    Debug.Log(l);
                    Debug.Log(distance * tileWidth / distanceUnit);
                    //yield return null;
                    //positionIteration -= moveDirection * distanceUnit;
                    positionIteration -= moveDirection / distanceDivide; 
                    distanceTraveled -= moveDirection.magnitude / distanceDivide;
                    iterationIndex = GetIndexFromWorldCoords(positionIteration);
                    //visionTestClone.transform.position = positionIteration;
                    //break;
                    if (mode == 2)
                    {
                        if (distanceStop == -1)
                        {
                            distanceStop = distanceTraveled;
                            stopPositionFOV = positionIteration;
                            break;
                        }
                    }
                }
                if (iterationIndex != 0) {
                    if (GRID[iterationIndex - 1] == 5)
                    {
                        tree += 1;
                    }
                    //if (iterationIndex != myPosition && iterationIndex != 0)
                    if (iterationIndex != myPosition)
                    {
                        float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                        
                        if (localElevation > myElevation)
                        {
                            hilltop += 1;
                        }
                        //bool[] isSmoked = SmokeList(smokes);
                        if (isSmoked[iterationIndex - 1]) {
                            if (smoke < 0)
                                smoke = 0;
                            smoke += 1;
                        }
                    }
                    //if (tree >= tileWidth * treeCoefficient / distanceUnit || hilltop >= tileWidth * hilltopCoefficient / distanceUnit)
                    if (tree >= tileWidth * treeCoefficient * distanceDivide || hilltop >= tileWidth * hilltopCoefficient * distanceDivide || smoke >= tileWidth * smokeCoefficient * distanceDivide)
                    {
                        if (mode == 1)
                        {
                            if (stopIndex == 0)
                            {
                                stopIndex = iterationIndex;
                                //break;
                            }
                        }
                        else if (mode == 2)
                        {
                            if (distanceStop == -1)
                            {
                                distanceStop = distanceTraveled;
                                stopPositionFOV = positionIteration;
                                break;
                            }
                        }
                    }
                }
                
                //positionIteration -= moveDirection * distanceUnit;
                
                /*if (mode == 1)
                {
                    visionTestClone.transform.position = positionIteration;
                    //yield return null;
                }
                else
                    Destroy(visionTestClone);
                
                //repeats--;
            }
            } else {*/
            for (int l = 0; l < repeats; l++)
            {
                positionIteration += moveDirection / distanceDivide;
                distanceTraveled += moveDirection.magnitude / distanceDivide;
                iterationIndex = GetIndexFromWorldCoords(positionIteration);
                if (iterationIndex != 0) {
                    if (GRID[iterationIndex - 1] == 5)
                    {
                        tree += 1;
                    }
                    //if (iterationIndex != myPosition && iterationIndex != 0)
                    if (iterationIndex != myPosition)
                    {
                        float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                        
                        if (localElevation > myElevation)
                        {
                            hilltop += 1;
                        }
                        //bool[] isSmoked = SmokeList(smokes);
                        if (isSmoked[iterationIndex - 1]) {
                            if (smoke < 0)
                                smoke = 0;
                            smoke += 1;
                        }
                    }
                    bool t = (tree >= tileWidth * treeCoefficient * distanceDivide);
                    bool h = (hilltop >= tileWidth * hilltopCoefficient * distanceDivide);
                    bool s = (smoke >= tileWidth * smokeCoefficient * distanceDivide);
                    if (t || h || s)
                    {
                        /*if (mode == 1)
                        {
                            if (stopIndex == 0)
                            {
                                stopIndex = iterationIndex;
                                //break;
                            }
                        }
                        else if (mode == 2)
                        {*/
                        if (!s) {
                            if (xray == 0) {
                                if (distanceStop == -1)
                                {
                                    distanceStop = distanceTraveled;
                                    stopPositionFOV = positionIteration;
                                    break;
                                }
                            }
                        } else {
                            if (distanceStop == -1)
                            {
                                distanceStop = distanceTraveled;
                                stopPositionFOV = positionIteration;
                                break;
                            }
                        }
                        //}
                    }
                }
            }
            //}
            /*positionIteration += moveDirection * repeats / distanceDivide;
            distanceTraveled += moveDirection.magnitude * repeats / distanceDivide;
            iterationIndex = GetIndexFromWorldCoords(positionIteration);
            if (iterationIndex != 0) {
                if (iterationIndex != myPosition && iterationIndex != 0)
                {
                    float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                    if (GRID[iterationIndex - 1] == 5)
                    {
                        tree += 1 * repeats;
                    }
                    if (localElevation > myElevation)
                    {
                        hilltop += 1 * repeats;
                    }
                }
                if (tree >= tileWidth * treeCoefficient * distanceDivide || hilltop >= tileWidth * hilltopCoefficient * distanceDivide)
                {
                    if (mode == 1)
                    {
                        if (stopIndex == 0)
                        {
                            stopIndex = iterationIndex;
                        }
                    }
                    else if (mode == 2)
                    {
                        if (distanceStop == -1)
                        {
                            distanceStop = distanceTraveled;
                            stopPositionFOV = positionIteration;
                        }
                    }
                }
            }
            repeats = 0;*/
            /*if (mode == 1 && stopIndex == 0) {
                Debug.Log("distance: " + distance);
                Debug.Log("distanceTraveled: " + distanceTraveled);
            }*/
            /*if (mode == 1) {
                if (smokeCoefficient == 0 && !alrInSmoke) {
                    //if (stopIndex == 0) {
                        if (j + 1 > 0 && isSmoked[j]) {
                            stopIndex = j + 1;
                        }
                    //}
                }
            }*/
        } else if (mode == 1) {
            Vector2 APos = GetCoordsFromIndex(myIndex + 1);
            Vector2 BPos = GetCoordsFromIndex(j + 1);
            int AX = Mathf.RoundToInt(APos.x); //1+
            int AY = Mathf.RoundToInt(APos.y); //1+
            int BX = Mathf.RoundToInt(BPos.x);
            int BY = Mathf.RoundToInt(BPos.y);
            int searchMode = 1; //bottom left to top right
            if (AX == BX) {
                if (AY == BY) {
                    searchMode = 0; //don't search
                } else if (AY > BY) {
                    searchMode = 2; //top left to bottom right
                } else if (AY < BY) {
                    searchMode = 1;
                }
            } else if (AX > BX) {
                if (AY == BY) {
                    searchMode = 3; //bottom right to top left
                } else if (AY > BY) {
                    searchMode = 4; //top right to bottom left
                } else if (AY < BY) {
                    searchMode = 3;
                }
            } else if (AX < BX) {
                if (AY == BY) {
                    searchMode = 1;
                } else if (AY > BY) {
                    searchMode = 2;
                } else if (AY < BY) {
                    searchMode = 1;
                }
            }
            float r = 0;
            bool vertical = false;
            if (AX == BX) {
                vertical = true;
            }
            bool horizontal = false;
            if (AY == BY) {
                horizontal = true;
            }
            if (!vertical) {
                r = ((float) BY - AY) / ((float) BX - AX);
            }
            /*if (searchMode == 0) {
            } else if (searchMode == 1) { //bottom left to top right
                for (int yPos = AY; yPos <= BY; yPos++) { //searches 1+, 1+
                    for (int xPos = AX; xPos <= BX; xPos++) { //xPos, yPos are 1+
                        //check square
                        if (stopIndex == 0) {
                            iterationIndex = GetIndexFromCoords(new Vector2(xPos, yPos)); 
                            
                            bool vert = false;
                            if (xPos == AX) {
                                vert = true;
                            }
                            bool hori = false;
                            if (yPos == AY) {
                                hori = true;
                            }
                            if (LiesInsideSquare(r, xPos, yPos, AX, AY, searchMode, vertical, horizontal)) {
                                if (visualTest == 1) {
                                    testObject.transform.position = new Vector2(GRIDX[iterationIndex - 1], GRIDY[iterationIndex - 1]);
                                    //yield return new WaitForSeconds(0.3f);
                                }
                                float distInSquare = 0;
                                if (xPos == BX && yPos == BY) {
                                    //do weird stuff
                                    //r is the same from A to B as from B to A
                                    distInSquare = DistanceInSquare(r, xPos, yPos, BX, BY, AX, AY, vertical, horizontal); //technically deltaX and deltaY are -1 * what they're supposed to be but the distance is still the same
                                } else {
                                    distInSquare = DistanceInSquare(r, xPos, yPos, AX, AY, BX, BY, vertical, horizontal);
                                }
                                distanceTraveled += distInSquare;
                                if (!(xPos == AX && yPos == AY)) {
                                    if (GRID[iterationIndex - 1] == 5) {
                                        tree += distInSquare;
                                    }
                                    float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                                    if (localElevation > myElevation)
                                    {
                                        hilltop += distInSquare;
                                    }
                                    //smoke
                                    if (isSmoked[iterationIndex - 1]) {
                                        if (smoke < 0)
                                            smoke = 0;
                                        smoke += distInSquare;
                                    }
                                    if (tree >= treeCoefficient || hilltop >= hilltopCoefficient || smoke >= smokeCoefficient) {
                                        if (mode == 1) {
                                            if (stopIndex == 0) {
                                                stopIndex = iterationIndex;
                                                if (visualTest == 1 || visualTest == 2) {
                                                    PrintStopReason(tree >= treeCoefficient, hilltop >= hilltopCoefficient, smoke >= smokeCoefficient, tree, treeCoefficient, distInSquare);
                                                }
                                                if (Mathf.Abs(r) == 1) {
                                                    Debug.Log("Total distance: " + distanceTraveled);
                                                }
                                            }
                                        } /*else if (mode == 2) {
                                            if (distanceStop == -1)
                                            {
                                                distanceStop = distanceTraveled;
                                                stopPositionFOV = new Vector2(GRIDX[iterationIndex - 1], GRIDY[iterationIndex - 1]);
                                                break;
                                            }
                                        }
                                        
                                    }
                                }
                                //distanceTraveled += distInSquare;
                            } else {
                                //Debug.Log("(" + xPos + ", " + yPos + ") doesn't lie on the path from (" + AX + ", " + AY + ") to ("  + BX + ", " + BY + ")!");
                            }
                        }
                    }
                }
            } else if (searchMode == 2) { //top left to bottom right
                for (int yPos = AY; yPos >= BY; yPos--) {
                    for (int xPos = AX; xPos <= BX; xPos++) {
                        //check square
                        if (stopIndex == 0) {
                            iterationIndex = GetIndexFromCoords(new Vector2(xPos, yPos)); 
                            
                            bool vert = false;
                            if (xPos == AX) {
                                vert = true;
                            }
                            bool hori = false;
                            if (yPos == AY) {
                                hori = true;
                            }
                            if (LiesInsideSquare(r, xPos, yPos, AX, AY, searchMode, vertical, horizontal)) {
                                if (visualTest == 1) {
                                    testObject.transform.position = new Vector2(GRIDX[iterationIndex - 1], GRIDY[iterationIndex - 1]);
                                    //yield return new WaitForSeconds(0.3f);
                                }
                                float distInSquare = 0;
                                if (xPos == BX && yPos == BY) {
                                    //do weird stuff
                                    distInSquare = DistanceInSquare(r, xPos, yPos, BX, BY, AX, AY, vertical, horizontal);
                                } else {
                                    distInSquare = DistanceInSquare(r, xPos, yPos, AX, AY, BX, BY, vertical, horizontal);
                                }
                                distanceTraveled += distInSquare;
                                if (!(xPos == AX && yPos == AY)) {
                                    if (GRID[iterationIndex - 1] == 5) {
                                        tree += distInSquare;
                                    }
                                    float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                                    if (localElevation > myElevation)
                                    {
                                        hilltop += distInSquare;
                                    }
                                    //smoke
                                    if (isSmoked[iterationIndex - 1]) {
                                        if (smoke < 0)
                                            smoke = 0;
                                        smoke += distInSquare;
                                    }
                                    if (tree >= treeCoefficient || hilltop >= hilltopCoefficient || smoke >= smokeCoefficient) {
                                        if (stopIndex == 0) {
                                            stopIndex = iterationIndex;
                                            if (visualTest == 1 || visualTest == 2) {
                                                PrintStopReason(tree >= treeCoefficient, hilltop >= hilltopCoefficient, smoke >= smokeCoefficient, tree, treeCoefficient, distInSquare);
                                            }
                                            if (Mathf.Abs(r) == 1) {
                                                Debug.Log("Total distance: " + distanceTraveled);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } else if (searchMode == 3) { //bottom right to top left
                for (int yPos = AY; yPos <= BY; yPos++) {
                    for (int xPos = AX; xPos >= BX; xPos--) {
                        //check square
                        if (stopIndex == 0) {
                            iterationIndex = GetIndexFromCoords(new Vector2(xPos, yPos)); 
                            
                            bool vert = false;
                            if (xPos == AX) {
                                vert = true;
                            }
                            bool hori = false;
                            if (yPos == AY) {
                                hori = true;
                            }
                            if (LiesInsideSquare(r, xPos, yPos, AX, AY, searchMode, vertical, horizontal)) {
                                if (visualTest == 1) {
                                    testObject.transform.position = new Vector2(GRIDX[iterationIndex - 1], GRIDY[iterationIndex - 1]);
                                    //yield return new WaitForSeconds(0.3f);
                                }
                                float distInSquare = 0;
                                if (xPos == BX && yPos == BY) {
                                    //do weird stuff
                                    distInSquare = DistanceInSquare(r, xPos, yPos, BX, BY, AX, AY, vertical, horizontal);
                                } else {
                                    distInSquare = DistanceInSquare(r, xPos, yPos, AX, AY, BX, BY, vertical, horizontal);
                                }
                                distanceTraveled += distInSquare;
                                if (!(xPos == AX && yPos == AY)) {
                                    if (GRID[iterationIndex - 1] == 5) {
                                        tree += distInSquare;
                                    }
                                    float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                                    if (localElevation > myElevation)
                                    {
                                        hilltop += distInSquare;
                                    }
                                    //smoke
                                    if (isSmoked[iterationIndex - 1]) {
                                        if (smoke < 0)
                                            smoke = 0;
                                        smoke += distInSquare;
                                    }
                                    if (tree >= treeCoefficient || hilltop >= hilltopCoefficient || smoke >= smokeCoefficient) {
                                        if (stopIndex == 0) {
                                            stopIndex = iterationIndex;
                                            if (visualTest == 1 || visualTest == 2) {
                                                PrintStopReason(tree >= treeCoefficient, hilltop >= hilltopCoefficient, smoke >= smokeCoefficient, tree, treeCoefficient, distInSquare);
                                            }
                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } else if (searchMode == 4) { //top right to bottom left
                for (int yPos = AY; yPos >= BY; yPos--) {
                    for (int xPos = AX; xPos >= BX; xPos--) {
                        //check square
                        if (stopIndex == 0) {
                            iterationIndex = GetIndexFromCoords(new Vector2(xPos, yPos)); 
                            
                            bool vert = false;
                            if (xPos == AX) {
                                vert = true;
                            }
                            bool hori = false;
                            if (yPos == AY) {
                                hori = true;
                            }
                            if (LiesInsideSquare(r, xPos, yPos, AX, AY, searchMode, vertical, horizontal)) {
                                //Debug.Log("(" + xPos + ", " + yPos + ") lies on the path from (" + AX + ", " + AY + ") to ("  + BX + ", " + BY + ")! r = " + r);
                                if (visualTest == 1) {
                                    testObject.transform.position = new Vector2(GRIDX[iterationIndex - 1], GRIDY[iterationIndex - 1]);
                                    //yield return new WaitForSeconds(0.3f);
                                }
                                float distInSquare = 0;
                                if (xPos == BX && yPos == BY) {
                                    //do weird stuff
                                    distInSquare = DistanceInSquare(r, xPos, yPos, BX, BY, AX, AY, vertical, horizontal);
                                    Debug.Log("Distance at end: " + distInSquare);
                                } else {
                                    distInSquare = DistanceInSquare(r, xPos, yPos, AX, AY, BX, BY, vertical, horizontal);
                                    Debug.Log("Full distance: " + distInSquare);
                                }
                                distanceTraveled += distInSquare;
                                if (!(xPos == AX && yPos == AY)) {
                                    if (GRID[iterationIndex - 1] == 5) {
                                        tree += distInSquare;
                                    }
                                    float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                                    if (localElevation > myElevation)
                                    {
                                        hilltop += distInSquare;
                                    }
                                    //smoke
                                    if (isSmoked[iterationIndex - 1]) {
                                        if (smoke < 0)
                                            smoke = 0;
                                        smoke += distInSquare;
                                    }
                                    if (tree >= treeCoefficient || hilltop >= hilltopCoefficient || smoke >= smokeCoefficient) {
                                        if (stopIndex == 0) {
                                            stopIndex = iterationIndex;
                                            if (visualTest == 1 || visualTest == 2) {
                                                PrintStopReason(tree >= treeCoefficient, hilltop >= hilltopCoefficient, smoke >= smokeCoefficient, tree, treeCoefficient, distInSquare);
                                            }
                                        }
                                    }
                                }
                            } else {
                                //Debug.Log("(" + xPos + ", " + yPos + ") doesn't lie on the path from (" + AX + ", " + AY + ") to ("  + BX + ", " + BY + ")!");
                            }
                        }
                    }
                }
            }*/
            int stepX = (BX >= AX) ? 1 : -1;
            int stepY = (BY >= AY) ? 1 : -1;
            int endX = BX + stepX;
            int endY = BY + stepY;
            for (int yPos = AY; yPos != endY; yPos += stepY) {
                for (int xPos = AX; xPos != endX; xPos += stepX) {
                   //check square
                    if (stopIndex == 0) {
                        iterationIndex = GetIndexFromCoords(new Vector2(xPos, yPos)); 
                            
                        bool vert = false;
                        if (xPos == AX) {
                            vert = true;
                        }
                        bool hori = false;
                        if (yPos == AY) {
                            hori = true;
                        }
                        if (LiesInsideSquare(r, xPos, yPos, AX, AY, searchMode, vertical, horizontal)) {
                            if (visualTest == 1) {
                                testObject.transform.position = new Vector2(GRIDX[iterationIndex - 1], GRIDY[iterationIndex - 1]);
                                //yield return new WaitForSeconds(0.3f);
                            }
                            float distInSquare = 0;
                            if (xPos == BX && yPos == BY) {
                                //do weird stuff
                                distInSquare = DistanceInSquare(r, xPos, yPos, BX, BY, AX, AY, vertical, horizontal);
                                //Debug.Log("I'm at (BX, BY)");
                            } else {
                                distInSquare = DistanceInSquare(r, xPos, yPos, AX, AY, BX, BY, vertical, horizontal);
                            }
                            distanceTraveled += distInSquare;
                            if (GRID[iterationIndex - 1] == 5) {
                                tree += distInSquare;
                            }
                            if (!(xPos == AX && yPos == AY)) {
                                
                                float localElevation = FindElevation(GRID[iterationIndex - 1], 2);
                                if (localElevation > myElevation)
                                {
                                    hilltop += distInSquare;
                                }
                                //smoke
                                if (isSmoked[iterationIndex - 1]) { //idk if smoke belongs in here
                                    //Debug.Log("isSmoked");
                                    if (smoke < 0) {
                                        smoke = 0;
                                    }
                                    smoke += distInSquare;
                                    //Debug.Log("smoke = " + smoke);
                                    //Debug.Log("smokeCoefficient = " + smokeCoefficient);
                                }
                                
                            }
                            bool t = (tree >= treeCoefficient);
                            bool h = (hilltop >= hilltopCoefficient);
                            bool s = (smoke >= smokeCoefficient);
                            if (t || h || s) {
                                if (stopIndex == 0) {
                                    if (xray == 1) {
                                        if (s) {
                                            stopIndex = iterationIndex;
                                        } else {
                                            if (iterationIndex - 1 == j) {
                                                stopIndex = iterationIndex;
                                            }
                                        }
                                    } else {
                                        stopIndex = iterationIndex;
                                    }
                                    if (visualTest == 1 || visualTest == 2) {
                                        PrintStopReason(t, h, s, tree, treeCoefficient, distInSquare, xPos, yPos);
                                    }
                                    if (xray == 1) {
                                        if (!s) {
                                            if (iterationIndex - 1 == j) {
                                                /*if (isSmoked[j]) {
                                                    if (!alrInSmoke) {
                                                        stoppedXray = true;
                                                    }
                                                } else {
                                                    stoppedXray = true;
                                                }*/
                                                stoppedXray = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    } 
                }
            }
        }
        //if (mode == 1) 
            //Debug.Log("Gap");


        
        //Debug.Log("control");
        if (mode == 1)
        {
            if (stopIndex != 0 && stopIndex != j + 1)
            {
                if (stoppedXray) {
                    /*if (smokeCoefficient == 0 && !alrInSmoke) {
                        if (isSmoked[j]) {
                            replaceVal = 0;
                        }
                    } else {
                        /*if (alrInSmoke) {
                            if (isSmoked[j]) {
                                replaceVal = 0;
                            }
                        } else {
                            replaceVal = 2;
                        //}
                        replaceVal = 2;
                    }*/
                    replaceVal = 2;
                    //replaceVal = 0;
                } else {
                    replaceVal = 0;
                }
            }
            if (stopIndex == j + 1 || (stopIndex == 0 && iterationIndex == j + 1)) {
                if (smokeCoefficient == 0 && !alrInSmoke) {
                    if (stopIndex > 0 && isSmoked[stopIndex - 1]) {
                        replaceVal = 0;
                    } /*else if (stopIndex == 0 && isSmoked[iterationIndex - 1]) {
                        replaceVal = 0;
                    }*/
                } /*else if (alrInSmoke) {
                    if (stopIndex > 0 && isSmoked[stopIndex - 1]) {
                        replaceVal = 0;
                    }
                }*/
            }
        }
        else if (mode == 2)
        {
            //if (distanceStop != -1 && distanceStop != distance) //this is for when mode 2 uses math
            if (distanceStop != -1 && distanceStop != distance * tileWidth) // distanceStop < (distance * tileWidth) - (Math.Abs(tileWidth / (Mathf.Cos(angleRadians)))))
            {
                replaceVal = 0;
            }
        }
        float caveRequirement = 2;
        if (mode == 1)
        {
            if (GRID[j] == 9)
            {
                if (distance > caveRequirement)
                {
                    replaceVal = 0;
                }
            }
        }
        else if (mode == 2)
        {

        }
        if (visualTest == 1 || visualTest == 2) {
            if (visualTest != 2) {
                //yield return new WaitForSeconds(1f);
            }
            Destroy(testObject);
        }
        //callback(replaceVal);
        //yield return null;
        return replaceVal;
    }
    void PrintStopReason(bool t, bool h, bool s, float tree, float treeCoefficient, float dist, int xPos, int yPos) {
        String stopReason = "Stop reason: ";
        if (t)
            stopReason += "tree (tree: " + tree + ", treeCoefficient: " + treeCoefficient + ", dist in one square: " + dist + ") ";
        if (h)
            stopReason += "hilltop ";
        if (s)
            stopReason += "smoke ";
        stopReason += "stopped at " + "(" + xPos + ", " + yPos + ")";
        Debug.Log(stopReason);
    }
    float DistanceInSquare(float r, int xPos, int yPos, int AX, int AY, int BX, int BY, bool vert, bool hori) {
        float deltaX = 0;
        float deltaY = 0;
        int comparisonMode = 0;
        //comparisonMode = 1 means |r| > |standardSlope|, 2 means |r| < |standardSlope|
        if (xPos == AX) {
            if (yPos == AY) {
                float standardSlope;
                if (AX == BX) {
                    if (AY == BY) {
                        deltaX = 0;
                        deltaY = 0;
                    } else if (AY > BY) {
                        deltaX = 0;
                        deltaY = -0.5f;
                    } else if (AY < BY) {
                        deltaX = 0;
                        deltaY = 0.5f;
                    }
                }
                if (AY == BY) {
                    if (AX == BX) {
                        deltaX = 0;
                        deltaY = 0;
                    } else if (AX > BX) {
                        deltaX = -0.5f;
                        deltaY = 0;
                    } else if (AX < BX) {
                        deltaX = 0.5f;
                        deltaY = 0;
                    }
                }
                if (!(AX == BX) && !(AY == BY)) {
                    if (AX > BX) {
                        if (AY > BY) {
                            standardSlope = 1;
                            if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                                deltaY = -0.5f;
                                deltaX = deltaY / r;
                            } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                                deltaX = -0.5f;
                                deltaY = r * deltaX;
                            } else if (Mathf.Abs(r) == Mathf.Abs(standardSlope)) {
                                deltaX = -0.5f;
                                deltaY = -0.5f;
                            }
                        } else if (AY < BY) {
                            standardSlope = -1;
                            if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                                deltaY = 0.5f;
                                deltaX = deltaY / r;
                            } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                                deltaX = -0.5f;
                                deltaY = r * deltaX;
                            } else if (Mathf.Abs(r) == Mathf.Abs(standardSlope)) {
                                deltaX = -0.5f;
                                deltaY = 0.5f;
                            }
                        }
                    } else if (AX < BX) {
                        if (AY > BY) {
                            standardSlope = -1;
                            if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                                deltaY = -0.5f;
                                deltaX = deltaY / r;
                            } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                                deltaX = 0.5f;
                                deltaY = r * deltaX;
                            } else if (Mathf.Abs(r) == Mathf.Abs(standardSlope)) {
                                deltaX = 0.5f;
                                deltaY = -0.5f;
                            }
                        } else if (AY < BY) {
                            standardSlope = 1;
                            if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                                deltaY = 0.5f;
                                deltaX = deltaY / r;
                            } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                                deltaX = 0.5f;
                                deltaY = r * deltaX;
                            } else if (Mathf.Abs(r) == Mathf.Abs(standardSlope)) {
                                deltaX = 0.5f;
                                deltaY = 0.5f;
                            }
                        }
                    }
                }
            } else if (yPos > AY) {
                comparisonMode = 2;
            } else if (yPos < AY) {
                comparisonMode = 2;
            }
        } else if (xPos > AX) {
            if (yPos == AY) {
                comparisonMode = 1;
            } else if (yPos > AY) {
                float standardSlope = ((float) yPos - AY - 0.5f) / ((float) xPos - AX - 0.5f); //bottomLeft
                if (Mathf.Approximately(r, standardSlope)) { //could possibly be abs but risky
                    deltaX = 1;
                    deltaY = 1;
                    //Debug.Log("Same slope!");
                } else if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                    comparisonMode = 1;
                } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                    comparisonMode = 2;
                }
            } else if (yPos < AY) {
                float standardSlope = ((float) yPos - AY + 0.5f) / ((float) xPos - AX - 0.5f); //topLeft
                if (Mathf.Approximately(r, standardSlope)) {
                    deltaX = 1;
                    deltaY = -1;
                    //Debug.Log("Same slope!");
                } else if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                    comparisonMode = 1;
                } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                    comparisonMode = 2;
                }
            }
        } else if (xPos < AX) {
            if (yPos == AY) {
                comparisonMode = 1;
            } else if (yPos > AY) {
                float standardSlope = ((float) yPos - AY - 0.5f) / ((float) xPos - AX + 0.5f); //bottomRight
                if (Mathf.Approximately(r, standardSlope)) { 
                    deltaX = -1;
                    deltaY = 1;
                } else if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                    comparisonMode = 1;
                } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                    comparisonMode = 2;
                }
            } else if (yPos < AY) {
                float standardSlope = ((float) yPos - AY + 0.5f) / ((float) xPos - AX + 0.5f); //topRight
                if (Mathf.Approximately(r, standardSlope)) { 
                    deltaX = -1;
                    deltaY = -1;
                } else if (Mathf.Abs(r) > Mathf.Abs(standardSlope)) {
                    comparisonMode = 1;
                } else if (Mathf.Abs(r) < Mathf.Abs(standardSlope)) {
                    comparisonMode = 2;
                }
            }
        }
        if (comparisonMode == 0) {
        } else if (comparisonMode == 1) { //xPos != AX
            if (hori) {
                if (xPos > AX) {
                    deltaX = 1;
                    deltaY = 0;
                } else if (xPos < AX) {
                    deltaX = -1;
                    deltaY = 0;
                }
            } else {
                float x1 = 0;
                if (AX > BX) {
                    x1 = xPos - AX + 0.5f;
                } else if (AX < BX) {
                    x1 = xPos - AX - 0.5f;
                }
                float y1 = r * x1;
                Vector2 entryPoint = new Vector2(AX + x1, AY + y1);
                Vector2 cornerPoint = new Vector2(0, 0);
                int directionMode = 0;
                if (AX > BX) {
                    if (AY > BY) {
                        cornerPoint = new Vector2(xPos - 0.5f, yPos - 0.5f); //bottomLeft
                        directionMode = 1;
                    } else if (AY < BY) {
                        cornerPoint = new Vector2(xPos - 0.5f, yPos + 0.5f); //topLeft
                        directionMode = 2;
                    } else if (AY == BY) {
                        Debug.Log("ERROR! AY = BY!!"); //never supposed to happen, apparently
                    }
                } else if (AX < BX) {
                    if (AY > BY) {
                        cornerPoint = new Vector2(xPos + 0.5f, yPos - 0.5f); //bottomRight
                        directionMode = 3;
                    } else if (AY < BY) {
                        cornerPoint = new Vector2(xPos + 0.5f, yPos + 0.5f); //topRight
                        directionMode = 4;
                    }
                } else if (AX == BX) {
                    Debug.Log("ERROR! AX = BX!!"); //never supposed to happen, apparently
                }
                float slope = (cornerPoint.y - entryPoint.y) / (cornerPoint.x - entryPoint.x);
                bool stopCalculating = false;
                if (directionMode == 1) {
                    if (r <= slope) {
                        deltaX = -1;
                        deltaY = -r;
                        stopCalculating = true;
                    }
                } else if (directionMode == 2) {
                    if (r >= slope) {
                        deltaX = -1;
                        deltaY = -r;
                        stopCalculating = true;
                    }
                } else if (directionMode == 3) {
                    if (r >= slope) {
                        deltaX = 1;
                        deltaY = r;
                        stopCalculating = true;
                    }
                } else if (directionMode == 4) {
                    if (r <= slope) {
                        deltaX = 1;
                        deltaY = r;
                        stopCalculating = true;
                    }
                }
                if (!stopCalculating) {
                    float ytot = 0;
                    if (AY > BY) {
                        ytot = yPos - AY - 0.5f;
                    } else if (AY < BY) {
                        ytot = yPos - AY + 0.5f;
                    }
                    deltaY = ytot - y1;
                    deltaX = deltaY / r;
                }
            }
        } else if (comparisonMode == 2) {
            if (vert) {
                if (yPos > AY) {
                    deltaX = 0;
                    deltaY = 1;
                } else if (yPos < AY) {
                    deltaX = 0;
                    deltaY = -1;
                }
            } else {
                float y1 = 0;
                if (AY > BY) {
                    y1 = yPos - AY + 0.5f;
                } else if (AY < BY) {
                    y1 = yPos - AY - 0.5f;
                }
                float x1 = y1 / r;
                Vector2 entryPoint = new Vector2(AX + x1, AY + y1);
                Vector2 cornerPoint = new Vector2(0, 0); // |r| < |enrique|
                int directionMode = 0;
                if (AX > BX) {
                    if (AY > BY) {
                        cornerPoint = new Vector2(xPos - 0.5f, yPos - 0.5f); //bottomLeft
                        directionMode = 1;
                    } else if (AY < BY) {
                        cornerPoint = new Vector2(xPos - 0.5f, yPos + 0.5f); //topLeft
                        directionMode = 2;
                    }
                } else if (AX < BX) {
                    if (AY > BY) {
                        cornerPoint = new Vector2(xPos + 0.5f, yPos - 0.5f); //bottomRight
                        directionMode = 3;
                    } else if (AY < BY) {
                        cornerPoint = new Vector2(xPos + 0.5f, yPos + 0.5f); //topRight
                        directionMode = 4;
                    }
                }
                float slope = (cornerPoint.y - entryPoint.y) / (cornerPoint.x - entryPoint.x);
                bool stopCalculating = false;
                if (directionMode == 1) {
                    if (r >= slope) {
                        deltaX = -1 / r;
                        deltaY = -1;
                        stopCalculating = true;
                    }
                } else if (directionMode == 2) {
                    if (r <= slope) {
                        deltaX = 1 / r;
                        deltaY = 1;
                        stopCalculating = true;
                    }
                } else if (directionMode == 3) {
                    if (r <= slope) {
                        deltaX = -1 / r;
                        deltaY = -1;
                        stopCalculating = true;
                    }
                } else if (directionMode == 4) {
                    if (r >= slope) {
                        deltaX = 1 / r;
                        deltaY = 1;
                        stopCalculating = true;
                    }
                }
                if (!stopCalculating) {
                    float xtot = 0;
                    if (AX > BX) {
                        xtot = xPos - AX - 0.5f;
                    } else if (AX < BX) {
                        xtot = xPos - AX + 0.5f;
                    }
                    deltaX = xtot - x1;
                    deltaY = r * deltaX;
                }
            }
        }
        float dist = FindDistanceDelta(deltaX, deltaY);
        if (dist > 1.42f) {
            Debug.Log("Distance exceeding max possible distance!");
            Debug.Log("DeltaX = " + deltaX + ", DeltaY = " + deltaY);
            Debug.Log("For (" + xPos + ", " + yPos + ") on the path from (" + AX + ", " + AY + ") to ("  + BX + ", " + BY + ")");
            Debug.Log("Comparison mode: " + comparisonMode);
            Debug.Log("r = " + r);
        }
        //Debug.Log("DeltaX = " + deltaX + ", DeltaY = " + deltaY);
        return dist;
    }
    float FindDistanceDelta(float x, float y) {
        return Mathf.Sqrt(x * x + y * y);
    }
    bool LiesInsideSquare(float r, int xPos, int yPos, int AX, int AY, int searchMode, bool vert, bool hori) {
        bool v = (xPos == AX);
        bool h = (yPos == AY);
        if (vert) {
            return v;
        }
        if (hori) {
            return h;
        }
        if (xPos == AX && yPos == AY) {
            return true;
        }
        float maxR = 0;
        float minR = 0;
        float topLeft = ((float) yPos - AY + 0.5f) / ((float) xPos - AX - 0.5f);
        float topRight = ((float) yPos - AY + 0.5f) / ((float) xPos - AX + 0.5f);
        float bottomLeft = ((float) yPos - AY - 0.5f) / ((float) xPos - AX - 0.5f);
        float bottomRight = ((float) yPos - AY - 0.5f) / ((float) xPos - AX + 0.5f);
        bool skipSM = false;
        if (!(xPos == AX && yPos == AY)) {
            if (v) {
                if (yPos > AY) {
                    return (r > bottomRight || r < bottomLeft); //subject to change
                } else if (yPos < AY) {
                    return (r > topLeft || r < topRight); //subject to change
                }
            }
            if (h) {
                if (xPos > AX) {
                    maxR = topLeft;
                    minR = bottomLeft;
                    skipSM = true;
                } else if (xPos < AX) {
                    maxR = bottomRight;
                    minR = topRight;
                    skipSM = true;
                }
            }
        }
        if (!skipSM) {
            if (searchMode == 1) { //bottom left to top right

                if (!vert && !hori) {
                    maxR = topLeft;
                    minR = bottomRight;
                }
            } else if (searchMode == 2) { //top left to bottom right
                if (!vert && !hori) {
                    maxR = topRight;
                    minR = bottomLeft;
                }
            } else if (searchMode == 3) { //bottom right to top left
                if (!vert && !hori) {
                    maxR = bottomLeft;
                    minR = topRight;
                }
            } else if (searchMode == 4) { //top right to bottom left
                if (!vert && !hori) {
                    maxR = bottomRight;
                    minR = topLeft;
                }
            }
        }
        /*minR = Mathf.Min(Mathf.Min(topLeft, topRight), Mathf.Min(bottomLeft, bottomRight));
        maxR = Mathf.Max(Mathf.Max(topLeft, topRight), Mathf.Max(bottomLeft, bottomRight));*/
        if (maxR <= minR) {
            Debug.Log("Error: maxR less than or equal to minR");
            Debug.Log("maxR: " + maxR);
            Debug.Log("minR: " + minR);
            Debug.Log("vert: " + vert);
            Debug.Log("hori: " + hori);
            Debug.Log("xPos: " + xPos);
            Debug.Log("yPos: " + yPos);
        }
        return (r > minR && r < maxR); //subject to change
    }
    IEnumerator UpdateIsSmoked(List<float> s) {
        isSmoked = SmokeList(s);
        if (smokeFolder) {
            Destroy(smokeFolder);
            Debug.Log("Smoke test");
        }
        smokeFolder = Instantiate(SFPrefab);
        for (int i = 0; i < isSmoked.Length; i++) {
            if (isSmoked[i]) {
                Vector3 spawnPos = new Vector3(GRIDX[i], GRIDY[i], Camera.main.nearClipPlane); //new Vector3(x / screenScaleX, y / screenScaleY, Camera.main.nearClipPlane);
                spawnPos.z = 0;
                GameObject smokeClone = Instantiate(smokePrefab, spawnPos, Quaternion.identity);
                smokeClone.transform.SetParent(smokeFolder.transform, true);
            }
        }
        yield return null;
    }
    public bool[] SmokeList(List<float> s) {
        bool[] retList = new bool[s.Count];
        for (int i = 0; i < retList.Length; i++) {
            retList[i] = false;
        }
        for (int i = 0; i < retList.Length; i++) {
            Vector2 iPos = GetCoordsFromIndex(i + 1);
            float iRadius = s[i];
            for (int j = 0; j < retList.Length; j++) {
                Vector2 jPos = GetCoordsFromIndex(j + 1);
                if (FindDistance(iPos, jPos) <= iRadius)
                    retList[j] = true;
            }
        }
        return retList;
    }
    float FindVision(int terrain, List<int> inv, List<int> fx, List<int> fxs)
    {
        float returnVal = baseVision;
        if (terrain == 2)
        {
            returnVal -= 2;
        }
        else if (terrain == 3)
        {
            returnVal += 2;
        }
        else if (terrain == 4)
        {
            returnVal += 3;
        }
        else if (terrain == 5)
        {
            returnVal -= 1;
        }
        else if (terrain == 6)
        {
            returnVal += 1;
        }
        else if (terrain == 7)
        {
            returnVal += 1.5f;
        }
        else if (terrain == 8)
        {
            returnVal += 3;
        }
        else if (terrain == 9)
        {
            returnVal -= 3;
        }
        for (int i = 0; i < fx.Count; i++) {
            if (fx[i] == 2) {
                returnVal += fxs[i];
            }
        }
        returnVal += AddArmorVisions(inv);
        return returnVal;
    }
    public float FindElevation(int terrain, int mode)
    {
        float returnVal = 0;
        if (terrain == 2)
        {
            returnVal -= 1;
        }
        else if (terrain == 3)
        {
            returnVal += 1;
        }
        else if (terrain == 4)
        {
            returnVal += 2;
        }
        else if (terrain == 9)
        {
            if (mode == 2)
            {
                returnVal += 1;
                //for bullets and vision
            }
        }
        return returnVal;
    }
    public float FindDistance(Vector2 vectorA, Vector2 vectorB)
    {
        float xDistance = Math.Abs(vectorA.x - vectorB.x);
        float yDistance = Math.Abs(vectorA.y - vectorB.y);
        float distance = Mathf.Sqrt((xDistance * xDistance) + (yDistance * yDistance));
        return distance;
    }
    bool ReturnToGrid = false;
    bool animationDone = false;
    IEnumerator RTG()
    {
        while (moves > 0)
        {
            if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                ReturnToGrid = true;
            }
            yield return null;
        }
    }
    IEnumerator RTGCheck()
    {
        while (moves > 0)
        {
            if (ReturnToGrid)
            {
                
                while (animationDone)
                {
                    yield return null;
                }
                while (!animationDone)
                {
                    yield return null;
                    if (animationDone)
                    {
                        break;
                    }
                    //Debug.Log("animationDone: " + animationDone);
                }
                ReturnToGrid = false;
                yield return new WaitForSeconds(0.2f);
                //animationDone = false;
                if (!ReturnToGrid)
                {
                    if (moves > 0) {
                        VCSpriterenderer.enabled = false;
                        yield return StartCoroutine(Vision(0, smokes, myEffects, myEffectStrengths));
                    }
                }
            }
            yield return null;
        }
    }

    bool moving = false;
    IEnumerator Move()
    {
        if (!statsBookOpen && !rollChancesOpen && !bookInventoryOpen && !replaying && !replaying2 && !replaying6) {
            if (Keyboard.current.upArrowKey.isPressed)
            {
                
                ChangePosition(0, 1, 1, moves, 0, GRID.Count, null);
                while (moving) {
                    yield return null;
                }
            }
            if (Keyboard.current.downArrowKey.isPressed)
            {
                
                ChangePosition(0, -1, 1, moves, 0, GRID.Count, null);
                while (moving) {
                    yield return null;
                }
            }
            if (Keyboard.current.rightArrowKey.isPressed)
            {
                
                ChangePosition(1, 0, 1, moves, 0, GRID.Count, null);
                while (moving) {
                    yield return null;
                }
            }
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                
                ChangePosition(-1, 0, 1, moves, 0, GRID.Count, null);
                while (moving) {
                    yield return null;
                }
            } 
        }
        yield return null; 
    }
    public List<float> ChangePosition(int x, int y, int mode, float myMoves, int prevP, int gridCount, BotScript BS)
    {
        if (myMoves > 0)
        {
            if (mode == 1) {
                moving = true;
            }
            int prevPosition = myPosition;
            if (mode == 2) {
                prevPosition = prevP;
            }
            int botPosition = prevP;
            bool subtractMoveX = false;
            bool subtractMoveY = false;
            int animationX = 0;
            int animationY = 0;
            if (x != 0)
            {
                subtractMoveX = true;
            }
            if (x != 0)
            {
                if (x > 0)
                {
                    int coefficient = 0;
                    if (prevPosition % columns.Value == 0)
                    {
                        coefficient = Mathf.FloorToInt(prevPosition / columns.Value) - 1;
                    }
                    else
                    {
                        coefficient = Mathf.FloorToInt(prevPosition / columns.Value);
                    }
                    int coefficient2 = 0;
                    if ((prevPosition + x) % columns.Value == 0)
                    {
                        coefficient2 = Mathf.FloorToInt((prevPosition + x) / columns.Value) - 1;
                    }
                    else
                    {
                        coefficient2 = Mathf.FloorToInt((prevPosition + x) / columns.Value);
                    }
                    int val = prevPosition - (coefficient * columns.Value);
                    if (coefficient < coefficient2)
                    {
                        if ((columns.Value - val) <= 0) 
                        {
                            subtractMoveX = false;
                        }
                        if (mode == 1) {
                            myPosition += (columns.Value - val);
                            animationX = (columns.Value - val);
                        } else if (mode == 2) {
                            botPosition += (columns.Value - val);
                        }
                    }
                    else
                    {
                        if (mode == 1) {
                            myPosition += x;
                            animationX = x;
                        } else if (mode == 2) {
                            botPosition += x;
                        }
                    }
                }
                else
                {
                    int coefficient = 0;
                    if (prevPosition % columns.Value == 0) 
                    {
                        coefficient = Mathf.FloorToInt(prevPosition / columns.Value) - 1;
                    }
                    else
                    {
                        coefficient = Mathf.FloorToInt(prevPosition / columns.Value);
                    }
                    int coefficient2 = 0;
                    if ((prevPosition + x) % columns.Value == 0)
                    {
                        coefficient2 = Mathf.FloorToInt((prevPosition + x) / columns.Value) - 1;
                    }
                    else
                    {
                        coefficient2 = Mathf.FloorToInt((prevPosition + x) / columns.Value);
                    }
                    int val = prevPosition - (coefficient * columns.Value);
                    if (coefficient > coefficient2)
                    {
                        if ((val - 1) <= 0)
                        {
                            subtractMoveX = false;
                        }
                        if (mode == 1) {
                            myPosition -= (val - 1);
                            animationX = -(val - 1);
                        } else if (mode == 2) {
                            botPosition -= (val - 1);
                        }
                    }
                    else
                    {
                        if (mode == 1) {
                            myPosition += x;
                            animationX = x;
                        } else if (mode == 2) {
                            botPosition += x;
                        }
                    }
                }
            }
            if (y != 0)
            {
                subtractMoveY = true;
            }
            if (y != 0)
            {
                if (y > 0)
                {
                    if ((prevPosition + (y * columns.Value)) > gridCount) //caution for GRID when mode == 2
                    {
                        if (Mathf.FloorToInt((gridCount - prevPosition) / columns.Value) * columns.Value <= 0)
                        {
                            subtractMoveY = false;
                        }
                        if (mode == 1) {
                            myPosition += Mathf.FloorToInt((gridCount - prevPosition) / columns.Value) * columns.Value;
                            animationY = Mathf.FloorToInt((gridCount - prevPosition) / columns.Value) * columns.Value;
                        } else if (mode == 2) {
                            botPosition += Mathf.FloorToInt((gridCount - prevPosition) / columns.Value) * columns.Value;
                        }
                    }
                    else
                    {
                        if (mode == 1) {
                            myPosition += y * columns.Value;
                            animationY = y * columns.Value;
                        } else if (mode == 2) {
                            botPosition += y * columns.Value;
                        }
                    }
                }
                else
                {
                    if ((prevPosition + (y * columns.Value)) < 1)
                    {
                        if (Mathf.FloorToInt((prevPosition - 1) / columns.Value) * columns.Value <= 0)
                        {
                            subtractMoveY = false;
                        }
                        if (mode == 1) {
                            myPosition -= Mathf.FloorToInt((prevPosition - 1) / columns.Value) * columns.Value;
                            animationY = -(Mathf.FloorToInt((prevPosition - 1) / columns.Value) * columns.Value);
                        } else if (mode == 2) {
                            botPosition -= Mathf.FloorToInt((prevPosition - 1) / columns.Value) * columns.Value;
                        }
                    }
                    else
                    {
                        if (mode == 1) {
                            myPosition += y * columns.Value;
                            animationY = y * columns.Value;
                        } else if (mode == 2) {
                            botPosition += y * columns.Value;
                        }
                    }
                }
            }
            //myPosition += x;
            //myPosition += y * columns.Value;
            
            
            if (subtractMoveX == true || subtractMoveY == true)
            {
                if (mode == 1) {
                    StartCoroutine(Mode1Move(prevPosition, animationX, animationY));
                    return null;
                } else if (mode == 2) {
                    int indexHelper = botPosition - 1;
                    float movesHelper = myMoves - MovesSubtract(prevPosition, botPosition, 1);
                    int workedHelper = 1;
                    return new List<float>{workedHelper, movesHelper, indexHelper};
                }
            } else {
                if (mode == 1) {
                    moving = false;
                } else if (mode == 2) {
                    int workedHelper = 0;
                    return new List<float>{workedHelper, 0, -1};
                }
            }
            //yield return new WaitForSeconds(0.1f);
        }
        return null;
    }
    IEnumerator Mode1Move(int prevPosition, int animationX, int animationY) {
        for (int i = 0; i < visionGrid.Count; i++)
        {
            SpriteRenderer VSpriteRenderer = visionObjects[i].GetComponent<SpriteRenderer>();
            Color color = VSpriteRenderer.color;
            color.a = 0;
            VSpriteRenderer.color = color;
            visionObjects[i].GetComponent<SpriteMask>().enabled = false;
        }
        VCSpriterenderer.enabled = true;
        if (replayable) {
            replayable = false;
            if (changedFrozen) {
                changedFrozen = false;
            }
            Debug.Log("replayable is false pt2");
        }
        if (replayable2) {
            replayable2 = false;
            playersDeltaHealth.Clear();
        }
        if (replayable3 == 3) {
            replayable3 = 0;
            Debug.Log("replayable3 is false");
        }
        if (replayable3 == 4) {
            replayable3 = 0;
        }
        if (replayable4) {
            replayable4 = false;
        }
        if (replayable5) {
            replayable5 = false;
        }
        if (replayable6) {
            replayable6 = false;
            R6Smoke = false;
        }
        moves -= MovesSubtract(prevPosition, myPosition, 1);
        UpdateMovesText(moves);
        animationDone = false;
        yield return StartCoroutine(Animation(prevPosition, myPosition, animationX, animationY, MovesSubtract(prevPosition, myPosition, 2)));
        animationDone = true;
        moving = false;

        //yield return StartCoroutine(CreateFOVRaycast(new Vector3(GRIDX[myPosition - 1], GRIDY[myPosition - 1], Camera.main.nearClipPlane)));
        
        SetCharacterPosition(myPlayerObject, myPosition);
        
        //yield return new WaitForSeconds(0.1f);
    }
    float MovesSubtract(int prev, int newP, int mode)
    {
        int prevTer = GRID[prev - 1];
        int newTer = GRID[newP - 1];
        float prevVal = SpecificMovesSubtract(prevTer, mode);
        float newVal = SpecificMovesSubtract(newTer, mode);
        if (prevTer == 4 && newTer == 3) {
            return newVal;
        } else {
            return Math.Max(newVal, prevVal);
        }
    }
    float SpecificMovesSubtract(int terrain, int mode)
    {
        float returnVal = 1;
        if (terrain == 1 || terrain == 5 || terrain == 6 || terrain == 7 || terrain == 8)
        {
            if (mode == 1)
            {
                returnVal = 1;
            }
            else
            {
                returnVal = 0.25f;
            }
        }
        else if (terrain == 2 || terrain == 3)
        {
            if (mode == 1)
            {
                returnVal = 2;
            }
            else
            {
                returnVal = 0.75f;
            }
        }
        else if (terrain == 4 || terrain == 9)
        {
            if (mode == 1)
            {
                returnVal = 3;
            }
            else
            {
                returnVal = 1.5f;
            }
        }
        return returnVal;
    }
    IEnumerator Animation(int prevPosition, int newPosition, int x, int y, float duration)
    {
        //int repeats = 80;
        //float duration = 0.25f;//repeats / FPS;
        float changeX = (GRIDX[newPosition - 1] - GRIDX[prevPosition - 1]) / duration;
        float changeY = (GRIDY[newPosition - 1] - GRIDY[prevPosition - 1]) / duration;
        //float changeX = (GRIDX[newPosition - 1] - GRIDX[prevPosition - 1]) / repeats;
        //float changeY = (GRIDY[newPosition - 1] - GRIDY[prevPosition - 1]) / repeats;
        float xPos = GRIDX[prevPosition - 1];
        float yPos = GRIDY[prevPosition - 1];
        float jumpHeight = tileHeight;
        float velocity = (4 * jumpHeight) / duration;
        //float velocity = (4 * jumpHeight) / repeats;
        velocity *= 1.1f;
        float vChange = (8 * jumpHeight) / (duration * duration);
        //vChange *= 0.98f;
        //float vChange = (8 * jumpHeight) / (repeats * repeats);
        float elapsed = 0;
        
        float targetX = GRIDX[newPosition - 1];
        float targetY = GRIDY[newPosition - 1];
        int count = 0;
        //for (int i = 1; i < repeats; i++)
        //while (elapsed < duration)
        while (velocity > 0 || yPos > targetY)
        {
            count += 1;
            elapsed += Time.deltaTime;
            xPos += changeX * Time.deltaTime;
            //xPos += changeX;
            if (changeX > 0) {
                if (xPos > targetX) {
                    xPos = targetX;
                }
            } else if (changeX < 0) {
                if (xPos < targetX) {
                    xPos = targetX;
                }
            }
            velocity -= vChange * Time.deltaTime;
            //velocity -= vChange;

            yPos += changeY * Time.deltaTime;

            yPos += velocity * Time.deltaTime;
            //yPos += velocity;
            if (velocity < 0) {
                if (yPos < targetY) {
                    yPos = targetY;
                }
            }
            /*xPos += changeX;
            velocity -= vChange;
            yPos += changeY;
            yPos += velocity;*/
            myPlayerObject.transform.position = new Vector3(xPos, yPos, Camera.main.nearClipPlane);
            float myElevation;
            float myVision;
            if (elapsed < duration / 2)
            {
                myElevation = FindElevation(GRID[prevPosition - 1], 1);
                myVision = FindVision(GRID[prevPosition - 1], INVENTORY, myEffects, myEffectStrengths);
            }
            else
            {
                myElevation = FindElevation(GRID[newPosition - 1], 1);
                myVision = FindVision(GRID[newPosition - 1], INVENTORY, myEffects, myEffectStrengths);
            }
            StartCoroutine(CreateFOVRaycast(new Vector3(xPos, yPos, Camera.main.nearClipPlane), myElevation, myVision, smokes));

            //yield return null;
            yield return new WaitForSeconds(Time.deltaTime / 2);
        }
        //Debug.Log(count);
        yield return new WaitForSeconds(0.1f);
    }
    void AnimationMove() {

    }
    IEnumerator CreateFOVRaycast(Vector3 startPosition, float myElevation, float myVision, List<float> s)
    {
        int xray = ScanInvXray(INVENTORY);
        float fov = 360f;
        Vector3 origin = startPosition;
        //GameObject visionTestClone = Instantiate(visionTest, origin, Quaternion.identity);
        origin.z = 0f;
        int rayCount = 360;
        float angle = 0f;
        float angleIncrease = fov / rayCount;
        float viewDistance = myVision * tileWidth;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        isSmoked = SmokeList(s);
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = new Vector3();
            int raycast = CheckVisionObstacles(origin, angle * Mathf.Deg2Rad, viewDistance, myElevation, 0, 2, null, 0, xray);
            /*int raycast = 0;
            StartCoroutine(CheckVisionObstacles(origin, angle * Mathf.Deg2Rad, viewDistance, myElevation, 0, 2, null, 0, xray, result => {
                raycast = result;
            }));*/
            //RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance);
            if (raycast == 1)
            {
                // No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else if (raycast == 0)
            {
                // Hit object
                vertex = (Vector3)stopPositionFOV;
            }
            vertices[vertexIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            
            vertexIndex++;

            angle -= angleIncrease;
        }
        

        visionMesh.vertices = vertices;
        visionMesh.uv = uv;
        visionMesh.triangles = triangles;
        //yield return new WaitForSeconds(1);
        yield return null;
    }
    public static Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 - 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    IEnumerator AirdropAndWait(int pnum, int adnum, int mode, int transmitter) {
        airdropConfirm = false;
        AirdropServerRpc(pnum, adnum, mode, transmitter);
        while (!airdropConfirm) {
            yield return null;
        }
        yield return null;
    }
    int airdropTurnServer = 10;
    bool airdropConfirm = false;
    [ServerRpc(RequireOwnership = false)]
    void AirdropServerRpc(int pnum, int adnum, int mode, int transmitter) {
        bool worked = false;
        //if (TURNS.Value >= 10 && TURNS.Value % 5 == 0) {
        //if (TURNS.Value % 1 == 0) {
        if (TURNS.Value >= airdropTurnServer) {
            //Debug.Log("Mod 5");
            if (airdropTurn == 0) {
                airdropTurn = playerAmount.Value;
            }
            if (TURN.Value == airdropTurn) {
                airdropTurnServer += 5;
                //airdropServerRpc
                if (adnum < 1) {
                    worked = true;
                }
            }
        }
        if (worked) {
            int pos = UnityEngine.Random.Range(1, GRID.Count + 1);
            for (int i = 0; i < botList.Count; i++) {
                if (botList[i]) {
                    playerList[i].GetComponent<BotScript>().botAirdrops.Add(pos);
                }
            }
            AirdropClientRpc(pos, pnum, mode, transmitter);
            airdropTurn -= 1;
            if (airdropTurn < 1) {
                airdropTurn = playerAmount.Value;
            }
        } else {
            ConfirmAirdropClientRpc(pnum, mode, transmitter);
        }
    }
    [ClientRpc]
    void AirdropClientRpc(int pos, int pnum, int mode, int transmitter) {
        replayable5 = true;
        prevAirdropPos = pos;
        airdrops.Add(pos);
        airdropGOs.Add(null);
        prevAirdropIndex = airdrops.Count - 1;
        StartCoroutine(AnimateAirdrop(pos, airdrops.Count - 1));
        if (mode == 1) {
            if (pnum == myPlayerNum) {
                airdropConfirm = true;
            }
        } else if (mode == 2) {
            if (transmitter == myPlayerNum) {
                airdropConfirm = true;
            }
        }
    }
    [ClientRpc]
    void ConfirmAirdropClientRpc(int pnum, int mode, int transmitter) { //only for cases where it doesn't work
        if (mode == 1) {
            if (pnum == myPlayerNum) {
                airdropConfirm = true;
            }
        } else if (mode == 2) {
            if (transmitter == myPlayerNum) {
                airdropConfirm = true;
            }
        }
        
    }
    IEnumerator BotWin() {
        StartCoroutine(CreateMessageText("Player " + (playerAliveList.IndexOf(true) + 1) + " won!", 0, 0, 3, true));
        StartCoroutine(PlaySound(sounds[32]));
        yield return new WaitForSeconds(2f);
        restartGame.RstrtGm(false);
    }
    [ServerRpc(RequireOwnership = false)]
    public void TurnDoneServerRpc(int playerNum, float[] a, float[] BDD, float[] LDs, int[] TI, int[] SI, int hp, int maxHp, bool skipped)
    {
        int potentialTurn = TURN.Value;
        if (CountAlive(playerAliveList) <= 1) {
            //win
            //Debug.Log("Win");
            //player 1 kills player 2, next turn is player 2's, 2 is technically still alive, 2 becomes dead, on 2's turn, skips his turn to next turn
            //player 1 kills player 3, next turn is player 2's, 3 is technically still alive, 3 becomes dead on 2's turn, 2 finishes turn, and 3 is skipped because dead
            if (botList[playerAliveList.IndexOf(true)]) {
                if (LocalScript.gamemode == 4) {
                    StartCoroutine(BotWin());
                }
            } else {
                WinClientRpc(playerAliveList.IndexOf(true) + 1); //myPlayerNum
            }
        } else {
            potentialTurn += 1;
            if (potentialTurn > playerAmount.Value)
            {
                potentialTurn = 1;
                TURNS.Value++;
            }
            while (playerAliveList[potentialTurn - 1] == false) {
                potentialTurn += 1;
                if (potentialTurn > playerAmount.Value)
                {
                    potentialTurn = 1;
                }
            }
            TURN.Value = potentialTurn;
            //Debug.Log("Keep going");
            
            NewTurnClientRpc(playerNum, TURN.Value, a, BDD, LDs, TI, SI, hp, maxHp, skipped, TURNS.Value);
        }
    }
    [ClientRpc]
    void WinClientRpc(int num) {
        if (num == myPlayerNum) {
            StartCoroutine(CreateMessageText("You won!", 0, 0, 3, false));
            StartCoroutine(PlaySound(sounds[32]));
            mainMenuButton.SetActive(true);
        }
    }
    int CountAlive(NetworkList<bool> list) {
        int count = 0;
        for (int i = 0; i < list.Count; i++) {
            if (list[i] == true) {
                count++;
            }
        }
        return count;
    }
    IEnumerator CreateOrderedPlayerList()
    {
        Debug.Log("COPL");
        //List<GameObject> returnList = new List<GameObject>();
        List<GameObject> playerListUnordered = new List<GameObject>();
        GameObject[] potentialPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in potentialPlayers)
        {
            var netObj = obj.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                playerListUnordered.Add(obj); 
            }
        }
        int lowestPlayerNum;
        int loopAmount = playerListUnordered.Count;
        for (int i = 0; i < loopAmount; i++)
        {
            lowestPlayerNum = loopAmount;
            foreach (GameObject obj in playerListUnordered)
            {
                if (obj.TryGetComponent<BotScript>(out _)) {
                    //bot
                    PlayerScript objScript = obj.GetComponent<PlayerScript>();
                    if (!objScript.amSpectator.Value) {
                        if (objScript.botNum == 0)
                        {
                            while (objScript.botNum == 0)
                            {
                                yield return null;
                            }
                        }
                        int objPN = objScript.botNum;
                        if (objPN < lowestPlayerNum)
                        {
                            lowestPlayerNum = objPN;
                        }
                    }
                } else {
                    //human
                    PlayerScript objScript = obj.GetComponent<PlayerScript>();
                    if (!objScript.amSpectator.Value) {
                        if (objScript.playerNum.Value == 0)
                        {
                            while (objScript.playerNum.Value == 0)
                            {
                                yield return null;
                            }
                        }
                        int objPN = objScript.playerNum.Value;
                        if (objPN < lowestPlayerNum)
                        {
                            lowestPlayerNum = objPN;
                        }
                    }
                }
            }
            int loopAmountB = playerListUnordered.Count;
            for (int j = loopAmountB; j > 0; j--)
            {
                GameObject obj = playerListUnordered[j - 1];
                if (obj.TryGetComponent<BotScript>(out _)) {
                    //bot
                    PlayerScript objScript = obj.GetComponent<PlayerScript>();
                    if (objScript.amSpectator.Value) {
                        spectatorList.Add(obj);
                        Debug.Log("bot added as a spectator");
                    } else {
                        int objPN = objScript.botNum;
                        if (objPN == lowestPlayerNum)
                        {
                            playerList.Add(obj);
                            Debug.Log("bot added");
                            playerPositionList.Add(objScript.botPosition);
                            enemyEffects.Add(new List<int>());
                            healths.Add(0);
                            maxHealths.Add(0);
                            prevHealths.Add(0);
                            prevMaxHealths.Add(0);
                            enemyHealthsVisible.Add(false);
                            playerListUnordered.RemoveAt(j - 1);
                            if (amSpectator) {
                                spectatorVGs.Add(null);
                            }
                            break;
                        }
                    }
                } else {
                    //human
                    PlayerScript objScript = obj.GetComponent<PlayerScript>();
                    if (objScript.amSpectator.Value) {
                        spectatorList.Add(obj);
                        Debug.Log("player added as a spectator");
                    } else {
                        int objPN = objScript.playerNum.Value;
                        if (objPN == lowestPlayerNum)
                        {
                            playerList.Add(obj);
                            playerPositionList.Add(objScript.position.Value);
                            enemyEffects.Add(new List<int>());
                            healths.Add(0);
                            maxHealths.Add(0);
                            prevHealths.Add(0);
                            prevMaxHealths.Add(0);
                            enemyHealthsVisible.Add(false);
                            playerListUnordered.RemoveAt(j - 1);
                            if (amSpectator) {
                                spectatorVGs.Add(null);
                            }
                            break;
                        }
                    }
                }
            }
        }
        ConfirmCOPLServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ConfirmCOPLServerRpc() {
        //COPLConfirm++; //only do if running on a dedicated server
        //give to client regardless of dedicated server or not 
        ConfirmCOPLClientRpc();
    }
    [ClientRpc]
    public void ConfirmCOPLClientRpc() {
        COPLConfirm++;
    }
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionServerRpc(NetworkObjectReference targetRef, int newPosition, bool autoUpdate, int pnum, int[] VG)
    {
        if (targetRef.TryGet(out NetworkObject targetObj))
        {
            var script = targetObj.GetComponent<PlayerScript>();
            if (script.isBot) {
                script.botPosition = newPosition;
                targetObj.GetComponent<BotScript>().position = newPosition;
                // only bots
                if (autoUpdate == true)
                {
                    PositionEnemyClientRpc(script.botNum, newPosition);
                }
            } else {
                script.position.Value = newPosition;
                if (autoUpdate == true)
                {
                    PositionEnemyClientRpc(script.playerNum.Value, newPosition);
                }
            }
            for (int i = 0; i < botList.Count; i++) {
                if (botList[i]) {
                    GameObject botObject = playerList[i];
                    BotScript botScript = botObject.GetComponent<BotScript>();
                    if (botScript.playerNum != pnum) {
                        /*if (!botScript.lScript) {
                            botScript.lScript = this;
                        }*/
                        if (autoUpdate) {
                            StartCoroutine(botScript.BotVisionHouse(true, pnum));
                        }
                    }
                }
            }
            if (amSpectator) {
                spectatorVGs[pnum - 1] = new List<int>(VG.ToList());
                spectatorCombinedVG = GetCombinedVisionGrids(spectatorVGs);
                ShowVisionGreys(spectatorCombinedVG);
                ShowPlayersForSpectator(spectatorCombinedVG);
            }
        }
    }
    void ShowPlayersForSpectator(List<int> SCVG) {
        for (int i = 0; i < playerList.Count; i++) {
            StartCoroutine(EnemyVisible(SCVG, playerList[i], playerPositionList[i], enemyEffects[i].Contains(1)));
        }
    }
    List<int> GetCombinedVisionGrids(List<List<int>> VGs) {
        List<int> retList = new List<int>();
        for (int i = 0; i < VGs[0].Count; i++) {
            bool nonZero = false;
            for (int j = 0; j < VGs.Count; j++) {
                if (VGs[j][i] != 0) {
                    nonZero = true;
                }
            }
            if (nonZero) {
                retList.Add(1);
            } else {
                retList.Add(0);
            }
        }
        return retList;
    }
    [ClientRpc]
    public void PositionEnemyClientRpc(int playerNum, int newPos)
    {
        if (playerNum != myPlayerNum)
        {
            if (replayable) {
                replayable = false;
                if (changedFrozen) {
                    changedFrozen = false;
                }
                Debug.Log("replayable is false");
            }
            StartCoroutine(CreateMessageText("The enemy has moved", 0, -100, 1, false));
            //check vision
            if (!amSpectator) {
                StartCoroutine(EnemyVisible(visionGrid, playerList[playerNum - 1], newPos, enemyEffects[playerNum - 1].Contains(1)));
            }
            SetCharacterPosition(playerList[playerNum - 1], newPos);
            playerPositionList[playerNum - 1] = newPos;
            if (replayable2) {
                replayable2 = false;
                playersDeltaHealth.Clear();
            }
            if (replayable3 == 3) {
                replayable3 = 0;
                Debug.Log("replayable3 is false");
            }
            if (replayable3 == 4) {
                replayable3 = 0;
            }
            if (replayable4) {
                replayable4 = false;
                Debug.Log("replayable 4 is false");
            }
            if (replayable6) {
                replayable6 = false;
                R6Smoke = false;
            }
        }
    }
    void OnClientConnected(ulong clientId)
    {
        StartCoroutine(DelayedSet(amSpectator, false, clientId));
        Debug.Log("Client connected");
    }
    bool serverConnected = false;
    IEnumerator DelayedSet(bool dontMake, bool bot, ulong? clientId)
    {
        Debug.Log("Delayed set");
        if (IsHost)
        {
            //Debug.Log("Start host");
            if (!dontMake) {
                if (!bot) {
                    yield return new WaitUntil(() => 
                        NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId.Value, out var targetClient) 
                        && targetClient.PlayerObject != null
                    );
                    Debug.Log("Done waiting");
                    if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId.Value, out var client))
                    {
                        playerAmount.Value += 1;
                        playerAliveList.Add(true);
                        botList.Add(bot);
                        skippedTurns.Add(0);
                        var playerObject = client.PlayerObject;
                        if (playerObject != null)
                        {
                            var playerScript = playerObject.GetComponent<PlayerScript>();
                            if (playerScript != null)
                            {
                                playerScript.playerNum.Value = playerAmount.Value;
                                playerScript.isBotNV.Value = false;
                                playerScript.isBot = false;
                                Debug.Log("set");
                                Debug.Log(playerAmount.Value);
                                Debug.Log(playerScript.playerNum.Value);
                                
                            }
                        }
                    }  
                } else {
                    Debug.Log("Start bot");
                    playerAmount.Value += 1;
                    playerAliveList.Add(true);
                    botList.Add(bot);
                    skippedTurns.Add(0);
                    GameObject botObject = Instantiate(botPF, new Vector2(), Quaternion.identity);
                    NetworkObject botNetworkObject = botObject.GetComponent<NetworkObject>();

                    if (botNetworkObject != null) {
                        botNetworkObject.Spawn();
                        var botScript = botNetworkObject.GetComponent<PlayerScript>();
                        if (botScript != null) {
                            botScript.botNum = playerAmount.Value; //playerAmount is 1+
                            botScript.isBot = true;
                            //^ should be network if not only bots
                            //botScript.isBot = true;
                            botScript.amSpectator.Value = false;
                        }
                    }
                }
            }
        }
        //yield return new WaitForSeconds(0.1f);
        if (IsServer)
        {
            if (playerAmount.Value >= playersRequired)
            {
                StartGameClientRpc();
                Debug.Log("Start Game Client Rpc");
                Debug.Log(playerAmount.Value);
                Debug.Log(playersRequired);
            }
            serverConnected = true;
        }
        yield return null;
    }
    public IEnumerator StartBots() {
        if (IsServer) {
            //yield return new WaitForSeconds(0.2f);
            while (!serverConnected) {
                yield return null;
            }
            Debug.Log("Starting bots");
            for (int i = 0; i < playersRequired - 1; i++) {
                yield return StartCoroutine(DelayedSet(false, true, null));
                
            }
        }
        
        yield return null;
    }
    public IEnumerator StartBotTraining() {
        Debug.Log("Start bot training");
        if (IsServer) {
            Debug.Log("SBT server");
            //yield return new WaitForSeconds(0.2f);
            while (!serverConnected) {
                yield return null;
            }
            Debug.Log("Starting bot training");
            for (int i = 0; i < playersRequired; i++) {
                yield return StartCoroutine(DelayedSet(false, true, null));
                
            }
        }
        
        yield return null;
    }
    public override void OnNetworkSpawn()
    {
        /*if (GRID == null)
        {
            GRID = new NetworkList<int>();
        }
        if (playerPositions == null)
        {
            playerPositions = new NetworkList<int>(new List<int>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        }*/
        
        StartCoroutine(StartGame());
        /*if (IsHost)
        {
            //playerAmount = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
            
            NetworkManager.OnClientConnectedCallback += (clientId) =>
            {
                myPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
                playerAmount.Value += 1;
                
                myPlayerObject.GetComponent<PlayerScript>().playerNum = playerAmount.Value;
            };
        }
        else
        {
            myPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject?.gameObject;
            while (myPlayerObject == null)
            {

            }
            myPlayerObject.GetComponent<PlayerScript>().playerNum = playerAmount.Value;
            Debug.Log(playerAmount.Value);
        }*/
    }
    IEnumerator StartGame()
    {

        //Camera.main.orthographicSize = Screen.height / 200f;
        /*tileWidth = tile.GetComponent<SpriteRenderer>().bounds.size.x;
        tileHeight = tile.GetComponent<SpriteRenderer>().bounds.size.y;*/
        Camera.main.orthographic = true;
        bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenWidth = topRight.x - bottomLeft.x;
        screenHeight = topRight.y - bottomLeft.y;
        gameHeight = Camera.main.orthographicSize * 2;
        gameWidth = gameHeight * Camera.main.aspect;
        screenScaleX = screenWidth / gameWidth;
        screenScaleY = screenHeight / gameHeight;
        Debug.Log(screenScaleY);
        Debug.Log(screenWidth);
        Debug.Log(Screen.width);

        turnIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(CANVAS.rect.width, CANVAS.rect.width * 6f / 120f);
        turnIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(CANVAS.rect.height / 2));
        turnIndicator.SetActive(true);

        yield return StartCoroutine(CreateMap(0));
        int count = 0;
        for (int i = 0; i < rows.Value; i++)
        {
            for (int j = 0; j < columns.Value; j++)
            {
                count += 1;
                Vector3 spawnPos = new Vector3(GRIDX[count - 1], GRIDY[count - 1], Camera.main.nearClipPlane); //new Vector3(x / screenScaleX, y / screenScaleY, Camera.main.nearClipPlane);
                spawnPos.z = 0;
                GameObject tileClone = Instantiate(tile, spawnPos, Quaternion.identity);
                tileClone.transform.SetParent(tiles.transform, true);
                SpriteRenderer TCSpriteRenderer = tileClone.GetComponent<SpriteRenderer>();
                if (!amSpectator && (GRID[count - 1] == 6 || GRID[count - 1] == 7 || GRID[count - 1] == 8)) {
                    TCSpriteRenderer.sprite = terrain[10];
                    mysteryBuildings.Add(count - 1);
                } else {
                    TCSpriteRenderer.sprite = terrain[GRID[count - 1]];
                }
                TCSpriteRenderer.sortingLayerName = "tile";
                tileObjects.Add(tileClone);
                tileClone = Instantiate(tile, spawnPos, Quaternion.identity);
                tileClone.transform.SetParent(visions.transform, true);
                TCSpriteRenderer = tileClone.GetComponent<SpriteRenderer>();
                TCSpriteRenderer.sprite = visionBlack;
                Color color = TCSpriteRenderer.color;
                color.a = 0.8f;
                tileClone.GetComponent<SpriteMask>().enabled = true;
                TCSpriteRenderer.color = color;
                TCSpriteRenderer.sortingLayerName = "vision";
                tileClone.name = "Vision";
                visionObjects.Add(tileClone);
                
                if (GRID[count - 1] != 2)
                {
                    GameObject tileOutline = Instantiate(tile, spawnPos, Quaternion.identity);
                    tileOutline.transform.SetParent(outlines.transform, true);
                    //TCSpriteRenderer.sortingLayerName = "tile";
                    SpriteRenderer TOSpriteRenderer = tileOutline.GetComponent<SpriteRenderer>();
                    TOSpriteRenderer.sprite = terrain[0];
                    color = TOSpriteRenderer.color;
                    color.a = 0.5f;
                    TOSpriteRenderer.color = color;
                    TOSpriteRenderer.sortingLayerName = "outline";
                    tileOutline.name = "Outline";
                }
            }
        }
        visionCover = Instantiate(visionCoverPrefab);
        visionCover.transform.localScale = new Vector3(tileWidth * columns.Value, tileHeight * rows.Value, 1);
        visionCover.transform.position = new Vector3((GRIDX[0] + GRIDX[GRIDX.Count - 1]) / 2, (GRIDY[0] + GRIDY[GRIDY.Count - 1]) / 2, 0);
        VCSpriterenderer = visionCover.GetComponent<SpriteRenderer>();
        VCSpriterenderer.enabled = false;
        yield return null;
    }
    IEnumerator CreateMap(int mode) {
        //mode == 0 means randomly generated
        int limitingDimension = 0; //1 is width, 2 is height
        limitCoefficientX = 1;
        limitCoefficientY = 1;
        //yield return new WaitForSeconds(2);
        if (IsHost)
        {
            //columns.Value = Mathf.FloorToInt(screenWidth / (tileWidth));
            //rows.Value = Mathf.FloorToInt(screenHeight / (tileHeight));
            if (mode == 0) {
                columns.Value = 23;
                rows.Value = 17;
            } else if (mode == 1) {
                columns.Value = 67;
                rows.Value = 35;
            }
            //Debug.Log(columns.Value);
        }
        else
        {
            //yield return new WaitForSeconds(1);
            
        }
        Debug.Log(tileWidth);
        Debug.Log(columns.Value);
        float supposedWidth = columns.Value * tileWidth; // * tilePixelX;
        float supposedHeight = rows.Value * tileHeight; // * tilePixelY;
        if (supposedWidth / screenWidth > 1 || supposedHeight / screenHeight > 1)
        {
            if (supposedWidth / screenWidth > supposedHeight / screenHeight)
            {
                limitingDimension = 1;
                limitCoefficientX = screenWidth / supposedWidth;
                limitCoefficientY = screenWidth / supposedWidth;
            }
            else
            {
                limitingDimension = 2;
                limitCoefficientX = screenHeight / supposedHeight;
                limitCoefficientY = screenHeight / supposedHeight;
            }
        }
        else if (supposedWidth / screenWidth < 1 || supposedHeight / screenHeight < 1)
        {
            if (supposedWidth / screenWidth > supposedHeight / screenHeight)
            {
                limitingDimension = 1;
                limitCoefficientX = screenWidth / supposedWidth;
                limitCoefficientY = screenWidth / supposedWidth;
            }
            else
            {
                limitingDimension = 2;
                limitCoefficientX = screenHeight / supposedHeight;
                limitCoefficientY = screenHeight / supposedHeight;
            }
        }
        /*if (supposedWidth / screenWidth > 1 || supposedWidth / screenWidth < 1)
        {
            limitCoefficientX = screenWidth / supposedWidth;
        }
        if (supposedHeight / screenHeight > 1 || supposedHeight / screenHeight < 1)
        {
            limitCoefficientY = screenHeight / supposedHeight;
        }*/
        limitCoefficientX *= 0.9f;
        limitCoefficientY *= 0.9f;
        /*if (limitingDimension == 1)
        {
            Camera.main.orthographicSize = Screen.height / 200f / limitCoefficientX;
        }
        else if (limitingDimension == 2)
        {
            Camera.main.orthographicSize = Screen.height / 200f / limitCoefficientY;
        }*/
        tileWidth *= limitCoefficientX;
        tileHeight *= limitCoefficientY;
        /*float mapWorldWidth = tileWidth * columns.Value;
        float mapWorldHeight = tileHeight * rows.Value;
        float screenAspect = (float)Screen.width / Screen.height;
        float requiredSizeX = mapWorldWidth / (2f * screenAspect);
        float requiredSizeY = mapWorldHeight / 2f;
        float cameraSize = Mathf.Max(requiredSizeY, requiredSizeX);
        Camera.main.orthographicSize = cameraSize;*/

        tile.transform.localScale = new Vector3(tileWidth, tileHeight, 1);

        float x = 0;
        Debug.Log(x);
        float y = 0;
        Vector2 gameSpace = Camera.main.ScreenToWorldPoint(new Vector2(x, y));
        x = gameSpace.x;
        y = gameSpace.y;
        x += (screenWidth - (tileWidth * columns.Value)) / 2; //tileWidth * tilePixelX;
        x += tileWidth / 2;
        y += (screenHeight - (tileHeight * rows.Value)) / 2; //tileHeight * tilePixelY;
        y += tileHeight / 2;
        Debug.Log(x);
        /*x -= screenWidth / 2;
        y -= screenHeight / 2;*/
        Debug.Log(tileWidth * tilePixelX * gameWidth / screenWidth);
        Debug.Log(rows.Value);
        if (mode == 0) {
            int count = 0;
            for (int i = 0; i < rows.Value; i++)
            {
                for (int j = 0; j < columns.Value; j++)
                {
                    count += 1;
                    if (IsHost)
                    {
                        //GRID.Add(UnityEngine.Random.Range(1, 9 + 1));
                        GRID.Add(TerrainGeneration(count - 1, count));
                        /*if (GRID.Count % 2 == 0) {
                            GRID.Add(3);
                        } else {
                            GRID.Add(4);
                        }*/
                        smokeLifespans.Add(0);
                        smokeOwners.Add(0);
                        //GRID.Add(5);
                        //GRID.Add(1);
                        //GRID.Add(4);
                        /*if (RandomChance(3)) {
                            GRID.Add(5);
                        } else {
                            GRID.Add(4);
                        }*/
                    }
                    smokes.Add(-1);
                    traps.Add(new List<List<float>>());
                    visionGrid.Add(0);
                }
            }
            
            if (MBAmount > GRID.Count)
            {
                MBAmount = GRID.Count;
            }
            if (IsHost)
            {
                yield return StartCoroutine(Caves());
                yield return StartCoroutine(MilitaryBases(1));
            }
        } else if (mode == 1) {
            if (true) {
                AddToGrid(61, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 5);
                AddToGrid(66, 2);
                AddToGrid(2, 1);
                AddToGrid(56, 2);
                AddToGrid(2, 3);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(2, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(10, 2);
                AddToGrid(1, 4);
                AddToGrid(43, 2);
                AddToGrid(2, 3);
                AddToGrid(7, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 5);
                AddToGrid(10, 2);
                AddToGrid(1, 4);
                AddToGrid(16, 2);
                AddToGrid(2, 2);
                AddToGrid(26, 2);
                AddToGrid(1, 3);
                AddToGrid(7, 1);
                AddToGrid(1, 5);
                AddToGrid(13, 2);
                AddToGrid(1, 4);
                AddToGrid(15, 2);
                AddToGrid(3, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(23, 2);
                AddToGrid(8, 1);
                AddToGrid(14, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(14, 2);
                AddToGrid(4, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(24, 2);
                AddToGrid(3, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(15, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(13, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(3, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(15, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(6, 2);
                AddToGrid(2, 1);
                AddToGrid(17, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(12, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(13, 2);
                AddToGrid(2, 5);
                AddToGrid(3, 2);
                AddToGrid(2, 5);
                AddToGrid(7, 2);
                AddToGrid(2,5);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(12, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(11, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 5);
                AddToGrid(2, 1);
                AddToGrid(15, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(12, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(3, 1);
                AddToGrid(8, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(2, 1);
                AddToGrid(16, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(20, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(8, 2);
                AddToGrid(1, 1);
                AddToGrid(6, 5);
                AddToGrid(2, 1);
                AddToGrid(10, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 2);
                AddToGrid(1, 1);
                AddToGrid(8, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(17, 2);
                AddToGrid(1, 3);
                AddToGrid(5, 5);
                AddToGrid(2, 1);
                AddToGrid(5, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(5, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(2, 1);
                AddToGrid(7, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(17, 2);
                AddToGrid(1, 1);
                AddToGrid(6, 5);
                AddToGrid(1, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(9, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(3, 2);
                AddToGrid(4, 5);
                AddToGrid(4, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(18, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(6, 2);
                AddToGrid(10, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(6, 2);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(5, 2);
                AddToGrid(1, 5);
                AddToGrid(20, 2);
                AddToGrid(3, 1);
                AddToGrid(8, 2);
                AddToGrid(1, 3);
                AddToGrid(9, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 1);
                AddToGrid(4, 2);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(4, 5);
                AddToGrid(5, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(20, 2);
                AddToGrid(1, 1);
                AddToGrid(10, 2);
                AddToGrid(2, 3);
                AddToGrid(8, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 3);
                AddToGrid(4, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(3, 4);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(24, 2);
                AddToGrid(1, 5);
                AddToGrid(12, 2);
                AddToGrid(2, 3);
                AddToGrid(2, 1);
                AddToGrid(3, 2);
                AddToGrid(6, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 4);
                AddToGrid(3, 3);
                AddToGrid(2, 4);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(19, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(16, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 2);
                AddToGrid(4, 1);
                AddToGrid(4, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 4);
                AddToGrid(5, 3);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(2, 1);
                AddToGrid(15, 2);
                AddToGrid(2, 1);
                AddToGrid(14, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(3, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(5, 3);
                AddToGrid(1, 1);
                AddToGrid(8, 3);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(3, 2);
                AddToGrid(1, 1);
                AddToGrid(14, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(2, 2);
                AddToGrid(1, 5);
                AddToGrid(11, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 3);
                AddToGrid(2, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 3);
                AddToGrid(3, 1);
                AddToGrid(3, 3);
                AddToGrid(3, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(5, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(13, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 3);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(13, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 3);
                AddToGrid(1, 5);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(5, 1);
                AddToGrid(3, 3);
                AddToGrid(3, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(3, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(2, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 1);
                AddToGrid(11, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 5);
                AddToGrid(11, 2);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 1);
                AddToGrid(1, 2);
                AddToGrid(3, 1);
                AddToGrid(3, 5);
                AddToGrid(3, 1);
                AddToGrid(3, 3);
                AddToGrid(2, 5);
                AddToGrid(2, 1);
                AddToGrid(4, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(12, 2);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 5);
                AddToGrid(14, 2);
                AddToGrid(1, 5);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(3, 1);
                AddToGrid(1, 5);
                AddToGrid(4, 1);
                AddToGrid(2, 5);
                AddToGrid(2, 1);
                AddToGrid(14, 2);
                AddToGrid(1, 4);
                AddToGrid(2, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 5);
                AddToGrid(9, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 2);
                AddToGrid(3, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(5, 5);
                AddToGrid(5, 1);
                AddToGrid(3, 5);
                AddToGrid(8, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(9, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 4);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(8, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 2);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(7, 1);
                AddToGrid(3, 5);
                AddToGrid(6, 1);
                AddToGrid(4, 5);
                AddToGrid(2, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 3);
                AddToGrid(7, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 4);
                AddToGrid(6, 5);
                AddToGrid(1, 2);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(8, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 2);
                AddToGrid(2, 5);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(7, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(2, 1);
                AddToGrid(2, 5);
                AddToGrid(4, 1);
                AddToGrid(4, 5);
                AddToGrid(2, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 4);
                AddToGrid(1, 3);
                AddToGrid(2, 1);
                AddToGrid(4, 2);
                AddToGrid(4, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(3, 2);
                AddToGrid(2, 5);
                AddToGrid(12, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 2);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(2, 1);
                AddToGrid(4, 5);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(2, 1);
                AddToGrid(4, 5);
                AddToGrid(1, 1);
                AddToGrid(3, 5);
                AddToGrid(4, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 2);
                AddToGrid(2, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(14, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 2);
                AddToGrid(2, 5);
                AddToGrid(3, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(2, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(15, 5);
                AddToGrid(7, 1);
                AddToGrid(3, 5);
                AddToGrid(3, 1);
                AddToGrid(12, 2);
                AddToGrid(1, 4);
                AddToGrid(3, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 4);
                AddToGrid(7, 2);
                AddToGrid(2, 3);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(2, 1);
                AddToGrid(3, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 5);
                AddToGrid(3, 1);
                AddToGrid(12, 5);
                AddToGrid(1, 1);
                AddToGrid(6, 5);
                AddToGrid(1, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 4);
                AddToGrid(1, 2);
                AddToGrid(1, 4);
                AddToGrid(2, 3);
                AddToGrid(1, 4);
                AddToGrid(2, 2);
                AddToGrid(2, 4);
                AddToGrid(13, 2);
                AddToGrid(2, 3);
                AddToGrid(4, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(3, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(10, 5);
                AddToGrid(1, 1);
                AddToGrid(3, 5);
                AddToGrid(2, 1);
                AddToGrid(14, 2);
                AddToGrid(3, 4);
                AddToGrid(1, 3);
                AddToGrid(19, 2);
                AddToGrid(1, 4);
                AddToGrid(5, 2);
                AddToGrid(1, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(3, 1);
                AddToGrid(8, 5);
                AddToGrid(2, 1);
                AddToGrid(4, 2);
                AddToGrid(1, 1);
                AddToGrid(9, 2);
                AddToGrid(1, 4);
                AddToGrid(1, 2);
                AddToGrid(1, 3);
                AddToGrid(1, 4);
                AddToGrid(1, 2);
                AddToGrid(4, 4);
                AddToGrid(1, 3);
                AddToGrid(20, 2);
                AddToGrid(1, 4);
                AddToGrid(6, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(2, 1);
                AddToGrid(1, 5);
                AddToGrid(1, 1);
                AddToGrid(2, 2);
                AddToGrid(3, 1);
                AddToGrid(17, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 2);
                AddToGrid(1, 3);
                AddToGrid(2, 2);
                AddToGrid(3, 4);
                AddToGrid(1, 3);
                AddToGrid(27, 2);
                AddToGrid(1, 1);
                AddToGrid(4, 5);
                AddToGrid(2, 2);
                AddToGrid(1, 1);
                AddToGrid(26, 2);
                AddToGrid(1, 3);
                AddToGrid(33, 2);
                AddToGrid(1, 1);
                AddToGrid(2, 5);
                AddToGrid(1, 1);
                AddToGrid(19, 2);
            }
            if (MBAmount > GRID.Count)
            {
                MBAmount = GRID.Count;
            }
            if (IsHost)
            {
                yield return StartCoroutine(Buildings());
                yield return StartCoroutine(Caves());
                yield return StartCoroutine(MilitaryBases(2));
            }
        }
        Debug.Log("rows = " + rows.Value);
        Debug.Log("columns = " + columns.Value);
        for (int i = 0; i < rows.Value; i++) {
            for (int j = 0; j < columns.Value; j++) {
                GRIDX.Add(x);
                GRIDY.Add(y);
                x += tileWidth;
            }
            gameSpace = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
            x = gameSpace.x;
            x += (screenWidth - (tileWidth * columns.Value)) / 2; //tileWidth * tilePixelX;
            x += tileWidth / 2;
            y += tileHeight;
        }
        StartCoroutine(UpdateIsSmoked(smokes));
    }
    void AddToGrid(int times, int terrain) {
        for (int i = 0; i < times; i++) {
            if (IsHost) {
                GRID.Add(terrain);
                smokeLifespans.Add(0);
                smokeOwners.Add(0);
                
            }
            smokes.Add(-1);
            traps.Add(new List<List<float>>());
            visionGrid.Add(0);
        }
        if (!(terrain >= 1 && terrain <= 9)) {
            Debug.Log("terrain glitch with " + times + " times! Terrain: " + terrain);
        }
        //Debug.Log("GRID Count = " + GRID.Count);
    }
    int TerrainGeneration(int index, int num)
    {
        int returnVal = 1;
        //if reworking chances, you may need to change it twice
        if (((num - 1) % columns.Value + 1) == 1)
        {
            if (num > columns.Value)
            {
                bool waterEligible = false;
                if (num % columns.Value == 0)
                {
                    if (GRID[index - columns.Value] != 3)
                    {
                        waterEligible = true;
                    }
                }
                else
                {
                    if (GRID[index - columns.Value + 1] != 3 && GRID[index - columns.Value] != 3)
                    {
                        waterEligible = true;
                    }
                }
                
                if (waterEligible)
                {
                    if (GRID[index - columns.Value] == 2)
                    {
                        if (RandomChance(2))
                        {
                            returnVal = 2;
                        }
                    }
                    else
                    {
                        if (RandomChance(20))
                        {
                            returnVal = 2;
                        }
                    }
                }
                if (RandomChance(3))
                {
                    returnVal = 5;
                }
                if (RandomChance(20))
                {
                    returnVal = 6;
                }
                if (RandomChance(50))
                {
                    returnVal = 7;
                }
                if (GRID[index - columns.Value] == 4)
                {
                    returnVal = 3;
                }
                else
                {
                    bool hillEligible = false;
                    if (num % columns.Value == 0)
                    {
                        if (GRID[index - columns.Value] != 2)
                        {
                            hillEligible = true;
                        }
                    }
                    else
                    {
                        if (GRID[index - columns.Value + 1] != 2 && GRID[index - columns.Value] != 2)
                        {
                            hillEligible = true;
                        }
                    }
                    
                    if (hillEligible)
                    {
                        if (GRID[index - columns.Value] == 3)
                        {
                            if (RandomChance(3))
                            {
                                returnVal = 3;
                            }
                        }
                        else
                        {
                            if (RandomChance(25))
                            {
                                returnVal = 3;
                            }
                        }
                    }
                }
                if (GRID[index - columns.Value] == 3 || GRID[index - columns.Value] == 4)
                {
                    if (RandomChance(2))
                    {
                        returnVal = 4;
                    }
                }
            }
            else
            {
                if (RandomChance(20))
                {
                    returnVal = 2;
                }
                if (RandomChance(3))
                {
                    returnVal = 5;
                }
                if (RandomChance(20))
                {
                    returnVal = 6;
                }
                if (RandomChance(50))
                {
                    returnVal = 7;
                }
                if (RandomChance(25))
                {
                    returnVal = 3;
                }
                if (RandomChance(50))
                {
                    returnVal = 4;
                }
            }
        }
        else
        {
            if (num > columns.Value)
            {
                bool waterEligible = false;
                if (num % columns.Value == 0)
                {
                    if (GRID[index - columns.Value - 1] != 3 && GRID[index - 1] != 3 && GRID[index - columns.Value] != 3)
                    {
                        waterEligible = true;
                    }
                }
                else
                {
                    if (GRID[index - columns.Value + 1] != 3 && GRID[index - columns.Value - 1] != 3 && GRID[index - 1] != 3 && GRID[index - columns.Value] != 3)
                    {
                        waterEligible = true;
                    }
                }
                if (waterEligible)
                {
                    if (GRID[index - 1] == 2 || GRID[index - columns.Value] == 2)
                    {
                        if (RandomChance(2))
                        {
                            returnVal = 2;
                        }
                    }
                    else
                    {
                        if (RandomChance(20))
                        {
                            returnVal = 2;
                        }
                    }
                }
                if (RandomChance(3))
                {
                    returnVal = 5;
                }
                if (RandomChance(20))
                {
                    returnVal = 6;
                }
                if (RandomChance(50))
                {
                    returnVal = 7;
                }
                if (GRID[index - 1] == 4 || GRID[index - columns.Value] == 4)
                {
                    returnVal = 3;
                }
                else
                {
                    bool hillEligible = false;
                    if (num % columns.Value == 0)
                    {
                        if (GRID[index - columns.Value - 1] != 2 && GRID[index - 1] != 2 && GRID[index - columns.Value] != 2)
                        {
                            hillEligible = true;
                        }
                    }
                    else
                    {
                        if (GRID[index - columns.Value + 1] != 2 && GRID[index - columns.Value - 1] != 2 && GRID[index - 1] != 2 && GRID[index - columns.Value] != 2)
                        {
                            hillEligible = true;
                        }
                    }
                    
                    if (hillEligible)
                    {
                        if (GRID[index - 1] == 3 || GRID[index - columns.Value] == 3)
                        {
                            if (RandomChance(3))
                            {
                                returnVal = 3;
                            }
                        }
                        else
                        {
                            if (RandomChance(25))
                            {
                                returnVal = 3;
                            }
                        }
                    }
                }
                if ((GRID[index - 1] == 3 || GRID[index - 1] == 4) && (GRID[index - columns.Value] == 3 || GRID[index - columns.Value] == 4))
                {
                    if (RandomChance(2))
                    {
                        returnVal = 4;
                    }
                }
            }
            else
            {
                if (GRID[index - 1] != 3)
                {
                    if (GRID[index - 1] == 2)
                    {
                        if (RandomChance(2))
                        {
                            returnVal = 2;
                        }
                    }
                    else
                    {
                        if (RandomChance(20))
                        {
                            returnVal = 2;
                        }
                    }
                }
                
                if (RandomChance(3))
                {
                    returnVal = 5;
                }
                if (RandomChance(20))
                {
                    returnVal = 6;
                }
                if (RandomChance(50))
                {
                    returnVal = 7;
                }
                if (GRID[index - 1] == 4)
                {
                    returnVal = 3;
                }
                if (GRID[index - 1] != 2)
                {
                    if (GRID[index - 1] == 3)
                    {
                        if (RandomChance(3))
                        {
                            returnVal = 3;
                        }
                    }
                    else
                    {
                        if (RandomChance(25))
                        {
                            returnVal = 3;
                        }
                    }
                    
                }
                if (GRID[index - 1] == 3 || GRID[index - 1] == 4)
                {
                    if (RandomChance(5))
                    {
                        returnVal = 4;
                    }
                }
            }
        }
        return returnVal;
    }
    bool RandomChance(int amount)
    {
        bool returnVal;
        if (UnityEngine.Random.Range(1, amount + 1) == 1)
        {
            returnVal = true;
        }
        else
        {
            returnVal = false;
        }
        return returnVal;
    }
    public bool RandomChanceFloat(float chance) {
        return (UnityEngine.Random.value <= chance);
    }
    IEnumerator Buildings() {
        for (int i = 0; i < GRID.Count; i++) {
            if (!(GRID[i] == 2 || GRID[i] == 3 || GRID[i] == 4)) {
                int replaceVal = GRID[i];
                if (RandomChance(20))
                {
                    replaceVal = 6;
                }
                if (RandomChance(50))
                {
                    replaceVal = 7;
                }
                if (replaceVal != GRID[i]) {
                    GRID[i] = replaceVal;
                }
            }
        }
        yield return null;
    }
    IEnumerator Caves()
    {
        for (int i = 0; i < GRID.Count; i++)
        {
            if (GRID[i] == 3)
            {
                if (i + 1 <= GRID.Count - columns.Value)
                {
                    if (RandomChance(2))
                    {
                        if (GRID[i + columns.Value] == 3 || GRID[i + columns.Value] == 4)
                        {
                            if (i + 1 > columns.Value)
                            {
                                if (GRID[i - columns.Value] != 3 && GRID[i - columns.Value] != 4 && GRID[i - columns.Value] != 9)
                                {
                                    GRID[i] = 9;
                                }
                            }
                            else
                            {
                                if (RandomChance(2))
                                {
                                    GRID[i] = 9;
                                }
                            }
                        }
                    }
                } 
                else
                {
                    //delete this whole part if you don't want caves on the ceiling
                    if (RandomChance(4))
                    {
                        if (i + 1 > columns.Value)
                        {
                            if (GRID[i - columns.Value] != 3 && GRID[i - columns.Value] != 4 && GRID[i - columns.Value] != 9)
                            {
                                GRID[i] = 9;
                            }
                        }
                        else
                        {
                            GRID[i] = 9;
                        }
                    }
                }
            }
        }
        yield return null;
    }
    IEnumerator MilitaryBases(int mode)
    {
        if (MBAmount == 1)
        {
            MB.Add(UnityEngine.Random.Range(1, GRID.Count + 1));
        }
        else
        {
            bool done = false;
            while (done == false)
            {
                done = MBAttempt(mode);
            }
        }
        for (int i = 0; i < MB.Count; i++)
        {
            GRID[MB[i] - 1] = 8;
        }
        yield return null;
    }
    bool MBAttempt(int mode)
    {
        bool success = false; 
        List<Vector2> MBVectors = new List<Vector2>();
        bool keepGoing = true;
        MB = new List<int>{};
        for (int i = 0; i < MBAmount; i++)
        {
            if (keepGoing)
            {
                int randomPosition = 0;
                bool samePosition = true;
                while (samePosition)
                {
                    randomPosition = UnityEngine.Random.Range(1, GRID.Count + 1);
                    bool diffPosition = true;
                    for (int j = 0; j < MB.Count; j++)
                    {
                        if (randomPosition == MB[j])
                        {
                            diffPosition = false;
                        }
                    }
                    if (diffPosition)
                    {
                        samePosition = false;
                    }
                }
                Vector2 myVector = GetCoordsFromIndex(randomPosition);
                MBVectors.Add(myVector);
                MB.Add(randomPosition);
                if (GRID[randomPosition - 1] == 2 || GRID[randomPosition - 1] == 3 || GRID[randomPosition - 1] == 4) {
                    keepGoing = false;
                }
                for (int k = 0; k < MBVectors.Count; k++)
                {
                    if (keepGoing)
                    {
                        if (i != k)
                        {
                            Vector2 otherVector = MBVectors[k];
                            int distance = (int)Math.Abs(myVector.x - otherVector.x) + (int)Math.Abs(myVector.y - otherVector.y);
                            float minDist = columns.Value / (MBAmount - 1) + rows.Value / (MBAmount - 1);
                            minDist -= 1;
                            if (mode == 1) {
                                if (distance < minDist)
                                {
                                    keepGoing = false;
                                }
                            } else if (mode == 2) {
                                if (distance < minDist) {
                                    keepGoing = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }
        if (keepGoing)
        {
            success = true;
        }
        return success;
    }
    public Vector2 GetCoordsFromIndex(int num)
    {
        //num is 1+
        int x = ((num - 1) % columns.Value) + 1;
        float yDiv = (num - 1) / columns.Value;
        int y = (int)Mathf.FloorToInt(yDiv);
        if (num > 0)
        {
            y += 1;
        }
        //Debug.Log(yDiv);
        //Debug.Log((int)Mathf.FloorToInt(yDiv));
        return new Vector2(x, y); //returns (1+, 1+)
    }
    public int GetIndexFromWorldCoords(Vector2 worldCoords)
    {
        //return 1+
        float leftX = GRIDX[0] - (tileWidth / 2);
        float rightX = GRIDX[GRIDX.Count - 1] + (tileWidth / 2);
        float bottomY = GRIDY[0] - (tileWidth / 2);
        float topY = GRIDY[GRIDY.Count - 1] + (tileWidth / 2);
        bool returnBool = true;
        if (worldCoords.x >= leftX && worldCoords.x <= rightX && worldCoords.y >= bottomY && worldCoords.y <= topY)
        {
            returnBool = true;
        }
        else
        {
            returnBool = false;
        }
        int x = (int)Math.Ceiling((((worldCoords.x - leftX) / (Math.Abs(rightX - leftX))) * columns.Value));
        int y = (int)Math.Ceiling(((worldCoords.y - bottomY) / (Math.Abs(topY - bottomY))) * rows.Value);
        if (returnBool)
        {
            int num = GetIndexFromCoords(new Vector2(x, y));
            return num;
        }
        else
        {
            return 0;
        }
    }
    int GetIndexFromCoords(Vector2 coords) //coords is (1+, 1+)
    {
        int x = Mathf.RoundToInt(coords.x); //1+
        int y = Mathf.RoundToInt(coords.y); //1+
        /*if (x > 0) {
            Debug.Log("x is " + x);
        }
        if (y > 0) {
            Debug.Log("y is " + y);
        }*/
        int num = x + ((y - 1) * columns.Value);
        return num; //num is 1+
    }
    void UpdateHealthText(int hp, int maxHp) {
        healthText.GetComponent<TextMeshProUGUI>().text = "Health: " + hp + "/" + maxHp;
    }
    public IEnumerator CreateMessageText(String txt, float startY, float endY, int mode, bool bypass4) {
        if (!(LocalScript.gamemode == 4 && !bypass4)) {
            GameObject MT = Instantiate(messageText, CANVAS);
            if (mode == 2) {
                StartCoroutine(WaitToDestroyMT(MT, 1));
            }
            MT.GetComponent<TMPro.TextMeshProUGUI>().text = txt;
            if (mode == 3) {
                MT.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(0, 1, 0, 1);
            }
            if (mode == 4) {
                MT.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 0, 0, 1);
            }
            if (mode == 3 || mode == 4) {
                MT.GetComponent<TMPro.TextMeshProUGUI>().fontSize += 50f;
            }
            Vector2 rt = MT.GetComponent<RectTransform>().anchoredPosition;
            rt.y = startY;
            MT.GetComponent<RectTransform>().anchoredPosition = rt;
            Color colour = MT.GetComponent<TextMeshProUGUI>().color;
            colour.a = 0f;
            MT.GetComponent<TextMeshProUGUI>().color = colour;
            float step = (endY - startY) / 51f;
            float changeY = endY - startY;
            float totalTime = 1.5f;
            if (!(mode == 3 || mode == 4)) {
                float timer = 0f;
                while (timer < totalTime) {
                    timer += Time.deltaTime; //might delay if MT is false AKA null AKA destroyed
                    if (MT) {
                        float progress = Mathf.Clamp01(timer / totalTime);
                        //yield return StartCoroutine(SetGhost(MT, progress));
                        rt = MT.GetComponent<RectTransform>().anchoredPosition;
                        rt.y = startY + changeY * progress;
                        MT.GetComponent<RectTransform>().anchoredPosition = rt;
                        colour = MT.GetComponent<TextMeshProUGUI>().color;
                        colour.a = progress;
                        MT.GetComponent<TextMeshProUGUI>().color = colour;
                        yield return null;
                    } //else break; //might not delay it if MT is false
                }

                /*for (int i = 0; i < 51; i++) {
                    if (MT) {
                        rt = MT.GetComponent<RectTransform>().anchoredPosition;
                        rt.y += step;
                        MT.GetComponent<RectTransform>().anchoredPosition = rt;
                        colour = MT.GetComponent<TextMeshProUGUI>().color;
                        colour.a += 1f / 51f;
                        MT.GetComponent<TextMeshProUGUI>().color = colour;
                        yield return new WaitForSeconds(0.02f);
                    }
                }*/
            }
            if (mode == 1) { 
                yield return new WaitForSeconds(1f);
                float timer = 0f;
                while (timer < totalTime) {
                    timer += Time.deltaTime;
                    float progress = Mathf.Clamp01(timer / totalTime);
                    //yield return StartCoroutine(SetGhost(MT, progress));
                    colour = MT.GetComponent<TextMeshProUGUI>().color;
                    colour.a = 1f - progress;
                    MT.GetComponent<TextMeshProUGUI>().color = colour;
                    yield return null;
                }
                /*for (int i = 0; i < 51; i++) {
                    colour = MT.GetComponent<TextMeshProUGUI>().color;
                    colour.a -= 1f / 51f;
                    MT.GetComponent<TextMeshProUGUI>().color = colour;
                    yield return new WaitForSeconds(0.02f);
                }*/
                Destroy(MT);
            } else if (mode == 2) {
            } else if (mode == 3 || mode == 4) {
                colour = MT.GetComponent<TextMeshProUGUI>().color;
                colour.a = 1f;
                MT.GetComponent<TextMeshProUGUI>().color = colour;
                rt = MT.GetComponent<RectTransform>().anchoredPosition;
                Vector2 ogrt = rt;
                while (true) {
                    rt = ogrt;
                    rt.x += UnityEngine.Random.Range(-100f, 100f);
                    rt.y += UnityEngine.Random.Range(-100f, 100f);
                    MT.GetComponent<RectTransform>().anchoredPosition = rt;
                    yield return null;
                }
                
            }
        }
        yield return null;
    }
    IEnumerator WaitToDestroyMT(GameObject MT, int type) {
        while (!(Mouse.current.leftButton.wasPressedThisFrame && mouseIndex != 0)) {
            yield return null;
        }
        Destroy(MT);
        yield return null;
    }
    public void DisplayBotTest(int mode, List<bool> arrivable, List<int> myVG, List<List<float>> dataLists, List<float> weightedList) {
        ClearTestFolder();
        if (mode == 1) {
            for (int i = 0; i < arrivable.Count; i++) {
                if (arrivable[i]) {
                    Instantiate(testIcon, new Vector2(GRIDX[i], GRIDY[i]), Quaternion.identity, testFolder.transform);
                }
            }
        } else if (mode == 2) {
            for (int i = 0; i < myVG.Count; i++) {
                if (myVG[i] == 1) {
                    Instantiate(testIcon, new Vector2(GRIDX[i], GRIDY[i]), Quaternion.identity, testFolder.transform);
                }
            }
        } else if (mode == 3) {
            for (int i = 0; i < dataLists[0].Count; i++) {
                float r = dataLists[0][i];
                float g = dataLists[1][i];
                float b = dataLists[2][i];
                GameObject TI = Instantiate(testIcon, new Vector2(GRIDX[i], GRIDY[i]), Quaternion.identity, testFolder.transform);
                SpriteRenderer SR = TI.GetComponent<SpriteRenderer>();
                SR.sprite = testIconCostumes[1];
                SR.color = new Color(r, g, b, 0.5f);
                TI.transform.localScale = new Vector3(tileWidth, tileHeight, 1);

                /*SR.color = new Color(1, 0, 0, r);
                TI.transform.localScale = new Vector3(tileWidth, tileHeight, 1);

                TI = Instantiate(testIcon, new Vector2(GRIDX[i], GRIDY[i]), Quaternion.identity, testFolder.transform);
                SR = TI.GetComponent<SpriteRenderer>();
                SR.sprite = testIconCostumes[1];
                SR.color = new Color(0, 1, 0, g);
                TI.transform.localScale = new Vector3(tileWidth, tileHeight, 1);

                TI = Instantiate(testIcon, new Vector2(GRIDX[i], GRIDY[i]), Quaternion.identity, testFolder.transform);
                SR = TI.GetComponent<SpriteRenderer>();
                SR.sprite = testIconCostumes[1];
                SR.color = new Color(0, 0, 1, b);
                TI.transform.localScale = new Vector3(tileWidth, tileHeight, 1);*/
            }
        } else if (mode == 4) {
            for (int i = 0; i < weightedList.Count; i++) {
                GameObject TI = Instantiate(testIcon, new Vector2(GRIDX[i], GRIDY[i]), Quaternion.identity, testFolder.transform);
                SpriteRenderer SR = TI.GetComponent<SpriteRenderer>();
                SR.sprite = testIconCostumes[1];
                SR.color = new Color(0, 0, 0, (weightedList[i] + 1) / 2);
                TI.transform.localScale = new Vector3(tileWidth, tileHeight, 1);
            }
        }
        
    }
    public void ClearTestFolder() {
        foreach (Transform child in testFolder.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}