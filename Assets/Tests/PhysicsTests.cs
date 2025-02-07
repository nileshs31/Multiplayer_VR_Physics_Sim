using NUnit.Framework;
using UnityEngine;
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
public class PhysicsTests
{
    private PhysicsObject testObject;

    [SetUp]
    public void Setup()
    {
        testObject = new PhysicsObject(new Vector3(0,2,0), 0.25f, 1f, 1f, true);
    }

    [Test]
    public void Gravity_AppliedCorrectly()
    {
        Vector3 initialVelocity = testObject.Velocity;
        testObject.ApplyGravity();
        Assert.AreEqual(initialVelocity.y + (testObject.Gravity * Time.deltaTime), testObject.Velocity.y, 0.01f);
    }

   [Test]
    public void Position_UpdatesCorrectly()
    {
        Vector3 initialPosition = testObject.Position;
        testObject.Velocity = new Vector3(0, -9.81f, 0);
        testObject.UpdatePosition();
        Assert.AreNotEqual(initialPosition, testObject.Position);
    }

    [Test]
    public void Collision_DetectionWorks()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.transform.position = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(testObject.Position, testObject.Radius);
        bool hasCollided = hitColliders.Length > 0;

        Assert.IsFalse(hasCollided);

        testObject.Position = new Vector3(0, -0.1f, 0);
        hitColliders = Physics.OverlapSphere(testObject.Position, testObject.Radius);
        hasCollided = hitColliders.Length > 0;

        Assert.IsTrue(hasCollided);
    }

    [Test]
    public void Impulse_AppliedCorrectlyOnX()
    {
        Vector3 initialVelocity = testObject.Velocity;
        testObject.ApplyImpulseOnX(5f);
        Assert.AreEqual(initialVelocity.x + (5f / testObject.Mass), testObject.Velocity.x, 0.01f);
    }

    [Test]
    public void Impulse_AppliedCorrectlyOnZ()
    {
        Vector3 initialVelocity = testObject.Velocity;
        testObject.ApplyImpulseOnZ(5f);
        Assert.AreEqual(initialVelocity.z + (5f / testObject.Mass), testObject.Velocity.z, 0.01f);
    }
}
