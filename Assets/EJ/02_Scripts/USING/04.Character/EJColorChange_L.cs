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
        print("11111 onpointerDOwn ����");
        oo.Invoke(this.gameObject);
        //oo.Invoke(sliderBackgroundImg);
        print("22222 Invoke ����");
    }
    Color color;
    public void ColorChange(/*GameObject go*/)
    {
        print("33333 COlorChagnge ����");

        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        color = GetComponent<Image>().color;

        int i = EJCharacter_L.instance.WhatClicked();

        char_L_Mats[i].color = color;

        Color.RGBToHSV(color, out float h, out float s, out float v);
        Color color2 = Color.HSVToRGB(h, s, v + 40);

        print("color��" + color);
        print("color2��" + color2 + "�Դϴ�");
        print("sliderBackgroundImg��" + sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_angle);


        //print("sliderBackgroundImg��" + sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1);

        //print("i��" + i + "�̴�");
        //sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color1 = color;
        //sliderBackgroundImg.GetComponent<EJ_UIGradient>().m_color2 = color2;

    }


    //valueChange�� �� ���� �ǽð����� �ٲ�� ���� ����ϱ�
}
