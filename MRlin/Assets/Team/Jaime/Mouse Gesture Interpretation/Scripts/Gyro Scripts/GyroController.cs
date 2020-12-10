
using UnityEngine;
using System.Threading;
using System;

public class GyroController : MonoBehaviour 
{

    private bool gyroEnabled; 
    private Gyroscope gyro;

	private Quaternion initAttitude;
	private Quaternion initRot;

    private Quaternion currentPos;

    public GameObject cursor;
    //public GameObject wandTip;

    Quaternion originalAttitude;
    Vector3 originalPos;
    Vector3 currentCursorPos;
    float distanceToCastingPlane;

    // Smooth slerp factor
    private const float lowPassFilterFactor = 0.2f;


    //-----------------------------------------------------------------------------------------
    // Filter constants:
    //-----------------------------------------------------------------------------------------

    public const float DEFAULT_Q = 0.001f;
    public const float DEFAULT_R = 0.01f;

    public const float DEFAULT_P = 1;

    //-----------------------------------------------------------------------------------------
    // Filter private fields:
    //-----------------------------------------------------------------------------------------

    private float q = DEFAULT_Q;
    private float r = DEFAULT_R;
    private float p = DEFAULT_P;
    private Vector4 x;
    private float k;

    private void Start()
     {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
 
        gyroEnabled = EnableGyro();
        Debug.Log(gyroEnabled);

        originalPos = originalAttitude * Vector3.forward;
        distanceToCastingPlane = Mathf.Abs(this.transform.position.z - cursor.transform.position.z);

        x = QuaternionToVector(gyro.attitude);
    }


    // Enables gyro use and reset the original offset
     private bool EnableGyro()
     {
         if (SystemInfo.supportsGyroscope)
         {
            gyro = Input.gyro;
            gyro.enabled = true;

			initAttitude = gyro.attitude;
			initRot = transform.rotation;
            return true;
         }
         return false;
     }

    // Resets the offset. This should be called from a reset button
    public void resetGyroOffset()
    {
        initAttitude = gyro.attitude;
        initRot = transform.rotation;
    }


     private void Update()
     {
        // Filter measurement.
        {
            k = (p + q) / (p + q + r);
            p = r * (p + q) / (r + p + q);
        }

        // filter result back into calculation.
        Vector4 result = x + (QuaternionToVector(gyro.attitude) - x) * k;
        x = result;

        currentPos = Quaternion.Slerp(transform.rotation,GyroToUnity(initRot *
             (Quaternion.Inverse(initAttitude) * VectorToQuaternion(result))),lowPassFilterFactor);
        //transform.rotation = currentPos;

        placeCursor();

    }

    // Returns the current device rotation after having processed the attitude
    public Quaternion getQuatRot()
    {
        return currentPos;
    }

    // Return the position of the cursor in the screen
    public Vector3 getCursorPos()
    {
        return currentCursorPos;
    }

    // Moves the cursor according to the gyro values.
    private void placeCursor()
    {
        Quaternion direction = /*currentPos;*/GyroToUnity(DeviceRotation.Get()); // Works better with DeviceRotation.Get(). currentPos would need a scalar multiplication.
        Vector3 v = direction * originalPos;
        Vector3 translation = v.normalized * distanceToCastingPlane;
        cursor.transform.position = this.transform.position + new Vector3(translation.x, translation.y, distanceToCastingPlane);
        currentCursorPos = cursor.transform.position;
    }

    // Just transform Quaternion to Vector4 so it is easier to add, multiply, substract... with the quaternion.
    // Otherwise, it would be necessary to implement the correspondant Quaternion operations
    Vector4 QuaternionToVector(Quaternion a)
    {
        return new Vector4(a.x, a.y, a.z, a.w);
    }
    // Vector4 to Quaternion to keep working with it.
    Quaternion VectorToQuaternion(Vector4 a)
    {
        return new Quaternion(a.x, a.y, a.z, a.w);
    }

    private static Quaternion GyroToUnity(Quaternion q)
     {
        return new Quaternion(-q.x, -q.z, -q.y, q.w);
     }


}
