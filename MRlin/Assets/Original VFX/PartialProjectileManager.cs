using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class PartialProjectileManager : MonoBehaviour
{

    public float speed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Explode", 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        speed = 0;
        //transform.position = other.transform.position - Vector3.forward * other.transform.localScale.z / 2;
        //Explode();
        //Invoke("Explode", 3.0f);
    }
    private void Explode()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(gameObject);
    }
}
