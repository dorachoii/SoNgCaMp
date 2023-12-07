using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EJ_pause : MonoBehaviour
{
    public GameObject canvasPause;
    public AudioSource audioSource;

    public EJNoteManager noteManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickPuase()
    {
        canvasPause.SetActive(true);
        //audiosource pause
        audioSource.Pause();

        //notemanager currentTime 기록
        Time.timeScale = 0;
    }

    public void ClickSongSelection()
    {
        canvasPause.SetActive(false);
        SceneManager.LoadScene(8);
    }

    //여기 해결 필요
    public void ClickRestart()
    {
        canvasPause.SetActive(false);
        //audiosource restart
        Time.timeScale = 1;
        noteManager.currTime = 0;


        //생성되어 있는 노트들 다 destroy해라!
        for (int i = 0; i < noteManager.gameNoteInstance_Rails.Length; i++) 
        {
            //Destroy(noteManager.gameNoteInstance_Rails[i]);
        
        }

        
        //notemanager currTime = 0
    }

    public void ClickResume()
    {
        canvasPause.SetActive(false);
        //audiosource pause에서 부터 시작
        audioSource.UnPause();
        //notemanager currtime 기록한 시점부터 시작
        Time.timeScale = 1;
    }
}
