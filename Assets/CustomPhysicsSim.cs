using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PhysicsObject
{
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public float Mass { get; set; } = 1f;
    public float Radius { get; set; }
    public float Gravity { get; set; } = -9.81f;
    public float BounceEfficiency { get; set; }
    public bool CanBounce { get; set; }

    public PhysicsObject(Vector3 initialPosition, float radius, float mass, float bounceEfficiency, bool canBounce)
    {
        Position = initialPosition;
        Radius = radius;
        Mass = mass;
        BounceEfficiency = bounceEfficiency;
        CanBounce = canBounce;
        Velocity = Vector3.zero;
    }

    public void ApplyGravity()
    {
        Velocity += new Vector3(0, Gravity * Time.deltaTime, 0);
    }

    public void UpdatePosition()
    {
        Position += Velocity * Time.deltaTime;
    }
    public void ApplyImpulseOnX(float impulseForce)
    {
        Velocity = new Vector3(impulseForce / Mass, Velocity.y, Velocity.z);
    }

    public void ApplyImpulseOnZ(float impulseForce)
    {
        Velocity = new Vector3(Velocity.x, Velocity.y, impulseForce / Mass);
    }
}

public class CustomPhysicsSim : NetworkBehaviour
{
    public PhysicsObject physicsObject;
    private bool isGrounded = false;
    private float groundLevel = 0f;
    private Vector3 initialPosition;
    public LayerMask groundLayer;
    public float bounceBackSpeed;
    public float mass, impulseForce;
    public bool isBouncingObject;
    public TextMeshProUGUI positionText, velocityText, massText, bounceBackText;

    // Boundary limits for X and Z
    public float maxX = 8f;
    public float minX = -8f;
    public float maxZ = 8f;
    public float minZ = -8f;


    private NetworkVariable<Vector3> objPos = new NetworkVariable<Vector3>(new Vector3(0,0,0), NetworkVariableReadPermission.Everyone);
    private NetworkVariable<Vector3> objVelocity = new NetworkVariable<Vector3>(new Vector3(0,0,0), NetworkVariableReadPermission.Everyone);
    void Start()
    {
        initialPosition = transform.position;
        physicsObject = new PhysicsObject(initialPosition, 0.175f, mass, bounceBackSpeed, isBouncingObject);
    }

    void FixedUpdate()
    {
        if (!isGrounded)
        {
            physicsObject.ApplyGravity();
            physicsObject.UpdatePosition();
            transform.position = physicsObject.Position;

            // Check if object is out of bounds in X or Z
            if (physicsObject.Position.x > maxX || physicsObject.Position.x < minX || physicsObject.Position.z > maxZ || physicsObject.Position.z < minZ)
            {
                StartCoroutine(ResetBall());
            }

            Collider[] hitColliders = Physics.OverlapSphere(physicsObject.Position, physicsObject.Radius, groundLayer);
            if (hitColliders.Length > 0 && physicsObject.Velocity.y < 0)
            {
                isGrounded = true;
                physicsObject.Position = new Vector3(physicsObject.Position.x, groundLevel + physicsObject.Radius, physicsObject.Position.z);

                if (isBouncingObject)
                {
                    physicsObject.Velocity = new Vector3(physicsObject.Velocity.x, Mathf.Abs(physicsObject.Velocity.y) * physicsObject.BounceEfficiency, physicsObject.Velocity.z);

                    if (physicsObject.Velocity.y < 0.2f)
                    {
                        physicsObject.Velocity = Vector3.zero;
                        StartCoroutine(ResetBall());
                    }
                    else
                    {
                        isGrounded = false;
                    }
                }
                else
                {
                    physicsObject.Velocity = Vector3.zero;
                    StartCoroutine(ResetBall());
                }
            }
        }
        if (IsServer)
        {
            objPos.Value = physicsObject.Position;
            objVelocity.Value = physicsObject.Velocity;
        }
        
        positionText.text = "Position - " + objPos.Value;
        velocityText.text = "Velocity - " + objVelocity.Value;
        massText.text = "Mass - " + physicsObject.Mass;
        bounceBackText.text = "Bounce Efficiency - " + physicsObject.BounceEfficiency;
    }

    IEnumerator ResetBall()
    {
        yield return new WaitForSeconds(1f);
        physicsObject.Position = initialPosition;
        physicsObject.Velocity = Vector3.zero;
        isGrounded = false;
        transform.position = initialPosition;
    }

    public void ResetRequest()
    {
        physicsObject.Position = initialPosition;
        physicsObject.Velocity = Vector3.zero;
        isGrounded = false;
        transform.position = initialPosition;
    }

    public void ApplyImpulseX()
    {
        ApplyImpulseXServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void ApplyImpulseXServerRpc()
    {
        ApplyImpulseXClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    void ApplyImpulseXClientRpc()
    {
        physicsObject.ApplyImpulseOnX(impulseForce);
    }

    public void ApplyImpulseZ()
    {
        ApplyImpulseZServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void ApplyImpulseZServerRpc()
    {
        ApplyImpulseZClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    void ApplyImpulseZClientRpc()
    {
        physicsObject.ApplyImpulseOnZ(impulseForce);
    }
}
