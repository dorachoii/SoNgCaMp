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

    //�ϴ�? 
    //Action<_Instrument> action;


    //�Ǳ� ���� �̺�Ʈ
    public Action SelectAction
    {
        get;
        set;
    }

    //�Ǳ� ��ư�� ������ �� 
    public void test() {

        //���𰡸� �����Ѵ�. �ٵ�? �Ǳ� ������ ������ 
        if (SelectAction != null) {
            SelectAction();
        }
    }


}
