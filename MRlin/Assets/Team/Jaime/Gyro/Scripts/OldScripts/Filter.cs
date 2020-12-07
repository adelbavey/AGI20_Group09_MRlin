using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : MonoBehaviour
{

    private const float alpha = 0.1f;
    private float dt = 0.0f;
    private float start_time = Time.time;
        
    float phi_hat = 0.0f;
    float theta_hat = 0.0f;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Quaternion complimentaryFilter(Quaternion att)
    {
        dt = Time.time - start_time;
        start_time = Time.time;

        float phi_hat_acc = Input.acceleration.x;
        float theta_hat_acc = Input.acceleration.y;
        float p,q,r;
        p = att.eulerAngles.x;
        q = att.eulerAngles.y;
        r = att.eulerAngles.z;
        
        // Calculate Euler angle derivates
        float phi_dot = p + Mathf.Sin(phi_hat) * Mathf.Tan(theta_hat) * q + Mathf.Cos(phi_hat) * Mathf.Tan(theta_hat) * r;
        float theta_dot = Mathf.Cos(phi_hat) * q - Mathf.Sin(phi_hat) * r;

        // Update complementary filter
        phi_hat = (1 - alpha) * (phi_hat + dt * phi_dot) + alpha * phi_hat_acc;
        theta_hat = (1 - alpha) * (theta_hat + dt * theta_dot) + alpha * theta_hat_acc;
        
        return Quaternion.Euler(phi_hat,theta_hat,0);
    }

    public void kalman()
    {
        /*
        //Average time sleep
        float dt = 0.0185f;

        //Convert gyroscope to radians
        float Gx_rad = Input.gyro.attitude.x*Mathf.PI/180.0f;
        float Gy_rad = Input.gyro.attitude.y*Mathf.PI/180.0f;
        float Gz_rad = Input.gyro.attitude.z*Mathf.PI/180.0f;

        //Accelerometer only
        float phi_hat_acc = Mathf.Atan2(Input.acceleration.y,Mathf.Sqrt(Mathf.Pow(Input.acceleration.x,2)+Mathf.Pow(Input.acceleration.z,2)));
        float theta_hat_acc = Mathf.Atan2(-Input.acceleration.x,Mathf.Sqrt(Mathf.Pow(Input.acceleration.y,2)+Mathf.Pow(Input.acceleration.z,2)));

        //Gyroscope only
        float phi_hat_gyr  =  
        */
    }
}
