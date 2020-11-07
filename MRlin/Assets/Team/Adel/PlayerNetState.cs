using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
Container for holding different values representing different players.
Meant to be a simple container that can be easily serialized and transferred between clients.
 */
public class PlayerNetState
{

    public uint netId;
    public string type;

    
    public int mouseNoClicks;

    //Constructor may not be needed.
    /*
    public PlayerNetState(int id, string ty, int click)
    {
        netId = id;
        type = ty;
        mouseNoClicks = click;
    }
    */

    public override string ToString()
    {
        return "netId: "+netId + "\n"+
            "type: "+type+"\n"+
            "mouseNoClicks: "+mouseNoClicks;
    }

}
