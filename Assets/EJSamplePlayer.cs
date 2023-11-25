using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJSamplePlayer : MonoBehaviour
{
    public AudioSource[] audiosources;

    public AudioClip[] audioClips_chord;
    public AudioClip[] audioClips_drum;

    public EJSampleBtnPlay ejSampleBtnPlay;

    int chordIdx;
    int drumIdx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickPlay_samples()
    {
        checkSelectedSample();

        if (!audiosources[0].isPlaying && !audiosources[1].isPlaying)
        {
            audiosources[0].PlayOneShot(audioClips_chord[chordIdx]);
            audiosources[1].PlayOneShot(audioClips_drum[drumIdx]);
        }else
        {
            audiosources[0].Stop();
            audiosources[1].Stop();
        }

    }

    public void checkSelectedSample()
    {
        chordIdx = ejSampleBtnPlay.whatSampleSelected[0] + 1;
        print("chordIdx´Â" + chordIdx);
        drumIdx = ejSampleBtnPlay.whatSampleSelected[1] + 1;
        print("drumIdx´Â" + drumIdx);
    }
}
