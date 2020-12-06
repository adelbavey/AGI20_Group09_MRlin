using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class SpellScript : NetworkBehaviour
{

    [SyncVar(hook = nameof(updateColor))]
    public Color color = Color.gray;

    void updateColor(Color Old, Color New)
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", New);
    }

    void OnCollisionEnter(Collision collision)
    {

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 5, Color.white);
        }
        /*
        if (collision.relativeVelocity.magnitude > 2)
            audioSource.Play();
        */


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
