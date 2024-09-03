using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatObstacleGenerator : MonoBehaviour
{

    [SerializeField] List<GameObject> possibleObsticales;
    [SerializeField][Range(0,10)] int minObstacles;
    [SerializeField][Range(1,10)] int maxObstacles;
    [SerializeField] float minSpacing = 10;
    [SerializeField] BoxCollider boundingBox;
    [SerializeField] float verticalOffset;
    int obsticalesGenerated = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if(possibleObsticales.Count > 0) GenerateObsticales();
    }

    private void GenerateObsticales()
    {
        while(obsticalesGenerated < minObstacles && obsticalesGenerated < maxObstacles)
        {
            GameObject toGenerate = possibleObsticales[Random.Range(0,possibleObsticales.Count)];
            float xBound = Random.Range(boundingBox.bounds.min.x, boundingBox.bounds.max.x);
            float zBound = Random.Range(boundingBox.bounds.min.z, boundingBox.bounds.max.z);
            Vector3 spawnPoint = new Vector3(xBound, 0,zBound);

            RaycastHit hit; 
            Physics.Raycast(new Vector3(xBound,boundingBox.bounds.max.y,zBound),Vector3.down, out hit);
            if (hit.collider)
            {
                spawnPoint.y = hit.point.y + verticalOffset;
                toGenerate = Instantiate(toGenerate, spawnPoint, transform.rotation);
                toGenerate.transform.Rotate(Vector3.up, Random.Range(0, 180));
                toGenerate.transform.up = hit.normal;
                toGenerate.transform.RotateAround(toGenerate.transform.position, toGenerate.transform.up, toGenerate.transform.localEulerAngles.y);
            }
            obsticalesGenerated++; // incremented regardless of success to prevent infnite loop when placed in invalid locations
        }
    }
}
