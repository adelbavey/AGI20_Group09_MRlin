using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// When attaching a new texture for the pattern in Unity, make sure that Read/Write is enabled. 
// If you have any problem just tell me (Jaime) because there is a couple of things to take into account.

public class MRlinGestureScript : MonoBehaviour 
{
	// Modidy when adding a new pattern
	public Texture2D textSpellO;
	public Texture2D textSpellZ;
	public Texture2D textSpell7;
	public Texture2D textSpellM;
	//public Texture2D textSpellNEW_GESTURE;

	public Texture2D display;
	bool iniCount = false;
	public float timer = 1.0f;
	GameObject image;

    public GameObject player;


	// Use this for initialization
	void Start () 
	{
		image = GameObject.Find ("Display");
        Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update () {
        if (image == null)
        {
            return;
        }
		else if (iniCount) {
			timer -= Time.deltaTime;
			if (timer <= 0.0f) {

				image.GetComponent<RawImage>().enabled = false;
				iniCount = false;
			}
		}
	}


    //This method is needed for the Gesture to run, here you set Texture for recognition and can set the sucess rate needed to be achieved
	void setGestureConfig()
	{
        if(player == null)
        {
            Debug.LogError("<b>GestureScript:</b> The code need a reference to the object that has the playerInput script");
            return;
        }

		// Modify when adding a new pattern
		player.GetComponent<PlayerInput>().setTextureO(textSpellO);
		player.GetComponent<PlayerInput>().setTexture7(textSpell7);
		player.GetComponent<PlayerInput>().setTextureZ(textSpellZ);
		player.GetComponent<PlayerInput>().setTextureM(textSpellM);
		//player.GetComponent<PlayerInput>().setTextureNEW_GESTURE(textSpellNEW_GESTURE);
		
		//player.GetComponent<PlayerInput>().setCorrectRate(0.7f);
	}

    //This method happen when the player look to the object and can be used for a variety of things, in this example we use to display the Pattern
	void displayPattern()
	{
        if (image == null)
        {
            return;
        }
		else if (timer > 0.0f) {
			iniCount = true;
			image.GetComponent<RawImage> ().texture = display;
			image.GetComponent<RawImage> ().enabled = true;
		}
	}

    //These methods happen when the player succesfully do the correspondant gesture
    void onGestureCorrectO()
	{
		Debug.Log("Gesture   correct   O");
        this.gameObject.GetComponent<Renderer>().material.color = new Color(0,255,255,100);
	}
	void onGestureCorrect7()
	{
		Debug.Log("Gesture   correct   7");
		this.gameObject.GetComponent<Renderer>().material.color = new Color(0, 255, 0, 100);
	}
	void onGestureCorrectZ()
	{
		Debug.Log("Gesture   correct   Z");
		this.gameObject.GetComponent<Renderer>().material.color = new Color(255,0,255,100);
	}
	void onGestureCorrectM()
	{
		Debug.Log("Gesture   correct   M");
		this.gameObject.GetComponent<Renderer>().material.color = Color.black;
	}

	// Modify when adding a new pattern
	/*void onGestureCorrectNEW_GESTURE()
	{
		Debug.Log("GestureCorrectNEW_GESTURE");
		this.gameObject.GetComponent<Renderer>().material.color = Color.black;
	}*/

	//This method happen when the player failed in the gesture
	void onGestureWrong()
	{
		Debug.Log("Gesture   Wrong");
		this.gameObject.GetComponent<Renderer>().material.color = new Color(255, 0, 0, 100);
	}


    /* This method can be used if you wanna create a clickable object only, with no gesture, 
       use this method to detect when the object is click, the object must have the tag "Clickable"
    void onClick ()
    {

    }*/
}
