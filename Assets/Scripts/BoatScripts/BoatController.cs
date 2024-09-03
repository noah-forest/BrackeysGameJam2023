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
    public Rigidbody boatBody;
    //[SerializeField]
    //Transform deck;
    [SerializeField]
    Rigidbody wheel;

    [SerializeField] private float boatForwardSpeed = 1;
    [SerializeField] private float boatJetSpeed = 20;
    [SerializeField] private float boatSteerVisualSpeed = 50;
    [SerializeField] private float breakForce;
    [SerializeField] private float rotCorrectionRate = 1;
     
    [SerializeField] private float maxSteeringAngle;
    [SerializeField] private float turnAnglePerSec = 5;
    [SerializeField] private float bonusLowSpeedThreshold = 1;
    [SerializeField] private float bonusLowSpeedScalar = 0.5f;
    [SerializeField] private float steeringEffectOnSpeed = 50;
    float steeringAngle;

    [SerializeField] float bounceDecayRate = 10;
    float curBounceForce;

    [SerializeField] WheelCollider[] wheelColliders = new WheelCollider[4];

    //used to try to maintain the positions of the boat and the rolling ball, which techincally move independantly
    Vector3 orbOffset;
    Vector3 moveVector;
    bool cantMove = false;
    // Start is called before the first frame update

    private void Start()
    {
        foreach (var wheel in wheelColliders)
        {
            wheel.ConfigureVehicleSubsteps(5, 12, 15);
        }
    }
    public void MoveRight()
    {
        steeringAngle = Mathf.Min(steeringAngle + turnAnglePerSec * Time.deltaTime, maxSteeringAngle);
        
    }

    public void MoveLeft()
    {
        steeringAngle = Mathf.Max(steeringAngle - turnAnglePerSec * Time.deltaTime, -maxSteeringAngle);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            MoveRight();
        }
        else if(Input.GetKey(KeyCode.D))
        {
            MoveLeft();
        }
        //Debug.Log($"Steering Angle =  {steeringAngle}");

    }

    private void FixedUpdate()
    {
        if (cantMove) return;
        //deck.Rotate(Vector3.right, boatBody.velocity.x);
        //wheel.AddTorque(boatBody.velocity.x * Vector3.forward * wheelVisualSpeed,ForceMode.Acceleration);

        wheelColliders[2].motorTorque = -boatForwardSpeed + curBounceForce + steeringAngle * steeringEffectOnSpeed;
        wheelColliders[3].motorTorque = -boatForwardSpeed + curBounceForce + steeringAngle * steeringEffectOnSpeed;
        curBounceForce = Mathf.Lerp(curBounceForce, 0, Time.fixedDeltaTime * bounceDecayRate);

        float bonus = 1 + Mathf.Clamp(bonusLowSpeedScalar * (bonusLowSpeedThreshold-boatBody.velocity.magnitude), -.5f, 2f);
        wheelColliders[2].steerAngle = steeringAngle * bonus;
        wheelColliders[3].steerAngle = steeringAngle * bonus;
        Debug.Log(boatBody.velocity.magnitude);

        Vector3 boatxz = boatBody.velocity * 0.01f;
        boatxz.y = 0;
        boatBody.AddForce(new Vector3(0, -boatxz.magnitude, 0), ForceMode.VelocityChange);
        boatBody.AddForce((boatJetSpeed - steeringAngle)* Vector3.back);
        boatBody.MoveRotation(Quaternion.Lerp(boatBody.rotation, Quaternion.identity, Time.fixedDeltaTime * rotCorrectionRate));
        steeringAngle = Mathf.Lerp(steeringAngle, 0, Time.fixedDeltaTime * rotCorrectionRate);
    }

    public void StopMovement()
    {
        boatBody.isKinematic = true;
        boatBody.velocity = Vector3.zero;
        cantMove = true; 
    }

    public void Bounce(Vector3 normal)
    {
        steeringAngle *= -1;
        curBounceForce = boatBody.velocity.magnitude * 2;
        boatBody.velocity = Vector3.Reflect(boatBody.velocity, normal);
    }

}
