using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteDrag : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IDragHandler,IBeginDragHandler,IEndDragHandler
{
    public Notes note;
    //public Notes.Pitch;
    
    public float changeDistance;
    bool IsDragging = false;
    Vector3 startPos;
    private void Awake()
    {
        note = GetComponent<Notes>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = Input.mousePosition;
    }

    float distance;
    int testCount = 0;
    
    //tuuo
    int length = System.Enum.GetValues(typeof(Notes.Pitch)).Length - 1 ;
    //3개의 나열된 어떠한 것.
    //나열된 것 중 한칸 앞으로 
    //나열된 모드에 따라서..

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(length);
        IsDragging = true;
        //드래그를 했을때
        //처음 위치에서 현재 위치의 거리가 
        float distance = Input.mousePosition.x - startPos.x;

        //거리의 절댓값이 변화 지수보다 크다면 
        //배열도 써도 돠고 링크드 리스트도  써도 되고 

        //크고 length가 조건에 맞다면 
        if ( (Mathf.Abs(distance) > changeDistance))
        {
            //변화를 해야한다.
            //증가
            if (distance > 0) {
                CheckMode(EditerMode.instance.currentState,note,1);
            }
            //감소
            if (distance < 0) {
                Debug.LogWarning("감소중 !!");
                CheckMode(EditerMode.instance.currentState, note, -1);
            }
            startPos = Input.mousePosition;
        }
        Debug.LogWarning("MyCount!!" + testCount);

    }
     
    public void CheckMode(EditerMode.EditerState state,Notes note,int arrow)
    {
        MIDIPlayer1.instance.midiStreamSynthesizer.NoteOff(0, note._IPitch);
        switch (state)
        {
            case EditerMode.EditerState.Pitch:
                Debug.Log("Pitch 변화중");
                //note.pitch
                //note._Pitch = (Notes.Pitch) AdderEnum(note._Pitch,arrow);
                note._IPitch = Adderint(note.Ipitch,Notes.MinNoteNum,Notes.MaxNoteNum,arrow);

                //Test
                MIDIPlayer1.instance.midiStreamSynthesizer.NoteOn(0, note.Ipitch, 100,0);

                //Save (Value Copy)
                NoteManager.instance.SaveData.Copy(note.info);
                break;
            case EditerMode.EditerState.Volume:
                
                break;
            case EditerMode.EditerState.Beat:
                note._Beat = (Notes.Beat)AdderEnum(note.beat, arrow);
                break;
            default:
                break;
        }

    }
    public void OnEndDrag(PointerEventData eventData)
    {

        //test
        MIDIPlayer1.instance.midiStreamSynthesizer.NoteOff(0, note._IPitch);
        IsDragging = false;
        Queue queue = new Queue();
        
        

    }

    //아무것도 안하고 떼면 노드 삭제

    //드래그를 하고 떼면 노드 정보 변경
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsDragging) {
            //노드를 삭제해 주십쇼 

            //*노드 삭제 코드 
            note.info.enable = false;
            gameObject.SetActive(false);
        }
    }
     
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.LogWarning("TestNoteDrag");
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //열거된 자료 더하기,빼기 하는 함수
    public Enum AdderEnum(Enum enumValue, int add) {
        //현재 index 가져오기
        int idx = Convert.ToInt32(enumValue);
        int length = Enum.GetValues(enumValue.GetType()).Length;

        //내가 더한게 0보다 크고 크기보다 작은지.
        int temp = idx + add;
        if (temp >= 0 && length > temp)
        {
            return (Enum)Enum.ToObject(enumValue.GetType(), temp);

        }
        else {
            return enumValue;
        }
    }

    //더할 것, (크기제한) , 얼마나 더할건지
    public int Adderint(int value,int minValue,int MaxValue,int add) {
        int temp = value + add;
        if (temp >= minValue && temp <= MaxValue) {
            return temp;
        }
        return value;
    }
}
