using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TestButton : MonoBehaviour,IPointerDownHandler
{


    //Ʈ�� ��ư Ŭ����

    //�� �Ǳ� ���� 
    public int InstruInfo;
    

    //�Ǳ� ����â
    public GameObject InstrumentSelectView;
    public void clickButton() {
        //��ư�� Ŭ���ϸ� �Ǳ� ����â�� ���̰�
        InstrumentSelectView.SetActive(true);

        //�� �Ǳ� ���� �ѱ��

        //InstrumentSelectView.Inst = new Inst();

         
        //���ʿ��� ���� ������ ���� ��..

    }


    public class Inst { 
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
