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
    //3���� ������ ��� ��.
    //������ �� �� ��ĭ ������ 
    //������ ��忡 ����..

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log(length);
        IsDragging = true;
        //�巡�׸� ������
        //ó�� ��ġ���� ���� ��ġ�� �Ÿ��� 
        float distance = Input.mousePosition.x - startPos.x;

        //�Ÿ��� ������ ��ȭ �������� ũ�ٸ� 
        //�迭�� �ᵵ �°� ��ũ�� ����Ʈ��  �ᵵ �ǰ� 

        //ũ�� length�� ���ǿ� �´ٸ� 
        if ( (Mathf.Abs(distance) > changeDistance))
        {
            //��ȭ�� �ؾ��Ѵ�.
            //����
            if (distance > 0) {
                CheckMode(EditerMode.instance.currentState,note,1);
            }
            //����
            if (distance < 0) {
                Debug.LogWarning("������ !!");
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
                Debug.Log("Pitch ��ȭ��");
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

    //�ƹ��͵� ���ϰ� ���� ��� ����

    //�巡�׸� �ϰ� ���� ��� ���� ����
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsDragging) {
            //��带 ������ �ֽʼ� 

            //*��� ���� �ڵ� 
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

    //���ŵ� �ڷ� ���ϱ�,���� �ϴ� �Լ�
    public Enum AdderEnum(Enum enumValue, int add) {
        //���� index ��������
        int idx = Convert.ToInt32(enumValue);
        int length = Enum.GetValues(enumValue.GetType()).Length;

        //���� ���Ѱ� 0���� ũ�� ũ�⺸�� ������.
        int temp = idx + add;
        if (temp >= 0 && length > temp)
        {
            return (Enum)Enum.ToObject(enumValue.GetType(), temp);

        }
        else {
            return enumValue;
        }
    }

    //���� ��, (ũ������) , �󸶳� ���Ұ���
    public int Adderint(int value,int minValue,int MaxValue,int add) {
        int temp = value + add;
        if (temp >= minValue && temp <= MaxValue) {
            return temp;
        }
        return value;
    }
}
