using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJ_concertMusic : MonoBehaviour
{
    public GameObject songSelection;
    public AudioClip[] songs;
    public AudioSource audiosource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickSongBtn()
    {
        if (!songSelection.activeSelf)
        {
            songSelection.SetActive(true);
        }else
        {
            songSelection.SetActive(false);
        }
    }

    public void ClickSong(int i)
    {
        if (audiosource.isPlaying)
        {
            audiosource.Stop();
            audiosource.PlayOneShot(songs[i]);
        }
    }
}
