using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackButton : MonoBehaviour
{
    public int myPage;
    public void Onclick() {
        Debug.Log("Track 변경됨");
        UIManager.instance.currentTrack = UIManager.instance.Tracks[myPage]; 
        UIManager.instance.TrackCanvas.gameObject.SetActive(false);
        UIManager.instance.EditerCanvas.gameObject.SetActive(true);
        //렌더링 방식을 변경
        UIManager.instance.noteList = UIManager.instance.currentTrack.Notelist;
        UIManager.instance.Rendering();
    }
}
