using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragChange : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public UnityEvent plusEvent;
    public UnityEvent minusEvent;



    public float changeDistance;
    bool IsDragging = false;
    Vector3 startPos;

    float distance;
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        IsDragging = true;
        //드래그를 했을때
        //처음 위치에서 현재 위치의 거리가 
        float distance = Input.mousePosition.x - startPos.x;

        //거리의 절댓값이 변화 지수보다 크다면 
        //배열도 써도 돠고 링크드 리스트도  써도 되고 

        //크고 length가 조건에 맞다면 
        if ((Mathf.Abs(distance) > changeDistance))
        {
            //변화를 해야한다.
            //증가
            if (distance > 0)
            {
                plusEvent.Invoke();
            }
            //감소
            if (distance < 0)
            {
                minusEvent.Invoke();
                
            }
            startPos = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
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
