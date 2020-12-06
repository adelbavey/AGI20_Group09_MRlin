using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ProjectileCasting : MonoBehaviour
{
    public AudioSource castingAudio;
    [SerializeField]
    private Transform SpellProjectile;
    [SerializeField]
    private VisualEffectAsset spellVFX1;
    [SerializeField]
    private VisualEffectAsset spellVFX2;
    [SerializeField]
    private VisualEffectAsset spellVFX3;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MageControllerNew>().OnCastingStarts += ProjectileCasting_OnCastingStarts;
    }

    private void ProjectileCasting_OnCastingStarts(object sender, MageControllerNew.OnCastingStartsEventArgs e)
    {
        if (e.spellElement == 1)
        {
            SpellProjectile.GetChild(0).GetComponent<VisualEffect>().visualEffectAsset = spellVFX1;
        }
        else if (e.spellElement == 2)
        {
            SpellProjectile.GetChild(0).GetComponent<VisualEffect>().visualEffectAsset = spellVFX2;
        }
        else if (e.spellElement == 3)
        {
            SpellProjectile.GetChild(0).GetComponent<VisualEffect>().visualEffectAsset = spellVFX3;
        }
        Transform spellTransform = Instantiate(SpellProjectile, e.wandTransform.position, Quaternion.Euler(2, 35, 0));
        castingAudio.volume = 0.8f;
        castingAudio.Play();
    }


    // Update is called once per frame
    void Update()
    {

    }


}