
/**********************************
 *********Stable Version***********
**********************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandGyroController : MonoBehaviour
{
    private bool gyroEnabled;
    private UnityEngine.Gyroscope gyro;
    private GameObject GyroControl;
    private Quaternion rot;

    private Quaternion initAttitude;
    private Quaternion initRot;
    public PlayerScript playerScript;



    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        gyroEnabled = EnableGyro();
    }
    private bool EnableGyro()
    {
        //if (SystemInfo.supportsGyroscope)
        //{
        
            gyro = Input.gyro;
            gyro.enabled = true;
        if (playerScript != null)
        {
            if (playerScript.connectedPlayer != null) initAttitude = playerScript.connectedPlayer.GetComponent<PlayerScript>().attitude;//Input.gyro.attitude;
            else initAttitude = Input.gyro.attitude;
        }
        else initAttitude = Input.gyro.attitude;
        initRot = transform.rotation;
            return true;
        
        return true;
        //}
        //return false;
    }
    private void Update()
    {
        if(playerScript != null)
        {
            if (playerScript.connectedPlayer != null) transform.rotation = GyroToUnity(initRot * (Quaternion.Inverse(initAttitude) * playerScript.connectedPlayer.GetComponent<PlayerScript>().attitude/*Input.gyro.attitude*/));
            else transform.rotation = GyroToUnity(initRot * (Quaternion.Inverse(initAttitude) * Input.gyro.attitude));
        }
        else transform.rotation = GyroToUnity(initRot * (Quaternion.Inverse(initAttitude) * Input.gyro.attitude));


    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(-q.x, -q.z, -q.y, q.w);
    }
}