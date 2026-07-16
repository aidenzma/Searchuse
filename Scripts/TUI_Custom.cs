using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem.UI;
#endif
using TMPro; 
using System.Threading.Tasks;
namespace Unity.Multiplayer.Center.NetcodeForGameObjectsExample
{
    /// <summary>
    /// A basic example of a UI to start a host or client.
    /// If you want to modify this Script please copy it into your own project and add it to your copied UI Prefab.
    /// </summary>
    public class TUI_Custom : MonoBehaviour
    {
        [SerializeField]
        Button m_StartHostButton;
        [SerializeField]
        Button m_StartClientButton;
        public TMP_InputField joinCodeInput;
        [SerializeField]
        Button m_PlayBotButton;
        [SerializeField]
        Button m_TrainBotButton;
        [SerializeField]
        GameObject image;
        [SerializeField]
        GameObject buttons;
        [SerializeField]
        GameObject typeCode;
        [SerializeField]
        GameObject XButton;
        [SerializeField]
        Sprite[] buttonCostumes;
        [SerializeField]
        GameObject mainMenuButton;
        public LocalScript lScript;
        bool clientClicked = false;
        void Awake()
        {
            if (!FindAnyObjectByType<EventSystem>())
            {
                var inputType = typeof(StandaloneInputModule);
#if ENABLE_INPUT_SYSTEM && NEW_INPUT_SYSTEM_INSTALLED
                inputType = typeof(InputSystemUIInputModule);                
#endif
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
                eventSystem.transform.SetParent(transform);
            }
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
            if (imageName == "X")
            {
                //go back to page
                ActivateButtons();
            }
        }
        void OnImageEnter(string imageName)
        {
            if (imageName == "X")
            {
                XButton.GetComponent<Image>().sprite = buttonCostumes[1];
            }
        }
        void OnImageExit(string imageName)
        {
            if (imageName == "X")
            {
                XButton.GetComponent<Image>().sprite = buttonCostumes[0];
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            if (LocalScript.gamemode == 4) {
                StartTrainingBot();
            } else {
                //m_StartHostButton.onClick.AddListener(StartHost);
                m_StartHostButton.onClick.AddListener(() => BeginHost(false));
                //m_StartClientButton.onClick.AddListener(StartClient);
                ClientAsync();
                m_PlayBotButton.onClick.AddListener(() => StartOfflineBotGame());
                m_TrainBotButton.onClick.AddListener(() => StartTrainingBot());
                AddClickListener(XButton, () => OnImageClicked("X"), () => OnImageEnter("X"), () => OnImageExit("X"));
            }
        }
        async void ClientAsync() {
            //m_StartClientButton.onClick.AddListener(async () => {await ClientClick();});
            m_StartClientButton.onClick.AddListener(StartClient);
            joinCodeInput.onSubmit.AddListener(async (_) => {await ClientClick(); });
        }
        async Task ClientClick() {
            if (!clientClicked) {
                if (await RelayScript.Instance.JoinRelay(joinCodeInput.text)) {
                    lScript.amSpectator = false;
                    LocalScript.gamemode = 2;
                    lScript.spectatorSet = true;
                    clientClicked = true;
                    
                    image.SetActive(false);
                    typeCode.SetActive(false);
                } else {
                    StartCoroutine(lScript.CreateMessageText("Code not found", -150, -150, 1, false));
                }
            }
        }
        void StartClient()
        {
            DeactivateButtons();
            typeCode.SetActive(true);
            XButton.GetComponent<Image>().sprite = buttonCostumes[0];
            XButton.SetActive(true);
        }

        void BeginHost(bool spec)
        {
            lScript.amSpectator = spec;
            LocalScript.gamemode = 1;
            lScript.spectatorSet = true;
            RelayScript.Instance.CreateRelay();
            DeactivateButtons();
            image.SetActive(false);
            mainMenuButton.SetActive(true);
        }

        private void StartOfflineBotGame() {
            var transport = Unity.Netcode.NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                // "127.0.0.1" is the universal computer code for "myself" (localhost).
                // This ensures absolutely zero data leaves your network card.
                transport.SetConnectionData("127.0.0.1", 7777); 
            }

            lScript.amSpectator = false;
            LocalScript.gamemode = 3;
            lScript.spectatorSet = true;
            // 3. Start the host session locally. No Relay server required!
            Unity.Netcode.NetworkManager.Singleton.StartHost();

            // 4. Clean up your UI menu just like you normally do
            DeactivateButtons();
            image.SetActive(false);
            StartCoroutine(lScript.StartBots()); //this starts a one-player vs everyone else = bots game
        }
        private void StartTrainingBot() {
            var transport = Unity.Netcode.NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                // "127.0.0.1" is the universal computer code for "myself" (localhost).
                // This ensures absolutely zero data leaves your network card.
                transport.SetConnectionData("127.0.0.1", 7777); 
            }

            lScript.amSpectator = true;
            LocalScript.gamemode = 4;
            lScript.spectatorSet = true;
            // 3. Start the host session locally. No Relay server required!
            Unity.Netcode.NetworkManager.Singleton.StartHost();

            // 4. Clean up your UI menu just like you normally do
            DeactivateButtons();
            image.SetActive(false);
            mainMenuButton.SetActive(true);
            StartCoroutine(lScript.StartBotTraining()); //this starts a one-player vs everyone else = bots game
        }
        void DeactivateButtons()
        {
            m_StartHostButton.interactable = false;
            m_StartClientButton.interactable = false;
            m_PlayBotButton.interactable = false;
            m_TrainBotButton.interactable = false;
            m_StartHostButton.gameObject.SetActive(false);
            m_StartClientButton.gameObject.SetActive(false);
            m_PlayBotButton.gameObject.SetActive(false);
            m_TrainBotButton.gameObject.SetActive(false);
            buttons.SetActive(false);
        }
        void ActivateButtons() {
            m_StartHostButton.interactable = true;
            m_StartClientButton.interactable = true;
            m_PlayBotButton.interactable = true;
            m_TrainBotButton.interactable = true;
            m_StartHostButton.gameObject.SetActive(true);
            m_StartClientButton.gameObject.SetActive(true);
            m_PlayBotButton.gameObject.SetActive(true);
            m_TrainBotButton.gameObject.SetActive(true);
            buttons.SetActive(true);
            XButton.SetActive(false);
            XButton.GetComponent<Image>().sprite = buttonCostumes[0];
            typeCode.SetActive(false);
        }
    }
}