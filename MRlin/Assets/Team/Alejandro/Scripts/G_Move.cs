using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class G_Move : MonoBehaviour
{

    public GameObject cube;
    private Transform posCube;
    public float speed;

    [SerializeField]
    public float alpha = 0.1f;
    private Quaternion prevAttitude;
    private Quaternion nextAttitude;
    private const float lowPassFilterFactor = 0.2f;
    private const int ITERATION_NUMBER = 200;

    private GUIStyle style = new GUIStyle();

    public GameObject cursor;
    public GameObject wandTip;

    Vector2 dxdy;
    Vector3 originalPos;
    Quaternion originalAttitude;
    float distanceToCastingPlane;

    private const float variationConst = 0.0f;

    
    private float dt = 0.0f;
    private float start_time;
        
    float phi_hat = 0.0f;
    float theta_hat = 0.0f;

    float bx,by,bz;

    void Awake()
    {
        start_time = Time.time;
        bx = 0; by = 0; bz = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        posCube = cube.GetComponent<Transform>();

        style.fontSize = 30;
        style.normal.textColor = new Color(1.0f,0,1.0f,1.0f);
        prevAttitude = Quaternion.identity;
        originalAttitude = GyroToUnity(DeviceRotation.Get()).normalized;
        originalPos = originalAttitude * Vector3.forward;
        distanceToCastingPlane = Mathf.Abs(wandTip.transform.position.z - cursor.transform.position.z);
        getGyroBias();
    }

    // Update is called once per frame
    void Update()
    {
        rotateWand();
        placeCursor();
        //Debug.Log("sdsd");
    }

    void OnGUI()
    {
        /*GUI.Label(new Rect(100, 100, 50, 50), $"Accelerometer x: {(int)(Input.acceleration.x * 10)}",style);
        GUI.Label(new Rect(100, 200, 50, 50), $"Accelerometer y: {(int)(Input.acceleration.y * 10)}",style);
        GUI.Label(new Rect(100, 300, 50, 50), $"Accelerometer z: {(int)(Input.acceleration.z * 10)}",style);*/
        GUI.Label(new Rect(100, 100, 50, 50), $"Gyro x: {(GyroToUnity(DeviceRotation.Get()).x)}",style);
        GUI.Label(new Rect(100, 200, 50, 50), $"Gyro y: {(GyroToUnity(DeviceRotation.Get()).y)}",style);
        GUI.Label(new Rect(100, 300, 50, 50), $"Gyro z: {(GyroToUnity(DeviceRotation.Get()).z)}",style);
        GUI.Label(new Rect(100, 400, 50, 50), $"Filter q: {prevAttitude}",style);
    }

    private void getGyroBias()
    {
        float timeIter = Time.time;

        for(int i = 0; i < ITERATION_NUMBER; i++)
        {
            bx += Input.acceleration.x;
            by += Input.acceleration.y;
            bz += Input.acceleration.z;
        }
        bx /= ITERATION_NUMBER;
        by /= ITERATION_NUMBER;
        bz /= ITERATION_NUMBER;
    }
    private void rotateWand ()
    {
        nextAttitude = GyroToUnity(DeviceRotation.Get());
        if (strongVariation(nextAttitude, prevAttitude))
        {   

            // WITH SPHERICAL LERPING
            // transform.rotation = Quaternion.Slerp(transform.rotation,
			// nextAttitude, lowPassFilterFactor);
            
            // WITHOUT SPHERICAL LERPING
            transform.rotation = complimentaryFilter(nextAttitude) /** nextAttitude*/;

            //transform.rotation = Quaternion.Euler(Input.gyro.rotationRateUnbiased);

            prevAttitude = transform.rotation;

        }

    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(-q.x, -q.z, -q.y, q.w)/**Quaternion.Euler(0,0,0)*/;
    }
    private void placeCursor()
    {
        Quaternion direction = GyroToUnity(DeviceRotation.Get());
        Vector3 v = direction * originalPos;
        Vector3 translation = v.normalized * distanceToCastingPlane;
        cursor.transform.position = wandTip.transform.position + new Vector3(translation.x,translation.y,distanceToCastingPlane);
    }
    
    public bool strongVariation (Quaternion a, Quaternion b)
    {
        if (Mathf.Abs(a.x - b.x) > variationConst) return true;
        if (Mathf.Abs(a.y - b.y) > variationConst) return true;
        if (Mathf.Abs(a.z - b.z) > variationConst) return true;
        if (Mathf.Abs(a.w - b.w) > variationConst) return true;
        
        return false;
    }

    public Quaternion complimentaryFilter(Quaternion att)
    {
        dt = Time.deltaTime;

        float phi_hat_acc = Input.acceleration.x;
        float theta_hat_acc = Input.acceleration.y;
        float p,q,r;
        p = att.eulerAngles.x;
        q = att.eulerAngles.y;
        r = att.eulerAngles.z;

        p -= bx;
        q -= by;
        r -= bz;
        
        // Calculate Euler angle derivates
        float phi_dot = p + Mathf.Sin(phi_hat) * Mathf.Tan(theta_hat) * q + Mathf.Cos(phi_hat) * Mathf.Tan(theta_hat) * r;
        float theta_dot = Mathf.Cos(phi_hat) * q - Mathf.Sin(phi_hat) * r;

        // Update complementary filter
        phi_hat = (1 - alpha) * (phi_hat + /*dt */ phi_dot) + alpha * phi_hat_acc;
        theta_hat = (1 - alpha) * (theta_hat + /*dt */ theta_dot) + alpha * theta_hat_acc;
        
        return Quaternion.Euler(phi_hat,theta_hat,0);
    }
}