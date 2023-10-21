using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileBody : Body
{
    // Homework: Incorporate launch position, launch speed, and launch angle into our new custom-physics system!
    // Plot 3D projectile motion by specifying a pitch (launch-angle about X) and yaw (launch-angle about Y)
    public Vector3 launchPosition;
    public float launchSpeed = 5.0f;
    public float launchPitch = 50.0f;
    public float launchYaw = 25.0f;
    public float terminalVelocity = 500.0f;
    public float dt;

    private void Start()
    {
        dt = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Simulate(Physics.gravity, Time.fixedDeltaTime);
        transform.position = new Vector3(
            transform.position.x + (vel.x * dt) * drag,
            transform.position.y + (vel.y * dt) * drag,
            transform.position.z + (vel.z * dt) * drag
        );
    }
}
