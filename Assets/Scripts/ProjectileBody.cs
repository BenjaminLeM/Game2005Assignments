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

    Body Fix(Body bodyA, Body bodyB) 
    {
        Vector3 normal = bodyB.transform.rotation * new Vector3(0, 1, 0);
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);
        Vector3 gravityForce = grav * bodyA.mass;

        //normal forces
        Vector3 gravityForcePerpendicular = Vector3.Dot(gravityForce, normal) * normal;
        Vector3 normalForce = -gravityForcePerpendicular;
        bodyA.AddForce(normalForce);

        //find net force in parallel direction to surface
        Vector3 fNetParallel = -(gravityForce - gravityForcePerpendicular);
        float fNetParallelMagnitude = fNetParallel.magnitude;
        Vector3 FrictionDirection = fNetParallel.normalized;

        //find velocity in perpendicular and parallel direction to the surface 
        Vector3 Velocity = bodyA.vel;
        Vector3 VelocityPerpendicular = Vector3.Dot(Velocity, normal) * normal;
        Vector3 VelocityParallel = Velocity - VelocityPerpendicular;

        float FrictionMagnitude = normalForce.magnitude*bodyA.coefficientOfFriction;
        Vector3 FrictionForce = -VelocityParallel.normalized * FrictionMagnitude;
        if (Vector3.Dot(Velocity, normal) < 0)
        {
            bodyA.AddForce(FrictionForce);
            bodyA.transform.position += normal * (bodyA.radius - projection);
        }
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + normalForce, Color.green);
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + gravityForce, Color.magenta);
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + FrictionForce, Color.yellow);
        Debug.DrawLine(bodyA.transform.position, bodyA.transform.position + Velocity, Color.red);


        //bodyA.vel += (FrictionForce * 0.3f) * dt;

        //bodyA.vel = Vector3.zero;
        //draw forces


        return bodyA;
    }

    void MomentumConservationCollision(Body bodyA, Body bodyB) 
    {
        Vector3 normal = (bodyB.transform.position - bodyA.transform.position).normalized;
        Vector3 displacement = bodyA.transform.position - bodyB.transform.position;
        float projection = Vector3.Dot(displacement, normal);

        Vector3 RelativeVelocity = bodyB.vel - bodyA.vel;

        Vector3 ClosingVelocity = Vector3.Dot(RelativeVelocity, normal) * normal;

        //average of the bounciness
        float Restitution = (bodyA.Bounciness + bodyB.Bounciness) / 2;

        Vector3 ClosingVelocityRetained = Restitution * ClosingVelocity;

        Vector3 ClosingVelocityFinal = -ClosingVelocityRetained;

        Vector3 Impulse = -(1 + Restitution) * ClosingVelocity * bodyA.mass * bodyB.mass / (bodyA.mass + bodyB.mass);

        Vector3 VelocityFinal = Impulse/bodyA.mass;

        if (Vector3.Dot(RelativeVelocity, normal) >= 0) 
        {}
        else
        {
            bodyA.vel += ((bodyB.mass / bodyA.mass) * ClosingVelocity);
            //bodyA.vel += (VelocityFinal);
        }
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
                        MomentumConservationCollision(bodyA, bodyB);
                    }
                    else
                    {
                        
                    }
                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 2) 
                {
                    if (checkSpherePlaneCollision(bodyA, bodyB))
                    {
                        //MomentumConservationCollision(bodyA, bodyB);
                        if (bodyA.isKinematic) 
                        {
                            bodyA = Fix(bodyA, bodyB);
                        }
                    }
                    else
                    {
                        
                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 2)
                {
                    if (checkSpherePlaneCollision(bodyB, bodyA))
                    {
                        
                    }
                    else
                    {
                        
                    }
                }
                else if (bodyA.GetShape() == 0 && bodyB.GetShape() == 3)
                {
                    if (checkSphereHalfPlaneCollision(bodyA, bodyB))
                    {
                        //MomentumConservationCollision(bodyA, bodyB);
                        if (bodyA.isKinematic)
                        {
                            bodyA = Fix(bodyA, bodyB);
                        }
                    }
                    else
                    {
                        
                    }
                }
                else if (bodyB.GetShape() == 0 && bodyA.GetShape() == 3)
                {
                    if (checkSphereHalfPlaneCollision(bodyB, bodyA))
                    {
                        
                    }
                    else
                    {
                        
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
            if (body.isKinematic)
            {
                body.Simulate(grav, dt);
                body.transform.localPosition += new Vector3(
                    (body.vel.x * dt) * body.drag,
                    (body.vel.y * dt) * body.drag,
                    (body.vel.z * dt) * body.drag);
            }
        }
        checkCollision();
        //Simulate(Physics.gravity, Time.fixedDeltaTime);
        //transform.position = new Vector3(
        //    transform.position.x + (vel.x * dt) * drag,
        //    transform.position.y + (vel.y * dt) * drag,
        //    transform.position.z + (vel.z * dt) * drag
        //);
    }
}