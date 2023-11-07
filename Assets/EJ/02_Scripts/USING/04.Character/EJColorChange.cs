using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class EJColorChange : MonoBehaviour, IPointerDownHandler
{
    public GameObject character_L;

    //public GameObject[] Color_Btns_Clicked;

    public UnityEvent<GameObject> oo;
    public void OnPointerDown(PointerEventData eventData)
    {      
        oo.Invoke(this.gameObject);        
    }

    public void ColorChange(/*GameObject go*/)
    {
        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        Color color = GetComponent<Image>().color;

        int i = EJCharacter_M.instance.WhatClicked();

        char_L_Mats[i].color = color;

        //print("i는" + i + "이다");
    }

    //valueChange할 때 색깔 실시간으로 바뀌는 법을 고안하기
}
