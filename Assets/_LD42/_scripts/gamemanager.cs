using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;
    public int[,] map;
    public GameObject mainCharacter;
    public static gamemanager instance;

    public int victoryCollider = 100;
    public int deathCollider = 1;


    public Tilemap tileMap;
    public Tile selectedTile;

    public int seed = 12345;

    public int currentLevel = 1;
    public int maxlevel = 20;
    public float timeLeft;
    public float timeDefault;


    //START UI STUFF
    public TextMeshProUGUI uiLevel;
    public TextMeshProUGUI uiTimeLeft;

    //Death Stuff
    public GameObject uiDeadPanel;
    public TextMeshProUGUI uiDeadLevelText;
    public Button uiDeadButton;

    //END UI STUFF

    public GameObject[] levels;
    [HideInInspector] public float knockbackforce = 2.5f;


    private Vector3 startPostion = new Vector3(0,0,0);

    public GameObject player;



    public bool dead = false;
    private bool nextLevel = false;
    public bool lockMovement = false;


    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Creating a new version of GameManager.");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("GameManager already exists.");
            Destroy(this.gameObject);
            return;
        }
    }

    // Use this for initialization
    void Start () {

        ////Init our map
        //GenerateMap(mapWidth, mapHeight);

        ////Perlin Noise it up
        //PerlinNoise(map, seed);

        ////Randomly set 0,0 and 10,10 just to see where they are to the map
        ////map[0, 0] = 1;
        ////map[9, 9] = 1;


        ////Render Map
        //RenderMap(map, tileMap, selectedTile);


        //V2 of the game
        //Debug.Log(string.Format("Load Level {0} Current Level: {1}", levels[currentLevel].name, levels[currentLevel - 1].name));



    }

    public void SetNextLevel() {
        nextLevel = true;
    }

    public bool GetNextLevel() {
        return nextLevel;
    }

    // Update is called once per frame
    void Update () {

        //Run timer
	    if (!dead) {
	        Timer();
	    }

	}

    public void SetLevelText(int level) {
        uiLevel.text = string.Format("Level: {0}", level);
    }

    public void GenerateMap(int width, int height, bool empty = true) {

        map = new int[width, height];

        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                Debug.Log(string.Format("X {0} Y {1}", x, y));

                if (empty) {
                    map[x, y] = 0;
                }
                else {
                    map[x, y] = 1;
                }

            }
        }

    }

    //Render Full tiles
    public void RenderMap(int[,] map, Tilemap tilemap, TileBase tile) {
        Debug.Log(string.Format("Render Map - W{0}H{1}", map.GetUpperBound(0), map.GetUpperBound(1) ));


        //Clear in case we have leftovers, regenerations etc
        tilemap.ClearAllTiles();

        //Loop through map
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                // 1 = Tile 0 = Nada
                if (map[x, y] == 1) {
                    Debug.Log(string.Format("X {0} Y {1} Set as 1", x,y ));
                    tilemap.SetTile(new Vector3Int(x,y,0), tile);
                }
            }
        }

    }

    //Clear Empty Hex's
    public void ReDrawMap(int[,] map, Tilemap tilemap, TileBase tile) {
        //Loop through map
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                // 1 = Tile 0 = Nada
                if (map[x, y] == 0)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    public void PerlinNoise(int[,] map, float seed)
    {
        int newPoint;
        //Used to reduced the position of the Perlin point
        float reduction = 0.5f;
        //Create the Perlin
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));

            //Make sure the noise starts near the halfway point of the height
            newPoint += (map.GetUpperBound(1) / 2);
            for (int y = newPoint; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }
    }

    public int GetColliderType(string colliderName)
    {

        switch (colliderName)
        {
        case "Foreground Tiles":
            //Debug.Log("Foreground Tile");
            return 0;
            break;
        case "Zombie":
        case "Deadly Tiles":
            //Debug.Log("Deadly Tile");
            return deathCollider;
            break;
        case "Victory Tiles":
            return victoryCollider;
            break;
        default:
            return -1;
                break;
        }

    }

    public void NextLevel() {
        if (currentLevel == maxlevel) {
            //TODO LAST LEVEL DO SOMETHING NICE
        }

        Debug.Log(string.Format("Next Level!"));
        ResetPlayePos();
        ResetTimer();
        currentLevel++;

        SetLevelText(currentLevel);
        LoadLevel(currentLevel);

        SetLockMovement(false);
        nextLevel = false;


    }

    /// <summary>
    /// Load level needs to turn off the old grid map and on the new one
    /// </summary>
    /// <param name="level"></param>
    public void LoadLevel(int level) {
        
        Debug.Log(string.Format("Load Level {0} Current Level: {1}", levels[currentLevel-1].name, levels[currentLevel-2].name));
        //Turn off current level
        levels[currentLevel - 2].SetActive(false);
        levels[currentLevel - 1].SetActive(true);
    }

    public void ResetPlayePos() {
        //Debug.Log(string.Format("Player Pos Start: {0} End: {1}", player.transform.position, startPostion));
        player.transform.position = startPostion;
    }

    public void Death() {
        Debug.Log(string.Format("You Dead!"));
        dead = true;

        //Set last level
        //TODO Add a Retry level button
        uiDeadLevelText.text = string.Format("Last Level Reached: {0:N0}", currentLevel);
            
        UIShowHideDeathPanel();
    }

    void Timer() {
        timeLeft -= Time.deltaTime;
        uiTimeLeft.text = string.Format("Time Left: {0:N0}", timeLeft);

        if (timeLeft <= 0)
        {
            Death();
        }

        return;
    }

    public void ResetTimer() {
        Debug.Log("Reset Timer");
        timeLeft = timeDefault;
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void ResetLevel() {

    }

    public void RestartGame() {
        ResetPlayePos();
        currentLevel = 1;
        dead = false;

        ResetTimer();
        UIShowHideDeathPanel();
    }

    public void UIShowHideDeathPanel() {

        if (uiDeadPanel.activeSelf)
        {
            //_myCamera.LockCamera(false);
            uiDeadPanel.SetActive(false);
        }
        else
        {
            //_myCamera.LockCamera(true);
            uiDeadPanel.SetActive(true);
        }
    }

    public void SetLockMovement(bool setting) {
        lockMovement = setting;
    }
}
