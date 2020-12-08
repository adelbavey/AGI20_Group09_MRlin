using System.Collections.Generic;
using UnityEngine;



public class PlayerInput : MonoBehaviour
{
    [Tooltip("The distance of the rayCast to see if a object is Interactive")]
    public float DistanceRayCast = 2.5f;

    //Modify when adding a new pattern
    private Texture2D gestureTextO;
    private Texture2D gestureText7;
    private Texture2D gestureTextZ;
    private Texture2D gestureTextM;
    //private Texture2D gestureText_NEW_GESTURE;


    public float correctRate = 0.7f; 

    //Modify when adding a new pattern
    private Gesture gestureCO = new Gesture();
    private Gesture gestureC7 = new Gesture();
    private Gesture gestureCZ = new Gesture();
    private Gesture gestureCM = new Gesture();
    //private Gesture gestureC_NEW_GESTURE = new Gesture();


    // Gyro variables
    Gyroscope gyro;

    Quaternion initAttitude;
    Quaternion initRot;

    [Tooltip("Decide whether the gyro is activated or not")]
    public bool gyroControl = false;
    
    private bool usingGyro = false;
    
    private const float lowPassFilterFactor = 0.2f;  // Smooth slerp factor


    private RaycastHit hit;
    private GameObject target;
    [Tooltip("Initial color of the lineRenderer")]
    public Color c1 = Color.yellow;
    [Tooltip("Final color of the lineRenderer")]
    public Color c2 = Color.red;
    private GameObject lineGO;
    private LineRenderer lineRenderer;
    private int i;
    [Tooltip("The initial width of the lineRenderer")]
    public float widthI = 0.009f;
    [Tooltip("The final width of the lineRenderer")]
    public float widthF;
    [Tooltip("The distance that the lineRenderer is being create from the camera")]
    public float zline = 0.5f;
    [Tooltip("Material used in the lineRenderer")]
    public Material lineMaterial;
    [Tooltip("If you want to hide and lock cursor after doing a gesture")]
    public bool hideMouse = true;
    [Tooltip("The width of the line that is created when you do the gesture, We recommend one unit more than the brush used in the creation of the texture")]
    [SerializeField]
    private int widhtTextLine = 4;

    private void Start()
    {
        this.lineGO = new GameObject("Line");
        this.lineGO.AddComponent<LineRenderer>();
        this.lineRenderer = this.lineGO.GetComponent<LineRenderer>();
        this.lineRenderer.material = this.lineMaterial;
        lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;
        lineRenderer.startWidth = widthI;
        lineRenderer.endWidth = widthF;
        this.lineRenderer.positionCount = 0;

        //Modify when adding a new pattern
        this.gestureCO.setTextWidth(this.widhtTextLine);
        this.gestureC7.setTextWidth(this.widhtTextLine);
        this.gestureCZ.setTextWidth(this.widhtTextLine);
        this.gestureCM.setTextWidth(this.widhtTextLine);
        //this.gestureC_NEW_GESTURE.setTextWidth(this.widhtTextLine);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if(gyroControl)
            usingGyro = EnableGyro();

        Debug.Log(usingGyro);

        if (this.lineMaterial == null)
        {
            Debug.LogError("<b>Mouse Gesture Interpretation:</b> Line Material need a material to display colors in the drawning line");
        }
    }



    private void Update()
    {
        if (!usingGyro && Input.GetMouseButtonUp(0))  
        {
            if (this.hideMouse)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            patternRecognition();
        }

        else if(usingGyro && (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            patternRecognition();
        }

    }

    private void patternRecognition()
    {
        this.lineRenderer.positionCount = 0;
        this.i = 0;

        //Modify when adding a new pattern
        this.gestureCO.setIsGesturing(false);
        this.gestureC7.setIsGesturing(false);
        this.gestureCZ.setIsGesturing(false);
        this.gestureCM.setIsGesturing(false);
        //this.gestureC_NEW_GESTURE.setIsGesturing(false);


        //Modify when adding a new pattern
        Texture2D texture2DO = gestureCO.MapPattern();
        Texture2D texture2D7 = gestureC7.MapPattern();
        Texture2D texture2DZ = gestureCZ.MapPattern();
        Texture2D texture2DM = gestureCM.MapPattern();
        //Texture2D texture2D_NEW_GESTURE = gestureC_NEW_GESTURE.MapPattern();

        if (texture2DO && texture2D7 && texture2DZ && texture2DM)   // && texture2D_NEW_GESTURE          Modify when adding a new pattern
        {
            bool oneCorrect = false;
            // We get in num the accurate rate of the gesture drawing
            float num = this.gestureCO.TestPattern(texture2DO, this.gestureTextO);
            if (num > this.correctRate)
            {
                this.target.SendMessage("onGestureCorrectO", null, (SendMessageOptions)1);
                oneCorrect = true;
            }

            num = this.gestureC7.TestPattern(texture2D7, this.gestureText7);
            if (num > this.correctRate)
            {
                this.target.SendMessage("onGestureCorrect7", null, (SendMessageOptions)1);
                oneCorrect = true;
            }

            num = this.gestureCZ.TestPattern(texture2DZ, this.gestureTextZ);
            if (num > this.correctRate)
            {
                this.target.SendMessage("onGestureCorrectZ", null, (SendMessageOptions)1);
                oneCorrect = true;
            }

            num = this.gestureCM.TestPattern(texture2DM, this.gestureTextM);
            if (num > this.correctRate)
            {
                this.target.SendMessage("onGestureCorrectM", null, (SendMessageOptions)1);
                oneCorrect = true;
            }

            //Modify when adding a new pattern
            /*num = this.gestureC_NEW_GESTURE.TestPattern(texture2D_NEW_GESTURE, this.gestureText_NEW_GESTURE);
            if (num > this.correctRate)
            {
                this.target.SendMessage("onGestureCorrect_NEW_GESTURE", null, (SendMessageOptions)1);
                oneCorrect = true;
            }*/

            if (!oneCorrect)
            {
                this.target.SendMessage("onGestureWrong", null, (SendMessageOptions)1);
            }
        }
        else // Gesture was to small or short so it did not map the texture correctly
        {
            this.target.SendMessage("onGestureWrong", null, (SendMessageOptions)1);
        }

        //Modify when adding a new pattern
        this.gestureCO.mouseData = new List<Vector3>();
        this.gestureC7.mouseData = new List<Vector3>();
        this.gestureCZ.mouseData = new List<Vector3>();
        this.gestureCM.mouseData = new List<Vector3>();
        //this.gestureC_NEW_GESTURE.mouseData = new List<Vector3>();

        //Modify when adding a new pattern
        this.gestureTextO = null;
        this.gestureText7 = null;
        this.gestureTextZ = null;
        this.gestureTextM = null;
        //this.gestureText_NEW_GESTURE = null;
    }

    private void FixedUpdate()
    {
        float num1 = (float)Camera.main.pixelWidth * 0.5f;
        float num2 = (float)Camera.main.pixelHeight * 0.5f;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(num1, num2, 0.0f));
        Debug.DrawRay(ray.origin, ray.direction * this.DistanceRayCast, new Color(1f, 0.922f, 0.016f, 1f));
        if (Physics.Raycast(ray, out hit, DistanceRayCast))
        {
            if (hit.collider)
            {
                if (this.hit.collider.gameObject.CompareTag("Interactive"))
                {
                    this.target = this.hit.collider.gameObject;
                    target.SendMessage("setGestureConfig", null, (SendMessageOptions)1);
                    target.SendMessage("displayPattern", null, (SendMessageOptions)1);
                    Vector3 vector;
                    if(!usingGyro && Input.GetMouseButton(0))
                    {
                        Vector3 pos = Input.mousePosition;
                        //Modify when adding a new pattern
                        
                        gestureCO.setIsGesturing(true);
                        gestureCO.mouseData.Add(pos);

                        gestureC7.setIsGesturing(true);
                        gestureC7.mouseData.Add(pos);

                        gestureCZ.setIsGesturing(true);
                        gestureCZ.mouseData.Add(pos);

                        gestureCM.setIsGesturing(true);
                        gestureCM.mouseData.Add(pos);


                        //Debug.Log($"Input position: {pos}");
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        lineRenderer.positionCount = i + 1;
                        vector = new Vector3((float)pos.x, (float)pos.y, this.zline);
                        lineRenderer.SetPosition(this.i, Camera.main.ScreenToWorldPoint(vector));
                        ++i;
                        return;
                    }
                    else if(usingGyro && Input.touchCount > 0)
                    {
                        for (int i = 0; i < Input.touchCount; ++i)
                        {
                            if (Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary)
                            {
                                // Here it is necessary to multiply by some number between 70 and 130 so the gesture is big enough in order to be processed
                                Vector3 pos = this.GetComponent<GyroController>().getCursorPos()*100;
                                
                                //Modify when adding a new pattern
                                gestureCO.setIsGesturing(true);
                                gestureCO.mouseData.Add(pos);

                                gestureC7.setIsGesturing(true);
                                gestureC7.mouseData.Add(pos);

                                gestureCZ.setIsGesturing(true);
                                gestureCZ.mouseData.Add(pos);

                                gestureCM.setIsGesturing(true);
                                gestureCM.mouseData.Add(pos);


                                //Debug.Log($"Input position: {pos}");
                                lineRenderer.positionCount = i + 1;
                                vector = new Vector3((float)pos.x, (float)pos.y, this.zline);
                                lineRenderer.SetPosition(this.i, Camera.main.ScreenToWorldPoint(vector));
                                ++i;
                            }
                        }
                        return;
                    }
                }
            }

            else if (hit.collider.gameObject.CompareTag("Clickable"))
            {
                Debug.Log("Clickableee");
                this.target = this.hit.collider.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    this.target.SendMessage("onClick", null, (SendMessageOptions)1);
                }
            }

            
        }
    }

    //Modify when adding a new pattern
    public Gesture getGestureO()
    {
        return this.gestureCO;
    }
    public Gesture getGesture7()
    {
        return this.gestureC7;
    }
    public Gesture getGestureZ()
    {
        return this.gestureCZ;
    }
    public Gesture getGestureM()
    {
        return this.gestureCM;
    }

    /*public Gesture getGestureNEW_GESTURE()
    {
        return this.gestureC_NEW_GESTURE;
    }*/


    public void setTextureO(Texture2D t)
    {
        gestureTextO = t;
    }
    public void setTexture7(Texture2D t)
    {
        gestureText7 = t;
    }
    public void setTextureZ(Texture2D t)
    {
        gestureTextZ = t;
    }
    public void setTextureM(Texture2D t)
    {
            gestureTextM = t;
    }
    //Modify when adding a new pattern
    /*public void setTextureM(Texture2D t)
    {
        gestureTextM = t;
    }*/

    public void setCorrectRate(float c)
    {
        correctRate = c;
    }



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

}

