﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class SpellScript : NetworkBehaviour
{

    [SyncVar(hook = nameof(updateColor))]
    public Color color = Color.gray;

    [SyncVar(hook = nameof(updatePlayerId))]
    public uint playerId = 0;
    public GameObject targetObj = null;

    void updateColor(Color Old, Color New)
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", New);
    }

    void updatePlayerId(uint Old, uint New)
    {
        if(New == 0)
        {
            NetworkIdentity.spawned.TryGetValue(Old, out NetworkIdentity identity);
            targetObj = identity.gameObject;
        }
        else
        {
            NetworkIdentity.spawned.TryGetValue(New, out NetworkIdentity identity);
            //bool otherIsPhone = identity.gameObject.GetComponent<PlayerScript>().type.Contains("Handheld");
            targetObj = identity.gameObject.GetComponent<PlayerScript>().opponentPlayer;
        }
        GetComponent<Rigidbody>().velocity = targetObj.transform.position - transform.position;
        //this.GetComponent<Renderer>().material.SetColor("_Color", New);
    }

    void CmdSetPlayerId(uint id)
    {
        playerId = id;
    }

    void OnCollisionEnter(Collision collision)
    {

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 5, Color.white);
        }

        if (collision.gameObject.name == "testTargetSphere")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            //GetComponent<Rigidbody>().velocity = -5*GetComponent<Rigidbody>().velocity;
            //GetComponent<Rigidbody>().velocity = player.GetComponent<PlayerScript>().transform.position - transform.position; 
            CmdSetPlayerId(0);
            Debug.Log("Do something here");
        }

        else if (collision.gameObject == targetObj /*collision.gameObject.tag == "Player"*/ /*&& collision.gameObject != player*/)
        {
            //If the GameObject has the same tag as specified, output this message in the console
            //GetComponent<Rigidbody>().velocity = -5 * GetComponent<Rigidbody>().velocity;
            //Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
            Debug.Log("Do something else here");
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