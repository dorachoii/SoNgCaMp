using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DH;
using static DH.D_MidiManager;

//악기 구조
public class _Instrument
{
    public Sprite sprite; //악기 이미지 
    public Instruments instruments; //어떤 악기인지
    public int Channel;
    
}
public class InstrumentButton : MonoBehaviour
{
    //악기 변경 화면
    public Canvas ChangeCanva;
    public void ChangeInstrument() {
        ChangeCanva.gameObject.SetActive(true);
        //BGM을 변경하든 어쩌구 하고 
        Debug.Log("Change!");
    }

    //악기 선택 화면
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
