using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackButton : MonoBehaviour
{
    public Track mytrack;
    public ChanelN ChanelBtn;
    //Ʈ�� �ǱⰡ Ŭ���Ǿ����� ����� �̺�Ʈ�� ����
    public UnityEvent<int> OnClickInsEv;

    //Ʈ���� Ŭ���Ǿ����� ����� �̺�Ʈ�� ����
    public UnityEvent<int> OnClickTrackEv;

    //Ʈ�� ��ư�� ��ư �ΰ�(�Ǳ� ,Ʈ����ư)
    public EventButton event1;
    public EventButton event2;

    public Image image;

    public int myPage;

    private void Start()
    {
        //��ư�� ������ �� �̷� �׼��� ������
        event1.action.AddListener( ()=> { UIManager.instance.currentTrack = mytrack; InstrumentManager.instance.ChangeTrack(image); });
        event2.action.AddListener(()=>  { UIManager.instance.ChangeTrack(mytrack); Debug.Log("??");  });
        //ChanelBtn.text.text = "CH" + myPage;
        ChanelBtn.Count = myPage; 
    }

    //1��ư�� ������ ��
    //--> ä���� ���� 
    //2��ư�� ������ ��

    

    public void Test()
    {


    }
                   
    

    

}
