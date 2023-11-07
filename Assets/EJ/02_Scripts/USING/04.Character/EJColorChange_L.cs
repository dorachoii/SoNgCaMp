using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEditor.iOS.Extensions.Common;

public class EJColorChange_L : MonoBehaviour, IPointerDownHandler
{
    public GameObject character_L;
    public GameObject sliderBackgroundImg;

    public UnityEvent<GameObject> oo;
    public void OnPointerDown(PointerEventData eventData)
    {
        print("11111 onpointerDOwn 실행");
        oo.Invoke(this.gameObject);
        //oo.Invoke(sliderBackgroundImg);
        print("22222 Invoke 실행");
    }
    Color color;
    public void ColorChange(/*GameObject go*/)
    {
        print("33333 COlorChagnge 실행");

        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        color = GetComponent<Image>().color;

        int i = EJCharacter_L.instance.WhatClicked();

        char_L_Mats[i].color = color;

        Color.RGBToHSV(color, out float h, out float s, out float v);
        Color color2 = Color.HSVToRGB(h, s, v + 40);

        print("color는" + color);
        print("color2는" + color2 + "입니다");
        print("sliderBackgroundImg는" + sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_angle);


        //print("sliderBackgroundImg는" + sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1);

        //print("i는" + i + "이다");
        //sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1 = color;
        //sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color2 = color2;

    }


    //valueChange할 때 색깔 실시간으로 바뀌는 법을 고안하기
}
