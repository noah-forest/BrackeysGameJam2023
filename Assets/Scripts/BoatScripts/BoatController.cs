using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles movement of boat
/// </summary>
public class BoatController : MonoBehaviour
{
    [SerializeField]
    public CharacterController characterController;
    //[SerializeField]
    //Transform deck;
    [SerializeField]
    Rigidbody wheel;
    [SerializeField] public BoatGridManager gridManager;
    [SerializeField] private float boatForwardSpeed = 1;
    [SerializeField] private float boatHorizontalSpeed = 50;
    [SerializeField] private float boatGravity = 9.8f;
    private int _currentLane = 0;
    public int CurrentLane
    {
        get { return _currentLane; }
        set 
        {
            if (gridManager && gridManager.boatLaneXPositions.Length > 0)
            {
                _currentLane = Mathf.Clamp(value, 0, gridManager.Width - 1);
                targetLaneX = gridManager.boatLaneXPositions[_currentLane];
            }
            else
            {
                _currentLane = 0;
                targetLaneX = 0;
            }
            
        }
    }
    public float targetLaneX;
    [SerializeField] float initialBounceForce = 15;
    [SerializeField] float bounceDecayRate = 5;
    float bounceForce;

    Vector3 moveVector;

    //used to try to maintain the positions of the boat and the rolling ball, which techincally move independantly
    Vector3 orbOffset;

    bool cantMove = false;
    // Start is called before the first frame update

    private void Start()
    {
        gridManager = BoatWorldManager.singleton.grid;
        CurrentLane = 0;
        //transform.position = gridManager.tileGrid[1, 0].transform.position;
    }
    public void MoveRight()
    {
        CurrentLane++;
    }

    public void MoveLeft()
    {
        CurrentLane--;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft();
        }
        else if(Input.GetKey(KeyCode.D))
        {
            
            MoveRight();
        }
        //Debug.Log($"Steering Angle =  {steeringAngle}");

    }

    private void FixedUpdate()
    {
        if (cantMove) return;
        moveVector = (transform.forward * (boatForwardSpeed + bounceForce));                            // initial Z movement 
        Vector3 distVect = (Vector3.right * transform.position.x - Vector3.right * targetLaneX);
        Vector3 xDir = distVect.normalized;
        moveVector += xDir * distVect.magnitude/boatForwardSpeed * boatHorizontalSpeed;                  // X movement
        //moveVector.z /= (Mathf.Abs(moveVector.x) * 0.5f) + 1;
        moveVector += Vector3.down * boatGravity;                                       // Y movement
        characterController.Move(moveVector * Time.fixedDeltaTime);
        bounceForce = Mathf.Max(0, bounceForce - bounceDecayRate * Time.fixedDeltaTime, 0);

    }

    public void StopMovement()
    {
        cantMove = true; 
    }

    public void Bounce()
    {
        bounceForce = initialBounceForce;
        if(CurrentLane == 0)
        {
            MoveRight();
        }else if(CurrentLane == gridManager.Width - 1)
        {
            MoveLeft();
        }
        else
        {
            if(Random.value >= 0.5)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }
        }
    }

}
