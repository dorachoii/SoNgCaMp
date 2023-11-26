using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using static Notes;

using static DH.D_MidiManager;

//Track 
public class Track {
    public int number;
    public Instruments instrument; 


    public List<NoteBlockInfo[]> Notelist = new List<NoteBlockInfo[]>();
    public Track() {
        //�Ҵ��ϴ� �۾�
        NoteBlockInfo[] noteInfos = new NoteBlockInfo[ 4 * 4];
        for (int i = 0; i < noteInfos.Length; i++)
        {
            NoteBlockInfo info = new NoteBlockInfo();
            info.Pitch = 60;
            noteInfos[i] = info;

        }
        Notelist.Add(noteInfos);
    }

}
[System.Serializable]
public class NoteBlockInfo{

    public NoteBlockInfo(int pitch, int beat, int velocity, bool enable) {
        Pitch = pitch;
        Beat = beat;
        this.beat = (Beat)beat;
        this.velocity = velocity;
        this.enable = enable;
    }

    public NoteBlockInfo() { }

    public int Pitch = 60;
    public Pitch pitch;

    //Enum�� ���� �Ⱦ����� 
    public int Beat;

    public Beat beat;
    //����
    public int velocity;
    public bool enable = false;

    public void Copy(NoteBlockInfo data) {
        this.Pitch = data.Pitch;
        this.beat = data.beat;
        this.velocity = data.velocity;
        this.enable = data.enable;
        this.Beat = data.Beat;
    }
}


public class Notes : MonoBehaviour
{
    //0~127 Values
    public const int MinNoteNum = 0;
    public const int MaxNoteNum = 127;
    public const int PitchPage = 12;

    public const int MinBeatNum = 1;
    public const int MaxBeatNum = 16;

    public NoteBlockInfo info;

    public int Ipitch;
    //��
    public int Ibeat;

    public Pitch pitch;
    public Beat beat;
    //����
    public int velocity;
    //public float beat;

    //������ �ΰ���� ���� ���Ŀ� ����
    public TMPro.TextMeshProUGUI pitchText;
    public TMPro.TextMeshProUGUI beatText;
    public TMPro.TextMeshProUGUI volumeText;

    public int Volume
    {
        get {
            return velocity;
        }
        set {
            velocity = value;
            volumeText.text = value + "";
            info.velocity = value;
        }
    }
    public int _IPitch
    {
        get { return Ipitch; }
        set { Ipitch = value;
            pitchText.text = IpitchToString(value);
            info.Pitch = value;
        }
    }
    public int _IBeat{
        get { return Ibeat; }

        set {
            Debug.Log("�������� �ȵǴ���?");
            Ibeat = value;
            beatText.text = value + "Beat";
            info.Beat = value;
        }
    }

    public Pitch _Pitch
    {
        get { return pitch; }
        set { pitch = value;
            //Text�� Null�� ��� Error �߻�. 
            pitchText.text = PitchToString(value); 
            info.pitch = value;
        }
    }
    public float _FBeat
    {
        get { return BeatTofloat(beat); }
    }

    public Beat _Beat
    {
        set {
            beat = value;
            beatText.text = BeatTofloat(value) + "��";
            info.beat = value;
        }
    }


    public static float BeatTofloat(Beat beat) {
        float value = 0;
        switch (beat)
        {
            case Beat.Quadruple:
                value = 1;
                break;
            case Beat.Triple:
                value = 3;
                break;
            case Beat.Duple:
                value = 2;
                break;
            case Beat.Half:
                value = 0.5f;
                break;
            case Beat.Sixteenth:
                value = 0.25f;
                break;

            case Beat.Whole:
                value = 4;
                break;
            case Beat.DotQuarter:
                value = 1.5f;
                break;
            default:
                break;
        }

        return value;
    }
    public enum Pitch { 
        DPa,DSol,DLa,DSi,
        Do,S_Do,
        Re,S_Re,
        Mi,Pa,S_Pa
        ,Sol,S_Sol,La, S_La,Si,
        HDo,HRe,HMi,HPa,HSol,HLa,HSi
    }
    public enum Beat {
        Quadruple, Triple , Duple , Half ,  Sixteenth,Whole ,DotQuarter
    }



    public static int PitchToint(Pitch pitch) {
        int pit = 0;
        switch (pitch)
        {
            case Pitch.DPa:
                pit = 53;
                break;
            case Pitch.DSol:
                pit = 55;
                break;
            case Pitch.DLa:
                pit = 57;
                break;
            case Pitch.DSi:
                pit = 59;
                break;
            case Pitch.Do:
                pit = 60;
                break;
            case Pitch.S_Do:
                pit = 61;
                break;
            case Pitch.Re:
                pit = 62;
                break;
            case Pitch.S_Re:
                pit = 63;
                break;
            case Pitch.Mi:
                pit = 64;
                break;
            case Pitch.Pa:
                pit = 65;
                break;
            case Pitch.S_Pa:
                pit = 66;
                break;
            case Pitch.Sol:
                pit = 67;
                break;
            case Pitch.S_Sol:
                pit = 68;
                break;
            case Pitch.La:
                pit = 69;
                break;
            case Pitch.S_La:
                pit = 70;
                break;
            case Pitch.Si:
                pit = 71;
                break;
            case Pitch.HDo:
                pit = 72;
                break;
            case Pitch.HRe:
                pit = 74;
                break;
            case Pitch.HMi:
                pit = 76;
                break;
            case Pitch.HPa:
                pit = 77;
                break;
            case Pitch.HSol:
                pit = 79;
                break;
            case Pitch.HLa:
                pit = 81;
                break;
            case Pitch.HSi:
                pit = 83;
                break;
            default:
                break;
        }
        return pit;
    }

    //�������Ѽ� ���ϸ� �ɵ�.
    public static string PitchToString(Pitch pitch) {
        string s_pitch = null;
        switch (pitch)
        {
            case Pitch.DPa:
                s_pitch = "-��";
                break;
            case Pitch.DSol:
                s_pitch = "-��";
                break;
            case Pitch.DLa:
                s_pitch = "-��";
                break;
            case Pitch.DSi:
                s_pitch = "-��";
                break;
            case Pitch.Do:
                s_pitch = "��";
                break;
            case Pitch.S_Do:
                s_pitch = "��#";
                break;
            case Pitch.Re:
                s_pitch = "��";
                break;
            case Pitch.S_Re:
                s_pitch = "��#";
                break;
            case Pitch.Mi:
                s_pitch = "��";
                break;
            case Pitch.Pa:
                s_pitch = "��";
                break;
            case Pitch.S_Pa:
                s_pitch = "��#";
                break;
            case Pitch.Sol:
                s_pitch = "��";
                break;
            case Pitch.S_Sol:
                s_pitch = "��#";
                break;
            case Pitch.La:
                s_pitch = "��";
                break;
            case Pitch.S_La:
                s_pitch = "��#";
                break;
            case Pitch.Si:
                s_pitch = "��";
                break;
            case Pitch.HDo:
                s_pitch = "+��";
                break;
            case Pitch.HRe:
                s_pitch = "+��";
                break;
            case Pitch.HMi:
                s_pitch = "+��";
                break;
            case Pitch.HPa:
                s_pitch = "+��";
                break;
            case Pitch.HSol:
                s_pitch = "+��";
                break;
            case Pitch.HLa:
                s_pitch = "+��";
                break;
            case Pitch.HSi:
                s_pitch = "+��";
                break;
            default:
                break;
        }
        return s_pitch;
    }

    static string[] str_pitch = { "Do", "Do#","Re","Re#","Mi","Pa","Pa#","Sol","Sol#","La","La#","Si" }; 
    public string IpitchToString(int pitch) {
        int octave = pitch / PitchPage;
        int notenum = pitch % PitchPage;
        string value = octave + " " + str_pitch[notenum];
        return value;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
