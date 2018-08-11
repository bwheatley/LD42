using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemanager : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;
    public int[,] map;
    public Sprite baseHex;
    public static gamemanager instance;

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
		
        GenerateMap(mapWidth, mapHeight);


	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void GenerateMap(int width, int height, bool empty = false) {

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


}
