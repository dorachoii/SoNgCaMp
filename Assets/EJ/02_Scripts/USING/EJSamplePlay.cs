using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EJSamplePlay : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public AudioSource[] audiosource_chords;
    public GameObject[] outline_chords;

    public AudioSource[] audiosource_drums;
    public GameObject[] outline_drums;

    public void ClickSamples_chords(int i)
    {
        if (!audiosource_chords[i].isPlaying && !outline_chords[i].activeSelf)
        {
            audiosource_chords[i].Play();
            outline_chords[i].SetActive(true);
        }
        else
        {
            audiosource_chords[i].Stop();
            outline_chords[i].SetActive(false);
        }

    }

    public void ClickSamples_drums(int i)
    {
        if (!audiosource_drums[i].isPlaying && !outline_drums[i].activeSelf)
        {           
            for (int k = 0; k < audiosource_drums.Length; k++)
            {
                if (i == k)
                {
                    audiosource_drums[i].Play();
                    outline_drums[i].SetActive(true);
                }else
                {
                    audiosource_drums[k].Stop();
                    outline_drums[k].SetActive(true);
                }
            }
        }
        else
        {
            audiosource_drums[i].Stop();
            outline_drums[i].SetActive(false);
        }

    }
}
