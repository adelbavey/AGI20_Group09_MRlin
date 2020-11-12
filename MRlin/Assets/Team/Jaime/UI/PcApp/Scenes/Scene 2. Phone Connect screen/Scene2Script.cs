using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Scene2Script : MonoBehaviour
{
    public float interval;
    public GameObject spaceText;
    public GameObject particles;

    private ParticleSystem particlesSys;
    private float alphaValue;
    private bool goingDown;
    private TextMeshProUGUI text;
    private Transform pos;

    private void Awake()
    {
        particlesSys = particles.GetComponent<ParticleSystem>();
        text = spaceText.GetComponent<TextMeshProUGUI>();
        if (spaceText == null) Debug.Log("Error. No spaceText attached");
        if (text == null) Debug.Log("Error. No text attached");
        alphaValue = 255;
        goingDown = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        particles.SetActive(true);
        InvokeRepeating("FlashText", 0, interval);
    }

    void FlashText()
    {
        if (goingDown)
        {
            if (alphaValue == 60)
            {
                alphaValue+=15;
                goingDown = false;
            }
            alphaValue-=15;
        }
        else
        {
            if (alphaValue == 255)
            {
                alphaValue-=15;
                goingDown = true;
            }
            alphaValue+=15;
        }
        if(text == null)
        {
            Debug.Log("Error. Not text attached");
        }
       
        text.alpha = alphaValue/255;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
