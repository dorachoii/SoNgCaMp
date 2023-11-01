using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public static PlaySound instance;

    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = this.GetComponent<AudioSource>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (source.time > 0.25) {
            source.time = 0.1f;
        }
    }

    public void play(AudioClip clip) {       
        source.time = 0.1f;
        source.clip = clip;
         source.Play();

    }
    public void oneplay(AudioClip clip) {
        source.PlayOneShot(clip);
    }
}
