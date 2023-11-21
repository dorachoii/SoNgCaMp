using System.Collections;
using System.Collections.Generic;

using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Networking;


[System.Serializable]
public struct CharacterInfo
{
    public int characterType;
    //0: listener, 1: rabbit, 2: panda, 3: dog

    public string hexString_cloth;
    public string hexString_skin;
    public string hexString_face;
    public string hexString_ribbon;

    //Color albedoColor = ColorUtility.HexToColor(hexString); 변환 후 사용

    public bool isCrownON;
    public bool isGlassON;
    public bool isBagON;
    public bool isCapON;
}


public class EJCharacter_L : MonoBehaviour
{
    //00. Server에 저장할 정보
    CharacterInfo characterInfo = new CharacterInfo();

    //01. character prefabs
    public GameObject character_L;
    //public GameObject[] characters_M;
    //public Transform centerPos;

    //02. UI
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

    //03. singleton
    public static EJCharacter_L instance;

    //04. Animator
    public Animator animator;

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
        if (!items[1].activeSelf)
        {
            items[1].SetActive(true);
            characterInfo.isCapON = true;
        }
        else 
        {
            items[1].SetActive(false);
            characterInfo.isCapON = false;
        }


        if (items[2].activeSelf)
        {
            items[2].SetActive(false);
            characterInfo.isCrownON = false;
        }
    }

    public void Click_Item_Headphone()
    {
        if (!items[2].activeSelf)
        {
            items[2].SetActive(true);
            characterInfo.isCrownON = true;
        }
        else
        {
            items[2].SetActive(false);
            characterInfo.isCrownON = false;
        }

        if (items[1].activeSelf)
        {
            items[1].SetActive(false);
            characterInfo.isCapON = false;
        }
    }

    public void Click_Item_Glasses()
    {
        if (!items[3].activeSelf)
        {
            items[3].SetActive(true);
            characterInfo.isGlassON = true;
        }
        else
        {
            items[3].SetActive(false);
            characterInfo.isGlassON = false;
        }
    }

    public void Click_Item_Bag()
    {
        if (!items[0].activeSelf)
        {
            items[0].SetActive(true);
            characterInfo.isBagON = true;
        }
        else
        {
            items[0].SetActive(false);
            characterInfo.isGlassON = false;
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

    //수정 필요
    public void ColorInfoCheck_L()
    {
        Material[] char_L_Mats = character_L.GetComponent<SkinnedMeshRenderer>().materials;

        characterInfo.hexString_cloth = char_L_Mats[0].color.ToString();        
        characterInfo.hexString_skin = char_L_Mats[2].color.ToString();
        characterInfo.hexString_ribbon = char_L_Mats[1].color.ToString();
        characterInfo.hexString_face = char_L_Mats[3].color.ToString();

        //Color albedoColor = ColorUtility.HexToColor(hexString)
    }

    public void Click_CompleteBtn()
    {
        animator.SetTrigger("Spin");

        ColorInfoCheck_L();
        //characterInfo를 server에 업로드 한다.
        print("캐릭터 정보는 이래요" + characterInfo.hexString_cloth);

        HttpInfo httpInfo = new HttpInfo();

        UserInfo_customizing userInfo_Customizing = new UserInfo_customizing(0, characterInfo.hexString_cloth, characterInfo.hexString_face, characterInfo.hexString_ribbon, characterInfo.hexString_skin, characterInfo.isBagON, characterInfo.isCapON, characterInfo.isCrownON, characterInfo.isGlassON, 3);

        httpInfo.Set(RequestType.POST, "api/v1/users/customize", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_Customizing);
        HttpManager.Get().SendRequest(httpInfo);    
    }

    
}
