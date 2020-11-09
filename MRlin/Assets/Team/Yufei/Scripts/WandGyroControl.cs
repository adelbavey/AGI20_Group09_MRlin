using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WandGyroControl : MonoBehaviour
{
    public GameObject cube;
    private Transform posCube;
    public float speed;

    private Quaternion prevAttitude;
    private Quaternion nextAttitude;
    private const float lowPassFilterFactor = 0.2f;

    private GUIStyle style = new GUIStyle();

    public GameObject cursor;
    public GameObject wandTip;

    Vector2 dxdy;
    Vector3 originalPos;
    Quaternion originalAttitude;
    float distanceToCastingPlane;

    private const float variationConst = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        posCube = cube.GetComponent<Transform>();

        style.fontSize = 60;
        style.normal.textColor = new Color(1.0f, 0, 1.0f, 1.0f);
        prevAttitude = Quaternion.identity;
        originalAttitude = GyroToUnity(DeviceRotation.Get()).normalized;
        originalPos = originalAttitude * Vector3.forward;
        distanceToCastingPlane = Mathf.Abs(wandTip.transform.position.z - cursor.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        rotateWand();
        placeCursor();
    }

    void OnGUI()
    {
        /*GUI.Label(new Rect(100, 100, 50, 50), $"Accelerometer x: {(int)(Input.acceleration.x * 10)}",style);
        GUI.Label(new Rect(100, 200, 50, 50), $"Accelerometer y: {(int)(Input.acceleration.y * 10)}",style);
        GUI.Label(new Rect(100, 300, 50, 50), $"Accelerometer z: {(int)(Input.acceleration.z * 10)}",style);*/
        //GUI.Label(new Rect(100, 100, 50, 50), $"Gyro x: {(GyroToUnity(DeviceRotation.Get()).x)}", style);
        //GUI.Label(new Rect(100, 150, 50, 50), $"Gyro y: {(GyroToUnity(DeviceRotation.Get()).y)}", style);
        //GUI.Label(new Rect(100, 200, 50, 50), $"Gyro z: {(GyroToUnity(DeviceRotation.Get()).z)}", style);
        //GUI.Label(new Rect(100, 250, 50, 50), $"Gyro w: {(GyroToUnity(DeviceRotation.Get()).w)}", style);

    }

    private void rotateWand()
    {
        nextAttitude = GyroToUnity(DeviceRotation.Get());
        if (strongVariation(nextAttitude, prevAttitude))
        {

            // WITH SPHERICAL LERPING
            // transform.rotation = Quaternion.Slerp(transform.rotation,
            // nextAttitude, lowPassFilterFactor);

            // WITHOUT SPHERICAL LERPING
            transform.rotation = nextAttitude;

            //transform.rotation = Quaternion.Euler(Input.gyro.rotationRateUnbiased);

            prevAttitude = transform.rotation;
        }
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        Vector3 tempEuler = new Vector3(0, 0, 0);
        Quaternion tempQuaternion = new Quaternion(-q.x, -q.z, -q.y, q.w)/**Quaternion.Euler(0,0,0)*/;
        tempEuler = tempQuaternion.eulerAngles;
        return Quaternion.Euler(tempEuler.z, tempEuler.y, -tempEuler.x);
    }
    private void placeCursor()
    {
        Quaternion direction = GyroToUnity(DeviceRotation.Get());
        Vector3 v = direction * originalPos;
        Vector3 translation = v.normalized * distanceToCastingPlane;
        cursor.transform.position = wandTip.transform.position + new Vector3(translation.x, translation.y, distanceToCastingPlane);
    }

    public bool strongVariation(Quaternion a, Quaternion b)
    {
        if (Mathf.Abs(a.x - b.x) > variationConst) return true;
        if (Mathf.Abs(a.y - b.y) > variationConst) return true;
        if (Mathf.Abs(a.z - b.z) > variationConst) return true;
        if (Mathf.Abs(a.w - b.w) > variationConst) return true;

        return false;
    }
}
