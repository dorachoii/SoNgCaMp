using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //일단? 
    //Action<_Instrument> action;


    //악기 선택 이벤트
    public Action SelectAction
    {
        get;
        set;
    }

    //악기 버튼을 눌렀을 떄 
    public void test() {

        //무언가를 실행한다. 근데? 악기 정보를 가지고 
        if (SelectAction != null) {
            SelectAction();
        }
    }


}
