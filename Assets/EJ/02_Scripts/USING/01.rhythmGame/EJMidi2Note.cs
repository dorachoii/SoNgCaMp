using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

//01. 누른 악기 버튼에 따라 접근할 트랙의 인덱스를 정해준다.
//02. 트랙의 
//03.
//04.

[RequireComponent(typeof(AudioSource))]
public class EJMidi2Note : MonoBehaviour
{    
    MidiSequencer midiSequencer;   
    public EJNoteManager ejnotemanager;
    private StreamSynthesizer midiStreamSynthesizer;
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    private float[] sampleBuffer;

    public string midiPath = "ichumonandodemo.mid.txt";    

    int instrumentIdx = 0;      //default : piano 0

    GameObject[] btns;
    
    public void ClickDrumBtn()
    {
        instrumentIdx = 114;
        //prefab번호도 할당해야함.
    }

    public void ClickPianoBtn()
    {
        instrumentIdx = 0;
    }

    public void ClickGuitarBtn()
    {
        instrumentIdx = 24;
    }

    //악기정보 값에 따라, UI다르게 띄울 수 있도록 정해야 함.
    
    private void Awake()
    {

        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        midiStreamSynthesizer.LoadBank(bankFilePath);
        midiSequencer = new MidiSequencer(midiStreamSynthesizer);
        midiSequencer.LoadMidi(midiPath, false);
      
    }

    // Start is called before the first frame update
    void Start()
    {
        List<MidiEventInfo> midiEvents_selectedTrack = midiSequencer.midiAllNoteEventsDic[instrumentIdx];

        print("22222 instrument index는" + instrumentIdx + "이고 midiEvent의 개수는" + midiEvents_selectedTrack.Count + "만큼 담겼다");

        for (int i = 0; i < midiEvents_selectedTrack.Count; i++)
        {
            GameNoteInfo gameNoteInfo = new GameNoteInfo();

            gameNoteInfo.pitch = midiEvents_selectedTrack[i].pitch;
            gameNoteInfo.railIdx = SetRailIdx(midiEvents_selectedTrack[i],gameNoteInfo);
            gameNoteInfo.type = 0/*SetNoteType(midiEvents_selectedTrack, i, gameNoteInfo)*/;
            gameNoteInfo.isLongNoteStart = false;
            gameNoteInfo.DRAG_release_idx = 0;
            gameNoteInfo.isNoteEnabled = false;
            gameNoteInfo.time = midiEvents_selectedTrack[i].startTime*120;

            if(i == 0)
            {
                gameNoteInfo.time = 0.5f;
            }
            else
            {
                gameNoteInfo.time += midiEvents_selectedTrack[i - 1].length;

            }

            ejnotemanager.allGameNoteInfo.Add(gameNoteInfo);
        }

        for (int i = 0; i < ejnotemanager.gameNoteInfo_Rails.Length; i++)
        {
            ejnotemanager.gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < ejnotemanager.allGameNoteInfo.Count; i++)
        {
            ejnotemanager.gameNoteInfo_Rails[ejnotemanager.allGameNoteInfo[i].railIdx].Add(ejnotemanager.allGameNoteInfo[i]);
        }
    }

    //0번째 읽어와서 도레미파솔 쇼츠 내려오게 하기
    int SetRailIdx(MidiEventInfo midiNote, GameNoteInfo gameNoteInfo)
    {
        return (midiNote.pitch % 6);
    }

    int SetNoteType(List<MidiEventInfo> midiNote, int idx, GameNoteInfo gameNoteInfo)
    {
        if (midiNote[idx].length < 2f)
        {
            return 0;   //short Note
        }
        else
        {
            if (idx == 0)   // 첫 노트는 무조건
            {
                return 1;       //long Note
            }
            else
            {
                if (midiNote[idx].length != midiNote[idx].length)
                {
                    return 2;
                }
                else
                {
                    return 1;   //long Note 일 때 짝 지어줘야 하는 것 생각
                }
            }
        }

    }

    
}
