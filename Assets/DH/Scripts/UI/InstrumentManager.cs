using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DH.D_MidiManager;

public class InstrumentManager : MonoBehaviour
{
    public static InstrumentManager instance;

    public Sprite[] InsImage;

    public enum Instype {
        Piano,
        ChromaticPercussion,
        Organ,
        Guitar,
        Bass,
        Strings,
        Ensemble,
        Brass,
        Reed,
        Pipe,
        SynthLead,
        SynthPad,
        SynthEffects,
        Ethnic,
        Percussive,
        SoundEffects
    }



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
            btn.img.sprite = iconlist[(int)retIns(i)];

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


    Image img;

    public void ChangeTrack(Image image)
    {
        img = image;
        //악기 버튼 활성화
        Inst.SetActive(true);
    }


    public void OutChange() {
        Inst.SetActive(false);
    }

    public GameObject Inst;


    public Sprite[] iconlist;

    //악기 버튼을 눌렀을 때
    public void ChangeInstrument(int number) { //이 악기로 교채하고 싶다. 

        //trackNum 의 악기를 number로 교체한다.
        UIManager.instance.currentTrack.instrument = (Instruments)number;
        //해당하는 버튼도 바꾸고싶다.

        if (UIManager.instance.currentTrack.number != 9)
            img.sprite = iconlist[(int)retIns(number)];

        //어떻게? 
        Inst.SetActive(false);


    }

    public static Instype retIns(int number) {

        if (number >= 1 && number <= 9)
        {
            return Instype.Piano;
        }
        else if (number >= 10 && number <= 16)
        {
            return Instype.ChromaticPercussion;
        }
        else if (number >= 17 && number <= 24)
        {
            return Instype.Organ;
        }
        else if (number >= 25 && number <= 32)
        {
            return Instype.Guitar;
        }
        else if (number >= 33 && number <= 40)
        {
            return Instype.Bass;
        }
        else if (number >= 41 && number <= 48)
        {
            return Instype.Strings;
        }
        else if (number >= 49 && number <= 56)
        {
            return Instype.Ensemble;
        }
        else if (number >= 57 && number <= 64)
        {
            return Instype.Brass;
        }
        else if (number >= 65 && number <= 72)
        {
            return Instype.Reed;
        }
        else if (number >= 73 && number <= 80)
        {
            return Instype.Pipe;
        }
        else if (number >= 81 && number <= 88)
        {
            return Instype.SynthLead;
        }
        else if (number >= 89 && number <= 96)
        {
            return Instype.SynthPad;
        }
        else if (number >= 97 && number <= 104)
        {
            return Instype.SynthEffects;
        }
        else if (number >= 105 && number <= 112)
        {
            return Instype.Ethnic;
        }
        else if (number >= 113 && number <= 119)
        {
            return Instype.Percussive;
        }
        else if (number >= 120 && number <= 127)
        {
            return Instype.SoundEffects;
        }

        return Instype.Piano;
    }

    
    



}
