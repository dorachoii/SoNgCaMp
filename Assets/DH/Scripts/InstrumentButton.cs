using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DH;
using static DH.D_MidiManager;

//�Ǳ� ����
public struct _Instrument
{
    public Sprite sprite;
    public Instruments instruments;
}
public class InstrumentButton : MonoBehaviour
{

    //�Ǳ� ���� ȭ��
    public Canvas ChangeCanva;
    public void ChangeInstrument() {
        ChangeCanva.gameObject.SetActive(true);
        //BGM�� �����ϵ� ��¼�� �ϰ� 
        Debug.Log("Change!");
    }

    //�Ǳ� ���� ȭ��

    public void SelectInstrument() { 
    
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
