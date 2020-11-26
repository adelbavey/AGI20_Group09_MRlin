using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCasting : MonoBehaviour
{
    public AudioSource castingAudio;
    [SerializeField] private Transform SpellProjectile;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MageControllerNew>().OnCastingStarts += ProjectileCasting_OnCastingStarts;
    }

    private void ProjectileCasting_OnCastingStarts(object sender, MageControllerNew.OnCastingStartsEventArgs e)
    {
        Transform spellTransform = Instantiate(SpellProjectile, e.wandTransform.position, Quaternion.Euler(2, 35, 0));
        castingAudio.volume = 0.8f;
        castingAudio.Play();
       
    }


    // Update is called once per frame
    void Update()
    {

    }


}