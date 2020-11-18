using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

/*
Container for holding different values representing different players.
Meant to be a simple container that can be easily serialized and transferred between clients.
 */
public class PlayerNetState : NetworkBehaviour
{

    public uint netId;
    public string type;

    [SyncVar]
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

public static class PlayerNetStateReaderWriter
{
    public static void WritePlayerNetState(this NetworkWriter writer, PlayerNetState dateTime)
    {
        writer.WriteInt64(dateTime.mouseNoClicks);
    }

    public static PlayerNetState ReadPlayerNetState(this NetworkReader reader)
    {
        return new PlayerNetState(){netId = 0, type ="0", mouseNoClicks = (int)reader.ReadInt64() };
    }
}
