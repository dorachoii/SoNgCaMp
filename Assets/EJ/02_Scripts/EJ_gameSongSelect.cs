using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EJ_gameSongSelect : MonoBehaviour
{
    //public AudioClip[] songs;
    //public AudioSource railSpeaker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickSong()
    {
        SceneManager.LoadScene(9);
    }

}
