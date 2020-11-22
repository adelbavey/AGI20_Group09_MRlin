
using UnityEngine;

public class GyroController : MonoBehaviour 
{
     private bool gyroEnabled; 
     private UnityEngine.Gyroscope gyro;
     private GameObject GyroControl;
     private Quaternion rot;

	private Quaternion initAttitude;
	 private Quaternion initRot;

	
     private void Start()
     {
         Screen.sleepTimeout = SleepTimeout.NeverSleep;
 
         gyroEnabled = EnableGyro();
     }
     private bool EnableGyro()
     {
         if (SystemInfo.supportsGyroscope)
         {
            gyro = Input.gyro;
            gyro.enabled = true;

			initAttitude = Input.gyro.attitude;
			initRot = transform.rotation;
            return true;
         }
         return false;
     }
     private void Update()
     {

		 transform.rotation = GyroToUnity(initRot*(Quaternion.Inverse(initAttitude) * Input.gyro.attitude));
     }

	 private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(-q.x, -q.z, -q.y, q.w);
    }
}
