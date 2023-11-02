using Melanchall.DryWetMidi.Multimedia;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class EJLongNoteTouchTest : MonoBehaviour
{
    //startNote: keydown
    //connectNote : key
    //endNote: keyup

    GameObject startNote, connectNote, endNote;

    bool isStartNoteSuccessed;
    bool isConnectNoteSuccessed;
    bool isEndNoteSuccessed;

    //keydown������ startNote ��ġ�� ��𿴴���
    //keyup ������ endNote ��ġ�� ��𿴴���
    
    public GameObject touchpadmodel;
    EJTouchPad touchpad;

    //bool isKeyDown = true;
    Stopwatch stopwatch = new Stopwatch();

    // Start is called before the first frame update
    void Start()
    {
        handState = HandState.None;
        touchpad = touchpadmodel.GetComponent<EJTouchPad>();
    }

    // Update is called once per frame
    void Update()
    {
        #region longNoteCheck01
        if (touchpad.isTriggered)
        {
            stopwatch.Start();

            //Ʈ���� �ǰ� ������ ����üũ, ���߿� ������ miss �߱�
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                stopwatch.Stop();
                long keydownTime = stopwatch.ElapsedMilliseconds;
                print(keydownTime);
                print("keydown");
                //isKeyDown = true;
                stopwatch.Start();
            }
            else if (Input.GetKey(KeyCode.Alpha0))
            {
                print("key");
            }
            else if (Input.GetKeyUp(KeyCode.Alpha0))
            {
                stopwatch.Stop();
                long keyupTime = stopwatch.ElapsedMilliseconds;
                print("keyup");
                print(keyupTime);
                //isKeyDown = false;
            }            
        }
        #endregion

        //01. �õ�1: key event �������� note�� �������� ��ġ üũ
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //KeyDown�� ������ startNote�� touchpad�� �Ÿ��� ���Ѵ�.
            float startP = Vector3.Distance(startNote.transform.position, touchpad.transform.position);

            //startNote�� ���� ���� ���� ���Դٸ� success����
            if (startP < 0.5f)
            {
                isStartNoteSuccessed = true;
            }
        }else if (Input.GetKey(KeyCode.Alpha2) && isStartNoteSuccessed)
        {
            //startNote�� ���� ���ȴٸ� connectNoteüũ�� �ض�.
            float connectP = Vector3.Distance(connectNote.transform.position, touchpad.transform.position);

            float connectLength = endNote.transform.position.y - startNote.transform.position.y;

            if (Mathf.Abs(connectP) < connectLength)
            {
                isConnectNoteSuccessed = true;
            }
        }else if (Input.GetKeyUp(KeyCode.Alpha2) && isConnectNoteSuccessed)
        {
            float endP = Vector3.Distance(endNote.transform.position, touchpad.transform.position);
            
            if (endP < 0.5)
            {
                isConnectNoteSuccessed = true;
            }
        }

       
    }

    //02. �õ�2: note �������� ��ġ �������� key event üũ

    enum HandState
    {
        None,
        Pressed,
        Pressing,
        Released
    }

    HandState handState;
    int score;

    private void OnTriggerStay(Collider other)
    {        
        //handState üũ  (Update�� �ű��)                       
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            handState = HandState.Pressed; 
        }
        else if (Input.GetKey(KeyCode.Alpha0))
        {
            handState = HandState.Pressing;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha0)) 
        {
            handState = HandState.Released;
        }

        //note�� ������ ���� Ű�̺�Ʈ ���� �� ���� ����
        if (other.CompareTag("startNote"))
        {
            if (handState == HandState.Pressed)
            {
                //�Ÿ� üũ �� ���� ����
                isStartNoteSuccessed = true;
            }else
            {
                //miss
            }
        }
        
        if (other.CompareTag("connectNote"))
        {
            //startNote�� ���Ȱ� Pressing �̶�� ���� üũ
            if (handState == HandState.Pressing && isStartNoteSuccessed)
            {
                //���� ����

                if(true/*connectNote�� touchPad�� �Ÿ��� 0�� �Ǹ�*/)
                {
                    isConnectNoteSuccessed = true;
                }

            }
            else
            {
                //miss
            }
        }

        if (other.CompareTag("endNote"))
        {
            if (handState == HandState.Released && isConnectNoteSuccessed)
            {
                //�Ÿ� üũ �� ���� ����
            }
        }
    }

}
