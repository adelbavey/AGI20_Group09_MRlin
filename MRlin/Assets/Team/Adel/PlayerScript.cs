using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

using Mirror;
using UnityEngine.UI;
//using System;

/*
 Attached to player, primary script the user uses to interact with the game.
 */

public class PlayerScript : NetworkBehaviour
{
    TextMesh isPC;
    GameObject GameStateObj;
    public GameObject canvasPre; //Prefab
    GameObject canvas;
    public GameObject castObject;

    //Sync type (Desktop/Handeld) between all clients
    [SyncVar(hook=nameof(changeText))]
    public string type;
    void changeText(string oldValue, string newValue)
    {
        isPC = GetComponentInChildren<TextMesh>();
        isPC.text = type + " ct " + netId;//type;
    }

    [SyncVar(hook=nameof(updateHealth))]
    public int health = 50;
    void updateHealth(int Old, int New)
    {
        GameObject.Find("GameStateObj").GetComponent<GameState>().changeText(0, 0);
    }

    [SyncVar(hook = nameof(updateInternalPos))]
    int internalPos=1;
    void updateInternalPos(int Old, int New)
    {
        this.transform.position = posObject.transform.GetChild(New).position;
    }

    [SyncVar (hook= nameof(updateCurrentPos))]
    string currentPos;
    void updateCurrentPos(string Old, string New)
    {
        posObject = GameObject.Find(New);
        this.transform.position = posObject.transform.position;
        this.transform.rotation = posObject.transform.rotation;

    }
    GameObject posObject;
    
    [SyncVar(hook = nameof(updateNumClicks))]
    public int mouseClicks = 0;
    void updateNumClicks(int Old, int New)
    {
        GameObject.Find("GameStateObj").GetComponent<GameState>().changeText(0, 0);

        //GameObject sphere = Instantiate("Sphere");

        //NetworkServer.Spawn(sphere);

    }

    [SyncVar]
    public Quaternion attitude = new Quaternion();

    [SyncVar(hook=nameof(updateDrawHeld))]
    public bool drawHeld = false;
    void updateDrawHeld(bool Old, bool New)
    {
        GameObject.Find("GameStateObj").GetComponent<GameState>().changeText(0, 0);
    }

    //Phone/Desktop connection-------------------------------
    [SyncVar(hook = nameof(cpid))]
    public uint connectedPlayerId = 0;
    void cpid(uint Old, uint New)
    {
        if (New == 0) { connectedPlayer = null; return; }
        NetworkIdentity.spawned.TryGetValue(New, out NetworkIdentity Identity);
        connectedPlayer = Identity.gameObject;
    }
    public GameObject connectedPlayer = null;
    //-------------

    //Opponent connection--------------------------
    [SyncVar(hook = nameof(opid))]
    public uint opponentPlayerId = 0;
    void opid(uint Old, uint New)
    {
        if (New == 0) { opponentPlayer = GameObject.Find("mirrorSphere");return; }
        NetworkIdentity.spawned.TryGetValue(New, out NetworkIdentity Identity);
        opponentPlayer = Identity.gameObject;
        //if (opponentPlayer.transform.position == GameObject.Find("pos1").transform.position) CmdMoveTo(GameObject.Find("pos2").transform.position);
    }
    public GameObject opponentPlayer = null;
    //------------------


    // --- Commands ----------------- (Run on server, with data sent from this client)

    [Command(ignoreAuthority = true)]
    void CmdDecreaseHealth(int n)
    {
        health -= n;
    }

    [Command(ignoreAuthority = true)]
    void CmdInternalMove(int incrementPos)
    {

        int temp = internalPos;
        temp += incrementPos;
        if (temp < 0 || temp >= posObject.transform.childCount) return;
        internalPos = temp;

    }

    [Command(ignoreAuthority = true)]
    void CmdMoveTo(string pos)
    {

        //this.transform.position = GameObject.Find(pos).transform.position;

        currentPos = pos;
        GameObject.Find(pos).GetComponent<Occupied>().occupied = netId;


    }

    [Command(ignoreAuthority = true)]
    public void CmdsetIsHeld(bool b)
    {
        drawHeld = b;
        if (connectedPlayerId != 0) connectedPlayer.GetComponent<PlayerScript>().drawHeld = b;
    }

    [Command(ignoreAuthority = true)]
    void CmdsetText(string ty)
    {
        type = ty+" cmd";
        
    }

    [Command(ignoreAuthority = true)]
    void CmdCounterMousePlus()
    {
        /*
        Guid g;
        //Guid.TryParse(AssetDatabase.AssetPathToGUID("./Sphere"), out g);
        Guid.TryParse(AssetDatabase.AssetPathToGUID("bbf4d4d7-7919-84f4-b990-966eef1e973d"), out g);
        GameObject s = Instantiate(ClientScene.prefabs[g], new Vector3(0, 0, 0), Quaternion.identity);

        NetworkServer.Spawn(s);
        */

        //GameObject s = Instantiate(castObject, new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f)), Quaternion.identity);
        //GameObject s = Instantiate(castObject, this.transform.position + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)), Quaternion.identity);
        GameObject s = Instantiate(castObject, (opponentPlayer.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
        s.GetComponent<Rigidbody>().velocity = opponentPlayer.transform.position - s.transform.position;


        NetworkServer.Spawn(s);

        mouseClicks += 1;
        GameObject.Find("GameStateObj").GetComponent<GameState>().mouseNoClicks += 1;
    }

    [Command(ignoreAuthority = true)]
    public void CmdSpellCast(int spellNo)
    {
        GameObject s;

        if(spellNo == 1)
        {
            s = Instantiate(castObject, (opponentPlayer.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
            //s.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            s.GetComponent<SpellScript>().color = Color.red;
        }
        else if(spellNo == 2)
        {
            s = Instantiate(castObject, (opponentPlayer.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
            //s.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            s.GetComponent<SpellScript>().color = Color.blue;
        }
        else if(spellNo == 3)
        {
            s = Instantiate(castObject, (opponentPlayer.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
            //s.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            s.GetComponent<SpellScript>().color = Color.green;
        }
        else
        {
            s = Instantiate(castObject, (opponentPlayer.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
        }

        s.GetComponent<SpellScript>().playerId = netId;
        //GameObject s = Instantiate(castObject, new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f)), Quaternion.identity);
        //GameObject s = Instantiate(castObject, this.transform.position + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)), Quaternion.identity);
        //GameObject s = Instantiate(castObject, (opponentPlayer.transform.position - transform.position).normalized + transform.position, Quaternion.identity);
        s.GetComponent<Rigidbody>().velocity = opponentPlayer.transform.position - s.transform.position;


        NetworkServer.Spawn(s);
    }

    [Command(ignoreAuthority = true)]
    void CmdSetAttitude(Quaternion att)
    {
        attitude = att;
        GameObject.Find("GameStateObj").GetComponent<GameState>().changeText(0, 0);
    }


    [Command(ignoreAuthority = true)]
    void CmdAddPNS(uint id, string type)
    {


        bool isPhone = type.Contains("Handheld");

        // For phone/desktop
        foreach (uint fid in GameObject.Find("GameStateObj").GetComponent<GameState>().playerIds)
        {
            NetworkIdentity.spawned.TryGetValue(fid, out NetworkIdentity identity);
            bool otherIsPhone = identity.gameObject.GetComponent<PlayerScript>().type.Contains("Handheld");

            
            if(identity.gameObject.GetComponent<PlayerScript>().connectedPlayerId == 0 && isPhone != otherIsPhone)
            {
                identity.gameObject.GetComponent<PlayerScript>().connectedPlayerId = id;
                connectedPlayerId = fid;
                break;
            }
        }

        // For opponent
        foreach (uint fid in GameObject.Find("GameStateObj").GetComponent<GameState>().playerIds)
        {
            NetworkIdentity.spawned.TryGetValue(fid, out NetworkIdentity identity);
            bool otherIsPhone = identity.gameObject.GetComponent<PlayerScript>().type.Contains("Handheld");


            if (identity.gameObject.GetComponent<PlayerScript>().opponentPlayerId == 0 && isPhone == otherIsPhone)
            {
                identity.gameObject.GetComponent<PlayerScript>().opponentPlayerId = id;
                opponentPlayerId = fid;
                break;
            }
        }


        GameObject.Find("GameStateObj").GetComponent<GameState>().playerIds.Add(id);

    }

    // --- /Commands ----------------------

    // Change text when synced type variable changes
    

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
        if(GameObject.Find("GameStateObj") != null) GameObject.Find("GameStateObj").GetComponent<GameState>().playerIds.Remove(netId);
        if (GameObject.Find(currentPos) != null) GameObject.Find(currentPos).GetComponent<Occupied>().occupied = 0;
        if (opponentPlayerId != 0)
        {
            opponentPlayer.GetComponent<PlayerScript>().opponentPlayerId = 0;
        }

        if (connectedPlayerId != 0)
        {
            connectedPlayer.GetComponent<PlayerScript>().connectedPlayerId = 0;
        }
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
        Input.gyro.enabled = true;
        opponentPlayer = GameObject.Find("mirrorSphere");
        


        //Check if the device running this is a desktop
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            type = "Desktop";
            if (GameObject.Find("pos1").GetComponent<Occupied>().occupied == 0) { CmdMoveTo("pos1"); 
                GameObject.Find("Main Camera").transform.position = GameObject.Find("camPos1").transform.position;
                GameObject.Find("Main Camera").transform.rotation = GameObject.Find("camPos1").transform.rotation;
            }
            else { CmdMoveTo("pos2");
                GameObject.Find("Main Camera").transform.position = GameObject.Find("camPos2").transform.position;
                GameObject.Find("Main Camera").transform.rotation = GameObject.Find("camPos2").transform.rotation;
            }

        }

        //Check if the device running this is a handheld
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            CmdMoveTo("posPhone");
            type = "Handheld";

            canvas = Instantiate(canvasPre);
            
            Button[] buttons = canvas.GetComponentsInChildren<Button>();


            foreach (Button but in buttons)
            {
                Debug.Log("fDSFASDFDS");
                if (but.gameObject.name == "Button1")
                {
                    Debug.Log("Button1");
                    but.onClick.AddListener(delegate { if (connectedPlayerId != 0) connectedPlayer.GetComponent<PlayerScript>().CmdSpellCast(1); else CmdSpellCast(1); });
                }
                    
                else if (but.gameObject.name == "Button2")
                {
                    Debug.Log("Button2");
                    but.onClick.AddListener(delegate { if (connectedPlayerId != 0) connectedPlayer.GetComponent<PlayerScript>().CmdSpellCast(2); else CmdSpellCast(2); });
                }

                else if (but.gameObject.name == "Button3")
                {
                    Debug.Log("Button3");
                    but.onClick.AddListener(delegate { if (connectedPlayerId != 0) connectedPlayer.GetComponent<PlayerScript>().CmdSpellCast(3); else CmdSpellCast(3); });
                }

                else if (but.gameObject.name == "ButtonLeft")
                {
                    Debug.Log("ButtonLeft");
                    but.onClick.AddListener(delegate { if (connectedPlayerId != 0) connectedPlayer.GetComponent<PlayerScript>().CmdInternalMove(-1); /*else CmdInternalMove(-1);*/ });
                }

                else if (but.gameObject.name == "ButtonRight")
                {
                    Debug.Log("ButtonRight");
                    but.onClick.AddListener(delegate { if (connectedPlayerId != 0) connectedPlayer.GetComponent<PlayerScript>().CmdInternalMove(1); /*else CmdInternalMove(1);*/ });
                }

            }

            //Ref button to this object
            //canvas.GetComponentInChildren<HoldButtonScript>().PhonePlayer = this.gameObject;
            //canvas.GetComponentInChildren<HoldButtonScript>().ps = this;

            //Event add //Works only to add to one eventtrigger currently
            EventTrigger[] triggers = canvas.GetComponentsInChildren<EventTrigger>();

            foreach (EventTrigger tr in triggers)
            {
                if (tr.gameObject.name == "HoldButton")
                {
                    Debug.Log("holdbutton");
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => { CmdsetIsHeld(true); });
                    tr.triggers.Add(entry);

                    EventTrigger.Entry entry2 = new EventTrigger.Entry();
                    entry2.eventID = EventTriggerType.PointerUp;
                    entry2.callback.AddListener((data) => { CmdsetIsHeld(false); });
                    tr.triggers.Add(entry2);
                }
            }

            



        }
        
        CmdsetText(type);
        CmdAddPNS(netId, type);

        //if (opponentPlayerId == 0) CmdMoveTo(GameObject.Find("pos1").transform.position);//this.transform.position = GameObject.Find("pos1").transform.position;
        //else CmdMoveTo(GameObject.Find("pos2").transform.position);//this.transform.position = GameObject.Find("pos2").transform.position;
        //GameObject.Find("Counter").GetComponent<counter>().players.Add(netId, netIdentity.gameObject);

        /*
        if(connectedPlayerId !=0)
        {
            NetworkIdentity.spawned.TryGetValue(connectedPlayerId, out NetworkIdentity Identity);
            connectedPlayer = Identity.gameObject;
        }
        */


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
        //GameStateObj = GameObject.Find("GameStateObj");




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
            //if(type.Contains("Handheld") && connectedPlayer != null)
            //    connectedPlayer.GetComponent<PlayerScript>().CmdCounterMousePlus();
            //else CmdCounterMousePlus();
        }
        

        if (Input.GetKeyDown(KeyCode.Z))
            CmdSpellCast(1);

        if (Input.GetKeyDown(KeyCode.X))
            CmdSpellCast(2);

        if (Input.GetKeyDown(KeyCode.C))
            CmdSpellCast(3);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            CmdInternalMove(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            CmdInternalMove(1);

        /*
        if(((Canvas)canvas.GetComponent("Canvas")).enabled == true)
        {
            if(canvas.GetComponent("Button1"))
        }
        */


        CmdSetAttitude(Input.gyro.attitude);

    }

    // Collision --------------------------------------

    void OnCollisionEnter(Collision collision)
    {

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 5, Color.white);
        }

        if (collision.gameObject.tag == "Fire" && collision.gameObject.GetComponent<SpellScript>().playerId != netId)
        {
            CmdDecreaseHealth(5);
        }


        /*
        if (collision.relativeVelocity.magnitude > 2)
            audioSource.Play();
        */


    }


    
}
