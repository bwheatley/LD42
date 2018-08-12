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
    public Node[,] grid;
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
    [HideInInspector]
    public int playerLives = 10;
    [HideInInspector]
    public int defaultLives = 10;
    public GameObject enemyHolder;
    public List<GameObject> enemies;

    //START UI STUFF
    public TextMeshProUGUI uiLevel;
    public TextMeshProUGUI uiTimeLeft;
    public TextMeshProUGUI uiLivesLeft;

    //Death Stuff
    public GameObject uiDeadPanel;
    public TextMeshProUGUI uiDeadLevelText;
    public Button uiDeadButton;
    public GameObject uiNextLevelText;

    //END UI STUFF

    public GameObject[] levels;
    [HideInInspector] public float knockbackforce = 5f;


    private Vector3 startPostion = new Vector3(0,0,0);

    public GameObject player;
    public bool dead = false;
    private bool nextLevel = false;
    public bool lockMovement = false;
    float currCountdownValue;

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

        //var _size = levels[currentLevel - 1].transform.GetChild(0).GetComponent<Tilemap>().cellBounds.size;
        //grid = new Node[_size.x,_size.y];

        //CheckMap(levels[currentLevel-1].transform.GetChild(0).GetComponent<Tilemap>());
        //CheckMap(levels[currentLevel - 1].transform.GetChild(1).GetComponent<Tilemap>());
        //CheckMap(levels[currentLevel - 1].transform.GetChild(2).GetComponent<Tilemap>());
        //CheckMap(levels[currentLevel - 1].transform.GetChild(3).GetComponent<Tilemap>());

        //GenerateNodes(levels[currentLevel - 1].transform.GetChild(0).GetComponent<Tilemap>().size);

        //Load Initial level
        SetLevelText(currentLevel);
        DisableAllLevels();
        LoadLevel(currentLevel-1);

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

        //GetTile(player.transform.position);

	}

    public Vector3Int GetTile(Vector3 position) {

        GridLayout gridLayout = levels[currentLevel - 1].GetComponent<GridLayout>();
        Vector3Int cellPosition = gridLayout.WorldToCell(position);

        //var _bg = levels[currentLevel - 1].transform.GetChild(0).GetComponent<Tilemap>()
        //    .GetTile(player.transform.position);
        Debug.Log(string.Format("Cell Position {0}", cellPosition));

        return cellPosition;
    }

    public PathFinding.Int2 GetTileInt2(Vector3 position)
    {

        GridLayout gridLayout = levels[currentLevel - 1].GetComponent<GridLayout>();
        Vector3Int cellPosition = gridLayout.WorldToCell(position);


        Debug.Log(string.Format("Cell Position {0}", cellPosition));

        PathFinding.Int2 pos = new PathFinding.Int2(cellPosition.x, cellPosition.y);

        return pos;
    }

    public void SetLevelText(int level) {
        uiLevel.text = string.Format("Level: {0}", level);
    }

    public void SetPlayerLives(int lives) {
        Debug.Log(string.Format("Lives to {0}", lives));
        playerLives = lives;
        uiLivesLeft.text = string.Format("Lives: {0:N0}", lives);
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

    public void GenerateNodes(Vector3Int size) {
        for (int x = 0; x <= size.x; x++)
        {
            for (int y = 0; y <= size.y; y++)
            {
                Debug.Log(string.Format("X {0} Y {1}", x, y));
                //grid[x, y] = new Node(walkable, worldPoint, x, y, _reachableIndex);
                var _worldPos = new PathFinding.Int2(x, y);
                grid[x,y] = new Node(true, _worldPos, x, y);


            }
        }

    }


    public void CheckMap(Tilemap tilemap) {
        Debug.Log(string.Format("Tilemap {0} - Size: {1}  ", tilemap.cellBounds, tilemap.cellBounds.size));
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

    public void ResetColliders() {
        //collide = 0;
        //newCollide = 0;
    }

    
    public IEnumerator StartCountdown(float countdownValue = 10)
    {
        currCountdownValue = countdownValue;
        while (currCountdownValue > 0)
        {
            currCountdownValue -= Time.deltaTime;
            Debug.Log("Countdown: " + currCountdownValue);
            //yield return new WaitForSeconds(1.0f);
            yield return null;
            //currCountdownValue--;
        }
    }

    public void NextLevel() {
        StartCoroutine(NextLevelIE());
    }

    public IEnumerator NextLevelIE() {
        if (currentLevel == maxlevel) {
            //TODO LAST LEVEL DO SOMETHING NICE
        }

        Debug.Log(string.Format("Next Level!"));

        UIShowHideUIElement(uiNextLevelText);
        var _animator = uiNextLevelText.GetComponent<Animator>();
        _animator.Play("next_level_text_animation", -1, 0f);

        StartCoroutine(StartCountdown(1));

        while (currCountdownValue > 0) {
            //Wait
            yield return null;
        }

        UIShowHideUIElement(uiNextLevelText);
        ResetPlayePos();
        IncrementTimer(timeLeft);
        currentLevel++;

        //GenerateNodes(levels[currentLevel - 1].transform.GetChild(0).GetComponent<Tilemap>().size);
        SetLevelText(currentLevel);
        LoadLevel(currentLevel);

        SetLockMovement(false);
        nextLevel = false;

        yield return null;
    }

 /// <summary>
    /// Load level needs to turn off the old grid map and on the new one
    /// </summary>
    /// <param name="level"></param>
    public void LoadLevel(int level) {
        
        //Turn off current level
        if (level >= 2) {
            Debug.Log(string.Format("Load Level {0} Current Level: {1}", levels[currentLevel - 1].name, levels[currentLevel - 2].name));
            levels[currentLevel - 2].SetActive(false);
        }

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

    public void IncrementTimer(float timetoAdd) {
        timeLeft = timeDefault + timetoAdd;
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void RetryLevel() {
        SetPlayerLives(playerLives-1);
        if (playerLives == 0) {
            RestartGame();
        }

        ResetPlayePos();
        dead = false;
        ResetTimer();
        UIShowHideDeathPanel();
    }

    public void DisableAllLevels() {
        for (int i = 0; i < maxlevel; i++) {
            levels[i].SetActive(false);
        }
    }

    public void RestartGame() {


        DisableAllLevels();
        ResetPlayePos();
        currentLevel = 1;
        playerLives = defaultLives;
        dead = false;
        LoadLevel(1);

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

    public void UIShowHideUIElement(GameObject _gameObject) {
        if (_gameObject.activeSelf)
        {
            //_myCamera.LockCamera(false);
            _gameObject.SetActive(false);
        }
        else
        {
            //_myCamera.LockCamera(true);
            _gameObject.SetActive(true);
        }
    }

    public void SetLockMovement(bool setting) {
        lockMovement = setting;
    }
}
