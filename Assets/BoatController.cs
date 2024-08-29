using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField]
    Rigidbody orb;
    [SerializeField]
    Transform deck;
    [SerializeField]
    Rigidbody wheel;

    [SerializeField]
    private float boatForwardSpeed = 1;
    [SerializeField]
    private float boatTurnPower = 10;
    [SerializeField]
    private float wheelVisualSpeed = 50;
    Vector3 orbOffset;
    Vector3 torque;

    // Start is called before the first frame update
    void Start()
    {
        orbOffset = orb.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            torque += Vector3.right * boatTurnPower;
            Debug.Log($"PRESSING A {torque}");

        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            torque += Vector3.left * boatTurnPower;

            Debug.Log($"PRESSING D { torque}");
        }


    }

    private void FixedUpdate()
    {

        deck.Rotate(Vector3.right, orb.velocity.x);
        wheel.AddTorque(orb.velocity.x * Vector3.forward * wheelVisualSpeed,ForceMode.Acceleration);


        orb.AddForce((boatForwardSpeed * -Vector3.forward + torque) * Time.fixedDeltaTime);
        transform.position = orb.transform.position - orbOffset;
        torque = Vector3.zero;
    }





}
