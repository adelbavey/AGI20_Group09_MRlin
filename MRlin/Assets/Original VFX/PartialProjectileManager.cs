using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class PartialProjectileManager : MonoBehaviour
{
    // speed is 0, projectile is moved within vfx graph.
    public float speed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // need Invoke to call Explode 3s after start.
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
    }

    /*
     * Self-destruct.
     */
    private void Explode()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject.Destroy(gameObject);
    }
}
