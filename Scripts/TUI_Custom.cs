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

        // Start is called before the first frame update
        void Start()
        {
            //m_StartHostButton.onClick.AddListener(StartHost);
            m_StartHostButton.onClick.AddListener(() => RelayScript.Instance.CreateRelay());
            m_StartHostButton.onClick.AddListener(() => DeactivateButtons());
            //m_StartClientButton.onClick.AddListener(StartClient);
            ClientAsync();
            m_PlayBotButton.onClick.AddListener(() => StartOfflineBotGame());
        }
        async void ClientAsync() {
            m_StartClientButton.onClick.AddListener(async () => {await ClientClick();});
        }
        async Task ClientClick() {
            if (!clientClicked) {
                if (await RelayScript.Instance.JoinRelay(joinCodeInput.text)) {
                    clientClicked = true;
                    DeactivateButtons();
                }
            }
        }
        void StartClient()
        {
            
            RelayScript.Instance.JoinRelay(joinCodeInput.text);
            DeactivateButtons();
        }

        void StartHost()
        {
            RelayScript.Instance.CreateRelay();
            //DeactivateButtons();
        }

        private void StartOfflineBotGame() {
            var transport = Unity.Netcode.NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                // "127.0.0.1" is the universal computer code for "myself" (localhost).
                // This ensures absolutely zero data leaves your network card.
                transport.SetConnectionData("127.0.0.1", 7777); 
            }

            // 3. Start the host session locally. No Relay server required!
            Unity.Netcode.NetworkManager.Singleton.StartHost();

            // 4. Clean up your UI menu just like you normally do
            DeactivateButtons();
            StartCoroutine(lScript.StartBots()); //this starts a one-player vs everyone else = bots game
        }

        void DeactivateButtons()
        {
            m_StartHostButton.interactable = false;
            m_StartClientButton.interactable = false;
            m_PlayBotButton.interactable = false;
            m_StartHostButton.gameObject.SetActive(false);
            m_StartClientButton.gameObject.SetActive(false);
            m_PlayBotButton.gameObject.SetActive(false);
        }
    }
}