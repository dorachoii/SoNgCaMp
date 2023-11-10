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
        //�巡�׸� ������
        //ó�� ��ġ���� ���� ��ġ�� �Ÿ��� 
        float distance = Input.mousePosition.x - startPos.x;

        //�Ÿ��� ������ ��ȭ �������� ũ�ٸ� 
        //�迭�� �ᵵ �°� ��ũ�� ����Ʈ��  �ᵵ �ǰ� 

        //ũ�� length�� ���ǿ� �´ٸ� 
        if ((Mathf.Abs(distance) > changeDistance))
        {
            //��ȭ�� �ؾ��Ѵ�.
            //����
            if (distance > 0)
            {
                plusEvent.Invoke();
            }
            //����
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
