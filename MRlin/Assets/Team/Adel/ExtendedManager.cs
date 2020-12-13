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
        //base.OnServerAddPlayer(conn);
        //--- from source
        /*
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);
        */
        GameObject player = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);
        //---

        GameObject.Find("GameStateObj").GetComponent<GameState>().numOfPlayers += 1;


    }

    //Client disconnect, decrease count and remove from state list
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        
        GameObject.Find("GameStateObj").GetComponent<GameState>().numOfPlayers -= 1;



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
