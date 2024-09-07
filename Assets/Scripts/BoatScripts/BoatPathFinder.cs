using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using Unity.VisualScripting;

namespace Assets.Scripts.BoatScripts
{
    public class BoatPathFinder
    {
        #region variables

        static bool debugPathGeneration = false;

        public void SetDebug(bool debug) { debugPathGeneration = debug; }


        public class SearchNode
        {
            public BoatWorldTile tile;
            public List<SearchNode> neighbors = new List<SearchNode>();

            public bool TryAddNeighbor(BoatWorldTile newNeighbor, Dictionary<BoatWorldTile, SearchNode> nodeMap)
            {
                String logstring = newNeighbor ? $"neighbor weight: {newNeighbor.weight}" : "neighbor is null" ;
                if(debugPathGeneration) Debug.Log($"[PATHER][SNODE] {tile.name} trying to add neighbor {newNeighbor} => {logstring}");
                if (newNeighbor == null || newNeighbor.weight == 0)
                {
                    if(debugPathGeneration) Debug.Log($"[PATHER][SNODE] {tile.name} ======= FAILED to add neighbor {newNeighbor}");

                    return false;
                }
                neighbors.Add(nodeMap[newNeighbor]);
                return true;
            }
        }

        public class PlannerNode
        {
            public SearchNode searchNode;
            public PlannerNode parentNode;
            /// <summary>
            /// hueristic cost. Based on distance to goal
            /// </summary>
            public float hCost; 
            /// <summary>
            /// Based on tile count and wieght
            /// </summary>
            public float gCost;
            /// <summary>
            /// total cost based on h and g costs
            /// </summary>
            public float fCost;

            public PlannerNode(SearchNode myNode, PlannerNode myParent, SearchNode goal, float hWeight)
            {
                searchNode = myNode;
                parentNode = myParent;
                hCost = GetDistanceTo(goal);
            }

            public float GetNewCost(PlannerNode newParent, float distBetweenTiles)
            {
                return gCost + distBetweenTiles * searchNode.tile.weight;
            }

            float GetDistanceTo(SearchNode node)
            {
                return Vector3.Distance(node.tile.gameObject.transform.position, searchNode.tile.gameObject.transform.position);
            }

        }

        struct CompareNodes : IComparer<PlannerNode>
        {
            public int Compare(PlannerNode x, PlannerNode y)
            {
                if (x.fCost == y.fCost)
                {
                    return 0;
                }
                else
                {
                    return x.fCost > y.fCost ? -1 : 1;
                }
            }

        }

        //no pointers may cause issues.
        Dictionary<BoatWorldTile, SearchNode> nodes = new Dictionary<BoatWorldTile, SearchNode>();
        Dictionary<SearchNode, PlannerNode> visited = new Dictionary<SearchNode, PlannerNode>();

        const float heuristicWeight = 1.2f;

        BoatGridManager tileMap;
        float distanceBetweenTiles;
        PriorityQueue<PlannerNode, float> aStarQ = new PriorityQueue<PlannerNode, float>();

        SearchNode goalNode;
        SearchNode startNode;

        /// <summary>
        /// this is a list, in order, of nodes in the completed path that connects the start tile to the goal tile
        /// </summary>
        public List<BoatWorldTile> solution = new List<BoatWorldTile>();
        bool invalidStartEndNodes = false;
        bool solutionFound = false;


        //these variables are used to proccess nodes

        PlannerNode curPathNode;
        bool isDupe;
        //aka NextNode;
        PlannerNode successor;
        float newGCost;

        #endregion variables

        #region Methods

        public void Initialize(BoatGridManager _tileMap)
        {
            if(debugPathGeneration) Debug.Log($"[PATHER][INIT] Initializing Pather on tilemap {_tileMap.name}");

            tileMap = _tileMap;

            //populate tile : node dictionary 
            if(debugPathGeneration) Debug.Log($"[PATHER][INIT] Generating searchNodes for each tile");

            distanceBetweenTiles = tileMap.tileSize;
            for(int x = 0; x < tileMap.Width; x++)
            {
                for(int y = 0; y < tileMap.Height; y++)
                {
                    BoatWorldTile tile = tileMap.GetTile(x,y);
                    SearchNode sNode = new SearchNode();
                    sNode.tile = tile;
                    nodes.Add(tile, sNode);
                }
            }
            if(debugPathGeneration) Debug.Log($"[PATHER][INIT] trying to assign neighbors to all tiles");

            //assign neighbors to all search nodes
            foreach (KeyValuePair<BoatWorldTile,SearchNode> pair in nodes)
            {
                if(debugPathGeneration) Debug.Log($"[PATHER][INIT] {pair.Key.name} ----- ADDING NEIGHBORS ----");

                int x = pair.Key.gridPosition.x;
                int y = pair.Key.gridPosition.y;
                BoatWorldTile neighborTile = tileMap.GetTile(x + 1, y);
                pair.Value.TryAddNeighbor(neighborTile, nodes); // 1
                neighborTile = tileMap.GetTile(x - 1, y);
                pair.Value.TryAddNeighbor(neighborTile, nodes); // 2
                neighborTile = tileMap.GetTile(x, y + 1);
                pair.Value.TryAddNeighbor(neighborTile, nodes); // 3
                neighborTile = tileMap.GetTile(x, y - 1);
                pair.Value.TryAddNeighbor(neighborTile, nodes); // 4

                //hexagon tile stuff. Not used as we are using a sqaure grid
                //int offset = x % 2 == 1 ? 1 : -1;
                //neighborTile = tileMap.GetTile(x + 1, y + offset);
                //pair.Value.TryAddNeighbor(neighborTile, nodes);
                //neighborTile = tileMap.GetTile(x - 1, y + offset);
                //pair.Value.TryAddNeighbor(neighborTile, nodes);
                if(debugPathGeneration) Debug.Log($"[PATHER][INIT] {pair.Key.name} ----- neighbors complete ----");

            }
        }
        public void Enter(int startX, int startY, int goalX, int goalY)
        {
            if(debugPathGeneration) Debug.Log($"[PATHER][ENTER] Setting start({startX},{startY}) and goal({goalX},{goalY}) nodes");

            startNode = nodes[tileMap.GetTile(startX, startY)];
            goalNode = nodes[tileMap.GetTile(goalX, goalY)];
            if(debugPathGeneration) Debug.Log($"[PATHER][ENTER] SET start({startNode.tile.name}) and goal({goalNode.tile.name}) tiles");

            PlannerNode pathStart = new PlannerNode(startNode,null, goalNode, heuristicWeight);
            if(debugPathGeneration) Debug.Log($"[PATHER][ENTER] Root Planner Node created: {pathStart.searchNode.tile.name}");

            if (startNode.tile.weight == 0 || goalNode.tile.weight == 0) invalidStartEndNodes = true; /// griuaghgirighghg pointers when they dont

            pathStart.fCost = pathStart.hCost;
            aStarQ.Enqueue(pathStart, pathStart.fCost);

        }
        public void GeneratePath()
        {
            if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Generating Path");
            while (aStarQ.Count > 0)
            {
                if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] LoopStart: aStarQ.count : {aStarQ.Count} --------------------------------------------------------------");
                curPathNode = aStarQ.Dequeue();
                if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] CurNode : {curPathNode.searchNode.tile} -> GoalNode: {goalNode.tile}");
                if (curPathNode.searchNode.tile.name == goalNode.tile.name)
                {
                    if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Goal Detected, exiting loop ");
                    break; // break if at goal.
                }

                if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] currentNode Neighbors: {curPathNode.searchNode.neighbors.Count}");
                //proccess all neighbors of current node
                foreach (SearchNode node in curPathNode.searchNode.neighbors)
                {
                    if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Looping through neighbors -----------------------------");
                    if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] CurrentNeighbor: {node.tile.name}");

                    isDupe = visited.ContainsKey(node);
                    String logstring = isDupe ? "dupe" : "uniuque";
                    if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Neighbor is {logstring}.");

                    successor = isDupe ? visited[node] : new PlannerNode(node, curPathNode, goalNode, heuristicWeight);
                    newGCost = successor.GetNewCost(curPathNode, distanceBetweenTiles);
                    if (isDupe) // already visited neighbor node
                    {
                        if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] newGcost: {newGCost} vs oldGcost: {successor.gCost}");
                        if (newGCost > successor.gCost)
                        {
                            if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Neighbors cost is worse than old version. Ignoring");
                            if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] NeighborLoop END          -----------------------------");
                            continue; // skip proccessing this dupe if the new cost is greater or equal to the old
                        }
                        if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Neighbors cost is BETTER than old version. Updating");
                        aStarQ.Remove(successor, out successor, out _); // otherwise, we remove the node and replace it with one with better cost values;
                        successor.gCost = newGCost;
                        successor.fCost = successor.gCost + successor.hCost;
                        successor.parentNode = curPathNode;
                        aStarQ.Enqueue(successor, successor.fCost); // Updated node is returned to Q
                    }
                    else // new neighbor node. Just calculate costs and add to visted and Q
                    {
                        if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Created new neighbor and assigned gcost: {newGCost}");
                        successor.gCost = newGCost;
                        successor.fCost = successor.gCost + successor.hCost;
                        visited.Add(node, successor);
                        aStarQ.Enqueue(successor, successor.fCost);
                    }
                    if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] NeighborLoop END          -----------------------------");

                }
                if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Loop--END: aStarQ.count : {aStarQ.Count} --------------------------------------------------------------");

            }
            if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] exited loop & building solution");

            //After break (When curnode = goal node)
            //BUILD SOLUTION
            for (PlannerNode pathNode = curPathNode; pathNode != null; pathNode = pathNode.parentNode)
            {
                solution.Add(pathNode.searchNode.tile);
            }
            if(debugPathGeneration) Debug.Log($"[PATHER][GENERATE] Solution.count: {solution.Count}");

            solutionFound = true;

        }
        public void Exit()
        {
            //clear arrays and reset bools etc
        }
        public void ShutDown()
        {
            //explode
        }

        public bool IsDone()
        {
            return aStarQ.Count == 0 || invalidStartEndNodes || solutionFound;
        } 

        #endregion Methods


    }
}
