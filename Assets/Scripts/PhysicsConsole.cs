using System;
using UnityEngine;

public class PhysicsConsole : MonoBehaviour
{
    private PhysicsObject testObject;
    private bool isGrounded = false;
    private Vector3 inputVelocity = Vector3.zero;

    void Start()
    {
        testObject = new PhysicsObject(new Vector3(0, 5, 0), 0.5f, 1f, 1f, true);
        Debug.Log("Physics Simulation Started. Use W/S for Z impulse, A/D for X impulse, Space to reset.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            inputVelocity.z = 5f / testObject.Mass;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            inputVelocity.z = -5f / testObject.Mass;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            inputVelocity.x = -5f / testObject.Mass;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            inputVelocity.x = 5f / testObject.Mass;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            testObject.Position = new Vector3(0, 5, 0);
            testObject.Velocity = Vector3.zero;
            inputVelocity = Vector3.zero;
            isGrounded = false;
            Debug.Log("Physics Reset.");
        }
    }

    void FixedUpdate()
    {
        if (!isGrounded)
        {
            testObject.ApplyGravity();
            testObject.Velocity += inputVelocity;
            testObject.UpdatePosition();
            inputVelocity = Vector3.zero; // Reset after applying
            Debug.Log("Current Position: " + testObject.Position + " | Current Velocity: " + testObject.Velocity);
        }

        if (testObject.Position.y <= 0)
        {
            isGrounded = true;
            testObject.Velocity = Vector3.zero;
            Debug.Log("Object has hit the ground and stopped moving.");
        }
    }
}
