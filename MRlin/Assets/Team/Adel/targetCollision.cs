using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to mirror sphere
public class targetCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal*5, Color.white);
        }
        /*
        if (collision.relativeVelocity.magnitude > 2)
            audioSource.Play();
        */


    }
}
