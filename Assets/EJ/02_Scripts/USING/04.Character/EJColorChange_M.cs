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

    public UnityEvent<GameObject> oo;

    public void OnPointerDown(PointerEventData eventData)
    {
        oo.Invoke(this.gameObject);
    }

    public void ColorChange()
    {
        int charIdx = EJCharacter_M.instance.WhatCharSelected();
        int changeModeIdx = EJCharacter_M.instance.WhatClicked();
        
        Material[] char_M_Mats = character_M[charIdx].GetComponent<MeshRenderer>().materials;
        Color color = GetComponent<Image>().color;

        char_M_Mats[changeModeIdx].color = color;
    }
}
