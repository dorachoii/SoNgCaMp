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

        //notemanager currentTime ���
        Time.timeScale = 0;
    }

    public void ClickSongSelection()
    {
        canvasPause.SetActive(false);
        SceneManager.LoadScene(8);
    }

    //���� �ذ� �ʿ�
    public void ClickRestart()
    {
        canvasPause.SetActive(false);
        //audiosource restart
        Time.timeScale = 1;
        noteManager.currTime = 0;


        //�����Ǿ� �ִ� ��Ʈ�� �� destroy�ض�!
        for (int i = 0; i < noteManager.gameNoteInstance_Rails.Length; i++) 
        {
            //Destroy(noteManager.gameNoteInstance_Rails[i]);
        
        }

        
        //notemanager currTime = 0
    }

    public void ClickResume()
    {
        canvasPause.SetActive(false);
        //audiosource pause���� ���� ����
        audioSource.UnPause();
        //notemanager currtime ����� �������� ����
        Time.timeScale = 1;
    }
}
