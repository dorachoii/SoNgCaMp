using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static HttpController;

public class EJCharacter_M : MonoBehaviour
{
    //00. Server에 저장할 정보
    public
    CharacterInfo characterInfo = new CharacterInfo();

    //01. character prefabs
    //public GameObject character_L;
    public GameObject[] characters_M;

    [System.Serializable]
    public class Array
    {
        public GameObject[] item = new GameObject[4];
   
    }        
    public Array[] character_items = new Array[3];

    public Transform centerPos;

    public Camera stageCam;

    //02. UI
    public GameObject CustomBtns;
    public GameObject[] explainBox;

    bool isExplainBoxON = true;

    public GameObject[] Color_Btns_Clicked;

    //public GameObject Btns_Item_L;
    public GameObject Btns_Item_M;
    public GameObject Btns_ColorChange;

    public GameObject[] items;

    public GameObject CP_Cloth;
    public GameObject CP_Face;
    public GameObject CP_Skin;

    //03. singleton
    public static EJCharacter_M instance;

    //04. Animator
    public Animator[] animator;
    CharacterInfo characterInfo_M;

    public GameObject completeFX;

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
        SelectCharacter();
    }

    public void SelectCharacter()
    {
        //***touch로 바꿀 시
        //UnityEngine.Touch touch = Input.GetTouch(0);
        //Ray ray = Camera.main.ScreenPointToRay(touch.position);

        Ray ray = stageCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Input.GetMouseButtonDown(0))
        {
            print("마우스 왼쪽 버튼을 눌렀다");
            if (Physics.Raycast(ray, out hitInfo, 100f, 1 << LayerMask.NameToLayer("character")))
            {
                print("부딪힌 녀석이 character다");
                print("hitInfo의 이름은" + hitInfo.transform.name);

                string characterName = hitInfo.transform.name;
                characterName = characterName.Replace("Object00", "");
                int characterIndex = int.Parse(characterName) - 1;

                //outline
                if (hitInfo.transform.gameObject.GetComponent<Outline>().enabled)
                {
                    //이동 코루틴
                    StartCoroutine(moveToCenterPoint(hitInfo.transform.parent.gameObject));

                    //나머지 애는 꺼지게
                    for (int i = 0; i < characters_M.Length; i++)
                    {
                        if (i != characterIndex)
                        {
                            characters_M[i].gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    hitInfo.transform.gameObject.GetComponent<Outline>().enabled = true;

                    for (int i = 0; i < characters_M.Length; i++)
                    {
                        if (i != characterIndex)
                        {
                            characters_M[i].gameObject.GetComponentInChildren<Outline>().enabled = false;
                        }
                    }                   
                }
            }
        }        
    }

    IEnumerator moveToCenterPoint(GameObject go)
    {
        while (!(Vector3.Distance(go.transform.position, centerPos.transform.position /*+Vector3.up*0.5f - Vector3.right *1.8f*/) < 0.01f))
        {
            go.transform.position = Vector3.Lerp(go.transform.position, centerPos.transform.position /*+ Vector3.up*0.5f - Vector3.right*1.8f*/, 0.1f);
            yield return null;
        }

        go.GetComponentInChildren<Outline>().enabled = false;
        Show_Btns();
    }

    public void Show_Btns()
    {
        CustomBtns.SetActive(true);

        explainBox[0].SetActive(false);
        explainBox[1].SetActive(true);
    }
    
    public void Click_Item()
    {
        explainBox[1].SetActive(false);
        CustomBtns.SetActive(false);
        Btns_Item_M.SetActive(true);
    }

    public void Click_ColorChange()
    {
        explainBox[1].SetActive(false);
        CustomBtns.SetActive(false);
        Btns_ColorChange.SetActive(true);
    }


    public void Click_Back()
    {
        explainBox[1].SetActive(true);
        CP_Face.SetActive(false);
        CP_Skin.SetActive(false);
        CP_Cloth.SetActive(false);

        print("clickBack을 눌렀다");
        if (Btns_Item_M.activeSelf)
        {
            print("clickBack을 눌렀고 itemBtn이 activeself이다");
            Btns_Item_M.SetActive(false);
        }
        else if (Btns_ColorChange.activeSelf)
        {
            print("clickBack을 눌렀고 colorChangeBtn이 activeself이다");
            Btns_ColorChange.SetActive(false);
        }

        CustomBtns.SetActive(true);
    }


    #region M_Items - testFINISHED!
    //동시 착용 안되는 것들: 모자와 크라운
    public void Click_Item_Cap()
    {
        int charIndex = WhatCharSelected();

        if (!character_items[charIndex].item[0].activeSelf)
        {
            character_items[charIndex].item[0].SetActive(true);
            characterInfo.isCapON = true;
        }
        else
        {
            character_items[charIndex].item[0].SetActive(false);
            characterInfo.isCapON = false;
        }


        if (character_items[charIndex].item[1].activeSelf)
        {
            character_items[charIndex].item[1].SetActive(false);
            characterInfo.isCrownON = false;
        }
    }

    public void Click_Item_Crown()
    {
        int charIndex = WhatCharSelected();

        if (!character_items[charIndex].item[1].activeSelf)
        {
            character_items[charIndex].item[1].SetActive(true);
            characterInfo.isCrownON = true;
        }
        else
        {
            character_items[charIndex].item[1].SetActive(false);
            characterInfo.isCrownON = false;
        }

        if (character_items[charIndex].item[0].activeSelf)
        {
            character_items[charIndex].item[0].SetActive(false);
            characterInfo.isCapON = false;
        }
    }

    public void Click_Item_Glasses()
    {
        int charIndex = WhatCharSelected();

        if (!character_items[charIndex].item[2].activeSelf)
        {
            character_items[charIndex].item[2].SetActive(true);
            characterInfo.isGlassON = true;
        }
        else
        {
            character_items[charIndex].item[2].SetActive(false);
            characterInfo.isGlassON = false;
        }
    }

    public void Click_Item_Bag()
    {
        int charIndex = WhatCharSelected();

        if (!character_items[charIndex].item[3].activeSelf)
        {
            character_items[charIndex].item[3].SetActive(true);
            characterInfo.isBagON = true;
        }
        else
        {
            character_items[charIndex].item[3].SetActive(false);
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

    public int WhatCharSelected()
    {
        if (characters_M[0].activeSelf)
        {
            return 0;
        }
        else if (characters_M[1].activeSelf)
        {
            return 1;
        }else if (characters_M[2].activeSelf)
        {
            return 2;
        }else
        {
            return -1;
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
    public void ColorInfoCheck_M()
    {
        // Material[] char_L_Mats = characters_M[0].GetComponent<SkinnedMeshRenderer>().materials; //선택된 애가 아니라 첫번째 애의 색을 가져옴??
        Material[] char_L_Mats = characters_M[WhatCharSelected()].GetComponent<SkinnedMeshRenderer>().materials;

        characterInfo.hexString_cloth = char_L_Mats[0].color.ToString();
        characterInfo.hexString_skin = char_L_Mats[1].color.ToString();
        characterInfo.hexString_ribbon = char_L_Mats[2].color.ToString(); 
        characterInfo.hexString_face = char_L_Mats[3].color.ToString();
        //Color albedoColor = ColorUtility.HexToColor(hexString)
    }

    public void Click_CompleteBtn()
    {
        explainBox[1].SetActive(false);
        explainBox[2].SetActive(true);
        CustomBtns.SetActive(false);

        GameObject fx = Instantiate(completeFX, centerPos.transform.position, Quaternion.identity);
        


        ColorInfoCheck_M();
        //server에 업로드 한다.

        if (characters_M[0].activeSelf)
        {
            animator[0].SetTrigger("Spin");
            characterInfo.characterType = 1;
        }
        else if (characters_M[1].activeSelf)
        {
            animator[1].SetTrigger("Spin");
            characterInfo.characterType = 2;

        }
        else if (characters_M[2].activeSelf)
        {
            animator[2].SetTrigger("Spin");
            characterInfo.characterType = 3;
        }

        HttpInfo httpInfo = new HttpInfo();


        ResponseDTO<LoginDTO2> dto = new ResponseDTO<LoginDTO2>();
       
        UserInfo_customizing userInfo_Customizing = new UserInfo_customizing(characterInfo.characterType, characterInfo.hexString_cloth, characterInfo.hexString_face, characterInfo.hexString_ribbon, characterInfo.hexString_skin, characterInfo.isBagON, characterInfo.isCapON, characterInfo.isCrownON, characterInfo.isGlassON, dto.results.authority.userNo);
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/users/byToken")
            .Data(TokenManager.Token)
            .Success((down)=> {
                dto = JsonUtility.FromJson<ResponseDTO<LoginDTO2>>(down.text);
                userInfo_Customizing.userNo = dto.results.authority.userNo;

                //커스터마이징 저장
                httpInfo.Set(RequestType.POST, "api/v1/users/customize", (DownloadHandler downHandler) =>
                {
                    //저장하면 로비로 이동
                    print(downHandler.text);

                    //요청하고 한번 더 불러오고

                    //로딩창
                    SceneController.PlayUI();

                    ConnectionManager.Get.onJoinRoom = () =>
                    {
                        //로비로 이동
                        PhotonNetwork.LoadLevel(4);
                    };
                    ConnectionManager.Get.ConnectToPhoton();

                }, true);
                httpInfo.body = JsonUtility.ToJson(userInfo_Customizing);
                Debug.LogError("SSSSSSS : "+ httpInfo.body);
                HttpManager.Get().SendRequest(httpInfo);
            })
            .Failure((down)=> {
                //요청실패시
                //로딩창
                SceneController.PlayUI();

                ConnectionManager.Get.onJoinRoom = () =>
                {
                    //로비로 이동
                    PhotonNetwork.LoadLevel(4);

                };
                ConnectionManager.Get.ConnectToPhoton();
            })
            .build();

        //토큰으로 조회
        StartCoroutine(send(4,rq));
        //지금은 그냥 ... 1로 합시다.

        Destroy(fx, 5);



    }

    IEnumerator send(float t, HttpRequest rq)
    {
        yield return new WaitForSeconds(t);

        yield return SendRequest(rq);
    }
}
