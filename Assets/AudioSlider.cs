using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{

    
    public AudioSource Melody;
    public AudioSource Drum;
    public Slider slider;

    public void Value() {
        Melody.volume = slider.value;
        Drum.volume = slider.value;
    }


}
