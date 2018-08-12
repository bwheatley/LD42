using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public bool movingToDest = false;
    public bool pathSuccessful = false;
    PathFinding.Int2[] path;
    bool PathFindingDebug = PathFinding.PathFindingDebug;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPathFound(PathFinding.Int2[] newPath, bool _pathSuccessful)
    {
        pathSuccessful = _pathSuccessful;
        if (_pathSuccessful)
        {
            if (PathFindingDebug)
            {
                Debug.Log(string.Format("Path Selected is {0}", newPath));
                foreach (PathFinding.Int2 _pathpart in newPath)
                {
                    Debug.Log(string.Format("Path Part X{0}|Y{1}", _pathpart.x, _pathpart.y));
                }
            }

            path = newPath;
            pathSuccessful = _pathSuccessful;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            this.movingToDest = false;
        }
    }

    public void TrackPlayer() {
        //Start = Enemy Position
        //End =  Players Position
        var posTile = gamemanager.instance.GetTile(transform.position);

        PathFinding.Int2 start = new PathFinding.Int2(posTile.x, posTile.y);

        var enPos = gamemanager.instance.GetTile(gamemanager.instance.player.transform.position);
        PathFinding.Int2 end = new PathFinding.Int2(enPos.x, enPos.y);
        bool overrideWalkable  = false;
        PathRequestManager.RequestPath(start, end, OnPathFound, overrideWalkable);
    }        


}
