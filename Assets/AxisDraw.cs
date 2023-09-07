using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisDraw : MonoBehaviour
{
    public float linelength = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Drawline(transform.position, transform.position + new Vector3(linelength, 0, 0), Color.red);

        Debug.Drawline(transform.position, transform.position + new Vector3(0, linelength, 0), Color.green);
        
        Debug.Drawline(transform.position, transform.position + new Vector3(0, 0, linelength), Color.blue);
    }
}
