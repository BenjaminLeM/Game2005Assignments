using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Body : Shape
{
    // Using transform.position since we're deriving from MonoBehaviour!
    //public Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 vel = new Vector3(0.0f, 0.0f, 0.0f);
    public float mass = 1.0f;
    public float massInverse = 1.0f;

    public float drag = 0.0f;
    public float radius = 0.5f;
    public bool isKinematic = true;
    public float coefficientOfFriction = 0.5f;
    public float Bounciness;
    public Vector3 NetForce 
    {
        get;
        private set;
    }
    public void AddForce(Vector3 force) 
    {
        NetForce += force;
    }
    public void ResetForces() 
    {
        NetForce = Vector3.zero;
    }
    public void Simulate(Vector3 acc, float dt)
    {
        Vector3 fGrav = acc * mass;
        AddForce(fGrav);
        //F = M * acc
        //-20 = 2 * -10
        //a = f/m
        //a = -20/2
        Vector3 Acceleration = NetForce / mass;
        vel += Acceleration * dt;
        transform.position = transform.position + vel * dt;
        if (vel.y <= -20.0f)
        {
            vel.y = -20.0f;
        }
        ResetForces();
    }
}