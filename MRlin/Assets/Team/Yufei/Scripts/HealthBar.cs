using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    private int shieldLeft;
    private float smoothTransition = 3.0f;
    private int health;


    public void Awake()
    {
        health = 100;
        slider.maxValue = 100;
        slider.value = health;
        transform.GetComponent<Slider>().wholeNumbers = false;
    }

    public void setHealth(int health)
    {
        if (this.health != health)
        {
            this.health = health;
            StartCoroutine(Shake(0.5f, 5.0f));
        }
    }

    public void setShield(int num)
    {
        if (shieldLeft != num)
        {
            shieldLeft = num;
        }

        switch (shieldLeft)
        {
            case 0:
                transform.GetChild(0).GetComponent<Image>().enabled = false;
                transform.GetChild(1).GetComponent<Image>().enabled = false;
                transform.GetChild(2).GetComponent<Image>().enabled = false;
                break;
            case 1:
                transform.GetChild(0).GetComponent<Image>().enabled = true;
                transform.GetChild(1).GetComponent<Image>().enabled = false;
                transform.GetChild(2).GetComponent<Image>().enabled = false;
                break;
            case 2:
                transform.GetChild(0).GetComponent<Image>().enabled = true;
                transform.GetChild(1).GetComponent<Image>().enabled = true;
                transform.GetChild(2).GetComponent<Image>().enabled = false;
                break;
            case 3:
                transform.GetChild(0).GetComponent<Image>().enabled = true;
                transform.GetChild(1).GetComponent<Image>().enabled = true;
                transform.GetChild(2).GetComponent<Image>().enabled = true;
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (slider.value != (float)health)
        {
            slider.value = Mathf.Lerp(slider.value, (float)health, smoothTransition * Time.deltaTime);
            if (Mathf.Abs(slider.value - (float)health) < 0.1f) slider.value = (float)health;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
