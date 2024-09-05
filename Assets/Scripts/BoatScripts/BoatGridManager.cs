using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoatGridManager : MonoBehaviour
{
    [SerializeField] int width,height;
    public int Width
    {
        get{ return width; }
    }
    public int Height
    {
        get { return height; }
    }
    [SerializeField] float gridY = 0;
    [SerializeField] BoatWorldTile tilePrefab;
    [SerializeField] int emptyTileWeight = 3;
    [SerializeField] int[] difficultyThresholds = new int[4];
    [SerializeField] List<BoatHazard> hazardList;

    public BoatWorldTile[,] tileGrid;
    [HideInInspector] public float[] boatLaneXPositions = new float[3];

    float tileSize = 5;

    private void Start()
    {
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
                tileGrid[x,y].gridPosition = new Vector2 (x, y);
                boatLaneXPositions[x] = tileGrid[x, y].transform.position.x;
                if (y > 3) InstantiateHazard(ref tileGrid[x,y]);
            }
        }
    }

    void InstantiateHazard(ref BoatWorldTile tile)
    {
        int roll = Random.Range(0, hazardList.Count + emptyTileWeight);
        if (roll >= hazardList.Count) return;


        BoatHazard hazard = hazardList[roll];
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
    }
}
