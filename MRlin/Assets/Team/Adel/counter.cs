using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

using Mirror;

public class playerList : SyncList<GameObject> { };
public class uintList : SyncList<uint> { };

public class playersDict : SyncDictionary<uint, GameObject> { };

/*
 Currently used as a centralized and synchronized class for accessing certain states of the networked game.
Meant to hold info from the different clients and distribute appropriately.
 */
public class counter : NetworkBehaviour
{
    [SyncVar(hook = nameof(changeText))]
    public int numOfPlayers = 0;

    [SyncVar(hook = nameof(changeText))]
    public int mouseNoClicks = 0;

    public playersDict players = new playersDict();
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
            "-----------------" + "\n";
        }
        
    }

    //When something is added/removed from PNS, this is called. Function name borrowd from Mirror docs, will change later.
    void OnEquipmentChange2(SyncDictionary<uint, GameObject>.Operation op, uint key, GameObject item)
    {
        // equipment changed,  perhaps update the gameobject
        Debug.Log(op + " - " + key);
        changeText(0, 0);
    }

    //When something is added/removed from PNS, this is called. Function name borrowd from Mirror docs, will change later.
    void OnEquipmentChange3(SyncList<uint>.Operation op, int index, uint oldItem, uint newItem)
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

        players.Callback += OnEquipmentChange2;
        playerIds.Callback += OnEquipmentChange3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
