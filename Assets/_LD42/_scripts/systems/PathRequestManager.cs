using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    PathFinding pathfinding;

    bool isProcessingPath;
    bool PathFindingDebug = PathFinding.PathFindingDebug;



    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathFinding>();
    }

    //void Start( ) {
    //    float _timer = .5f;
    //    InvokeRepeating( "TryProcessNext", _timer, _timer );
    //}

    public static void RequestPath(PathFinding.Int2 pathStart, PathFinding.Int2 pathEnd, Action<PathFinding.Int2[], bool> callback, bool walkableOverride)
    {
        if (pathStart != pathEnd)
        {
            Debug.Log(string.Format("Path Requested: Start {0} End {1}", pathStart, pathEnd));
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext(walkableOverride);
        }
        else
        {
            Debug.Log("You can not request a path for the same hex as you are in");
        }
    }

    /// <summary>
    /// If we pass in w/o a walkableoverride just call the main process with false set
    /// </summary>
    void TryProcessNext()
    {
        TryProcessNext(false);
    }

    void TryProcessNext(bool walkableOverride)
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            Debug.Log(string.Format("PathREQQueue Count {0}, START {1} END {2} walkableoverride {3}", pathRequestQueue.Count, currentPathRequest.pathStart, currentPathRequest.pathEnd, walkableOverride));
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, walkableOverride);
        }
    }

    public void FinishedProcessingPath(PathFinding.Int2[] path, bool success, bool walkableOverride)
    {
        if (PathFindingDebug)
        {
            Debug.Log(string.Format("Processing finished! Success {0} - WalkOverRide? {1}", success, walkableOverride));
            Debug.Log(string.Format("Path - {0} START! ", path));
            foreach (PathFinding.Int2 myint in path)
            {
                Debug.Log(string.Format("Array {0}", myint));
            }
            Debug.Log("End PRINTOUT");
        }

        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        if (walkableOverride)
        {

        }
        TryProcessNext(walkableOverride);
    }

    struct PathRequest
    {
        public PathFinding.Int2 pathStart;
        public PathFinding.Int2 pathEnd;
        public Action<PathFinding.Int2[], bool> callback;

        public PathRequest(PathFinding.Int2 _start, PathFinding.Int2 _end, Action<PathFinding.Int2[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }
}
