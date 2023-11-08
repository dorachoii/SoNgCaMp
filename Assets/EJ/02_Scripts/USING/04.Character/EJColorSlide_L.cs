using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class EJColorSlide_L : MonoBehaviour
{
    public Slider slider;
    Color color1, color2;

    public GameObject character_L;
    public GameObject sliderBackgroundImg;
    
    Material[] char_L_Mats;
    int i;

    // Start is called before the first frame update
    bool isColored = true;
    void Start()
    {
        isColored = true;
        slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //해당 메쉬에 color 넣어주기
        if (isColored)
        {
            isColored = false;
            WhatColorAreYou();
        }
        char_L_Mats[i].color = Color.Lerp(color1, color2, slider.value);
    }

    public void WhatColorAreYou()
    {

        isColored = false;
        color1 = sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1;
        color2 = sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color2;

        char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        i = EJCharacter_L.instance.WhatClicked();

    }
}
