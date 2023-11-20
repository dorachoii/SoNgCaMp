using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class EJColorChange_M : MonoBehaviour, IPointerDownHandler
{
    public GameObject[] character_M;
    public GameObject sliderBackgroundImg;

    

    public UnityEvent<GameObject> oo;

    public void OnPointerDown(PointerEventData eventData)
    {
        oo.Invoke(this.gameObject);
    }

    public void ColorChange()
    {
        

        int charIdx = EJCharacter_M.instance.WhatCharSelected();
        int changeModeIdx = EJCharacter_M.instance.WhatClicked();
        
        Material[] char_M_Mats = character_M[charIdx].GetComponent<SkinnedMeshRenderer>().materials;
        Color color = GetComponent<Image>().color;

        char_M_Mats[changeModeIdx].color = color;

        Color.RGBToHSV(color, out float h, out float s, out float v);
        Color color2 = Color.HSVToRGB(h, s + 2, v - 5);

        sliderBackgroundImg.GetComponent<Image>().enabled = false;

        sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1 = color;
        sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color2 = color2;

        sliderBackgroundImg.GetComponent<Image>().enabled = true;

        GetComponent<EJColorSlide_M>().enabled = true;
    }
}
