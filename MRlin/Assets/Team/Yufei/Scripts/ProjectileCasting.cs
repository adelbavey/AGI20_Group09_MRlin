using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCasting : MonoBehaviour
{
    [SerializeField] private Transform SpellProjectile;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MageControllerNew>().OnCastingStarts += ProjectileCasting_OnCastingStarts;
    }

    private void ProjectileCasting_OnCastingStarts(object sender, MageControllerNew.OnCastingStartsEventArgs e)
    {
        Transform spellTransform = Instantiate(SpellProjectile, e.wandTransform.position, Quaternion.identity);
        spellTransform.GetComponent<SpellProjectileNew>().SetUp(e.wandTransform, e.runeElements, transform);
    }


    // Update is called once per frame
    void Update()
    {

    }
}