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
    public int boatLaneSpawn;
    public Vector3 boatOffset;
    public BoatMaster boat;
    public BoatGridManager grid;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
