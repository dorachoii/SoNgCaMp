using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

public class EJMidi2Note : MonoBehaviour
{
    MidiSequencer midiSequencer;   
    public string midiPath = "Midis/Groove.mid";
    int trackIdx;
    GameObject[] btns;

    EJNoteManager ejnotemanager;

    int instrumentIdx;
    
    void ClickDrumBtn()
    {
        instrumentIdx = 114;
    }

    void ClickPianoBtn()
    {
        instrumentIdx = 0;
    }

    void ClickGuitarBtn()
    {
        instrumentIdx = 24;
    }

    //�Ǳ����� ���� ����, UI�ٸ��� ��� �� �ֵ��� ���ؾ� ��.
    
    private void Awake()
    {
        midiSequencer.LoadMidi(midiPath, false);        
    }

    // Start is called before the first frame update
    void Start()
    {
        List<MidiEventInfo> midiEvents_selectedTrack = midiSequencer.midiAllNoteEventsDic[instrumentIdx];

        for (int i = 0; i < midiEvents_selectedTrack.Count; i++)
        {
            GameNoteInfo gameNoteInfo = new GameNoteInfo();

            gameNoteInfo.railIdx = SetRailIdx(midiEvents_selectedTrack[i],gameNoteInfo);
            gameNoteInfo.type = SetNoteType(midiEvents_selectedTrack, i, gameNoteInfo);
            gameNoteInfo.isLongNoteStart = false;
            gameNoteInfo.DRAG_release_idx = 0;
            gameNoteInfo.isNoteEnabled = false;

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
        switch (midiNote.pitch % 6)
        {
            case 0: return 0; break;
            case 1: return 1; break;
            case 2: return 2; break;
            case 3: return 3; break;
            case 4: return 4; break;
            case 5: return 5; break;
            default: return 0;
        }
    }

    int SetNoteType(List<MidiEventInfo> midiNote, int idx, GameNoteInfo gameNoteInfo)
    {
        if (midiNote[idx].length < 3f)
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
