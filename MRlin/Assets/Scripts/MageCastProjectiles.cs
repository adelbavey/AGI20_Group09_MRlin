using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageCastProjectiles : MonoBehaviour
{
    [SerializeField] private Transform SpellProjectile;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MageController>().OnCastingStarts += MageCastProjectiles_OnCastingStarts;
        // ignore collision between spell and mage
        Physics.IgnoreCollision(SpellProjectile.GetComponent<Collider>(), transform.GetComponent<Collider>());
    }

    private void MageCastProjectiles_OnCastingStarts(object sender, MageController.OnCastingStartsEventArgs e)
    {
        Transform spellTransform = Instantiate(SpellProjectile, e.wandTransform.position, Quaternion.identity);
        spellTransform.GetComponent<SpellProjectile>().SetUp(e.wandTransform, e.runeElements);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
