using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EJLogIn_UI : MonoBehaviour
{
    //UI
    public GameObject a;
    public GameObject b;
    public GameObject c;
    public GameObject d;

    //01. ID,PW â + Btn(resister, login)
    public TMP_InputField ID;
    public TMP_InputField PW;

    //02. Who are you (musician / listener)
    //03. Input - musician (nickName, instrument, genre, wanna make)
    public TMP_InputField NickName_m;

    public TMP_Dropdown instrument_m;
    public TMP_Dropdown genre_m;
    public TMP_Dropdown mood_m;

    //04. Input - Listener (nickName, wanna listen)
    public TMP_InputField NickName_l;
    public TMP_Dropdown mood_l;


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Click_register()
    {
        a.SetActive(false);
        b.SetActive(true);
    }

    public void Click_m()
    {
        b.SetActive(false);
        c.SetActive(true);
    }

    public void Click_l()
    {
        b.SetActive(false);
        d.SetActive(true);
    }

    //ctrl+Z �����
    public void Click_Back()
    {
        if (b.activeSelf)
        {
            b.SetActive(false);
            a.SetActive(true);
        }else if (c.activeSelf)
        {
            c.SetActive(false);
            b.SetActive(true);
        }else if (d.activeSelf)
        {
            d.SetActive(false);
            b.SetActive(true);
        }else
        {
            a.SetActive(true);
        }
        
        //Idontknow
    }

    [SerializeField]
    int connectedIndex;
    public void LogInCheck()
    {
        HttpInfo httpInfo = new HttpInfo();
        UserInfo_login userInfo_Login = new UserInfo_login(ID.text, PW.text);


        //GET? ��ȸ�ؼ� �ִٸ�.
        httpInfo.Set(RequestType.POST, "api/v1/authentication/login", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
            //�α��ν� TokenManager Set Token. 
            //LoginResponse. ��.

            //ResponseDTO ��ȯ
            ResponseDTO<LoginDTO> dto = JsonUtility.FromJson<ResponseDTO<LoginDTO>>(downHandler.text);
            PlayerManager.Get.Add("LoginInfo", dto.results.loginResponse);
            //��ū ����
            TokenManager.Token = dto.results.loginResponse.authority[0].accessToken;

            //���濬�����.
            ConnectionManager.Get.onJoinRoom = () =>
            {
                PhotonNetwork.LoadLevel(connectedIndex);
            };
            ConnectionManager.Get.ConnectToPhoton();
        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_Login);
        HttpManager.Get().SendRequest(httpInfo);

        //****if(�α��ο� �����ߴٸ�){ ������������ �Ѿ��}
    }

    public void Resister_m()
    {
        //POST�� �ø���.
        HttpInfo httpInfo = new HttpInfo();
        UserInfo_register userInfo_register_m = new UserInfo_register(ID.text, PW.text, NickName_m.text,instrument_m.value, genre_m.value, mood_m.value, true);

        httpInfo.Set(RequestType.POST, "api/v1/users", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_register_m);
        HttpManager.Get().SendRequest(httpInfo);
    }

    public void Register_l()
    {
        //POST�� �ø���.

        HttpInfo httpInfo = new HttpInfo();
        UserInfo_register userInfo_register_l = new UserInfo_register(ID.text, PW.text, NickName_l.text, 0, 0,mood_l.value, false);

        print(NickName_l.text);
        print(mood_l.value);

        httpInfo.Set(RequestType.POST, "api/v1/users", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_register_l) ;
        HttpManager.Get().SendRequest(httpInfo);
    }

}
