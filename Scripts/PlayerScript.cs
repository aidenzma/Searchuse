using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    public NetworkVariable<int> playerNum = new NetworkVariable<int>(0);
    public NetworkVariable<int> position = new NetworkVariable<int>(0);
    public int botNum = 0;
    public int botPosition = 0;
    public NetworkVariable<bool> isBotNV = new NetworkVariable<bool>(true);
    public bool isBot = false; 
    public LocalScript scriptManagerScript;
    SpriteRenderer spriteRenderer;
    //public Sprite[] costumes;
    private float speed = 0.02f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        //isBot = false;
        if (IsOwner)
        {
            scriptManagerScript = GameObject.Find("ScriptManager").GetComponent<LocalScript>();
            while (playerNum.Value == 0 && botNum == 0)
            {
                yield return null;
            }
            if (isBot) {
                Debug.Log("HI I AM A BOT!");
            }
            Debug.Log("Data:");
            Debug.Log(playerNum.Value);
            if (isBot) {

            } else {
                scriptManagerScript.myPlayerNum = playerNum.Value;
            }
            Debug.Log(scriptManagerScript.myPlayerNum);
        }
    }
    public IEnumerator InitiateCostume(int mode, Sprite s)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (mode == 1)
        {
            spriteRenderer.sprite = s;
            spriteRenderer.sortingOrder = 1;
        }
        else if (mode == 2)
        {
            spriteRenderer.sprite = s;
            spriteRenderer.sortingOrder = 2;
        }
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
    }
    /*IEnumerator Move()
    {
        
        if (Keyboard.current.wKey.isPressed)
        {
            transform.position += new Vector3 (0, speed, 0);
        }
        if (Keyboard.current.sKey.isPressed)
        {
            transform.position += new Vector3 (0, -speed, 0);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            transform.position += new Vector3 (speed, 0, 0);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.position += new Vector3 (-speed, 0, 0);
        }
        yield return null; 
    }*/
}
