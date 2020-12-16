using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

using Mirror;

public class uintList : SyncList<uint> { };


/*
 Currently used as a centralized and synchronized class for accessing certain states of the networked game.
Meant to hold info from the different clients and distribute appropriately.
 */
public class GameState : NetworkBehaviour
{
    [SyncVar(hook = nameof(changeText))]
    public int numOfPlayers = 0;

    [SyncVar(hook = nameof(changeText))]
    public int mouseNoClicks = 0;

    public uintList playerIds = new uintList();

    public void changeText(int oldval, int newval)
    {
        TextMesh counterComp = GetComponentInChildren<TextMesh>(); 
        counterComp.text = "Num players: "+ numOfPlayers+"\n"+ 
            "Num clicks: "+mouseNoClicks+"\n"+
            "-----------------" + "\n";

        //Debug.Log(netId + " :: "+ players.Count);
        foreach (uint id in playerIds)
        {
            /*
            counterComp.text += pns.Value +"\n" +
            "-----------------" + "\n";
            */

            //NetworkIdentity.spawned.TryGetValue(pns.Key, out NetworkIdentity identity);
            NetworkIdentity.spawned.TryGetValue(id, out NetworkIdentity identity);
            PlayerScript ps = identity.gameObject.GetComponentInChildren<PlayerScript>();
            //PlayerScript ps = pns.Value.GetComponentInChildren<PlayerScript>();
            //Debug.Log(netId + " :: "+ps.netId);

            counterComp.text += "netId: " + ps.netId + "\n" +
            "type: " + ps.type + "\n" +
            "mouseNoClicks: " + ps.mouseClicks + "\n" +
            "attitude: " + ps.attitude + "\n" +
            "connected id: " + ps.connectedPlayerId + "\n" +
            "opponent id: " + ps.opponentPlayerId + "\n" +
            "held button: " + ps.drawHeld + "\n" +
            "health: " + ps.health + "\n" +
            "-----------------" + "\n";
        }
        
    }

    //When something is added/removed from Ids list, this is called.
    void OnIdsChange(SyncList<uint>.Operation op, int index, uint oldItem, uint newItem)
    {
        // equipment changed,  perhaps update the gameobject
        Debug.Log(op + " - " + index);
        changeText(0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        TextMesh counterComp = GetComponentInChildren<TextMesh>();
        counterComp.text = numOfPlayers+"";

        playerIds.Callback += OnIdsChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
