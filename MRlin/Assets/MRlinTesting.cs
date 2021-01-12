using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MRlinTesting : MonoBehaviour
{
    public bool testingMode;
    public Transform background;
    public int patternIndex;

    [SerializeField]
    private List<Sprite> sprites;

    private int[] patternIndices = { 0, 1, 0, 2, 3, 2, 1, 1, 3, 2, 0, 3 };
    private int currentIndex;

    private float currentTime;
    private bool isCountingTime;
    private int totalTrials;

    // Start is called before the first frame update
    void Start()
    {
        if (testingMode)
        {
            Debug.Log("User testing!");
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        patternIndex = 0;
        currentIndex = 0;
        gameObject.GetComponent<Image>().sprite = sprites[patternIndices[currentIndex]];

        isCountingTime = true;
        currentTime = 0.0f;

        totalTrials = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentTime);
        if (isCountingTime) currentTime += Time.deltaTime;
    }

    public void setPatternIndex(int a)
    {
        totalTrials += 1;
        patternIndex = a - 1;

        if (patternIndex == patternIndices[currentIndex])
        {
            background.GetComponent<Image>().color = Color.green;
            gameObject.GetComponent<Image>().color = Color.green;
            isCountingTime = false;
            StartCoroutine(goNext());
        }
        else
        {
            background.GetComponent<Image>().color = Color.red;
            gameObject.GetComponent<Image>().color = Color.red;
        }
    }

    private IEnumerator goNext()
    {
        yield return new WaitForSeconds(1.0f);
        if (currentIndex < 11)
        {
            currentIndex++;
            gameObject.GetComponent<Image>().sprite = sprites[patternIndices[currentIndex]];
            background.GetComponent<Image>().color = Color.white;
            gameObject.GetComponent<Image>().color = Color.white;
            isCountingTime = true;
        }
        else
        {
            Debug.Log("Average time:");
            Debug.Log(currentTime / 12.0f);
            Debug.Log("Success rate:");
            Debug.Log((12.0f / (float)totalTrials));
        }
    }
}
