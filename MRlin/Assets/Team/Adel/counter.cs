using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

using Mirror;

public class SyncDictionaryStates : SyncDictionary<uint, PlayerNetState> { };
public class playerList : SyncList<GameObject> { };

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

    //public List<PlayerNetState> playerNetStates = new List<PlayerNetState>();
    //public readonly SyncDictionary<string, string> playerNetStates = new SyncDictionary<string, string>();
    
    //Dictionary between netids and the corresponding player state, that different clients can poll.
    public SyncDictionaryStates playerNetStates = new SyncDictionaryStates();

    //public playerList players = new playerList();
    //public SyncList<int> mc;
    public playersDict players = new playersDict();

    public void changeText(int oldval, int newval)
    {
        TextMesh counterComp = GetComponentInChildren<TextMesh>(); 
        counterComp.text = "Num players: "+ numOfPlayers+"\n"+ 
            "Num clicks: "+mouseNoClicks+"\n"+
            "-----------------" + "\n";

        Debug.Log(netId + " :: "+ players.Count);
        foreach (KeyValuePair<uint, GameObject> pns in players)
        {
            /*
            counterComp.text += pns.Value +"\n" +
            "-----------------" + "\n";
            */

            NetworkIdentity.spawned.TryGetValue(pns.Key, out NetworkIdentity identity);
            PlayerScript ps = identity.gameObject.GetComponentInChildren<PlayerScript>();
            //PlayerScript ps = pns.Value.GetComponentInChildren<PlayerScript>();
            //Debug.Log(netId + " :: "+ps.netId);

            counterComp.text += "netId: " + ps.netId + "\n" +
            "type: " + ps.type + "\n" +
            "mouseNoClicks: " + ps.mouseClicks + "\n" +
            "-----------------" + "\n";
        }
        
    }

    //When something is added/removed from PNS, this is called. Function name borrowd from Mirror docs, will change later.
    void OnEquipmentChange(SyncDictionary<uint, PlayerNetState>.Operation op, uint key, PlayerNetState item)
    {
        // equipment changed,  perhaps update the gameobject
        Debug.Log(op + " - " + key);
        changeText(0, 0);
    }

    //When something is added/removed from PNS, this is called. Function name borrowd from Mirror docs, will change later.
    void OnEquipmentChange2(SyncDictionary<uint, GameObject>.Operation op, uint key, GameObject item)
    {
        // equipment changed,  perhaps update the gameobject
        Debug.Log(op + " - " + key);
        changeText(0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        TextMesh counterComp = GetComponentInChildren<TextMesh>();
        counterComp.text = numOfPlayers+"";

        playerNetStates.Callback += OnEquipmentChange;
        players.Callback += OnEquipmentChange2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
