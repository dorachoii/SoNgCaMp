using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJLongNoteScore : MonoBehaviour
{
    Transform touchPad;
    
    GameObject startNote;
    GameObject endNote;

    Transform startNoteTriggered;
    Transform endNoteTriggered;

    bool startNoteSuccess;
    bool linkNoteSuccess;
    bool endNoteSuccess;

    //���� üũ�ؼ� ������ �� true��� ������ üũ�Ѵ�.
    // start�� end�� ������ ��ճ��� �Ҵ��ϴ� �ý����� �ʿ��ϴ�.
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isKeyDown;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isKeyDown = true;
            startNoteTriggered = startNote.transform;

        }
        else if (Input.GetKey(KeyCode.Space))
        {
            
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            isKeyDown= false;
            endNoteTriggered = endNote.transform;
        }

        
    }

    //start
    private void OnTriggerStay(Collider other)
    {
        if (isKeyDown)
        {
            print("success");

            //start�� triggerpad ��ġ �����ؼ� ���� ����
            //end�� triggerpad ��ġ �����ؼ� ���� ����
            // ���� üũ 

        }

        if (!isKeyDown)
        {
            print("fail");
        }
    }

    //
}
