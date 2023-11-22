using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

using Melanchall.DryWetMidi.Core;


public class EJColorChange_L : MonoBehaviour, IPointerDownHandler
{
    public GameObject character_L;
    public GameObject sliderBackgroundImg;

    public UnityEvent<GameObject> oo;
    public void OnPointerDown(PointerEventData eventData)
    {
        oo.Invoke(this.gameObject);
    }

    Color color;
    public void ColorChange(/*GameObject go*/)
    {
        GetComponent<EJColorSlide_L>().enabled = false;

        Material[] char_L_Mats = character_L.GetComponent<SkinnedMeshRenderer>().materials;
        color = GetComponent<Image>().color;

        int i = EJCharacter_L.instance.WhatClicked();

        char_L_Mats[i].color = color;

        Color.RGBToHSV(color, out float h, out float s, out float v);
        Color color2 = Color.HSVToRGB(h, s+2, v -5);


        sliderBackgroundImg.GetComponent<Image>().enabled = false;
        sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1 = color;
        sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color2 = color2;
        sliderBackgroundImg.GetComponent<Image>().enabled = true;
        GetComponent<EJColorSlide_L>().enabled = true;

    }

}
