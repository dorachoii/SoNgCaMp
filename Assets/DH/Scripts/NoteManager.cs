using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using DH;
using DHMidi;

public class NoteManager : MonoBehaviour
{

    public static int noteSize = 5;

    public static NoteManager instance;


    public NoteBlockInfo SaveData = new NoteBlockInfo();
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

        byte[] bytes = BitConverter.GetBytes(10);
        Debug.Log(bytes);






        //127 �̻��̸� ������ ���� ����Ʈ�� ��ŸŸ��.
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


    public void ReadNote() {


        midifile.TrackLsit.Clear();
        DummyHeaderData dummy = new DummyHeaderData();
        midifile.Header = new HeaderChunk(dummy.H_Ctype,dummy.H_Length,dummy.H_Data);

        bytelist.Clear();
        list = Board.GetComponentsInChildren<Notes>();

        //1����Ʈ = 8��Ʈ
        //16���� �ϳ� = 1����Ʈ

        //1101 0111
        //���ڸ� ���� ���ϴµ� ���� ����
        //�ֻ��� ��Ʈ�� 1�̸� �ȵ�
        //���� 1�̶�� 

        //�Ľ��ϴ� �����ʿ�

        UIManager.instance.Tracks.ForEach((Action<Track>)(track =>
        {
            DummyTrackData dummy = new DummyTrackData();
            TrackChunk trackchunk = new TrackChunk(dummy.C_Ctype, dummy.C_Length, dummy.C_Data);

            //Track�� �ִ� Note �����͵��� Data�� �ű���.

            List<byte> bytelist = new List<byte>();

            NoteBlockInfo prevNote = null;
            int count = 1;//�󸶳� ������ �����ϴ���
            int shim = 0;
            //��ϴ� �����ϱ�. 
            foreach (NoteBlockInfo[] infos in track.Notelist)
            {
                
                foreach (NoteBlockInfo info in infos)
                {
                    //ī��Ʈ�� ������ �ؾ���

                    //���� ĭ�� ������ �ƴ϶��
                    if (info.enable) {
                        shim = count;
                        //������ �ƴҽ� �ڿ� ��Ʈ�� ���� 
                        if (prevNote != null)
                        {
                            //�� ���ڰ� ū��� �ɰ��� �ƴϸ� �׳��ϱ�
                            prevNote.Beat = (count > prevNote.Beat ? prevNote.Beat : count);
                            //��ǥ�� count - ��Ʈ 
                            shim = (count > prevNote.Beat ? count - prevNote.Beat : 0);
                            //NoteOff Event (�ڿ� ��Ʈ ����)
                            bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(prevNote.Beat * 0.5f)));
                            bytelist.Add((byte)prevNote.Pitch); //�̰� ������
                            bytelist.Add(0);
                            count = 0;
                        }

                        //�� �����ϰ�
                        //NoteOnEvent
                        bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime((shim) * 0.5f))); //��ǥ�� �ᱹ count�� �ȴ�.
                        bytelist.Add((byte)info.Pitch);
                        bytelist.Add(120);
                        count = 0;
                        //�������� ���� �������� �ɰ�.
                        prevNote = info;
                    }
                    count++;
                }
                //Ž������� �������� �ʱ�ȭ�� �ȵ� ����� ���� ��, null�� �ƴҽ� ���� �ʿ�.


            }
            //�������� �ѹ� �ʿ��� 
            if (prevNote != null) {

                prevNote.Beat = (count > prevNote.Beat ? prevNote.Beat : count);
                bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(prevNote.Beat * 0.5f))); //��ĭ�� 0.5��
                bytelist.Add((byte)prevNote.Pitch); //�̰� ������
                bytelist.Add(0);
            }

            //�� Ʈ���� �����͵��� ��� �ܾ List�� �־��.
            //�װ���? track�� ����. 
            trackchunk.AddData = bytelist.ToArray();

            //������ �����ʹϱ�?
            byte[] last_data = { 0x00,
        0xFF, 0x2F, 0x00 };

            trackchunk.AddData = last_data;

            midifile.TrackLsit.Add(trackchunk);
        }));
    
        //test  ���������� UIManager�� �̰� ������ �����


        //foreach (NoteInfo[] infos in UIManager.instance.noteList) {  
        //    foreach (NoteInfo info in infos) {
        //        if (!info.enable) continue;

        //        //NpteOnEvent
        //        bytelist.AddRange(D_MidiManager.ConvertDeltaTime(1));
        //        bytelist.Add((byte)info.Pitch);
        //        bytelist.Add(120);

        //        //NoteOff Event
        //        bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime( Notes.BeatTofloat(info.beat)  )));
        //        bytelist.Add((byte)info.Pitch); //�̰� ������
        //        bytelist.Add(0);
        //    }

        //}
            



        //foreach (Notes note in list)
        //{
        //    //deltatime to byte
        //    Debug.Log("Parsing!!");

        //    //������ ����� ����.. ��¥�� �ڿ��� �����ϱ�
            
        //    //�� �̺�Ʈ�� ���� ��, 1tick �ڿ� �ٷ� ���� �̺�Ʈ�� �����Ѵ�.
        //    bytelist.AddRange(D_MidiManager.ConvertDeltaTime(1));


        //    //Test Data ========
        //    byte[] bytes1 = D_MidiManager.ConvertDeltaTime(1000);
        //    Debug.LogWarning(1 + "�� ���� �ٲٴ� ���Դϴ�."); 
        //    bytes1.ToList().ForEach(i => Debug.Log(i)); 


        //    bytelist.Add((byte)note.PitchToint(note.pitch));

        //    bytelist.Add(120
        //        );
                
        //    //Test Data ========
        //    byte[] bytes2 = D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime( note._FBeat));
        //    Debug.LogWarning(note.beat + "�� ���� �ٲٴ� ���Դϴ�.");
        //    bytes2.ToList().ForEach( i => Debug.Log(i) );  

        //    //byte[] e_pitchbytes = BitConverter.GetBytes(note.PitchToint(note.pitch));
        //    bytelist.AddRange(D_MidiManager.ConvertDeltaTime(D_MidiManager.ConvertSecondsToDeltatime(note._FBeat)));  
        //    bytelist.Add((byte)note.PitchToint(note.pitch));
        //    //bytelist.AddRange(pitchbytes);
        //    bytelist.Add(0);


        //}
    }
}

