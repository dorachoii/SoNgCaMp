using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static SoundManager instance;
    public static SoundManager Get
    {
        get { 
            if (!instance) {
                GameObject root = new GameObject("SoundManager");
                instance = root.AddComponent<SoundManager>(); 

                GameObject sfxObj = new GameObject("SFXChannel");
                GameObject bgmObj = new GameObject("BGMChannel");

                instance.SFXChannel = sfxObj.AddComponent<AudioSource>();
                instance.BGMChannel = bgmObj.AddComponent<AudioSource>();

                sfxObj.transform.parent = root.transform;
                bgmObj.transform.parent = root.transform;

                
            }
            return instance;    
        }

    }

    public AudioSource SFXChannel;
    public AudioSource BGMChannel;
    // Start is called before the first frame update

    public void PlayBGM(AudioClip clip) {
        BGMChannel.clip = clip;
        BGMChannel.Play();
    }
    public void PlaySFX(AudioClip clip) {
        SFXChannel.PlayOneShot(clip);
    }
}
