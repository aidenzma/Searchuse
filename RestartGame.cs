using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class RestartGame : MonoBehaviour
{
    public LocalScript lScript;
    public void RstrtGm(bool stayMainMenu) {
        lScript.NetworkDisable();
        if (NetworkManager.Singleton != null) {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (stayMainMenu) {
            LocalScript.gamemode = 0;
        }
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
