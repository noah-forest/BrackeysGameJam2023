using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles movement of boat
/// </summary>
public class BoatController : MonoBehaviour
{
    [SerializeField]
    public Rigidbody orb;
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

    //used to try to maintain the positions of the boat and the rolling ball, which techincally move independantly
    Vector3 orbOffset;
    Vector3 turnForce;
    bool cantMove = false;
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
            turnForce += Vector3.right * boatTurnPower;
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            turnForce += Vector3.left * boatTurnPower;
        }


    }

    private void FixedUpdate()
    {
        if (cantMove) return;
        deck.Rotate(Vector3.right, orb.velocity.x);
        wheel.AddTorque(orb.velocity.x * Vector3.forward * wheelVisualSpeed,ForceMode.Acceleration);


        orb.AddForce((boatForwardSpeed * -Vector3.forward + turnForce) * Time.fixedDeltaTime);
        transform.position = orb.transform.position - orbOffset;
        turnForce = Vector3.zero;
    }

    public void StopMovement()
    {
        orb.isKinematic = true;
        orb.velocity = Vector3.zero;
        cantMove = true;
        turnForce = Vector3.zero;
    }



}
