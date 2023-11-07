using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
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
        return distance < bodyA.radius;
    }
    bool checkSpherePlaneCollision(Body bodyA, Body bodyB)
    {
        Vector3 normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);
        return Mathf.Abs(projection) < bodyA.radius;
    }
    bool checkSphereHalfPlaneCollision(Body bodyA, Body bodyB)
    { 
        Vector3 normal = bodyB.transform.rotation * new Vector3(0,1,0);
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);
        return projection < bodyA.radius;
    }
    private void checkCollision()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            Body bodyA = bodies[i];
            for (int j = 0; j < bodies.Count; j++)
            {
                Body bodyB = bodies[j];
                //checks for collision detection type
                if (bodyA.GetShape() == 0 && bodyB.GetShape() == 0)
                {
                    if (checkSphereSphereCollision(bodyA, bodyB))
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    }
                    else
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    }
                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 2) 
                {
                    if (checkSpherePlaneCollision(bodyA, bodyB))
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                        if (bodyA.isProjectile) 
                        {
                            bodyA.isProjectile = false;
                        }
                    }
                    else
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 2)
                {
                    if (checkSpherePlaneCollision(bodyB, bodyA))
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    }
                    else
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    }
                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 3)
                {
                    if (checkSphereHalfPlaneCollision(bodyA, bodyB))
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                        if (bodyA.isProjectile)
                        {
                            bodyA.isProjectile = false;
                        }
                    }
                    else
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 3)
                {
                    if (checkSphereHalfPlaneCollision(bodyB, bodyA))
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    }
                    else
                    {
                        bodyA.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
                    }
                }
            }
        }
    }
    private void Start()
    {
        dt = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
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
    private void Update()
    {
        checkCollision();
    }
}