using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float launchVelocity = 10.0f;
    public float launchAngle = 30.0f;
    public float launchHeight = 3.0f;

    private float velX = 10.0f;  // Doesn't change between frames
    private float velY = 10.0f;  // Under constant acceleration (gravity)

    void Start()
    {
        LaunchBall();
    }

    void LaunchBall()
    {
        // 1. Update velX and velY based on horizontal and vertical components of launch velocity & launch angle
        velX =  Mathf.Cos(launchAngle) * launchVelocity;
        velY =  Mathf.Sin(launchAngle) * launchVelocity;
        // 2. Assign position to new launch height and re-launch the ball!
        transform.position = new Vector3(-12.0f, launchHeight, 32.0f);
    }

    void FixedUpdate()
    {
        // Restart the launch
        if (Input.GetKey(KeyCode.Space))
        {
            LaunchBall();
        }

        float dt = Time.fixedDeltaTime;
        float acc = Physics.gravity.y;

        // Update velocity using acceleration over time (velX remains constant)
        velY = velY + acc * dt;

        // Update position using velocity over time
        transform.position = new Vector3(
            transform.position.x + velX * dt,
            transform.position.y + velY * dt,
            transform.position.z
        );
    }
}
