using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static HttpController;

public class EJLogIn_UI : MonoBehaviour
{
    //UI
    public GameObject a;
    public GameObject b;
    public GameObject c;
    public GameObject d;

    //01. ID,PW 창 + Btn(resister, login)
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

    //ctrl+Z 만들기
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


        //GET? 조회해서 있다면.
        httpInfo.Set(RequestType.POST, "api/v1/authentication/login", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
            //로그인시 TokenManager Set Token. 
            //LoginResponse. 필.

            //ResponseDTO 변환
            ResponseDTO<LoginDTO> dto = JsonUtility.FromJson<ResponseDTO<LoginDTO>>(downHandler.text);
            PlayerManager.Get.Add("LoginInfo", dto.results.loginResponse);
            //토큰 저장
            TokenManager.Token = dto.results.loginResponse.authority[0].accessToken;

            SceneController.PlayUI();
            //포톤연결까지.
            ConnectionManager.Get.onJoinRoom = () =>
            {
                PhotonNetwork.LoadLevel(4);
                //SceneController.LoadSceneAsync(true,4,()=> { Debug.Log("처리할것들."); });
            };
            ConnectionManager.Get.ConnectToPhoton();
        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_Login);
        HttpManager.Get().SendRequest(httpInfo);

        //****if(로그인에 성공했다면){ 다음페이지로 넘어가라}
    }

    public void Resister_m()
    {
        //POST로 올리기.
        HttpInfo httpInfo = new HttpInfo();
        UserInfo_register userInfo_register_m = new UserInfo_register(ID.text, PW.text, NickName_m.text,instrument_m.value, genre_m.value, mood_m.value, true);

        httpInfo.Set(RequestType.POST, "api/v1/users", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
            registerLogin(userInfo_register_m);
        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_register_m);
        HttpManager.Get().SendRequest(httpInfo);
    }

    public void Register_l()
    {
        //POST로 올리기.

        HttpInfo httpInfo = new HttpInfo();
        UserInfo_register userInfo_register_l = new UserInfo_register(ID.text, PW.text, NickName_l.text, 0, 0,mood_l.value, false);

        print(NickName_l.text);
        print(mood_l.value);

        httpInfo.Set(RequestType.POST, "api/v1/users", (DownloadHandler downHandler) =>
        {
            print(downHandler.text);
            //Custom Scene
            //회원가입하면 로그인 하고 커스텀 -> 진행
            registerLogin(userInfo_register_l);
            //

        }, true);

        httpInfo.body = JsonUtility.ToJson(userInfo_register_l) ;
        HttpManager.Get().SendRequest(httpInfo);
    }

    //회원가입 후 바로 로그인
    public void registerLogin(UserInfo_register regInfo)
        
    {
        UserInfo_login loginInfo = new UserInfo_login(regInfo.userEmail,regInfo.userPwd);
        string data = JsonUtility.ToJson(loginInfo);
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/authentication/login")
            .Data(data)
            .Type(ReqType.POST)
            .Success((down) =>
            {
                Debug.Log("로그인 성공 ");
                //데이터 저장
                ResponseDTO<LoginDTO>  dto = JsonUtility.FromJson<ResponseDTO<LoginDTO>>(down.text);


                TokenManager.Token = dto.results.loginResponse.authority[0].accessToken;
                //로컬에 정보 저장
                //PlayerManager.Get.Add("LoginInfo", dto.loginResponse);

                //커스텀 씬 이동

                int sceneIndex = regInfo.musician ? 3 : 13; 
                SceneController.StartLoadSceneAsync(this,false,sceneIndex,null);

            })
            .build();

        StartCoroutine(SendRequest(rq));
;

    }

}
