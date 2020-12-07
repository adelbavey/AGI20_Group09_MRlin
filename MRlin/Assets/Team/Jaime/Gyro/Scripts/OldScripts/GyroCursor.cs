using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroCursor : MonoBehaviour
{
    public GameObject cursor;
    public GameObject wandTip;

    Vector2 dxdy;
    Vector3 originalPos;
    Quaternion originalAttitude;
    float distanceToCastingPlane;

    private GUIStyle style = new GUIStyle();
    // Start is called before the first frame update
    void Start()
    {
        //originalPos = cursor.transform.position;
        originalAttitude = Input.gyro.attitude;
        originalPos = originalAttitude * Vector3.forward;
        distanceToCastingPlane = Mathf.Abs(wandTip.transform.position.z - cursor.transform.position.z); 
        //distanceToCastingPlane = 10;
    }

    // Update is called once per frame
    void Update()
    {
        //gyroConversion();
        //lerping();
        //cursor.transform.position = new Vector3(dxdy.x, dxdy.y + originalPos.y, originalPos.z);
        placeCursor();
    }

    private void gyroConversion () 
    {
        Vector3 deviation = (originalAttitude * Vector3.forward) - (Input.gyro.attitude * Vector3.forward);
        Vector3 angleWithoutY = new Vector3(deviation.x,0,deviation.z);
        Vector3 angleWithoutX = new Vector3(0,deviation.y,deviation.z);
        float alfaX = Vector3.Angle((originalAttitude * Vector3.forward), angleWithoutY);
        //alfaX = Quaternion.Angle(originalAttitude, Input.gyro.attitude);
        float alfaY = Vector3.Angle((originalAttitude * Vector3.forward), angleWithoutX);
        //alfaY = Quaternion.Angle(originalAttitude, Input.gyro.attitude);
        //float alfaY = Vector3.Angle((originalAttitude * Vector3.forward), angleWithoutX);
        dxdy.x = distanceToCastingPlane * Mathf.Sin(alfaX*Mathf.Deg2Rad) * Mathf.Cos(alfaX*Mathf.Deg2Rad);
        dxdy.y = distanceToCastingPlane * Mathf.Sin(alfaY*Mathf.Deg2Rad) * Mathf.Cos(alfaY*Mathf.Deg2Rad);
        
  	}

    private float remap (float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
    private void lerping () 
    {
        dxdy.x = remap(Input.gyro.attitude.eulerAngles.x, -1, 1, -2.5f, 2.5f)*4;
        dxdy.y = remap(Input.gyro.attitude.eulerAngles.y, -1, 1, -2.5f ,2.5f)*4;
  	}

    private void placeCursor()
    {
        Quaternion direction = Input.gyro.attitude;
        Vector3 v = direction * originalPos;
        cursor.transform.position = fixY(v.normalized * distanceToCastingPlane) ;
    }

    private Vector3 fixY (Vector3 v)
    {
        return new Vector3(-v.y,v.x,v.z);
    }

    void OnGUI()
    {
        style.fontSize = 60;
        style.normal.textColor = new Color(1.0f,0,1.0f,1.0f);
        GUI.Label(new Rect(300, 400, 50, 50), $"Cursor x: {dxdy.x}",style);
        GUI.Label(new Rect(300, 500, 50, 50), $"Cursor y: {dxdy.y}",style);
    }

    
   

}
