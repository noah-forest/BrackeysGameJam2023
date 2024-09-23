using Assets.Scripts.BoatScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.BoatScripts.BoatPathFinder;

public class BoatWorldTile : MonoBehaviour
{
    public BoatHazard hazard;
    public Vector2Int gridPosition;
    public float pathWeight = 1;
    public MeshRenderer meshRenderer;
    public Material paintMaterial;
    public List<BoatWorldTile> neighbors = new List<BoatWorldTile> ();
    public const int F = 0, R = 1, B = 2, L = 3;

    public void Paint()
    {
        meshRenderer.material = paintMaterial;
    }
    public bool TryAddNeighbor(BoatWorldTile newNeighbor)
    {
        if (newNeighbor == null || newNeighbor.pathWeight == 0)
        {
            return false;
        }
        neighbors.Add(newNeighbor);
        return true;
    }
}
