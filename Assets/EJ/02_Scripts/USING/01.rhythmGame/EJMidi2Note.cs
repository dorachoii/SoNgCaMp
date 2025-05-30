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

    GameNoteInfo gameNoteInfo;


    // Start is called before the first frame update
    void Start()
    {
        List<MidiEventInfo> midiEvents_selectedTrack = midiSequencer.midiAllNoteEventsDic[instrumentIdx];

        print("111 instrument index는" + instrumentIdx + "이고 midiEvent의 개수는" + midiEvents_selectedTrack.Count + "만큼 담겼다");

        //i= midiEvent의 index
        for (int i = 0; i < midiEvents_selectedTrack.Count; i++)
        {
            bool isLongNote = false;
            bool isDragNote = false;


            // shortNote
            if (midiEvents_selectedTrack[i].length * 960 < 480)
            {
                #region 멘토님 

                //bool isDrag = false;

                //if(i < midiEvents_selectedTrack[i].length - 1)
                //{
                //    if(midiEvents_selectedTrack[i + 2].pitch > midiEvents_selectedTrack[i + 1].pitch && midiEvents_selectedTrack[i + 1].pitch > midiEvents_selectedTrack[i ].pitch)
                //    {
                //        if(i == 1 && ejnotemanager.allGameNoteInfo[i - 1].type != (int)GameNoteType.DRAG_LEFT)
                //        {
                //            //드래그 되야함
                //            isDrag = true;
                //        }
                //        else if(i > 1 && ejnotemanager.allGameNoteInfo[i - 1].type != (int)GameNoteType.DRAG_LEFT &&
                //            ejnotemanager.allGameNoteInfo[i - 2].type != (int)GameNoteType.DRAG_LEFT)
                //        {
                //            //드래그 되야함
                //            isDrag = true;
                //        }
                //    }
                //}

                //if(isDrag)
                //{
                //    isDragNote = true;
                //    gameNoteInfo = new GameNoteInfo();

                //    gameNoteInfo.pitch = midiEvents_selectedTrack[i].pitch;
                //    gameNoteInfo.railIdx = 2;
                //    gameNoteInfo.type = (int)GameNoteType.DRAG_LEFT;
                //    gameNoteInfo.DRAG_release_idx = 0;
                //    gameNoteInfo.isNoteEnabled = false;
                //    gameNoteInfo.time = midiEvents_selectedTrack[i].startTime * 120;
                //}
                //else
                //{
                //    gameNoteInfo = new GameNoteInfo();

                //    gameNoteInfo.pitch = midiEvents_selectedTrack[i].pitch;
                //    gameNoteInfo.railIdx = SetRailIdx(midiEvents_selectedTrack[i], gameNoteInfo);
                //    gameNoteInfo.type = (int)GameNoteType.SHORT;

                //    //gameNoteInfo.isLongNoteStart = false;
                //    //gameNoteInfo.DRAG_release_idx = 0;
                //    gameNoteInfo.isNoteEnabled = false;
                //    gameNoteInfo.time = midiEvents_selectedTrack[i].startTime * 120;


                //    //전전이 drag다 라면 기다면  empty Note로생성
                //}

                #endregion

                #region 은정
                //연달아 세 음이 오름차순이라면!

                if (i > 1 && midiEvents_selectedTrack[i].pitch > midiEvents_selectedTrack[i - 1].pitch && midiEvents_selectedTrack[i - 1].pitch > midiEvents_selectedTrack[i - 2].pitch)
                {
                    isDragNote = true;

                    gameNoteInfo = new GameNoteInfo();

                    gameNoteInfo.pitch = midiEvents_selectedTrack[i].pitch;
                    gameNoteInfo.railIdx = 2;
                    gameNoteInfo.type = (int)GameNoteType.DRAG_LEFT;
                    gameNoteInfo.DRAG_release_idx = 0;
                    gameNoteInfo.isNoteEnabled = false;
                    gameNoteInfo.time = midiEvents_selectedTrack[i].startTime * 120;
                    ejnotemanager.allGameNoteInfo.Add(gameNoteInfo);

                    gameNoteInfo = new GameNoteInfo();

                    gameNoteInfo.pitch = midiEvents_selectedTrack[i-1].pitch;
                    gameNoteInfo.railIdx = 1;
                    gameNoteInfo.type = (int)GameNoteType.DRAG_empty;
                    gameNoteInfo.DRAG_release_idx = 0;
                    gameNoteInfo.isNoteEnabled = false;
                    gameNoteInfo.time = midiEvents_selectedTrack[i-1].startTime * 120;
                    ejnotemanager.allGameNoteInfo.Add(gameNoteInfo);

                    gameNoteInfo = new GameNoteInfo();

                    gameNoteInfo.pitch = midiEvents_selectedTrack[i-2].pitch;
                    gameNoteInfo.railIdx = 0;
                    gameNoteInfo.type = (int)GameNoteType.DRAG_empty;
                    gameNoteInfo.DRAG_release_idx = 0;
                    gameNoteInfo.isNoteEnabled = false;
                    gameNoteInfo.time = midiEvents_selectedTrack[i-2].startTime * 120;
                    

                }
                else
                {
                    gameNoteInfo = new GameNoteInfo();

                    gameNoteInfo.pitch = midiEvents_selectedTrack[i].pitch;
                    gameNoteInfo.railIdx = SetRailIdx(midiEvents_selectedTrack[i], gameNoteInfo);
                    gameNoteInfo.type = (int)GameNoteType.SHORT;

                    //gameNoteInfo.isLongNoteStart = false;
                    //gameNoteInfo.DRAG_release_idx = 0;
                    gameNoteInfo.isNoteEnabled = false;
                    gameNoteInfo.time = midiEvents_selectedTrack[i].startTime * 120;
                }
                #endregion

            }
            // longNote
            else
            {
                isLongNote = true;
                //Long_start
                gameNoteInfo = new GameNoteInfo();

                gameNoteInfo.pitch = midiEvents_selectedTrack[i].pitch;
                gameNoteInfo.railIdx = SetRailIdx(midiEvents_selectedTrack[i], gameNoteInfo);
                gameNoteInfo.type = (int)GameNoteType.LONG;

                gameNoteInfo.isLongNoteStart = true;
                //gameNoteInfo.DRAG_release_idx = 0;
                gameNoteInfo.isNoteEnabled = false;
                gameNoteInfo.time = midiEvents_selectedTrack[i].startTime * 120;

            }

            ejnotemanager.allGameNoteInfo.Add(gameNoteInfo);

            if (isLongNote)
            {
                //Long_end
                GameNoteInfo gameNoteInfo_end = new GameNoteInfo();

                gameNoteInfo_end.pitch = midiEvents_selectedTrack[i].pitch;
                gameNoteInfo_end.railIdx = SetRailIdx(midiEvents_selectedTrack[i], gameNoteInfo);
                gameNoteInfo_end.type = (int)GameNoteType.LONG;

                gameNoteInfo_end.isLongNoteStart = false;
                gameNoteInfo_end.isNoteEnabled = false;
                gameNoteInfo_end.time = midiEvents_selectedTrack[i].endTime * 120;
                ejnotemanager.allGameNoteInfo.Add(gameNoteInfo_end);
            }

            if (isDragNote)
            {
                GameNoteInfo gameNoteInfo_dragRight = new GameNoteInfo();

                gameNoteInfo_dragRight.pitch = midiEvents_selectedTrack[i].pitch;
                gameNoteInfo_dragRight.railIdx = 3;
                gameNoteInfo_dragRight.type = (int)GameNoteType.DRAG_RIGHT;
                gameNoteInfo_dragRight.DRAG_release_idx = 5;
                gameNoteInfo_dragRight.isLongNoteStart = false;

                gameNoteInfo_dragRight.isNoteEnabled = false;
                gameNoteInfo_dragRight.time = midiEvents_selectedTrack[i].startTime * 120;
                ejnotemanager.allGameNoteInfo.Add(gameNoteInfo_dragRight);

                gameNoteInfo = new GameNoteInfo();

                gameNoteInfo.pitch = midiEvents_selectedTrack[i - 1].pitch;
                gameNoteInfo.railIdx = 4;
                gameNoteInfo.type = (int)GameNoteType.DRAG_empty;
                gameNoteInfo.DRAG_release_idx = 0;
                gameNoteInfo.isNoteEnabled = false;
                gameNoteInfo.time = midiEvents_selectedTrack[i - 1].startTime * 120;
                ejnotemanager.allGameNoteInfo.Add(gameNoteInfo);

                gameNoteInfo = new GameNoteInfo();

                gameNoteInfo.pitch = midiEvents_selectedTrack[i - 2].pitch;
                gameNoteInfo.railIdx = 5;
                gameNoteInfo.type = (int)GameNoteType.DRAG_empty;
                gameNoteInfo.DRAG_release_idx = 0;
                gameNoteInfo.isNoteEnabled = false;
                gameNoteInfo.time = midiEvents_selectedTrack[i - 2].startTime * 120;
                ejnotemanager.allGameNoteInfo.Add(gameNoteInfo);
            }

            print("222 Midi2Note에서 MidiEvent를 GameNote로 변환했고, 이것을 옮겨 담은 결과 allGameNoteInfo에는" + ejnotemanager.allGameNoteInfo.Count + "개가 담겼다.");
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


    //longNote라면 endNote생성도 생각해야함.
    int SetNoteType(List<MidiEventInfo> midiNote, int idx, GameNoteInfo gameNoteInfo)
    {
        //long일 경우 endNote는 여기서 생성      
        if (midiNote[idx].length * 960 < 480)
        {
            return 0;   //short Note
        }
        else
        {
            //드래그는 나중에 고안하고 일단 longNote생성
            return 1;

            gameNoteInfo.isLongNoteStart = true;

            GameNoteInfo gameNoteInfo_end = new GameNoteInfo();
            gameNoteInfo_end.pitch = midiNote[idx].pitch;
            gameNoteInfo_end.railIdx = gameNoteInfo.railIdx;
            gameNoteInfo_end.type = gameNoteInfo.type;
            gameNoteInfo_end.isLongNoteStart = false;
            gameNoteInfo_end.isNoteEnabled = false;
            gameNoteInfo_end.time = midiNote[idx].endTime * 120;

            ejnotemanager.allGameNoteInfo.Add(gameNoteInfo_end);
        }
    }
}
