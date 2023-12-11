using Melanchall.DryWetMidi.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

//01. Note_Instantiate & Destroy
//02. scoreCheck

public class EJNoteManager : MonoBehaviour
{
    //임의
    //150 - 120이 그나마 
    float bpm = 120;

    public Camera maincam;

    //01. Note_Instantiate
    public GameObject[] notePrefabs;
    public Transform[] noteSpawnRail;
    public Transform[] touchpads;

    //Touch와 연동해서 껐다 켤 것
    public GameObject[] QuadRails;
    public GameObject[] QuadTouches;
    int[] pitches_rail = new int[6];

    public GameObject touchFX;
    public GameObject touchfireFX;
    public GameObject fireworkFX;

    public AudioSource[] audiosource;
    public AudioClip[] SFXs;

    public Transform[] touchFX_pos;
    public Transform[] touchfireFX_pos;
    public Transform[] fireworkFX_pos;

    public Material[] QuadRails_Mat;

    GameObject note;
    GameObject startNote;
    GameObject endNote;

    const int railCount = 6;
    public float currTime;

    //01-1.noteData _ 일종의 대기열 느낌
    public List<GameNoteInfo> allGameNoteInfo = new List<GameNoteInfo>();
    public List<GameNoteInfo>[] gameNoteInfo_Rails = new List<GameNoteInfo>[railCount];

    //01-2.Hierarchy - instance noteData
    List<EJGameNote>[] gameNoteInstance_Rails = new List<EJGameNote>[railCount];
    EJGameNote[] gameStartNoteArr = new EJGameNote[railCount];

    //02. Note_pressCheck
    bool[] isTouchPadPressed = new bool[railCount];
    bool[] isDragPressed = new bool[railCount];
    int touchStartedIdx;
    int touchReleasedIdx;

    enum DraggingState
    {
        None,
        Dragging_RIGHT,
        Dragging_LEFT,
    }

    DraggingState draggingState;

    Touch touch;
    TouchPhase phase;
    Vector2 deltaPos;

    public Material missMat;

    //03. scoreCheck;
    float distAbs;     //touchPad와 note사이의 거리 체크
    float dist;

    float badZone = 2.9f;
    float goodZone = 2f;
    float greatZone = 1f;
    float excellentZone = 0.3f;

    int badScore = 1;
    int goodScore = 2;
    int greatScore = 3;
    int excellentScore = 5;
    int missScore = -1;

    float pressScore = 1;

    public Canvas canvas;
    public GameObject[] scoreTexts;


    public TextMeshProUGUI songName;
    public TextMeshProUGUI songArtist;

    void Start()
    {

        // instantiated note in hierarchy <<< Add EJNote Component 
        for (int i = 0; i < gameNoteInstance_Rails.Length; i++)
        {
            //notes properties list per Rails
            gameNoteInstance_Rails[i] = new List<EJGameNote>();
        }

        //InputTestSHORTNotes();    //test FINISHED!!!
        //InputTestLONGNotes();     //test FINISHED_1차!!!
        //InputTestDRAGNote();
        //InputTestMIXEDNote();

        //누르는 버튼에 따라 실행

        int songindex = PlayerPrefs.GetInt("SongIndex");


        switch (songindex)
        {
            case 0:
                InputTestFLOP();
                songName.text = "Flop";
                songArtist.text = "Jaedal";
                bpm = 100;

                break;
            case 1:
                inputAPEX();
                songName.text = "APEX";
                songArtist.text = "Silica gel";
                bpm = 120;

                break;
            case 2:
                InputCameraMan();
                songName.text = "Camera Man";
                songArtist.text = "Jett, Muteko";
                bpm = 120;
                break;

            case 3:
                input150beats();
                songName.text = "Rock Star";
                songArtist.text = "Jett, Muteko";

                bpm = 120;
                break;

            default:
                InputCameraMan();
                songName.text = "Camera Man";
                songArtist.text = "Jett, Muteko";
                bpm = 120;
                break;
        }

        //StartCoroutine(Test());

    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(0.2f);
        MIDIPlayer.instance.PlayOneMidiEvent(gameNoteInstance_Rails[0][0].noteInfo.pitch);
        yield return new WaitForSeconds(0.2f);
        MIDIPlayer.instance.PlayOneMidiEvent(gameNoteInstance_Rails[1][0].noteInfo.pitch);
        yield return new WaitForSeconds(0.2f);
        MIDIPlayer.instance.PlayOneMidiEvent(gameNoteInstance_Rails[2][0].noteInfo.pitch);
        yield return new WaitForSeconds(0.2f);
        MIDIPlayer.instance.PlayOneMidiEvent(gameNoteInstance_Rails[4][0].noteInfo.pitch);
        yield return new WaitForSeconds(0.2f);
        MIDIPlayer.instance.PlayOneMidiEvent(gameNoteInstance_Rails[5][0].noteInfo.pitch);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            //StartCoroutine(finaleFX());
            //StartCoroutine(fireworkkFX());

            startcoFinaleFX();
        }



        currTime += Time.deltaTime;

        //01. Note_Instantiate & Destroy    //test FINISHED!!!
        #region 01. Note_Instantiate & Destroy

        //01-1. Note_Instantiate
        //note instantiate per rails

        //0~5까지 반복하면서 ex) 0번 레일일 때
        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            // ex) 0번 레일에 만들어질 노트가 있다면
            // i = railIndex 체크 중
            if (gameNoteInfo_Rails[i].Count > 0)
            {
                //Note_Instantiate on Time
                //대기열에 있는 0번 레일의 0번 노트의 생성시간에 생성
                //if (currTime >= noteInfo_Rails[i][0].time)
                if (currTime >= (float)(gameNoteInfo_Rails[i][0].time / bpm))
                {
                    //Note_Instantiate by NoteType, SpawnRail
                    //01-1-1.NoteType
                    //notePrefabs[type], noteSpawnRail[0],
                    note = Instantiate(notePrefabs[gameNoteInfo_Rails[i][0].type], noteSpawnRail[i].position + Vector3.forward * (-0.5f), Quaternion.identity);

                    note.transform.forward = notePrefabs[0].transform.forward;
                    note.transform.SetParent(noteSpawnRail[i].transform);

                    //현재 instantiated된 Note의 info에 대기열의 정보를 담아주고
                    //새로운 리스트의 배열에 넣어주고 싶음.
                    EJGameNote noteInstance = note.GetComponent<EJGameNote>();

                    #region 함수로 묶어준 부분 안되면 풀기
                    //noteInstance.noteInfo = noteInfo_Rails[i][0];
                    //noteInstance_Rails[i].Add(noteInstance);
                    ////Instantiated되면 대기열에서 지워주기
                    //noteInfo_Rails[i].RemoveAt(0);
                    #endregion

                    noteInstantiate(i, noteInstance);

                    //01-1-2.NoteType_LONG
                    //LONG이라면 endNote를 생성
                    if (noteInstance.noteInfo.type == (int)GameNoteType.LONG)
                    {
                        if (noteInstance.noteInfo.isLongNoteStart)
                        {
                            print("*00000 noteInstantiate 실행 - noteInstance의 type은" + noteInstance.noteInfo.type + "noteInstance의 isLongStart는" + noteInstance.noteInfo.isLongNoteStart + "현재열의 0번에 담긴 것은" + gameNoteInstance_Rails[i][0]);

                            gameStartNoteArr[i] = noteInstance;
                            //startNote = firstNoteInstance.gameObject;
                        }
                        else
                        {
                            print("*11111 noteInstantiate 실행 - noteInstance의 type은" + noteInstance.noteInfo.type + "noteInstance의 isLongStart는" + noteInstance.noteInfo.isLongNoteStart + "현재열의 0번에 담긴 것은" + gameNoteInstance_Rails[i][0]);

                            int startNoteIdx = gameNoteInstance_Rails[i].Count - 1 - 1;
                            gameNoteInstance_Rails[i][startNoteIdx].GetComponent<EJGameNote>().connectNote(noteInstance.gameObject);

                            //생성된 startNote 칸을 지워준다.
                            //그래야 endNote를 0번째 인덱스로 체크할 수 있으니까
                            //noteInstance_Rails[i].RemoveAt(0);
                            //noteRemove(i);
                            print("*22222 noteInstantiate 실행 - noteInstance의 type은" + noteInstance.noteInfo.type + "noteInstance의 isLongStart는" + noteInstance.noteInfo.isLongNoteStart + "현재열의 0번에 담긴 것의 isLongNoteStart는" + gameNoteInstance_Rails[i][0].noteInfo.isLongNoteStart);
                        }
                    }


                    //01-2. Note_AutoDestroy
                    //print("*55555 noteInstance의 enable상태는" + noteInstance.noteInfo.isNoteEnabled);

                    noteInstance.autoDestroyAction = (railIdx, noteInfo, isPassed) =>
                    {
                        //Pass without Press
                        if (isPassed) isTouchPadPressed[railIdx] = false;
                        //Pass >> remove from List
                        //
                        gameNoteInstance_Rails[railIdx].Remove(noteInfo);


                        //!!!!!autoDestroy가 될 때, 해당 레일의 음이 플레이되고 있다면 꺼라.


                        //if (noteInstance.noteInfo.type == (int)NoteType.LONG && !noteInstance.noteInfo.isLongNoteStart && noteInstance.noteInfo.isNoteEnabled) return;

                        if (noteInstance.noteInfo.isNoteEnabled)
                        {
                            //LongNote가 성공하고도 계속 눌려있는 경우기 때문에 miss가 아님!
                            if (noteInstance.noteInfo.type == (int)GameNoteType.LONG && !noteInstance.noteInfo.isLongNoteStart)
                            {

                            }
                            else
                            {
                                print("*****현재 autoDestroyAction에 담긴 실행되는 Note의 isLongStart값은" + noteInstance.noteInfo.isLongNoteStart);
                                //showScoreText(4);
                                EJScoreManager.instance.StartShowScoreText("Miss", railIdx, 0);
                                EJcamShake.instance.StartShake(0.2f, 0.5f, 1);
                            }
                        }
                    };




                }
            }
        }
        #endregion


        //02-1.scoreCheck
        #region scoreCheck by touchPhase

#if UNITY_EDITOR //check FINISHED!!!
        //if (Input.touchCount > 0)
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            //이걸로 다시 만들기
            //Drag 0번 눌르고 처음에서 began되고 마지막에서 떼지는지 체크해야함.
            //떼지않고 다른 버튼이 눌린다면 전 버튼이 사라지도록 해야함.

            if (/*touch.phase == TouchPhase.Began*/Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("touchPad")))
                {
                    //TouchPad 번호 확인
                    string touchPadName = hitInfo.transform.name;
                    touchPadName = touchPadName.Replace("Touch0", "");
                    int touchIdx = int.Parse(touchPadName) - 1;

                    touchStartedIdx = touchIdx;

                    dicCurrTouchPadIdx[0] = -1;
                    touchedFX(touchIdx, 0);

                    if (gameNoteInstance_Rails[touchIdx].Count > 0)
                    {
                        //NoteType 확인
                        if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.SHORT)
                        {
                            ScoreCheck_SHORT(touchIdx);
                        }
                        else if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.LONG && gameNoteInstance_Rails[touchIdx][0].noteInfo.isLongNoteStart)
                        {
                            EnterCheck_LONG(touchIdx);
                        }
                        else if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT || gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
                        {
                            EnterCheck_DRAG(touchIdx);
                        }
                    }
                }
            }   //check FINISHED!!!

            if (/*touch.phase == TouchPhase.Moved*/Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Ray ray = Camera.main.ScreenPointToRay(touch);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("touchPad")))
                {
                    string touchPadName = hitInfo.transform.name;
                    touchPadName = touchPadName.Replace("Touch0", "");
                    int touchIdx = int.Parse(touchPadName) - 1;

                    touchedFX(touchIdx, 0);

                    if (gameNoteInstance_Rails[touchStartedIdx].Count > 0)
                    {
                        if (touchStartedIdx != touchIdx)
                        {
                            print("방향 체크 전 touchId는" + touchIdx + "touchStartedIdx는" + touchStartedIdx);

                            //방향 체크
                            if (touchIdx < touchStartedIdx)     //왼쪽 드래그
                            {
                                draggingState = DraggingState.Dragging_LEFT;

                                if (gameNoteInstance_Rails[touchStartedIdx].Count > 0 &&
                                    gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
                                {
                                    print("현재 idx는" + touchIdx + "이고" + " startedIdx는" + touchStartedIdx);
                                    //print("왼쪽으로 드래그되고 있습니다");

                                    //deltaPos가 오른쪽인지를 체크하기! 같은 방향으로 움직이고 있는지
                                    PressingScore(touchStartedIdx);
                                    //showScoreText(9);
                                }
                            }
                            else    //오른쪽 드래그
                            {
                                draggingState = DraggingState.Dragging_RIGHT;

                                if (gameNoteInstance_Rails[touchStartedIdx].Count > 0 &&
                                    gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT)
                                {
                                    //print("오른쪽으로 드래그되고 있습니다");
                                    PressingScore(touchStartedIdx);
                                    //showScoreText(10);
                                }
                            }
                        }
                        else //같은 버튼을 꾹 누르는 것   checkFINISHED !!!
                        {
                            draggingState = DraggingState.None;

                            if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.LONG)
                            {
                                PressingScore(touchIdx);
                                //print("*55666 LongNote가 눌리고 있습니다");
                                //showScoreText(7);
                            }
                        }
                    }
                }
            }  //check FINISHED!!!

            if (/*touch.phase == TouchPhase.Ended*/Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Ray ray = Camera.main.ScreenPointToRay(touch);
                RaycastHit hitInfo;

                releasedFX(0);
                dicCurrTouchPadIdx.Remove(0);

                if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("touchPad")))
                {
                    string touchPadName = hitInfo.transform.name;
                    touchPadName = touchPadName.Replace("Touch0", "");
                    int touchIdx = int.Parse(touchPadName) - 1;

                    //뗀 곳의 pad 번호 확인
                    touchReleasedIdx = touchIdx;

                    if (gameNoteInstance_Rails[touchStartedIdx].Count > 0)
                    {
                        if (touchStartedIdx != touchReleasedIdx)
                        {
                            if (gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT || gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
                            {
                                ExitCheck_DRAG(touchStartedIdx);
                            }
                        }
                        else
                        {
                            if (gameNoteInstance_Rails[touchIdx].Count > 0 && gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.LONG && !gameNoteInstance_Rails[touchIdx][0].noteInfo.isLongNoteStart)
                            {
                                ExitCheck_LONG(touchIdx);
                            }
                        }
                    }
                }
            }   //check FINISHED!!!

        }
#endif


        if (Input.touchCount > 0)
        {
            print("현재 touchCount는" + Input.touchCount);

            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                //touch.fingerId = i;

                if (touch.phase == TouchPhase.Began)
                {

                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("touchPad")))
                    {
                        //TouchPad 번호 확인
                        string touchPadName = hitInfo.transform.name;
                        touchPadName = touchPadName.Replace("Touch0", "");
                        int touchIdx = int.Parse(touchPadName) - 1;

                        touchStartedIdx = touchIdx;

                        dicCurrTouchPadIdx[touch.fingerId] = -1;
                        touchedFX(touchIdx, touch.fingerId);

                        print(i + "번째 touch일 때" + touchIdx + "번의 터치패드가 눌렸고" + "touchedFX 함수가 실행되었다.");

                        if (gameNoteInstance_Rails[touchIdx].Count > 0)
                        {
                            //NoteType 확인
                            if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.SHORT)
                            {
                                ScoreCheck_SHORT(touchIdx);
                            }
                            else if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.LONG && gameNoteInstance_Rails[touchIdx][0].noteInfo.isLongNoteStart)
                            {
                                EnterCheck_LONG(touchIdx);
                            }
                            else if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT || gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
                            {
                                EnterCheck_DRAG(touchIdx);
                            }
                        }
                    }
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("touchPad")))
                    {
                        string touchPadName = hitInfo.transform.name;
                        touchPadName = touchPadName.Replace("Touch0", "");
                        int touchIdx = int.Parse(touchPadName) - 1;

                        touchedFX(touchIdx, touch.fingerId);

                        if (gameNoteInstance_Rails[touchStartedIdx].Count > 0)
                        {
                            if (touchStartedIdx != touchIdx)
                            {
                                //방향 체크
                                if (touchIdx < touchStartedIdx)     //왼쪽 드래그
                                {
                                    draggingState = DraggingState.Dragging_LEFT;

                                    if (gameNoteInstance_Rails[touchStartedIdx].Count > 0 &&
                                        gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
                                    {
                                        print("현재 idx는" + touchIdx + "이고" + " startedIdx는" + touchStartedIdx);
                                        print("왼쪽으로 드래그되고 있습니다");

                                        //deltaPos가 오른쪽인지를 체크하기! 같은 방향으로 움직이고 있는지
                                        PressingScore(touchStartedIdx);
                                        //showScoreText(9);
                                    }
                                }
                                else    //오른쪽 드래그
                                {
                                    draggingState = DraggingState.Dragging_RIGHT;

                                    if (gameNoteInstance_Rails[touchStartedIdx].Count > 0 &&
                                        gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT)
                                    {
                                        print("오른쪽으로 드래그되고 있습니다");
                                        PressingScore(touchStartedIdx);
                                        //showScoreText(10);
                                    }
                                }
                            }
                            else //같은 버튼을 꾹 누르는 것   checkFINISHED !!!
                            {
                                draggingState = DraggingState.None;

                                if (gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.LONG)
                                {
                                    PressingScore(touchIdx);
                                    //showScoreText(7);
                                }
                            }
                        }
                    }

                }

                if (touch.phase == TouchPhase.Ended)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hitInfo;

                    releasedFX(touch.fingerId);
                    dicCurrTouchPadIdx.Remove(touch.fingerId);

                    if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("touchPad")))
                    {
                        string touchPadName = hitInfo.transform.name;
                        touchPadName = touchPadName.Replace("Touch0", "");
                        int touchIdx = int.Parse(touchPadName) - 1;

                        //뗀 곳의 pad 번호 확인
                        touchReleasedIdx = touchIdx;

                        if (gameNoteInstance_Rails[touchStartedIdx].Count > 0)
                        {
                            if (touchStartedIdx != touchReleasedIdx)
                            {
                                if (gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT || gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
                                {
                                    ExitCheck_DRAG(touchStartedIdx);
                                }
                            }
                            else
                            {
                                if (gameNoteInstance_Rails[touchIdx].Count > 0 && gameNoteInstance_Rails[touchIdx][0].noteInfo.type == (int)GameNoteType.LONG && !gameNoteInstance_Rails[touchIdx][0].noteInfo.isLongNoteStart)
                                {
                                    ExitCheck_LONG(touchIdx);
                                }
                            }
                        }
                    }

                }
            }
        }


        #endregion
    }


    void noteInstantiate(int n, EJGameNote noteInstance)
    {
        //현재 리스트에 대기열의 정보를 넣어서 Insert
        noteInstance.noteInfo = gameNoteInfo_Rails[n][0];
        gameNoteInstance_Rails[n].Add(noteInstance);
        //대기열에서 Remove
        gameNoteInfo_Rails[n].RemoveAt(0);
    }

    void noteRemove(int n)
    {
        //현재 리스트에서 Remove
        gameNoteInstance_Rails[n].RemoveAt(0);
    }

    void noteUnable(int n)
    {
        //현재 리스트의 unable
        gameNoteInstance_Rails[n][0].noteInfo.isNoteEnabled = false;

        //현재 리스트에서 remove
        //noteInstance_Rails[n].RemoveAt(0);
    }

    //현재 touch된 부분만 켜지고 나머지는 꺼지도록 check!!!
    //int currTouchPadIdx = -1;

    //Dictionary<fingerID, railIdx>
    Dictionary<int, int> dicCurrTouchPadIdx = new Dictionary<int, int>();

    //fingerId별 startIdx가 필요    
    //*****Dictionary<fingerID, startIdx>
    Dictionary<int, int> dicStartIdx = new Dictionary<int, int>();



    //pitch를 Rail로 만들기

    void touchedFX(int n, int fingerId)
    {
        //n = railIdx
        //fingerId = touchpad

        print("touchTEST: touchedFX가 실행되었고 실행된 레일은" + n + "이다.");

        if (dicCurrTouchPadIdx.ContainsKey(fingerId) == false) return;
        if (n == dicCurrTouchPadIdx[fingerId]) return;

        if (gameNoteInstance_Rails[n].Count > 0)
        {
            pitches_rail[n] = gameNoteInstance_Rails[n][0].noteInfo.pitch;

        }

        if (dicCurrTouchPadIdx[fingerId] != -1)
        {
            releasedFX(fingerId);
        }

        if (/*!touchpads[n].GetComponent<MeshRenderer>().enabled &&*/ !QuadTouches[n].activeSelf)
        {
            /*touchpads[n].GetComponent<MeshRenderer>().enabled = true;*/
            QuadTouches[n].SetActive(true);
            QuadRails[n].GetComponent<MeshRenderer>().material = QuadRails_Mat[n];

            GameObject touchEffect = Instantiate(touchFX, touchFX_pos[n]);
            Destroy(touchEffect, 1);


            MIDIPlayer.instance.NoteOn(pitches_rail[n]);


            //on 해주고 해당음을 뗄 때 off해주고
            //MIDIPlayer.instance.PlayOneMidiEvent();
        }

        dicCurrTouchPadIdx[fingerId] = n;
    }


    void releasedFX(int fingerId)
    {
        print("touchTEST:  releasedFX가 실행되었고 실행된 fingerId" + fingerId + "이다.");


        if (dicCurrTouchPadIdx.ContainsKey(fingerId) == false) return;

        int n = dicCurrTouchPadIdx[fingerId];
        print("releasedFX함수 실행");

        if (n == -1) return;

        if (/*touchpads[n].GetComponent<MeshRenderer>().enabled &&*/ QuadTouches[n].activeSelf)
        {
            /*touchpads[n].GetComponent<MeshRenderer>().enabled = false;*/
            QuadTouches[n].SetActive(false);
            QuadRails[n].GetComponent<MeshRenderer>().material = QuadRails_Mat[n + 6];

            MIDIPlayer.instance.NoteOff(pitches_rail[n]);
            print("touchTEST:  releasedFX가 실행되었고 NoteOFF가 실행되었다.");
        }

        if (gameNoteInstance_Rails[n].Count == 0)
        {
            pitches_rail[n] = 20;
        }
    }

    int finaleCount = 30;
    int fireworkCount = 15;
    int n = 0;
    int k = 0;
    int dir = 1;

    public IEnumerator finaleFX()
    {
        for (int i = 0; i < finaleCount; i++)
        {
            n += dir;

            print("n은" + n);

            if (n == 5)
            {
                dir *= -1;

            }

            if (n == 0)
            {
                dir *= -1;

            }
            GameObject fx = Instantiate(touchfireFX, touchfireFX_pos[n]);
            //audiosource.PlayOneShot(SFXs[0]);
            yield return new WaitForSeconds(0.3f);
        }
        n = 0;
    }


    public IEnumerator fireworkkFX()
    {
        for (int i = 0; i < fireworkCount; i++)
        {
            k += dir;

            print("은" + n);

            if (k == 2)
            {
                dir *= -1;

            }

            if (k == 0)
            {
                dir *= -1;

            }

            GameObject fx = Instantiate(fireworkFX, fireworkFX_pos[k]);
            audiosource[1].PlayOneShot(SFXs[1]);

            yield return new WaitForSeconds(1);
        }
        k = 0;
    }

    public void startcoFinaleFX()
    {
        audiosource[0].PlayOneShot(SFXs[0]);
        StartCoroutine(finaleFX());
        StartCoroutine(fireworkkFX());
    }


    public void ScoreCheck_SHORT(int n)
    {
        if (gameNoteInstance_Rails[n][0] == null) return;

        dist = gameNoteInstance_Rails[n][0].transform.position.y - touchpads[n].transform.position.y;
        distAbs = Mathf.Abs(dist);


        if (distAbs > badZone)
        {
            //내려오는 중이니까 터치해도 의미가 없다.
            return;
            //passDestroy가 되지 않기 위함
        }
        else if (distAbs > goodZone)
        {
            //Bad
            //showScoreText(3);
            EJScoreManager.instance.StartShowScoreText("Bad", n, badScore);
            EJScoreManager.instance.SCORE += badScore;
        }
        else if (distAbs > greatZone)
        {
            //Good
            //showScoreText(2);
            EJScoreManager.instance.StartShowScoreText("Good", n, goodScore);
            EJScoreManager.instance.SCORE += goodScore;
        }
        else if (distAbs > excellentZone)
        {
            //Great
            //showScoreText(1);
            EJScoreManager.instance.StartShowScoreText("Great", n, greatScore);
            EJScoreManager.instance.SCORE += greatScore;
        }
        else
        {
            //Excellent
            //showScoreText(0);
            EJScoreManager.instance.StartShowScoreText("Excellent", n, excellentScore);
            EJScoreManager.instance.SCORE += excellentScore;
        }

        //Handheld.Vibrate();
        PressDestroy(n);
    } //check_FINISHED!!!

    public void EnterCheck_LONG(int n)
    {
        print("*33333 EnterCheckLong이 실행되었고 현재 noteInstance의 0번째 칸엔" + gameNoteInstance_Rails[n][0] + "이 담겼다");
        if (gameNoteInstance_Rails[n][0] == null) return;

        dist = gameNoteInstance_Rails[n][0].transform.position.y - touchpads[n].transform.position.y;
        distAbs = Mathf.Abs(dist);


        if (distAbs < badZone)
        {

            //success
            print("*44444 LongNote가 Enter에 성공했고 현재 noteInstance의 0번째 칸의 isLongNotestart값은" + gameNoteInstance_Rails[n][0].noteInfo.isLongNoteStart + "이 담겼다");
            //showScoreText(5);
            noteUnable(n);
            noteRemove(n);
            print("*55555 LongNote가 Enter에 성공했고 noteRemove실행 후 현재 noteInstance의 0번째 칸의 isLongNotestart값은" + gameNoteInstance_Rails[n][0].noteInfo.isLongNoteStart + "이 담겼다");
        }
        else
        {
            //아직 내려오기 전
            //터치패드 지난 후엔 autoDestroy
            print("***** startNote에 대한 badZone이후가 들어옴");
            noteUnable(n);
            //성공하면 endNote와 체크하도록 지워줘야함
            noteRemove(n);

        }
    } //check_FINISHED!!!

    public void ExitCheck_LONG(int n)
    {
        print("*66666 LongNote에 대한 ExitCheck가 실행되었다.");

        dist = gameNoteInstance_Rails[n][0].transform.position.y - touchpads[n].transform.position.y;
        distAbs = Mathf.Abs(dist);

        if (distAbs < badZone)
        {
            //success
            //showScoreText(0);
            EJScoreManager.instance.StartShowScoreText("Excellent", n, excellentScore);
            //noteUnable(n);
            //misscheck를 하면 안된다!!!

        }
        else if (dist > badZone)
        {
            //miss
            print("*77777 LongNote에 대한 Exit이 실패했고, 현재 longNote의 enable상태는" + gameNoteInstance_Rails[n][0].noteInfo.isNoteEnabled);
            noteUnable(n);
            print("*88888 LongNote에 대한 Exit이 실패했고, unable함수 실행 후, 현재 longNote의 enable상태는" + gameNoteInstance_Rails[n][0].noteInfo.isNoteEnabled);

            //noteInstance_Rails[n].RemoveAt(0);

            MissCheck();
            print("*99999 longNoteExitCheck에서의 misscheck가 실행되었다");
            //print("*22222-2 remove함수 실행 후" + noteInstance_Rails[n][0] + "은 null 이어야 하는데!");
            //unabled
        }

        noteRemove(n);
        //noteInstance_Rails[n].RemoveAt(0);
    }   //check_FINISHED!!!

    public void EnterCheck_DRAG(int n)
    {
        if (gameNoteInstance_Rails[n][0] == null) return;

        dist = gameNoteInstance_Rails[n][0].transform.position.y - touchpads[n].transform.position.y;
        distAbs = Mathf.Abs(dist);


        if (distAbs < badZone)
        {
            //success
            print("***dragEnter 성공 했다!");
        }
        //else if (dist < 0)
        //{
        //    //혹시나 startIndex누르지 않고 autoDestroy에 걸리는 타이밍엔 unable 상태 체크 안해도 될거 같긴 함.
        //    noteUnable(n);
        //    print("***noteUnable 되었다!");
        //    return;
        //}


    }   //check FINISHED!!!

    public void ExitCheck_DRAG(int n)
    {
        if (gameNoteInstance_Rails[n][0] == null) return;

        //이미 뗀 곳이 올바른 위치라는 것을 확인한 후니까!!!
        distAbs = Mathf.Abs(touchpads[n].transform.position.y - gameNoteInstance_Rails[n][0].transform.position.y);
        dist = gameNoteInstance_Rails[n][0].transform.position.y - touchpads[n].transform.position.y;

        if (distAbs < badZone)
        {
            //success
            if (touchReleasedIdx == gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.DRAG_release_idx && draggingState == DraggingState.Dragging_LEFT && gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_LEFT)
            {
                //success

                print("왼쪽 드래그 노트 성공했어요!");
                PressDestroy(touchStartedIdx);
                //showScoreText(0);
                EJScoreManager.instance.StartShowScoreText("Excellent", n, excellentScore);
            }
            else if (touchReleasedIdx == gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.DRAG_release_idx && draggingState == DraggingState.Dragging_RIGHT && gameNoteInstance_Rails[touchStartedIdx][0].noteInfo.type == (int)GameNoteType.DRAG_RIGHT)
            {
                print("오른쪽 드래그 노트 성공했어요!");
                PressDestroy(touchStartedIdx);
                //showScoreText(0);
                EJScoreManager.instance.StartShowScoreText("Excellent", n, excellentScore);
            }
            else
            {
                MissCheck();
            }

        }
        else
        {
            //miss
            MissCheck();
        }
    }


    public void PressingScore(int n)
    {
        if (gameNoteInstance_Rails[n][0] == null) return;

        distAbs = Mathf.Abs(touchpads[n].transform.position.y - gameNoteInstance_Rails[n][0].transform.position.y);
        dist = gameNoteInstance_Rails[n][0].transform.position.y - touchpads[n].transform.position.y;

        //if (distAbs < badZone)
        {
            EJScoreManager.instance.SCORE += pressScore * Time.deltaTime;

        }
        //else
        {
            //note가 판정 범위 내에 있지 않은 경우 score증가 X
        }
    }

    public void MissCheck()
    {
        //showScoreText(4);
        Handheld.Vibrate();
        EJScoreManager.instance.StartShowScoreText("Miss", 0, 0);

        //안되는 이유..? 한프레임이라?
        EJcamShake.instance.StartShake(0.2f, 0.5f, 1);
    }

    public void PressDestroy(int n)
    {
        Destroy(gameNoteInstance_Rails[n][0].gameObject);
        gameNoteInstance_Rails[n].RemoveAt(0);

        //note에서 FX나오기
    }

    public void MissUnabled(int n)
    {
        print("note가 enabled == false되었다");
        //long이나 drag가 눌리다가 끝까지 눌리지 못한 경우
        //passDestroy까지의 기간 동안 점수 체크가 되지 못하도록 해야함.

        gameNoteInstance_Rails[n][0].noteInfo.isNoteEnabled = false;
        gameNoteInstance_Rails[n].RemoveAt(0);
    }

    //01. NoteType.SHORT test
    #region SHORT
    void InputTestSHORTNotes()
    {
        GameNoteInfo info = new GameNoteInfo();

        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 7 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 8 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }
    }
    #endregion

    //02. NoteType.Long test
    #region LONG
    void InputTestLONGNotes()
    {
        GameNoteInfo info = new GameNoteInfo();

        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 1 * bpm;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 4 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        //info.railIdx = 3;
        //info.type = (int)NoteType.LONG;
        //info.time = 4;
        //info.isLongNoteStart = true;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info.railIdx = 3;
        //info.type = (int)NoteType.LONG;
        //info.time = 5;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }
    }
    #endregion

    //03. NoteType.Drag test
    #region DRAG
    void InputTestDRAGNote()
    {
        GameNoteInfo info = new GameNoteInfo();

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 1 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 4 * bpm;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 4 * bpm;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 4 * bpm;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 6 * bpm;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 6 * bpm;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }

    }
    #endregion

    //04. Mixed test
    #region MIXED
    void InputTestMIXEDNote()
    {
        GameNoteInfo info = new GameNoteInfo();

        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 7;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 9;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 2;
        info.isLongNoteStart = true;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 4;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 7;
        info.isLongNoteStart = true;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 8;
        info.isLongNoteStart = false;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 5;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 5;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }

    }

    #endregion

    //05. FLOP test
    #region FLOP
    void InputTestFLOP()
    {
        GameNoteInfo info = new GameNoteInfo();

        #region Pattern01

        //마디 1) Pattern 1 - Short
        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 31;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 61;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 76;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 121;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 151;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 181;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 211;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 2) Pattern 1
        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 241;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 271;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 301;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 316;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 361;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 391;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 421;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 451;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 3) Pattern 1
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 481;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 511;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 541;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 556;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 601;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 631;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 661;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 691;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 4) Pattern 1
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 721;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 751;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 781;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 796;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 841;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 871;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 901;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 931;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        #endregion

        #region Pattern02
        //마디 5) Pattern 2
        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 961;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 976;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 983.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 998.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1006;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1021;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1028.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1043.5f;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1058.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1081f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1096f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1103.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1118.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1126f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1141f;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1178.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 6) Pattern 2
        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1201;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1216;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1223.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1238.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1246;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1261;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1305;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1320;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1327.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1342.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1350;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.LONG;
        info.time = 1365;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.LONG;
        info.time = 1411;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 7) Pattern 2
        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1441;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1456;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1463.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1478.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1486;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1501;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1508.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1523.5f;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //***
        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1550;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1561;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1576;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1583.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1598.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1606;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1621;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1658.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 8) Pattern 2

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1681;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1696;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1703.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1718.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1726;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1741;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1785;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1800;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1807.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1822.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1830;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 1845;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 1891;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region Pattern03

        //마디 9) Pattern 3
        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 1921;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 1921;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 1951;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 1981;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 1981;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2011;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2041;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2041;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2071;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2101;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2101;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2131;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //마디 10) Pattern 3

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2161;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2161;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2191;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2221;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2221;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2251;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2281;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2281;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2311;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2341;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2363.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2371;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2393.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 11) Pattern 3
        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2401;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2401;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2431;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2461;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2461;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2491;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2521;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2521;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2551;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2581;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2581;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2611;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //마디 12) Pattern 3
        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2641;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2641;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2671;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2701;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 2701;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2731;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.DRAG_RIGHT;
        //info.time = 2761;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 5;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.DRAG_LEFT;
        //info.time = 2761;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = 0;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2791;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2821;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 2;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2843.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 2851;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2873.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region Pattern04

        //마디 13-14) Pattern 4
        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 2881;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 3090;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 3091;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 3300;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = false;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3301;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3323.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 3331;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 3353.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //마디 15-16) Pattern 4

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 3361;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 3570;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 3571;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 3780;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 4;
        //info.type = (int)NoteType.SHORT;
        //info.time = 3781;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        //info = new NoteInfo();
        //info.railIdx = 3;
        //info.type = (int)NoteType.SHORT;
        //info.time = 3803.5f;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3811;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3833.5f;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion


        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }
    }
    #endregion


    int madi = 120;
    int madiMinus = 150;
    int madiplus = 60;
    void InputCameraMan()
    {
        GameNoteInfo info = new GameNoteInfo();

        #region PatternA

        #region 1마디
        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 16;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 31;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 46;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 61;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 2마디
        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 181;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 196;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 211;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 226;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 241;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region 3마디
        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 361;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 376;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 391;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 406;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 421;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region 4마디

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 541 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 571 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 616 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //1-2-1-3-4-5

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 661 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 676 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 691 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 706 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 721 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 736 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion


        #endregion

        #region PatternB-1

        #region 1마디

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 781 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 826 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 871 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 931 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 931 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 2마디

        //2-2-2-0-1-2-4-5

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1021 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1066 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1111 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1141 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1156 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1171 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1201 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1231 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 3마디


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1261 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1306 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1351 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 1411 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 1471 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 4마디

        //2-2-2-0-1-2-4-5

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1501 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1546 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1591 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1621 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1636 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1651 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1681 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1711 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #endregion

        #region PatternC-1

        #region 1마디

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 1741 + madi;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 1890 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1891 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1891 + 150 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 2116 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 2116 + 135 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2281 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2341 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 2401 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 2401 + 210 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2671 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2701 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2761 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2791 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2806 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion


        #endregion

        //여기부터 다시
        #region PatternB-2

        #region 1마디

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2851 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2896 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2941 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 3001 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 3001 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 2마디

        //2-2-2-0-1-2-4-5

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3061 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3091 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3136 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3181 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3211 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3226 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3241 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3271 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 3마디

        //
        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3301 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3301 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3331 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 3376 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 0;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 3376 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = 5;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 4마디

        //2-2-2-0-1-2-4-5

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3421 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3481 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3541 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3571 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3616 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3661 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3691 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3706 + madi;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #endregion

        #region PatternC-2

        #region 1마디

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 3811 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 3811 + 150 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 3961 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 3961 + 150 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 4036 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 4036 + 135 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4201 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4261 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 4321 + madi - madiMinus;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 4321 + 210 + madi - madiMinus;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4591 + madi - 60;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4621 + madi - 60;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4681 + madi - 60;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4711 + madi - 60;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4726 + madi - 60;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion


        #endregion

        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }
    }


    int madi2 = 240;
    void input150beats()
    {
        GameNoteInfo info = new GameNoteInfo();

        #region 1마디

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 181;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 241;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 421;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 481;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 661;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //info = new GameNoteInfo();
        //info.railIdx = 1;
        //info.type = (int)GameNoteType.LONG;
        //info.time = 721;
        //info.isLongNoteStart = true;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allGameNoteInfo.Add(info);

        //info = new GameNoteInfo();
        //info.railIdx = 1;
        //info.type = (int)GameNoteType.LONG;
        //info.time = 901;
        //info.isLongNoteStart = false;
        //info.DRAG_release_idx = -1;
        //info.isNoteEnabled = true;
        //allGameNoteInfo.Add(info);
        #endregion

        #region 2마디

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 961 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 1080 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1081 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1081 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1201 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1320 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1321 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1381 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1441 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1501 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1561 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1621 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1681 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1801 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1861 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 3마디

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1921 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 2040 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2041 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2101 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2161 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 2161 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2281 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2341 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2401 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 2401 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2641 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 2641 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 4마디

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 2881 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 3061 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 3121 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 3301 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 3361 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 3361 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        //
        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3481 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3511 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3601 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3661 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3691 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3721 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3781 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region 5마디

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3841 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3961 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3991 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4081 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4141 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4171 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4201 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4261 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 4321 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 4440 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4441 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4471 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4561 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4621 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4651 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4681 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4741 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4771 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region 6마디

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 4801 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 4801 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4921 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4951 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5041 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5101 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5131 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5161 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5221 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5281 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5311 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5341 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5371 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5401 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5431 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5521 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5551 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5581 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5611 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5641 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5671 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5791 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 5851 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region 7마디

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6001 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6031 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6061 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6091 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6121 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6151 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6271 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 6331 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 6391 - madi2;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 6511 - madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        #endregion

        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {

            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }
    }

    void inputAPEX()
    {
        GameNoteInfo info = new GameNoteInfo();

        #region 1-1
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 31;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 61;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 76;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 106;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 136;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 166;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 226;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 1-2
        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 241;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 271;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 301;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 316;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 346;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 376;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 406;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 466;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 1-3

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 481;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 511;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 541;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 556;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 586;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 616;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 646;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 706;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 1-4

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 721;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 751;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 781;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 796;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 826;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 856;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 886;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 946;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 2-1

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 961;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 961;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 991;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1021;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1036;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1066;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1126;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1171;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1186;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        #endregion

        #region 2-2


        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 1201;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 1201;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1231;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1261;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1291;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1321;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1380;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1411;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1426;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 2-3


        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 1441;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 1441;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1471;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1501;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1516;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 1561;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1591;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1621;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1651;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        #endregion

        #region 2-4

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1681;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1711;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1741;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1756;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1786;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 1905;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 2-5

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1906;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1921;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1966;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 1996;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2026;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2041;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2086;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2116;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2146;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2161;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2206;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2236;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 2266;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 2385;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);



        #endregion

        #region 2-6

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2386;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2401;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2446;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2476;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2506;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2521;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2566;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2596;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2626;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2641;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2686;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 0;
        info.type = (int)GameNoteType.SHORT;
        info.time = 2716;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 2746;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 2865;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);



        #endregion

        #region 3-1

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 2866;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 2866;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3001;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3016;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3061;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3106;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3166 + madi2;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3196;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3211;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3271;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3316;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3406;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3436;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3451;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3511;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3556;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3646;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3661;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3691;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3721;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        #endregion

        #region 3-2

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.DRAG_RIGHT;
        info.time = 3781;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.DRAG_LEFT;
        info.time = 3781;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3916;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3931;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 3976;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4021;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4051;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4111;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4141;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4156;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4171;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4216;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4261;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4291;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4351;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4381;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4396;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4411;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4456;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4501;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4591;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4606;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4636;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.SHORT;
        info.time = 4666;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);


        #endregion


        #region 4

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 4696;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 4815;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 4816;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 4875;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.LONG;
        info.time = 4876;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.LONG;
        info.time = 4935;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 4836;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 5055;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 5056;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 5115;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 5116;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 3;
        info.type = (int)GameNoteType.LONG;
        info.time = 5175;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 5176;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 2;
        info.type = (int)GameNoteType.LONG;
        info.time = 5295;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 5296;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 4;
        info.type = (int)GameNoteType.LONG;
        info.time = 5355;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);
        //

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.LONG;
        info.time = 5356;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 5;
        info.type = (int)GameNoteType.LONG;
        info.time = 5415;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 5416;
        info.isLongNoteStart = true;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);

        info = new GameNoteInfo();
        info.railIdx = 1;
        info.type = (int)GameNoteType.LONG;
        info.time = 5656;
        info.isLongNoteStart = false;
        info.DRAG_release_idx = -1;
        info.isNoteEnabled = true;
        allGameNoteInfo.Add(info);



        #endregion


        for (int i = 0; i < gameNoteInfo_Rails.Length; i++)
        {
            gameNoteInfo_Rails[i] = new List<GameNoteInfo>();
        }

        for (int i = 0; i < allGameNoteInfo.Count; i++)
        {
            gameNoteInfo_Rails[allGameNoteInfo[i].railIdx].Add(allGameNoteInfo[i]);
        }
    }


    //scoreManager Script로 이전
    void showScoreText(int n)
    {
        GameObject scoreText = Instantiate(scoreTexts[n], canvas.transform.position - Vector3.forward * 2, Quaternion.identity);
        scoreText.transform.SetParent(canvas.transform);

        Destroy(scoreText, 0.5f);
    }

}
