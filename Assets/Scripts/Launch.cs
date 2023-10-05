using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public GameObject projectilePrefab;
    public ProjectileBody body;
    public float launchSpeed = 0.0f;
    public float launchPitch = 0.0f;
    public float launchYaw = 0.0f;
    public float drag = 1.0f;
    public float gravity = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Shoot() 
    {
        GameObject newObject = Instantiate(projectilePrefab);
        newObject.transform.position = transform.position;
        body = newObject.GetComponent<ProjectileBody>();
        body.vel = new Vector3(launchSpeed * (Mathf.Cos(Mathf.Deg2Rad * launchYaw) * Mathf.Cos(Mathf.Deg2Rad * launchPitch)),
                                launchSpeed * (Mathf.Sin(Mathf.Deg2Rad * launchYaw) * Mathf.Cos(Mathf.Deg2Rad * launchPitch)),
                                launchSpeed * (Mathf.Sin(Mathf.Deg2Rad * launchPitch)));
        body.drag = drag;
        body.mass = gravity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            Shoot();
        }
    }
}
