using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRlinTesting : MonoBehaviour
{
    public bool testingMode;
    public Transform background;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
