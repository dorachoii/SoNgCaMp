using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TrackButton : MonoBehaviour
{
    //Ʈ�� �ǱⰡ Ŭ���Ǿ����� ����� �̺�Ʈ�� ����
    public UnityEvent<int> OnClickInsEv;

    //Ʈ���� Ŭ���Ǿ����� ����� �̺�Ʈ�� ����
    public UnityEvent<int> OnClickTrackEv;

    //Ʈ�� ��ư�� ��ư �ΰ�(�Ǳ� ,Ʈ����ư)
    public EventButton event1;
    public EventButton event2;

    public int myPage;

    private void Awake()
    {
        //OnClickTrackEv =   
    }
    private void Start()
    {
        //��ư�� ������ �� �̷� �׼��� ������
        event1.action.AddListener( ()=> { OnClickInsEv.Invoke(myPage); });
        event2.action.AddListener(()=>  { OnClickTrackEv.Invoke(myPage); ; });
    }
}
