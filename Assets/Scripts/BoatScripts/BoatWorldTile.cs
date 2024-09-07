using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatWorldTile : MonoBehaviour
{
    public BoatHazard hazard;
    public Vector2Int gridPosition;
    public float weight = 1;
    public MeshRenderer meshRenderer;
    public Material paintMaterial;

    public void Paint()
    {
        meshRenderer.material = paintMaterial;
    }
}
