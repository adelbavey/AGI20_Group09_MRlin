using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ProjectileCasting : MonoBehaviour
{
    [SerializeField]
    private AudioSource castingAudio;
    [SerializeField]
    private Transform SpellProjectile;
    [SerializeField]
    private VisualEffectAsset spellVFX1;
    [SerializeField]
    private VisualEffectAsset spellVFX2;
    [SerializeField]
    private VisualEffectAsset spellVFX3;

    private VisualEffect projectileVFX;
    private float playRate;
    private Vector3 shootingPath;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MageControllerNew>().OnCastingStarts += ProjectileCasting_OnCastingStarts;
        projectileVFX = SpellProjectile.GetChild(0).GetComponent<VisualEffect>();
    }

    private void ProjectileCasting_OnCastingStarts(object sender, MageControllerNew.OnCastingStartsEventArgs e)
    {
        if (e.spell == 1)
        {
            projectileVFX.visualEffectAsset = spellVFX1;
            playRate = 1.414f;
        }
        else if (e.spell == 2)
        {
            projectileVFX.visualEffectAsset = spellVFX2;
            playRate = 1.0f;
        }
        else if (e.spell == 3)
        {
            projectileVFX.visualEffectAsset = spellVFX3;
            playRate = 0.707f;
        }
        else if (e.spell == 4)
        {
            return;
        }
        else if (e.spell == 0)
        {
            return;
        }

        shootingPath = (e.islandTargetTransform.position + new Vector3(0, -1.0f, 0)) - e.wandTransform.position;
        projectileVFX.SetFloat("Distance", shootingPath.magnitude * 0.5f);
       
        Transform spellTransform = Instantiate(SpellProjectile, e.wandTransform.position, Quaternion.LookRotation(shootingPath));
        spellTransform.GetChild(0).GetComponent<VisualEffect>().playRate = this.playRate * 1.1f;
        
        castingAudio.volume = 0.7f;
        castingAudio.Play();
    }


    // Update is called once per frame
    void Update()
    {

    }


}