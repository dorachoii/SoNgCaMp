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
    //트랙 악기가 클릭되었을때 진행될 이벤트를 담음
    public UnityEvent<int> OnClickInsEv;

    //트랙이 클릭되었을때 진행될 이벤트를 담음
    public UnityEvent<int> OnClickTrackEv;

    //트랙 버튼은 버튼 두개(악기 ,트랙버튼)
    public EventButton event1;
    public EventButton event2;

    public Image image;

    public int myPage;

    private void Start()
    {
        //버튼을 눌렀을 때 이런 액션을 취하자
        event1.action.AddListener( ()=> { UIManager.instance.currentTrack = mytrack; InstrumentManager.instance.ChangeTrack(image); });
        event2.action.AddListener(()=>  { UIManager.instance.ChangeTrack(mytrack); Debug.Log("??");  });
        //ChanelBtn.text.text = "CH" + myPage;
        ChanelBtn.Count = myPage; 
    }

    //1버튼이 눌렸을 때
    //--> 채널을 설정 
    //2버튼이 눌렸을 때

    

    public void Test()
    {


    }
                   
    

    

}
