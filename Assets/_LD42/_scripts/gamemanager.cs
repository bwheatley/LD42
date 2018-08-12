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
    public GameObject uiStartPanel;
    public GameObject uiWinPanel;
    public TextMeshProUGUI uiDeadLevelText;
    public Button uiDeadButton;
    public GameObject uiNextLevelText;

    //END UI STUFF

    public LayerMask hitLayers;
    public bool paused = true;
    public float minimumMoveRange = .75f;


    //Audio
    public AudioClip characterWalk;
    public AudioSource characterAudioSource;
    public AudioSource characterLevelDone;


    public bool playeMoving = false;
    public GameObject[] levels;
    public TileBase[] tiles;
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
        uiStartPanel.SetActive(true);
        SetLevelText(currentLevel);
        DisableAllLevels();
        LoadLevel(currentLevel-1);

        RandomizeBG(levels[currentLevel - 1]);

    }

    public void QuitGame() {
        Application.Quit();
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
	    if (!dead && !paused) {
	        Timer();
	    }

        //GetTile(player.transform.position);

	}

    public void PlayWalk(bool playSound) {

        switch (playSound) {
            case true:
                if (characterAudioSource.isPlaying == false)
                {
                    characterAudioSource.clip = characterWalk;
                    characterAudioSource.Play();
                }
                break;
            case false:
                characterAudioSource.Stop();
                if (characterAudioSource.isPlaying == true)
                {
                    characterAudioSource.clip = characterWalk;
                    characterAudioSource.Stop();
                }
                break;
        }


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

    public void RandomizeBG(GameObject level) {
        Debug.Log(string.Format("Randomize Level: {0}", level.name));

        var _tilemap = level.transform.GetChild(0).GetComponent<Tilemap>();
        var mapSize = _tilemap.cellBounds.size;

        //map = new int[mapSize.x,mapSize.y];

        GenerateMap(mapSize.x, mapSize.y, false);
        RenderMap(map, _tilemap, tiles[0]);
        ReDrawMap(map, _tilemap, tiles[0]);
        //PerlinNoise(map, _tilemap, tiles[1], seed);
    }

    public int GetRandomTile(int min, int max) {
        var _int = Random.Range(min, max);
        return _int;
    }

    public void GenerateMap(int width, int height, bool empty = true) {

        map = new int[width, height];

        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                //Debug.Log(string.Format("X {0} Y {1}", x, y));

                if (empty) {
                    map[x, y] = 0;
                }
                else {
                    map[x, y] = 1;
                }

            }
        }

    }

    public static float CalculateNoiseValue(PathFinding.Int2 pos, Vector2 offset, float scale)
    {
        float noiseX = Mathf.Abs((pos.x + offset.x) * scale);
        float noiseY = Mathf.Abs((pos.y + offset.y) * scale);

        //return Mathf.Max( 0, Noise.Generate( noiseX, noiseY ) );
        
        return Mathf.Max(0, Mathf.PerlinNoise(noiseX, noiseY));
        //Just remember we can change this to 1d noise later!
    }


    public void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
    {
        Debug.Log(string.Format("Render Map - W{0}H{1}", map.GetUpperBound(0), map.GetUpperBound(1)));


        //Clear in case we have leftovers, regenerations etc
        //tilemap.ClearAllTiles();

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile1 = tilemap.GetTile(position);
            if (tile1 != tile)
            {
                tilemap.SetTile(position, tile);
                //HandleReplaceTile(tilemap, tile1, position);
            }
        }

    }

    //Clear Empty Hex's
    public void ReDrawMap(int[,] map, Tilemap tilemap, TileBase tile)
    {

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile1 = tilemap.GetTile(position);
            tilemap.SetTile(position, tiles[GetRandomTile(0, 4)]);
            if (tile1 != tile)
            {
            }
        }
    }

    public void PerlinNoise(int[,] map, Tilemap tilemap, TileBase tile, float seed)
    {
        int newPoint;
        //Used to reduced the position of the Perlin point
        float reduction = 0.5f;

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(position.x, seed) - reduction) * tilemap.cellBounds.xMax);
            newPoint += (tilemap.cellBounds.yMax / 2);

            TileBase tile1 = tilemap.GetTile(position);

            //Make sure the noise starts near the halfway point of the height           
            for (int y = newPoint; y >= 0; y--)
            {
                if (newPoint > position.y) {
                    tilemap.SetTile(position, tile);
                }
            }


            if (tile1 != tile)
            {
                
            }
        }

        //Create the Perlin
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            

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
            WinGame();
            yield return null;
        }

        Debug.Log(string.Format("Next Level!"));

        paused = true;
        UIShowHideUIElement(uiNextLevelText);
        var _animator = uiNextLevelText.GetComponent<Animator>();
        _animator.Play("next_level_text_animation", -1, 0f);
        characterAudioSource.Stop();        //No sound walking if you're not walking
        characterLevelDone.Play();

        StartCoroutine(StartCountdown(.75f));

        ResetPlayePos();
        IncrementTimer(timeLeft);
        currentLevel++;

        //GenerateNodes(levels[currentLevel - 1].transform.GetChild(0).GetComponent<Tilemap>().size);
        SetLevelText(currentLevel);
        RandomizeBG(levels[currentLevel-1]);
        LoadLevel(currentLevel);

        while (currCountdownValue > 0)
        {
            //Wait
            yield return null;
        }

        UIShowHideUIElement(uiNextLevelText);
        SetLockMovement(false);
        nextLevel = false;
        paused = false;


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

    public void WinGame() {
        UIShowHideUIElement(uiWinPanel);

    }

    public void StartGame() {
        UIShowHideUIElement(uiStartPanel);
        paused = false;
    }

    public void SetLockMovement(bool setting) {
        lockMovement = setting;
    }
}
