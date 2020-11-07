using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;


/*
 Adds extra functionality to regular Network Manager. Mostly working with callbacks.
 */
public class ExtendedManager : NetworkManager
{



    // ----- Callbacks ----------


    /*
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        GameObject counter = GameObject.Find("Counter");
        TextMesh tm = counter.GetComponentInChildren<TextMesh>();
        tm.text = (int.Parse(tm.text) + 1) + "";
    }*/


    //Add player, update player count
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        GameObject.Find("Counter").GetComponent<counter>().numOfPlayers += 1;



    }

    //Client disconnect, decrease count and remove from state list
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        
        GameObject.Find("Counter").GetComponent<counter>().numOfPlayers -= 1;

        GameObject.Find("Counter").GetComponent<counter>().playerNetStates.Remove(conn.identity.netId);

        base.OnServerDisconnect(conn);
    }




    // ---------------------------


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
