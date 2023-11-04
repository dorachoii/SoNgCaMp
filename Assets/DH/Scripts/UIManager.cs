using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Stack<GameObject> UiLayer = new Stack<GameObject>();
    



    public static UIManager instance;
    
    public Canvas TrackCanvas;
    public Canvas EditerCanvas;
    private void Awake()
    {
        instance = this;
        //NoteInfo[] noteInfos = new NoteInfo[5 * 5];
        //for (int i = 0; i < noteInfos.Length; i++)
        //{
        //    NoteInfo info = new NoteInfo();
        //    info.pitch = Notes.Pitch.Do;
        //    noteInfos[i] = info;

        //}
        //noteList.Add(noteInfos);
    }
    


    public void Onclick() {
        
 
        NoteManager.instance.ReadNote();
        Debug.Log("Midi ������ �����մϴ�.");


        MidiFileWriter.write(NoteManager.instance.midifile.GetData());
    }




    public InputField field;
    //���� �ϳ� �� ƽ ( tick per beats)
    int Division = 480;
    //�д� ���� (Beat Per minute)
    int BPM = 120;

    //Seconds to tick
    public void Parse()
    {
        string sec = field.text;
        //���� �ϳ� �� �� ���ϱ�
        float seconds = 60 / BPM;

        //60 / 120 = 0.5 �� ���� �ϳ� �� �ʴ� 0.5����.
        //480 tick = 0.5�� 960 tick = 1��

        //�� * 960 tick = deltatime
        int inputsec = int.Parse(sec);
        Debug.Log(inputsec * 960);

    }


    public Transform Board;
    public GameObject Button;
    public int page = 1;
    int pageLength = 50;

    public List<NoteBlockInfo[]> noteList = new List<NoteBlockInfo[]>();

    
    private void Start()
    {
        trackButton();
    }
    public List<NoteBlockInfo[]> NoteList
    {
        get { return noteList; }
    }

    public const int MaxNote = 4;
    public List<PageButton> pageList = new List<PageButton>();
    public void plusButton() {
        Debug.Log("MY PAGE = " + page);
        if (page + 1 > pageLength) return;

        //�Ҵ��ϴ� �۾�
        NoteBlockInfo[] noteInfos = new NoteBlockInfo[MaxNote * MaxNote];
        for (int i = 0; i < noteInfos.Length; i++) {
            NoteBlockInfo info = new NoteBlockInfo();
            info.pitch = Notes.Pitch.Do;
            info.Beat = 16;
            noteInfos[i] = info;
        }

        noteList.Add(noteInfos);
        GameObject go = Instantiate(Button,Board);
        PageButton button = go.GetComponent<PageButton>();
        pageList.Add(button);
        button.text.text = page.ToString();
        button.idx = page - 1;

        page++;

    }

    //-2�� �ϴ� ������ �ε����� 0���� �����ϴ� - 1, 1�� ������ �־�� �ؼ� -1 �� -2
    public void minusButton() {
        Debug.Log("MY PAGE = " + page);
        if (page - 2 < 0) return;
        //��Ʈ�� ��� �迭 ����
        noteList.RemoveAt(page - 2);


        Debug.Log("Minus!!");
        PageButton button = pageList[page - 2];
        button.idx = page - 2;

        pageList.RemoveAt(page - 2);
        Destroy(button.gameObject);
        page--;
    }

    public void Save(int page) { 
        
    }

    public void PlayMidi() {
        bool IsPlay = MIDIPlayer1.instance.ShouldPlayFile;
        MIDIPlayer1.instance.ShouldPlayFile = IsPlay ? false : true; //false �϶��� true true �϶��� false
    }


    int trackPage = 0;
    public GameObject TrackButton;
    public Transform TrackBoard;
    //���� Ʈ��
    public List<Track> Tracks = new List<Track>();


    public Track currentTrack;
    public InputField filed;
    public void ChangeInstruments() {
        int val = int.Parse(field.text);
        currentTrack.instrument = (DH.D_MidiManager.Instruments)val;
    }



    //Ʈ�� ����
    public void trackButton()
    {
        GameObject go =  Instantiate(TrackButton, TrackBoard);
        TrackButton trButton =  go.GetComponent<TrackButton>();
        trButton.myPage = trackPage;
        //Ʈ�� ����
        Tracks.Add(new Track());
        trackPage++;
    }

    public void EscapeButton() {
        EditerCanvas.gameObject.SetActive(false);
        TrackCanvas.gameObject.SetActive(true);
    }

    
    public void Rendering() {
        Debug.LogError("MyPageCount!!" + "0 ���� " + (pageList.Count - 1) + "���� �ݺ��Ѵ�.");

        //int count = pageList.Count;
        //for (int i = 0; i < count; i++) {
        //    PageButton button = pageList[i];
        //    pageList.RemoveAt(i); //����Ʈ���� �����
        //    Destroy(button.gameObject); //���ӿ�����Ʈ�� ������. ��������?
        //    Debug.LogError("������ �ȴ�.");

        //}


        //������, Remove �Ҷ����� Size�� �����ؼ� �ݺ����� �ǵ��Ѵ�� ������� ����.
        pageList.ForEach(i => {
            Destroy(i.gameObject);
        } );
        pageList.Clear();
        page = 1;

        for (int i = 0; i < noteList.Count; i++)
        {

            Debug.LogError("�߰��� �ȴ�.");
            //�̸�ŭ ��ư �߰�
            GameObject go = Instantiate(Button, Board);
            PageButton button = go.GetComponent<PageButton>();
            pageList.Add(button);
            button.text.text = page.ToString();
            button.idx = page - 1;
            page++;
        }


    }
}