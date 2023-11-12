using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DHMidi;
using DHMidi.Event;
using static DH.D_MidiManager;
using DH;

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

        LoadMidi();
    }
    


    public void Onclick() {
        
 
        NoteManager.instance.ReadNote();
        Debug.Log("Midi 파일을 생성합니다.");


        MidiFileWriter.write(NoteManager.instance.midifile.GetData());
    }




    public InputField field;
    //박자 하나 당 틱 ( tick per beats)
    int Division = 480;
    //분당 박자 (Beat Per minute)
    int BPM = 120;

    //Seconds to tick
    public void Parse()
    {
        string sec = field.text;
        //박자 하나 당 초 구하기
        float seconds = 60 / BPM;

        //60 / 120 = 0.5 즉 박자 하나 당 초는 0.5초임.
        //480 tick = 0.5초 960 tick = 1초

        //초 * 960 tick = deltatime
        int inputsec = int.Parse(sec);
        Debug.Log(inputsec * 960);

    }


   // public Transform Board;
    public GameObject Button;
    public int page = 1;
    int pageLength = 50;

    public List<NoteBlockInfo[]> noteList = new List<NoteBlockInfo[]>();

    
    private void Start()
    {
        //trackButton();
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

        //할당하는 작업
        NoteBlockInfo[] noteInfos = new NoteBlockInfo[MaxNote * MaxNote];
        for (int i = 0; i < noteInfos.Length; i++) {
            NoteBlockInfo info = new NoteBlockInfo();
            info.pitch = Notes.Pitch.Do;
            info.Beat = 16;
            noteInfos[i] = info;
        }

        noteList.Add(noteInfos);

        //GameObject go = Instantiate(Button,Board);
        //PageButton button = go.GetComponent<PageButton>();
        //pageList.Add(button);
        //button.text.text = page.ToString();
        //button.idx = page - 1;

        page++;

    }

    //-2를 하는 이유는 인덱스는 0부터 시작하니 - 1, 1은 무조건 있어야 해서 -1 총 -2
    public void minusButton() {
        Debug.Log("MY PAGE = " + page);
        if (page - 2 < 0) return;
        //노트를 담는 배열 생성
        noteList.RemoveAt(page - 2);


        Debug.Log("Minus!!");
        PageButton button = pageList[page - 2];
        button.idx = page - 2;

        pageList.RemoveAt(page - 2);
        Destroy(button.gameObject);
        page--;
    }


    public int currentPage = 0;
    public void AdderPage(int adder) {

        Debug.Log("TEST");
        //더한값이 크거나 작으면 곤란 곤란
        currentPage = (adder + currentPage) > page || (adder + currentPage) < 0 ? currentPage : currentPage + adder;
        click(currentPage);
        

    }

    public void click(int idx)
    {
        //몇 번째 노트 데이터를 알려주세요.
        NoteBlockInfo[] notes = NoteList[idx];
        Debug.Log("Loading!!");

        //받은 데이터 렌더링 하기
        for (int i = 0; i < notes.Length; i++)
        {
            
            if (notes[i] != null)
            {
                Debug.LogError("왜 안되는지");

                //불러올때는 노트의 정보가
                Notes note = Board.instance.drags[i].Tile;
                NoteBlockInfo info = note.info = notes[i];
                note._IBeat = info.Beat;
                note._IPitch = info.Pitch;

                Debug.Log(info.pitch);
                note.gameObject.SetActive(info.enable);
            }


        }

    }











    public void Save(int page) { 
        
    }

    public void PlayMidi() {
        

        bool IsPlay = MIDIPlayer1.instance.ShouldPlayFile;
        MIDIPlayer1.instance.ShouldPlayFile = IsPlay ? false : true; //false 일때는 true true 일때는 false
    }


    int trackPage = 0;
    public GameObject TrackButton;
    public Transform TrackBoard;
    //다중 트랙
    public List<Track> Tracks = new List<Track>();


    public Track currentTrack;
    public InputField filed;
    public void ChangeInstruments() {
        int val = int.Parse(field.text);
        currentTrack.instrument = (DH.D_MidiManager.Instruments)val;
    }



    //transform
    public Transform TrackBtn;
    //트랙 증가
    public void trackButton()
    {
        GameObject go =  Instantiate(TrackButton, TrackBoard);
        TrackBtn.SetAsLastSibling();
        TrackButton trButton =  go.GetComponent<TrackButton>();


        //trButton.
        trButton.myPage = trackPage;

        //trButton.OnClickInsEv.AddListener(InstrumentManager.instance.ChangeTrack);
        //trButton.OnClickTrackEv.AddListener(ChangeTrack);
        //트랙 증가

        Track tr = new Track() { number = trackPage };
        Tracks.Add(tr) ;

        if (trackPage < 15) { trackPage++; }
        

        

        trButton.mytrack = tr;

        

        //버튼을 맨 마지막으로 보냄


        Debug.Log("근데 왜 안됨?");
    }

    public void EscapeButton() {
        EditerCanvas.gameObject.SetActive(false);
        TrackCanvas.gameObject.SetActive(true);
    }

    
    public void Rendering() {
        currentPage = 0;
        Debug.LogError("MyPageCount!!" + "0 부터 " + (pageList.Count - 1) + "까지 반복한다.");

        //int count = pageList.Count;
        //for (int i = 0; i < count; i++) {
        //    PageButton button = pageList[i];
        //    pageList.RemoveAt(i); //리스트에서 지우고
        //    Destroy(button.gameObject); //게임오브젝트를 삭제함. 뭐가문제?
        //    Debug.LogError("삭제가 된다.");

        //}


        //이유가, Remove 할때마다 Size가 감소해서 반복문이 의도한대로 진행되지 않음.

        //pageList.ForEach(i => {
        //    Destroy(i.gameObject);
        //} );
        //pageList.Clear();
        //page = 0;

        for (int i = 0; i < noteList.Count - 1; i++)
        {

            Debug.LogError("추가가 된다.");
            //이만큼 버튼 추가
            //GameObject go = Instantiate(Button, Board);
            //PageButton button = go.GetComponent<PageButton>();
            //pageList.Add(button);
            //button.text.text = page.ToString();
            //button.idx = page - 1;
            page++;
        }
        click(0);
        page = currentTrack.Notelist.Count -1 ;
    }


    public Image InsImage;
    public TextMeshProUGUI InsText;
    public void ChangeTrack(int chanel) {
        currentTrack = Tracks[chanel];
        TrackCanvas.gameObject.SetActive(false);
        EditerCanvas.gameObject.SetActive(true);

        //렌더링 방식을 변경
        noteList = currentTrack.Notelist;
        Rendering();
    }

    public void ChangeTrack(Track track)
    {
        currentTrack = track;
        TrackCanvas.gameObject.SetActive(false);
        EditerCanvas.gameObject.SetActive(true);
        //렌더링 방식을 변경

        if (track.number == 9)
        {
            InsImage.sprite = InstrumentManager.instance.iconlist[(int)InstrumentManager.Instype.Percussive];
            InsText.text = "Drum Kit";
        }
        else {
            int ins = (int)currentTrack.instrument;
            InsImage.sprite = InstrumentManager.instance.iconlist[(int)InstrumentManager.retIns(ins)];
            InsText.text = currentTrack.instrument.ToString();
        }
        
        noteList = currentTrack.Notelist;
        Rendering();
    }



    void LoadMidi() {
        MidiFile midifile =  MidiFileWriter.ReadMidi("example.mid.txt");




        midifile.TrackLsit.ForEach(track => {
            Track tr = new Track ();
            
            MidiEvent prevEvent = null;
            track.eventList.ForEach(evt =>
            {
                //일단 채널 번호
                //악기 정보 

                MidiEvent midievt = evt is MidiEvent ? (MidiEvent)evt : null;
                if (midievt != null) {

                    switch (midievt.etype) {
                        case 0xC:
                            tr.instrument = (Instruments)midievt.fData;
                            tr.number = midievt.chanel;
                            break;
                        case 0x9:
                            prevEvent = midievt;
                            
                            break;


                        case 0x8:
                            //시작이 되었다면 끝맺음이 필요
                            if (prevEvent != null) {
                                int deltatime = midievt.deltatime;
                                int pitch = prevEvent.fData;
                                int velocity = prevEvent.sData;

                                NoteBlockInfo noteinfo =  new NoteBlockInfo();

                                
                                 
                                int condelta = ConvertSecondsToDeltatime(0.5f);
                                noteinfo.Beat = deltatime / condelta; 
                                noteinfo.Pitch = pitch;
                                noteinfo.velocity = velocity;

                            }


                            break;
                    }

                }
                    

                //노드 정보



            });


        });

    }

   
}