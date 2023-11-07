using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DH.D_MidiManager;

public class InstrumentManager : MonoBehaviour
{
    public static InstrumentManager instance;

    public Transform Content;
    public GameObject InstruBtn;
    //중간자 

    //나는 트랙의 번호를 받을거야


    private void Awake()
    {
        instance = this;

        for (int i = 0; i < (int)Instruments.Gunshot; i++) {
            Debug.Log(i + "번 반복");
            GameObject go = Instantiate(InstruBtn, Content);
            ChangeInstButton btn =  go.GetComponent<ChangeInstButton>();
            btn.ChangeChanel = i;
            btn.text.text = (Instruments)i + "";
        
        }

    }
    //악기 선택 창에서 클릭을 했을 시 

    int trackNum;
    public void ChangeTrack(int num) {
        trackNum = num;
        Debug.Log("트랙이" + num + "으로 변경되었습니다.");

        //악기 버튼 활성화
        Inst.SetActive(true);
        
        
        
    }

    public GameObject Inst;

    //악기 버튼을 눌렀을 때
    public void ChangeInstrument(int number) { //이 악기로 교채하고 싶다. 
        //trackNum 의 악기를 number로 교체한다.
        UIManager.instance.Tracks[trackNum].instrument = (Instruments)number;

        Inst.SetActive(false);

        
    }

    
    



}
