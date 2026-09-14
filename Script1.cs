using UnityEngine;
using System.Collections.Generic;

public class Script1 : MonoBehaviour
{
    public GameObject tile;
    float tileWidth = 3f;
    float tileHeight = 3f;
    float tilePixelX = 18f;
    float tilePixelY = 18f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera.main.orthographicSize = Screen.height / 200f;
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        float screenWidth = Screen.width; //topRight.x - bottomLeft.x;
        float screenHeight = Screen.height; //topRight.y - bottomLeft.y;
        float gameHeight = Camera.main.orthographicSize * 2;
        float gameWidth = gameHeight * Camera.main.aspect;
        float screenScaleX = screenWidth / gameWidth;
        float screenScaleY = screenHeight / gameHeight;
        Debug.Log(screenScaleY);
        Debug.Log(screenWidth);
        Debug.Log(Screen.width);
        int columns = Mathf.FloorToInt(screenWidth / (tileWidth * tilePixelX));
        int rows = Mathf.FloorToInt(screenHeight / (tileHeight * tilePixelY));
        List<int> GRID = new List<int>();
        List<float> GRIDX = new List<float>();
        List<float> GRIDY = new List<float>();
        float x = (screenWidth - (tileWidth * tilePixelX * columns)) / 2; //tileWidth * tilePixelX;
        x += tileWidth * tilePixelX / 2;
        Debug.Log(x);
        float y = (screenHeight - (tileHeight * tilePixelY * rows)) / 2; //tileHeight * tilePixelY;
        y += tileHeight * tilePixelY / 2;
        /*x -= screenWidth / 2;
        y -= screenHeight / 2;*/
        Debug.Log(tileWidth * tilePixelX * gameWidth / screenWidth);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GRID.Add(1);
                GRIDX.Add(x);
                GRIDY.Add(y);
                Vector3 spawnPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.nearClipPlane)); //new Vector3(x / screenScaleX, y / screenScaleY, Camera.main.nearClipPlane);
                spawnPos.z = 0;
                Instantiate(tile, spawnPos, Quaternion.identity);
                x += tileWidth * tilePixelX;
                
            }
            x = (screenWidth - (tileWidth * tilePixelX * columns)) / 2; //tileWidth * tilePixelX;
            x += tileWidth * tilePixelX / 2;
            //x -= screenWidth / 2;
            y += tileHeight * tilePixelY;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
