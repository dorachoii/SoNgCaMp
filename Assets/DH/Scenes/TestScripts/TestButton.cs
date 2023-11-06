using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TestButton : MonoBehaviour,IPointerDownHandler
{


    //트랙 버튼 클래스

    //내 악기 정보 
    public int InstruInfo;
    

    //악기 선택창
    public GameObject InstrumentSelectView;
    public void clickButton() {
        //버튼을 클릭하면 악기 선택창이 보이고
        InstrumentSelectView.SetActive(true);

        //내 악기 참조 넘기기

        //InstrumentSelectView.Inst = new Inst();

         
        //그쪽에서 뭔가 지지고 볶고 함..

    }


    public class Inst { 
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
