using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextMesh isPC = GetComponentInChildren<TextMesh>();
        //Check if the device running this is a desktop
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            //m_DeviceType = "Desktop";
            isPC.text = "Desktop";
        }

        //Check if the device running this is a handheld
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            isPC.text = "Handheld";
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
