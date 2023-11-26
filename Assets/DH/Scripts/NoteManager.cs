using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using DH;
using DHMidi;
using TMPro;
public class NoteManager : MonoBehaviour
{

    public static int noteSize = 5;

    public static NoteManager instance;


    public NoteBlockInfo SaveData = new NoteBlockInfo(60,16,120,true);
    private void Awake()
    {
        instance = this;
    }
    public Transform Board;
    public Notes[] list;
    public static List<byte> bytelist = new List<byte>();
    // Start is called before the first frame update
    void Start()
    {
        D_MidiManager.ConvertDeltaTime(240);


        byte[] bytes = BitConverter.GetBytes(10);
        Debug.Log(bytes);






        //127 이상이면 무조건 다음 바이트도 델타타임.
        //
        

    }

    public InputField field;
    public void CreateNote(GameObject go) {
        GameObject note = Instantiate(go, Board);
        Notes Cnote = note.GetComponent<Notes>();
        //Cnote.beat = float.Parse(field.text);
        field.text = "";
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public MidiFile midifile = new MidiFile();

    public int BPM { get; set; }
    //BPM 
    public TMP_InputField bpmField;
    public void ReadNote() {

            Debug.LogError(bpmField.text);
        BPM = bpmField.text == null || bpmField.text == "" ? 120 : int.Parse(bpmField.text);

        midifile.TrackLsit.Clear();
        DummyHeaderData dummy = new DummyHeaderData();
        midifile.Header = new HeaderChunk(dummy.H_Ctype,dummy.H_Length,dummy.H_Data);

        bytelist.Clear();
        //list = Board.GetComponentsInChildren<Notes>();

        //1바이트 = 8비트
        //16진수 하나 = 1바이트

        //1101 0111
        //앞자리 수로 비교하는데 원래 수의
        //최상위 비트가 1이면 안됨
        //따라서 1이라면 

        //파싱하는 과정필요
        int trackCount = 0;
        UIManager.instance.Tracks.ForEach((Action<Track>)(track =>
        {
        DummyTrackData dummy = new DummyTrackData();
        TrackChunk trackchunk = new TrackChunk(dummy.C_Ctype, dummy.C_Length, dummy.C_Data);

        //Track에 있는 Note 데이터들을 Data에 옮기자.

        List<byte> bytelist = new List<byte>();

        NoteBlockInfo prevNote = null;
        int count = 0;//얼마나 공백이 존재하는지
        int shim = 0;

            //BPM SET!!
            bytelist.AddRange(new byte[] { 0x00, 0xFF, 0x51, 0x03 });
            byte[] bpmList = D_MidiManager.RemoveZero(BitConverter.GetBytes(D_MidiManager.ConvertBpmToMicro(BPM)));
            bytelist.AddRange(bpmList);



            //악기 설정
            if (track.number != 9) {
                bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(0)));
                bytelist.AddRange(D_MidiManager.ChangeInstument(track.number, track.instrument));
            }


            //트랙의 시작 시 시작이벤트 넣자. 
            bytelist.AddRange(new byte[] { 0x00, (byte)(0x90 + track.number), 0x3C, 0x00 });
            trackCount++;
            //블록당 읽으니까. 
            foreach (NoteBlockInfo[] infos in track.Notelist)
        {

            foreach (NoteBlockInfo info in infos)
            {
                //카운트는 무조건 해야함

                //지금 칸이 공백이 아니라면
                if (info.enable) {
                        info.Pitch = track.number == 9 ? (int)track.instrument : info.Pitch;      

                    shim = count;
                    //공백이 아닐시 뒤에 노트를 끊고 
                    if (prevNote != null)
                    {
                        //내 박자가 큰경우 쪼개기 아니면 그냥하기
                        prevNote.Beat = (count > prevNote.Beat ? prevNote.Beat : count);
                        //쉼표는 count - 비트 
                        shim = (count > prevNote.Beat ? count - prevNote.Beat : 0);
                        //NoteOff Event (뒤에 노트 끊고)
                        bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(prevNote.Beat * 0.5f)));
                        bytelist.Add((byte)prevNote.Pitch); //이거 수정필
                        bytelist.Add(0);
                        count = 0;
                    }

                    //나 시작하고
                    //NoteOnEvent
                    bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime((shim) * 0.5f))); //쉼표는 결국 count가 된다.
                    bytelist.Add((byte)info.Pitch);
                    bytelist.Add(120);
                    count = 0;
                    //끝났으니 내가 이전으로 될게.
                    prevNote = info;
                }
                count++;
            }
            //탐색종료시 마지막에 초기화가 안된 대상이 있을 시, null이 아닐시 종료 필요.


            }
            //마지막때 한번 필요해 
            if (prevNote != null) {

                prevNote.Beat = (count > prevNote.Beat ? prevNote.Beat : count);
                bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(prevNote.Beat * 0.5f))); //한칸에 0.5박
                bytelist.Add((byte)prevNote.Pitch); //이거 수정필
                bytelist.Add(0);
            }

            //한 트랙의 데이터들을 모두 긁어서 List에 넣어둠.
            //그것을? track에 붙임. 
            trackchunk.AddData = bytelist.ToArray();

            //마지막 데이터니까?
            byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };

            trackchunk.AddData = last_data;

            midifile.TrackLsit.Add(trackchunk);
        }));


        if (trackCount > 1) {
            midifile.Header.Format = 1;
        }
        //test  차원에서지 UIManager에 이게 있으면 곤란함


        //foreach (NoteInfo[] infos in UIManager.instance.noteList) {  
        //    foreach (NoteInfo info in infos) {
        //        if (!info.enable) continue;

        //        //NpteOnEvent
        //        bytelist.AddRange(D_MidiManager.ConvertDeltaTime(1));
        //        bytelist.Add((byte)info.Pitch);
        //        bytelist.Add(120);

        //        //NoteOff Event
        //        bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime( Notes.BeatTofloat(info.beat)  )));
        //        bytelist.Add((byte)info.Pitch); //이거 수정필
        //        bytelist.Add(0);
        //    }

        //}
            



        //foreach (Notes note in list)
        //{
        //    //deltatime to byte
        //    Debug.Log("Parsing!!");

        //    //시작은 상관이 없네.. 어짜피 뒤에서 끝나니까
            
        //    //뒤 이벤트가 끝난 후, 1tick 뒤에 바로 다음 이벤트를 실행한다.
        //    bytelist.AddRange(D_MidiManager.ConvertDeltaTime(1));


        //    //Test Data ========
        //    byte[] bytes1 = D_MidiManager.ConvertDeltaTime(1000);
        //    Debug.LogWarning(1 + "를 지금 바꾸는 중입니다."); 
        //    bytes1.ToList().ForEach(i => Debug.Log(i)); 


        //    bytelist.Add((byte)note.PitchToint(note.pitch));

        //    bytelist.Add(120
        //        );
                
        //    //Test Data ========
        //    byte[] bytes2 = D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime( note._FBeat));
        //    Debug.LogWarning(note.beat + "를 지금 바꾸는 중입니다.");
        //    bytes2.ToList().ForEach( i => Debug.Log(i) );  

        //    //byte[] e_pitchbytes = BitConverter.GetBytes(note.PitchToint(note.pitch));
        //    bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(note._FBeat)));  
        //    bytelist.Add((byte)note.PitchToint(note.pitch));
        //    //bytelist.AddRange(pitchbytes);
        //    bytelist.Add(0);


        //}
    }
}

