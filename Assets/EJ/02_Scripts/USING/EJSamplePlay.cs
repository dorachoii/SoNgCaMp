using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EJSamplePlay : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject trackCanvas;
    public GameObject samplesCanvas;
    public GameObject chordCanvas;
    public GameObject drumCanvas;
    
    public int[] whatSampleSelected = new int[2];

    // Start is called before the first frame update
    public void ClickSampleBtn()
    {
        startCanvas.SetActive(false);
        samplesCanvas.SetActive(true);
    }
    public void ClickZeroBtn()
    {
        startCanvas.SetActive(false);
        samplesCanvas.SetActive(false);
        trackCanvas.SetActive(true);
    }
    

    public AudioSource[] audiosource_chords;
    public GameObject[] outline_chords;

    public AudioSource[] audiosource_drums;
    public GameObject[] outline_drums;

    public void ClickSamples_chords(int i)
    {
        if (!audiosource_chords[i].isPlaying && !outline_chords[i].activeSelf)
        {
            for (int k = 0; k < audiosource_chords.Length; k++)
            {
                if (i == k)
                {
                    audiosource_chords[i].Play();
                    print(audiosource_chords[i].name + "playÁß");
                    outline_chords[i].SetActive(true);
                    whatSampleSelected[0] = i;
                }else
                {
                    audiosource_chords[i].Stop();
                    outline_chords[k].SetActive(false);
                }
            }
        }
        else
        {
            audiosource_chords[i].Stop();
            //outline_chords[i].SetActive(false);

            chordCanvas.SetActive(false);
            drumCanvas.SetActive(true);
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
                    whatSampleSelected[1] = i;
                }else
                {
                    audiosource_drums[k].Stop();
                    outline_drums[k].SetActive(false);
                }
            }
        }
        else
        {
            audiosource_drums[i].Stop();
            //outline_drums[i].SetActive(false);
            samplesCanvas.SetActive(false);
            trackCanvas.SetActive(true);
        }

    }
}
