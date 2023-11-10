using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class EventButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler,IPointerExitHandler,IPointerClickHandler

{
    public UnityEvent action;

    public void OnPointerClick(PointerEventData eventData)
    {
        action.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //action.Invoke();sss

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

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("¿Ö¾ÈµÊ?");
        //action.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }
}
