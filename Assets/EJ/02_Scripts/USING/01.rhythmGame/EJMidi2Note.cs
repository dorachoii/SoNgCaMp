using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

//01. ���� �Ǳ� ��ư�� ���� ������ Ʈ���� �ε����� �����ش�.
//02. Ʈ���� 
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
        //prefab��ȣ�� �Ҵ��ؾ���.
    }

    public void ClickPianoBtn()
    {
        instrumentIdx = 0;
    }

    public void ClickGuitarBtn()
    {
        instrumentIdx = 24;
    }

    //�Ǳ����� ���� ����, UI�ٸ��� ��� �� �ֵ��� ���ؾ� ��.
    
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

        print("22222 instrument index��" + instrumentIdx + "�̰� midiEvent�� ������" + midiEvents_selectedTrack.Count + "��ŭ ����");

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

    //0��° �о�ͼ� �������ļ� ���� �������� �ϱ�
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
            if (idx == 0)   // ù ��Ʈ�� ������
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
                    return 1;   //long Note �� �� ¦ ������� �ϴ� �� ����
                }
            }
        }

    }

    
}
