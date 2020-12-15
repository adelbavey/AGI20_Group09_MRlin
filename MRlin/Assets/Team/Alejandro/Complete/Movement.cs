using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vexpot.Integration;
using Vexpot.ColorTrackerDemo;

public class Movement : MonoBehaviour
{

    public Vector3 positionTarget = new Vector3(0, 0, 0);
    FruitTrailRenderer ftr = new FruitTrailRenderer();

    // Start is called before the first frame update
    void Start()
    {
        ftr = GameObject.FindGameObjectWithTag("TrackCamera").GetComponent<FruitTrailRenderer>();
 
    }

    // Update is called once per frame
    void Update()
    {
        
        positionTarget = ftr.getTargetPos();
        transform.position = new Vector3(-positionTarget.x,transform.position.y, transform.position.z);
    }
}
