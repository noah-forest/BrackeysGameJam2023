using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatWorldManager : MonoBehaviour
{
    // Start is called before the first frame update

    #region singleton

    public static BoatWorldManager singleton;

    private void Awake()
    {
        if (singleton)
        {
            Destroy(this.gameObject);
            return;
        }

        singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    public Vector3 boatSpawn = new Vector3();
    public Vector3 boatOffset;
    public BoatMaster boat;
    public Camera cam;
    public Vector3 camBoatOffset;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        camBoatOffset = cam.transform.position - boatSpawn + boatOffset;
    }

    private void Update()
    {
        if(boat) cam.transform.position = boat.transform.position + camBoatOffset;
    }

}
