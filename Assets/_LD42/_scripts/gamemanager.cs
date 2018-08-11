using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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



	}
	
	// Update is called once per frame
	void Update () {
		
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
        Debug.Log(string.Format("Next Level!"));

    }

    public void Death() {
        Debug.Log(string.Format("You Dead!"));

    }



}
