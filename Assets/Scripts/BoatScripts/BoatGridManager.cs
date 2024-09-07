using Assets.Scripts.BoatScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI.Table;

public class BoatGridManager : MonoBehaviour
{
    [SerializeField] int width, height;
    public int Width
    {
        get { return width; }
    }
    public int Height
    {
        get { return height; }
    }
    [SerializeField] float gridY = 0;
    [SerializeField] BoatWorldTile tilePrefab;
    [SerializeField] int emptyTileWeight = 3;
    [SerializeField] int[] difficultyThresholds = new int[4];



    [Serializable]
    struct HazardEntry
    {
        public BoatHazard hazardPrefab;
        public float spawnWeight;
        public int minNumber;
        public int maxNumber;
    }
    [SerializeField] List<HazardEntry> hazardList;

    private BoatWorldTile[,] tileGrid;
    [HideInInspector] public float[] boatLaneXPositions = new float[3];

    BoatPathFinder pathFinder = new BoatPathFinder();
    public float tileSize { get; private set;}

    public bool logPathLogic;
    public bool paintSolution;

    private void Start()
    {
        tileSize = 5;
        if (tilePrefab)
        {
            tileGrid = new BoatWorldTile[width, height];
            boatLaneXPositions = new float[width];
            int threshInc = (int)((float)height * 0.20f);
            for (int i = 0; i < difficultyThresholds.Length; i++)
            {
                difficultyThresholds[i] = threshInc * i;
            }
            
            GenerateGrid();
            CreateValidPath();


        }
    }

    void GenerateGrid()
    {
        for(int x = 0; x < width; x++)
        {
            
            for (int y = 0; y < height; y++)
            {
                tileGrid[x,y] = Instantiate(tilePrefab, new Vector3(x*tileSize, gridY, -y*tileSize), Quaternion.identity,transform);
                tileGrid[x,y].name = $"Tile ({x},{y})";
                tileGrid[x,y].gridPosition = new Vector2Int (x, y);
                boatLaneXPositions[x] = tileGrid[x, y].transform.position.x;
                if (y > 3) InstantiateHazard(ref tileGrid[x,y]);
                Debug.Log($"{tileGrid[x, y].name} : weight :{tileGrid[x, y].weight} ");
            }
        }
    }

    void InstantiateHazard(ref BoatWorldTile tile)
    {
        int roll = UnityEngine.Random.Range(0, hazardList.Count + emptyTileWeight);
        if (roll >= hazardList.Count)
        {
            tile.weight = 1;
            return;
        }


        BoatHazard hazard = hazardList[roll].hazardPrefab;
        var collider = hazard.GetComponent<CapsuleCollider>();
        Vector3 yOffset = 0.8f * Vector3.up;//collider ? collider.height * 0.5 * Vector3.up : Vector3.zero;
        tile.hazard = Instantiate(hazard, tile.transform.position + yOffset, Quaternion.identity, transform);
        BattleHazard battle = tile.hazard.GetComponent<BattleHazard>();
        if (battle)
        {
            for (int i = difficultyThresholds.Length-1; i >= 0; i--)
            {
                int threshold = difficultyThresholds[i];
                if (tile.gridPosition.y >= threshold)
                {
                    battle.battleDifficulty = (BattleDifficulty)i;
                    break;
                }
            }
        }

        tile.weight = hazard.pathFindingInfluence;
    }

    void CreateValidPath()
    {
        pathFinder.SetDebug(logPathLogic);
        pathFinder.Initialize(this);
        pathFinder.Enter(1, 0, 1, height - 1);
        pathFinder.GeneratePath();
        if (pathFinder.IsDone())
        {
            Debug.Log($"[BOAT GRID][PATHFINDER] solution length: {pathFinder.solution.Count}");
            CleanSolution(pathFinder.solution, paintSolution);
        }
    }
    public BoatWorldTile GetTile(int x, int y)
    {
        if ((0 <= x) && (0 <= y) && (x < width) && (y < height))
        {
            return tileGrid[x,y];
        }
        else
        {
            // Maybe we should assert...naaah.
            return null;
        }
    }

    void CleanSolution(List<BoatWorldTile> solution, bool paintSolution = false)
    {
        foreach(BoatWorldTile tile in solution)
        {
            if(paintSolution) tile.Paint();
            if (tile.weight >= 5) Destroy(tile.hazard.gameObject);
        }
    }
}
