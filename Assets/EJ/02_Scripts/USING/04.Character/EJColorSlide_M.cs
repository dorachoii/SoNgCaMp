using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EJColorSlide_M : MonoBehaviour
{
    public Slider slider;
    Color color1, color2;

    public GameObject[] character_M;
    public GameObject sliderBackgroundImg;

    Material[] char_M_Mats;

    int charIdx;
    int changeModeIdx;

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
        if (isColored)
        {
            isColored = false;
            WhatColorAreYou();
        }
        char_M_Mats[changeModeIdx].color = Color.Lerp(color1, color2, slider.value);
    }

    public void WhatColorAreYou()
    {

        isColored = false;
        color1 = sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1;
        color2 = sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color2;

        charIdx = EJCharacter_M.instance.WhatCharSelected();
        changeModeIdx = EJCharacter_M.instance.WhatClicked();

        char_M_Mats = character_M[charIdx].GetComponent<MeshRenderer>().materials;

    }
}
