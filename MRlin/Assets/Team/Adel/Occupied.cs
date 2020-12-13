using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

//Attached to position objects to decide starting position
public class Occupied : NetworkBehaviour
{
    [SyncVar]
    public uint occupied = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
