using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    public GameObject effect;
    public GameObject firePoint;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire(effect);
        }
    }

    void Fire(GameObject effect)
    {
        GameObject shot = Instantiate(effect, firePoint.transform.position, Quaternion.identity);
    }
}
