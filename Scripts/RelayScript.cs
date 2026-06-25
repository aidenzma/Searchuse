using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using System.Collections;
using TMPro;
using System.Threading.Tasks;


public class RelayScript : MonoBehaviour
{
    public static RelayScript Instance;
    public GameObject messageText;
    public RectTransform CANVAS;
    public GameObject joinCodeText;
    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // -------------------------
    // HOST
    // -------------------------
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code: " + joinCode);
            if (!joinCodeText) {
                StartCoroutine(CreateMessageText("Join Code: " + joinCode, 200, 200));
            }
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            /*var hostRelayData = Unity.Services.Relay.RelayServiceExtensions.ToRelayServerData(allocation, "dtls");
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(hostRelayData);*/
            /*UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            // Loop through endpoints to grab the standard DTLS server data
            string hostIp = allocation.RelayServer.IpV4;
            ushort port = (ushort)allocation.RelayServer.Port;
            foreach (var endpoint in allocation.ServerEndpoints)
            {
                if (endpoint.ConnectionType == "dtls")
                {
                    hostIp = endpoint.Host;
                    port = (ushort)endpoint.Port;
                }
            }

            // Pass everything to UnityTransport (The 7th argument 'false' means standard UDP)
            transport.SetRelayServerData(
                hostIp,
                port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData,
                false // false = UDP/DTLS (Best for desktop host)
            );*/


            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    // -------------------------
    // CLIENT
    // -------------------------
    public async Task<bool> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );
            /*var clientRelayData = Unity.Services.Relay.RelayServiceExtensions.ToRelayServerData(allocation, "wss");

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.UseWebSockets = true; 

            transport.SetRelayServerData(clientRelayData);*/


            NetworkManager.Singleton.StartClient();
            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return false;
        }
    }
    IEnumerator CreateMessageText(String txt, float startY, float endY) {
        GameObject MT = Instantiate(messageText, CANVAS);
        joinCodeText = MT;
        MT.GetComponent<TMPro.TextMeshProUGUI>().text = txt;
        Vector2 rt = MT.GetComponent<RectTransform>().anchoredPosition;
        rt.y = startY;
        MT.GetComponent<RectTransform>().anchoredPosition = rt;
        Color colour = MT.GetComponent<TextMeshProUGUI>().color;
        colour.a = 0f;
        MT.GetComponent<TextMeshProUGUI>().color = colour;
        float step = (endY - startY) / 51f;
        for (int i = 0; i < 51; i++) {
            rt = MT.GetComponent<RectTransform>().anchoredPosition;
            rt.y += step;
            MT.GetComponent<RectTransform>().anchoredPosition = rt;
            colour = MT.GetComponent<TextMeshProUGUI>().color;
            colour.a += 1f / 51f;
            MT.GetComponent<TextMeshProUGUI>().color = colour;
            yield return new WaitForSeconds(0.02f);
        }
        /*yield return new WaitForSeconds(1f);
        for (int i = 0; i < 51; i++) {
            colour = MT.GetComponent<TextMeshProUGUI>().color;
            colour.a -= 1f / 51f;
            MT.GetComponent<TextMeshProUGUI>().color = colour;
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(MT);*/
    }
}