using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DH.D_MidiManager;

public class InstrumentManager : MonoBehaviour
{
    public static InstrumentManager instance;

    //�߰��� 

    //���� Ʈ���� ��ȣ�� �����ž�


    private void Awake()
    {
        instance = this;
    }
    //�Ǳ� ���� â���� Ŭ���� ���� �� 

    int trackNum;
    public void ChangeTrack(int num) {
        trackNum = num;
        Debug.Log("Ʈ����" + num + "���� ����Ǿ����ϴ�.");
        
        //�Ǳ� ��ư Ȱ��ȭ

    }

    //�Ǳ� ��ư�� ������ ��
    public void ChangeInstrument(int number) { //�� �Ǳ�� ��ä�ϰ� �ʹ�. 
        //trackNum �� �Ǳ⸦ number�� ��ü�Ѵ�.
        UIManager.instance.Tracks[trackNum].instrument = (Instruments)number;

        
    }




}
