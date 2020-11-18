﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using UnityEngine.UI;
using System;

/*
 Attached to player, primary script the user uses to interact with the game.
 */

public class PlayerScript : NetworkBehaviour
{
    //Sync type (Desktop/Handeld) between all clients
    [SyncVar(hook=nameof(changeText))]
    public string type;

    TextMesh isPC;

    
    [SyncVar(hook = nameof(updateNumClicks))]
    public int mouseClicks = 0;

    void updateNumClicks(int Old, int New)
    {
        GameObject.Find("Counter").GetComponent<counter>().playerNetStates[netId].mouseNoClicks = mouseClicks;
        GameObject.Find("Counter").GetComponent<counter>().changeText(0, 0);
    }
    

    // --- Commands ----------------- (Run on server, with data sent from this client)

    [Command(ignoreAuthority = true)]
    void CmdsetText(string ty)
    {
        type = ty+" cmd";
        
    }

    //OLD
    [Command(ignoreAuthority = true)]
    void CmdCounterPlus()
    {
        GameObject counter = GameObject.Find("Counter");
        TextMesh tm = counter.GetComponentInChildren<TextMesh>();
        tm.text = int.Parse(tm.text)+1+"";

    }

    [Command(ignoreAuthority = true)]
    void CmdCounterMousePlus()
    {

        mouseClicks += 1;
        //GameObject.Find("Counter").GetComponent<counter>().playerNetStates[netId].mouseNoClicks = mouseClicks;
        //GameObject.Find("Counter").GetComponent<counter>().playerNetStates[netId].mouseNoClicks += 1;
        GameObject.Find("Counter").GetComponent<counter>().mouseNoClicks += 1;
    }

    //OLD
    [Command(ignoreAuthority = true)]
    void CmdCounterMinus()
    {
        GameObject counter = GameObject.Find("Counter");
        TextMesh tm = counter.GetComponentInChildren<TextMesh>();
        tm.text = (int.Parse(tm.text) - 1) + "";

    }

    [Command(ignoreAuthority = true)]
    void CmdAddPNS(uint id, string type)
    {
        //GameObject counter = GameObject.Find("Counter");
        //TextMesh tm = counter.GetComponentInChildren<TextMesh>();

        GameObject.Find("Counter").GetComponent<counter>().playerNetStates.Add(id, new PlayerNetState() { netId = id, type = type, mouseNoClicks= 0 });

        NetworkIdentity.spawned.TryGetValue(id, out NetworkIdentity identity);
        GameObject.Find("Counter").GetComponent<counter>().players.Add(id, identity.gameObject);

    }

    // --- /Commands ----------------------

    // Change text when synced type variable changes
    void changeText(string oldValue, string newValue)
    {
        isPC = GetComponentInChildren<TextMesh>();
        isPC.text = type + " ct "+netId;//type;
    }

    // --------------- Network callbacks

    public override void OnStartServer()
    {
        // disable client stuff
        //CmdCounterPlus();
    }

    public override void OnStopServer()
    {
        // disable client stuff
        //CmdCounterMinus();
    }

    public override void OnStartClient()
    {
        // register client events, enable effects
        
    }

    public override void OnStopClient()
    {
        // register client events, enable effects
        //CmdCounterMinus();
    }

    public override void OnStartLocalPlayer()
    {
        // register client events, enable effects

        //Check if the device running this is a desktop
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            type = "Desktop";

        }

        //Check if the device running this is a handheld
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {

            type = "Handheld";

        }

        CmdsetText(type);
        CmdAddPNS(netId, type);
        //GameObject.Find("Counter").GetComponent<counter>().players.Add(netId, netIdentity.gameObject);

    }

    // ------------------

    // Start is called before the first frame update
    void Start()
    {
        // If not local player, find synced type variable
        if (!isLocalPlayer)
        {
            // exit from update if this is not the local player
            isPC = GetComponentInChildren<TextMesh>();
            isPC.text = type + " no local "+ netId;//type;
            return;
        }



    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        // Register mouse down
        if (Input.GetMouseButtonDown(0)) { 
            CmdCounterMousePlus();
        }

    }
}