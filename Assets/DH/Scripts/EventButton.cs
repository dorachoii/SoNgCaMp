using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class EventButton : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent action;
    public void OnPointerDown(PointerEventData eventData)
    {
        action.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        //JsonUtility.ToJson();
        //UnityEngine.Networking.UnityWebRequest.Post();
    }

  
    // Update is called once per frame
    void Update()
    {
        
    }


}
