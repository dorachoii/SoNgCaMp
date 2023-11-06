using System.Collections;
using System.Collections.Generic;

using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EJCharacter : MonoBehaviour
{
    public GameObject character_L;

    public GameObject CustomBtns;
    public GameObject[] explainBox;

    bool isExplainBoxON = true;

    public GameObject[] Color_Btns_Clicked;

    public GameObject Btns_Item_L;
    public GameObject Btns_Item_M;
    public GameObject Btns_ColorChange;

    public GameObject[] items;

    public GameObject CP_Cloth;
    public GameObject CP_Face;
    public GameObject CP_Skin;

    public static EJCharacter instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isExplainBoxON)
        {
            if (Btns_Item_L.activeSelf || Btns_ColorChange.activeSelf)
            {
                for (int i = 0; i < explainBox.Length; i++)
                {
                    explainBox[i].SetActive(false);
                }
                print("explainBoxON이 몇번 호출될까요~~");
                isExplainBoxON = false;
            }
        }
    }

    //musician중 누르면 해당 위치로 Spawn되게

    public void Click_Item()
    {
        CustomBtns.SetActive(false);
        Btns_Item_L.SetActive(true);
    }

    public void Click_ColorChange()
    {
        CustomBtns.SetActive(false);
        Btns_ColorChange.SetActive(true);
    }


    public void Click_Back()
    {
        CP_Face.SetActive(false);
        CP_Skin.SetActive(false);
        CP_Cloth.SetActive(false);

        print("clickBack을 눌렀다");
        if (Btns_Item_L.activeSelf)
        {
            print("clickBack을 눌렀고 itemBtn이 activeself이다");
            Btns_Item_L.SetActive(false);
        }
        else if (Btns_ColorChange.activeSelf)
        {
            print("clickBack을 눌렀고 colorChangeBtn이 activeself이다");
            Btns_ColorChange.SetActive(false);
        }
        
        CustomBtns.SetActive(true);


    }


    #region L_Items
    //동시 착용 안되는 것들: 모자와 헤드폰
    public void Click_Item_Cap()
    {
        if (!items[0].activeSelf)
        {
            items[0].SetActive(true);
        }
        else 
        {
            items[0].SetActive(false);
        }


        if (items[1].activeSelf)
        {
            items[1].SetActive(false);
        }
    }

    public void Click_Item_Headphone()
    {
        if (!items[1].activeSelf)
        {
            items[1].SetActive(true);
        }
        else
        {
            items[1].SetActive(false);
        }

        if (items[0].activeSelf)
        {
            items[0].SetActive(false);
        }
    }

    public void Click_Item_Glasses()
    {
        if (!items[2].activeSelf)
        {
            items[2].SetActive(true);
        }
        else
        {
            items[2].SetActive(false);
        }
    }

    public void Click_Item_Bag()
    {
        if (!items[3].activeSelf)
        {
            items[3].SetActive(true);
        }
        else
        {
            items[3].SetActive(false);
        }
    }
    #endregion

    #region L_Colors  


    public void Click_Color_face()
    {
        if (!Color_Btns_Clicked[0].activeSelf)
        {
            Color_Btns_Clicked[0].SetActive(true);
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[3].SetActive(false);

            CP_Face.SetActive(true);
            CP_Skin.SetActive(false);
            CP_Cloth.SetActive(false);
        }
        else
        {
            Color_Btns_Clicked[0].SetActive(false);
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[3].SetActive(false);

            CP_Face.SetActive(false);
            CP_Skin.SetActive(false);
            CP_Cloth.SetActive(false);
        }


        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        //char_L_Mats[3].color = Color.red;
        
    }
    public void Click_Color_skin()
    {
        if (!Color_Btns_Clicked[1].activeSelf)
        {
            Color_Btns_Clicked[1].SetActive(true);
            Color_Btns_Clicked[0].SetActive(false);
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[3].SetActive(false);

            

            CP_Skin.SetActive(true);
            CP_Face.SetActive(false);
            CP_Cloth.SetActive(false);
        }
        else
        {
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[0].SetActive(false);
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[3].SetActive(false);

            CP_Skin.SetActive(false);
            CP_Face.SetActive(false);
            CP_Cloth.SetActive(false);
        }

        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        //char_L_Mats[1].color = Color.red;
    }
    public void Click_Color_ribbon()
    {
        if (!Color_Btns_Clicked[2].activeSelf)
        {
            Color_Btns_Clicked[2].SetActive(true);
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[0].SetActive(false);
            Color_Btns_Clicked[3].SetActive(false);

            CP_Cloth.SetActive(true);
            CP_Face.SetActive(false);
            CP_Skin.SetActive(false);
        }
        else
        {
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[0].SetActive(false);
            Color_Btns_Clicked[3].SetActive(false);

            CP_Cloth.SetActive(false);
            CP_Face.SetActive(false);
            CP_Skin.SetActive(false);
        }

        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        //char_L_Mats[2].color = Color.red;
    }

    public void Click_Color_cloth()
    {
        if (!Color_Btns_Clicked[3].activeSelf)
        {
            Color_Btns_Clicked[3].SetActive(true);
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[0].SetActive(false);

            CP_Cloth.SetActive(true);
            CP_Face.SetActive(false);
            CP_Skin.SetActive(false);
        }
        else
        {
            Color_Btns_Clicked[3].SetActive(false);
            Color_Btns_Clicked[1].SetActive(false);
            Color_Btns_Clicked[2].SetActive(false);
            Color_Btns_Clicked[0].SetActive(false);

            CP_Cloth.SetActive(false);
            CP_Face.SetActive(false);
            CP_Skin.SetActive(false);
        }

        Material[] char_L_Mats = character_L.GetComponent<MeshRenderer>().materials;
        //char_L_Mats[0].color = Color.red;
    }

    public int WhatClicked()
    {

        if (Color_Btns_Clicked[0].activeSelf)
        {
            //face
            return 3;
        }
        else if (Color_Btns_Clicked[1].activeSelf)
        {
            //skin
            return 1;
        }
        else if (Color_Btns_Clicked[2].activeSelf)
        {
            //ribbon
            return 2; ;
        }
        else if (Color_Btns_Clicked[3].activeSelf)
        {
            //cloth
            return 0;
        }
        else
        {
            return -1;
        }
    }

    #endregion

}
