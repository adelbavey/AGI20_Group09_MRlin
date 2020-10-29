using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;



public class G_Move : MonoBehaviour
{

    public GameObject cube;
    private Transform posCube;
    public float speed;

    private Quaternion prevAttitude;
    private Quaternion nextAttitude;

    private GUIStyle style = new GUIStyle();

    private const float variationConst = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        posCube = cube.GetComponent<Transform>();

        style.fontSize = 60;
        style.normal.textColor = new Color(1.0f,0,1.0f,1.0f);
        prevAttitude = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        rotateWand();

    }

    void OnGUI()
    {
        GUI.Label(new Rect(100, 100, 100, 100), $"Accelerometer x: {(int)(Input.acceleration.x * 10)}",style);
        GUI.Label(new Rect(100, 200, 100, 100), $"Accelerometer y: {(int)(Input.acceleration.y * 10)}",style);
        GUI.Label(new Rect(100, 300, 100, 100), $"Accelerometer z: {(int)(Input.acceleration.z * 10)}",style);
    }

    private void rotateWand ()
    {
        nextAttitude = GyroToUnity(DeviceRotation.Get());
        if (strongVariation(nextAttitude, prevAttitude))
        {
            transform.rotation = nextAttitude;
            prevAttitude = transform.rotation;
        }

    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(-q.x, -q.z, -q.y, q.w)/**Quaternion.Euler(0,0,0)*/;
    }

    private bool strongVariation (Quaternion a, Quaternion b)
    {
        if (Mathf.Abs(a.x - b.x) > variationConst) return true;
        if (Mathf.Abs(a.y - b.y) > variationConst) return true;
        if (Mathf.Abs(a.z - b.z) > variationConst) return true;
        if (Mathf.Abs(a.w - b.w) > variationConst) return true;
        
        return false;
    }
}
