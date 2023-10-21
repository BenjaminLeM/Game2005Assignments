using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileBody : MonoBehaviour
{
    // Homework: Incorporate launch position, launch speed, and launch angle into our new custom-physics system!
    // Plot 3D projectile motion by specifying a pitch (launch-angle about X) and yaw (launch-angle about Y)
    public Vector3 launchPosition;
    public float dt;
    public Vector3 grav = new Vector3(0, -9.8f, 0);
    public List<Body> bodies = new List<Body>();
    void checkForNewBodies()
    {
        Body[] allObjects = FindObjectsOfType<Body>(false);
        foreach (Body BodyFound in allObjects)
        {
            if (!bodies.Contains(BodyFound))
            {
                bodies.Add(BodyFound);
            }
        }
    }
    bool checkSphereSphereCollision(Body bodyA, Body bodyB)
    {
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float distance = displacement.magnitude;
        return distance < bodyA.radius + bodyB.radius;
    }
    private void checkCollision()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            Body bodyA = bodies[i];
            for (int j = 0; j < bodies.Count; j++)
            {
                Body bodyB = bodies[j];

            }
        }
    }
    private void Start()
    {
        dt = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        checkForNewBodies();
        foreach (Body body in bodies)
        {
            if (body.isProjectile)
            {
                body.Simulate(grav, dt);
                body.transform.localPosition += new Vector3(
                    (body.vel.x * dt) * body.drag,
                    (body.vel.y * dt) * body.drag,
                    (body.vel.z * dt) * body.drag);
            }
        }

        //Simulate(Physics.gravity, Time.fixedDeltaTime);
        //transform.position = new Vector3(
        //    transform.position.x + (vel.x * dt) * drag,
        //    transform.position.y + (vel.y * dt) * drag,
        //    transform.position.z + (vel.z * dt) * drag
        //);
    }
}