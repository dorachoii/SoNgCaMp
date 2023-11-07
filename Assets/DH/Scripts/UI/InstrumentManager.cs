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
    //�߰��� 

    //���� Ʈ���� ��ȣ�� �����ž�


    private void Awake()
    {
        instance = this;

        for (int i = 0; i < (int)Instruments.Gunshot; i++) {
            Debug.Log(i + "�� �ݺ�");
            GameObject go = Instantiate(InstruBtn, Content);
            ChangeInstButton btn =  go.GetComponent<ChangeInstButton>();
            btn.ChangeChanel = i;
            btn.text.text = (Instruments)i + "";
        
        }

    }
    //�Ǳ� ���� â���� Ŭ���� ���� �� 

    int trackNum;
    public void ChangeTrack(int num) {
        trackNum = num;
        Debug.Log("Ʈ����" + num + "���� ����Ǿ����ϴ�.");

        //�Ǳ� ��ư Ȱ��ȭ
        Inst.SetActive(true);
        
        
        
    }

    public GameObject Inst;

    //�Ǳ� ��ư�� ������ ��
    public void ChangeInstrument(int number) { //�� �Ǳ�� ��ä�ϰ� �ʹ�. 
        //trackNum �� �Ǳ⸦ number�� ��ü�Ѵ�.
        UIManager.instance.Tracks[trackNum].instrument = (Instruments)number;

        Inst.SetActive(false);

        
    }

    
    



}
