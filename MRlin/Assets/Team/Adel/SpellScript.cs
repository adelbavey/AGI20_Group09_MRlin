using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

using Mirror;

public class SpellScript : NetworkBehaviour
{

    [SerializeField]
    private VisualEffectAsset spellVFX1;
    [SerializeField]
    private VisualEffectAsset spellVFX2;
    [SerializeField]
    private VisualEffectAsset spellVFX3;

    [SyncVar(hook = nameof(updateColor))]
    public Color color = Color.gray;

    [SyncVar(hook = nameof(updatePlayerId))]
    public uint playerId = 0;
    public GameObject targetObj = null;

    void updateColor(Color Old, Color New)
    {
        this.GetComponent<Renderer>().material.SetColor("_Color", New);
        if (New == Color.red)
        {
            this.GetComponent<VisualEffect>().visualEffectAsset = spellVFX1;
        }
        else if (New == Color.blue) {

            this.GetComponent<VisualEffect>().visualEffectAsset = spellVFX2;
        }

        else if (New == Color.green)
        {
            this.GetComponent<VisualEffect>().visualEffectAsset = spellVFX3;
        }
    }

    void updatePlayerId(uint Old, uint New)
    {
        if(New == 0)
        {
            NetworkIdentity.spawned.TryGetValue(Old, out NetworkIdentity identity);
            targetObj = identity.gameObject;
        }
        else
        {
            NetworkIdentity.spawned.TryGetValue(New, out NetworkIdentity identity);
            targetObj = identity.gameObject.GetComponent<PlayerScript>().opponentPlayer;
        }
        GetComponent<Rigidbody>().velocity = targetObj.transform.position - transform.position;
    }

    void CmdSetPlayerId(uint id)
    {
        playerId = id;
    }

    void OnCollisionEnter(Collision collision)
    {

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 5, Color.white);
        }

        if (collision.gameObject.name == "mirrorSphere")
        {
            
            CmdSetPlayerId(0);
            Debug.Log("Do something here");
        }

        else if (collision.gameObject == targetObj)
        {
            NetworkServer.Destroy(gameObject);
            Debug.Log("Do something else here");
        }

        /*
        if (collision.relativeVelocity.magnitude > 2)
            audioSource.Play();
        */


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
