using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 
/// </summary>
public class TouchableItem : MonoBehaviour, IPointerEnterHandler
{
    private Rigidbody body;

    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
    }	

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Check that this object has a Rigidbody component attached.
        if (!body) return;
        // eventData.delta contains the TrackerResult.linearVelocity data. 
        // You can used to know the force direction.
        Vector2 reducedForce = eventData.delta * 0.13f;
        body.AddForce(reducedForce, ForceMode.Impulse);
    }

   
}
