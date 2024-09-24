using Assets.Scripts.BoatScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatHazard : MonoBehaviour, IBoatInteractable
{
    protected static BoatWorldManager boatManager;
    protected static AudioSource soundPlayer;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected AudioClip impactSound;
    [SerializeField] protected float impactVolume = 5;
    /// <summary>
    /// The higher this value, the more 'difficult' it will be for the path to go through this hazard, however 0 will make the tile impassible
    /// </summary>
    public float pathFindingInfluence = 1;
    


    private void Start()
    {
        if(!boatManager) boatManager = BoatWorldManager.singleton;
        if(!soundPlayer) soundPlayer = GameManager.singleton?.MusicPlayer;

    }
    public virtual void InteractWithBoat(BoatMaster boat, ControllerColliderHit hit)
    {
        //Debug.Log($"{boatManager} :: {boat}");
        if (!boatManager) boatManager = BoatWorldManager.singleton;
        boatManager.boatSpawn = boat.transform.position;
        boatManager.boatLaneSpawn = boat.controller.CurrentLane;
        //Debug.Log($"Boat impacted {gameObject.name}");
        if(impactEffect) Instantiate(impactEffect, transform.position, transform.rotation);
        soundPlayer?.PlayOneShot(impactSound, impactVolume);

    }
}
