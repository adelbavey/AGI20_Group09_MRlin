using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onPhone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
