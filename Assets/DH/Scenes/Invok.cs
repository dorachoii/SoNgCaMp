using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Invok : MonoBehaviour,IPointerDownHandler
{

    public GameObject test;
    public UnityEvent even;

    public void OnPointerDown(PointerEventData eventData)
    {
        even.Invoke();
    }

    public void invo() {

        Debug.Log(test);
        Debug.Log(test.GetComponent<Transform>().position);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
