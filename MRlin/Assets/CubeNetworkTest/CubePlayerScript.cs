using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

// The control options for this player
public class CubePlayerScript : NetworkBehaviour
{

    private GameObject cube;
    private Gyroscope m_gyro;

    // Start is called before the first frame update
    void Start()
    {
        // Find the inworld cube
        cube = GameObject.Find("SharedCube");

        // See if android, if it is, enable gyro controls
        m_gyro = Input.gyro;
        m_gyro.enabled = false;
        if (Application.platform == RuntimePlatform.Android)
        {
            m_gyro.enabled = true;
        }
        

    }

    // Update is called once per frame
    void Update()
    {

        // Use gyro if android, otherwise use keyboard
        if (m_gyro.enabled)
        {
            CmdQuat(m_gyro.attitude);
        }
        else
        {
            float moveX;
            moveX = Input.GetAxis("Horizontal");
            CmdRotate(moveX);
        }
                  
        //Debug.Log("Text: " + cube);
    }

    // Send new horizontal rotation to server (try commenting out [Command] and see what happens when using server+client and another client)
    [Command]
    void CmdRotate(float moveX)
    {
        cube.transform.Rotate(0, moveX, 0);
    }

    // Send new rotation to server
    [Command]
    void CmdQuat(Quaternion att)
    {
        cube.transform.rotation = att;
    }
}
