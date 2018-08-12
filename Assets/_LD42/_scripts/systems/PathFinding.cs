using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;

public class PathFinding : MonoBehaviour
{

    public struct Int2 {

        [SerializeField] public int x;
        [SerializeField] public int y;

        public Int2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(Int2 a, Int2 b) {
            return (a.x == b.x && a.y == b.y);
        }

        public static bool operator !=(Int2 a, Int2 b) {
            return (a.x != b.x || a.y != b.y);
        }

        public override bool Equals(System.Object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            Int2 i = (Int2) obj;
            return (x == i.x && y == i.y);
        }
    }

    public static bool PathFindingDebug = false;

    PathRequestManager requestManager;
    Grid grid;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }


    public void StartFindPath(Int2 startPos, Int2 targetPos, bool walkableOverride)
    {
        StartCoroutine(FindPath(startPos, targetPos, walkableOverride));
    }

    public Node NodeFromWorldPoint(PathFinding.Int2 worldPosition)
    {
        //return GameData.grid[worldPosition.x, worldPosition.y];
        return new Node(); //THIS IS NOT RIGHT JUST PLACEHOLDER
    }

    IEnumerator FindPath(Int2 startPos, Int2 targetPos, bool walkableOverRide)
    {
        int _cellCounter = 0;
        bool _breakme = false;
        int _clickcount = 0;
        bool multiZoneWalk = true;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Int2[] waypoints = new Int2[0];
        bool pathSuccess = false;

        Node startNode = NodeFromWorldPoint(startPos);
        Node targetNode = NodeFromWorldPoint(targetPos);
        startNode.parent = startNode;

        if (PathFindingDebug)
        {
            Debug.Log(string.Format("Start Node {0},{1} | StartPos {2} Target Node {3},{4} | TargetPos {5}", startNode.gridX, startNode.gridY, startPos, targetNode.gridX, targetNode.gridY, targetPos));
            Debug.Log(string.Format("Start Node Walkable{0}, TargetNode Walkable {1} Multizonewalk {2} Walkableoverride {3}", startNode.walkable, targetNode.walkable, multiZoneWalk, walkableOverRide));
        }

        //I remembered the startnode check because wherever you are you should be able to get out.
        if ((targetNode.walkable && multiZoneWalk) || walkableOverRide == true)
        {
            //Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            //TODO fix this we added placeholder
            Heap<Node> openSet = new Heap<Node>(100);

            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0 && !_breakme)
            {
                //Debug.Log( string.Format( "OpenSet has {0} items", openSet.Count ) );
                Node currentNode = openSet.RemoveFirst();                                       //Pull out the first step and put it into the next set
                closedSet.Add(currentNode);                                                     //Add current node into the closed set

                if (PathFindingDebug)
                {
                    //Debug.Log( string.Format( "Current HEX {0},{1}", currentNode.gridX, currentNode.gridY ) );
                }

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }
                else if (_cellCounter > 10000)
                {
                    _cellCounter++;
                    _breakme = true;
                }
                else if (_cellCounter < 90000)
                {
                    _cellCounter++;
                    _clickcount++;
                    if (_clickcount == 1000)
                    {
                        //Debug.Log( string.Format( "Counter {0}", _cellCounter ) );
                        _clickcount = 0;
                    }
                }

                //This one should only run if the path is not walkable and we get really close?
                if (!targetNode.walkable && (currentNode.hCost == 14 || currentNode.hCost == 10) && currentNode.walkable)
                {
                    if (PathFindingDebug)
                    {
                        //Debug.Log( String.Format( "GCost {0} HCost {1} FCost {2} - Current Node {3}", currentNode.gCost, currentNode.hCost, currentNode.fCost, currentNode.worldPosition ) );
                    }
                    sw.Stop();
                    if (PathFindingDebug)
                    {
                        //Debug.Log( string.Format( "Closest Path found: {0} ms", sw.ElapsedMilliseconds ) );
                    }
                    pathSuccess = true;
                    targetNode = currentNode;
                    break;
                }

                //TODO fix this LOOP
                //foreach (Node neighbour in grid.GetNeighbors(currentNode))
                //{
                //    if (PathFindingDebug)
                //    {
                //        //Debug.Log( string.Format( "Get Neighbors - Current Neighbor X{0}|Y{1}", neighbour.gridX, neighbour.gridY ) );
                //    }

                //    if (!neighbour.walkable || closedSet.Contains(neighbour))
                //    {
                //        if (PathFindingDebug)
                //        {
                //            //Debug.Log( string.Format( "Neighbor Not Walkable or ClosedSet Contains neighbor X{0}|Y{1}!", neighbour.gridX, neighbour.gridY ) );
                //        }
                //        continue;
                //    }

                //    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + TurningCost(currentNode, neighbour);
                //    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                //    {
                //        neighbour.gCost = newMovementCostToNeighbour;
                //        neighbour.hCost = GetDistance(neighbour, targetNode);
                //        neighbour.parent = currentNode;

                //        if (!openSet.Contains(neighbour))
                //            openSet.Add(neighbour);
                //        else
                //            openSet.UpdateItem(neighbour);
                //    }
                //}
            }
            yield return null;

        }
        else
        {
            Debug.Log("Path not walkable");
        }

        yield return null;

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        else
        {
            Debug.Log("Path Not found!");
        }

        if (PathFindingDebug)
        {
            //Debug.Log( "WayPoint Array Print out " );
            //foreach ( Int2 myint in waypoints ) {
            //    Debug.Log( string.Format( "Array {0}", myint ) );
            //}
            //Debug.Log( "End PRINTOUT" );
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess, walkableOverRide);

    }

    int TurningCost(Node from, Node to)
    {
        return 0;
        Vector2 dirOld = new Vector2(from.gridX - from.parent.gridX, from.gridY - from.parent.gridY);
        Vector2 dirNew = new Vector2(to.gridX - from.gridX, to.gridY - from.gridY);
        if (dirNew == dirOld)
            return 0;
        else if (dirOld.x != 0 && dirOld.y != 0 && dirNew.x != 0 && dirNew.y != 0)
        {
            return 5;
        }
        else
        {
            return 10;
        }
    }

    Int2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            if (PathFindingDebug)
            {
                Debug.Log(String.Format("Retrace - CurrentNode X{0}Y{1} StartNode X{2}Y{3} - CurrentNode Parent X{4}Y{5}", currentNode.gridX, currentNode.gridY, startNode.gridX, startNode.gridY, currentNode.parent.gridX, currentNode.parent.gridY));
            }

            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        if (PathFindingDebug)
        {
            Debug.Log(string.Format("Path Done Points Contained {0}", path.Count));
        }

        Int2[] waypoints = SimplifyPath(path);
        if (PathFindingDebug)
        {
            Debug.Log(string.Format("Waypoints Count {0}", waypoints.Length));
        }
        Array.Reverse(waypoints);
        return waypoints;

    }

    Int2[] SimplifyPath_new(List<Node> path)
    {
        List<Int2> waypoints = new List<Int2>();
        Int2 directionOld = new Int2();
        directionOld.x = 0;
        directionOld.y = 0;
        //A single hex needs a special case
        if (path.Count == 1)
        {
            if (PathFindingDebug)
            {
                Debug.Log(string.Format("Path X{0}|Y{1} - WorldPos! {2}", path[0].gridX, path[0].gridY, path[0].worldPosition));
            }
            waypoints.Add(path[0].worldPosition);
        }

        for (int i = 1; i < path.Count; i++)
        {
            Int2 directionNew = new Int2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            //Int2 directionNew = new Int2( path[i+1].gridX - path[i].gridX, path[i+1].gridY - path[i].gridY );
            if (PathFindingDebug)
            {
                Debug.Log(string.Format("Path Worldposition {0}", path[i].worldPosition));
            }
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }


    Int2[] SimplifyPath(List<Node> path)
    {
        List<Int2> waypoints = new List<Int2>();
        Int2 directionOld = new Int2();
        directionOld.x = 0;
        directionOld.y = 0;
        //A single hex needs a special case
        if (path.Count == 1)
        {
            if (PathFindingDebug)
            {
                Debug.Log(string.Format("Path X{0}|Y{1} - WorldPos! {2}", path[0].gridX, path[0].gridY, path[0].worldPosition));
            }
            waypoints.Add(path[0].worldPosition);
        }


        for (int i = 0; i < path.Count - 1; i++)
        {
            Int2 directionNew = new Int2(path[i + 1].gridX - path[i].gridX, path[i + 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    public static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
