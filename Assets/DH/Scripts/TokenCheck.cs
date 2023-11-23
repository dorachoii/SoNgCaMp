using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static HttpController;

public class TokenCheck : MonoBehaviour
{
    [SerializeField]
    bool isTokenCheck;
    private void Awake()
    {
        if(isTokenCheck)
            Checking();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //토큰 체킹
    public void Checking() {
        string token = TokenManager.Token;
        Debug.Log(token);
        //요청하기.
        HttpRequest rq = new HttpBuilder().Uri("/api/v1/users/byToken")
            .Data(token)
            .Type(ReqType.GET)
            .Success((download)=> {
                Access(download);
            })
            .Failure((download)=> {
                GoBack(download);
            })
            .build();
        Debug.Log(rq.host + rq.uri + rq.data);

        StartCoroutine(SendRequest(rq));
    }

    [SerializeField]
    int accesIndex;
    public void Access(DownloadHandler download) {

        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if (accesIndex > SceneManager.sceneCountInBuildSettings)
            return;
        
        //토큰 있음.. 입장 가능...
        ResponseDTO<LoginDTO2> responce = JsonUtility.FromJson<ResponseDTO<LoginDTO2>>(download.text);
        Debug.Log(download.text);

        
        PlayerManager.Get.Add("LoginInfo", responce.results.authority);
        //포톤연결까지.
        ConnectionManager.Get.onJoinRoom = () =>
        {

            PhotonNetwork.LoadLevel(accesIndex);
        };
        ConnectionManager.Get.ConnectToPhoton();
    }

    [SerializeField]
    int backIndex;
    public void GoBack(DownloadHandler download) {
        if (backIndex > SceneManager.sceneCountInBuildSettings)
            return;

        Debug.Log("토큰 없음..");
        //로그인 페이지로 이동.
        SceneManager.LoadScene(backIndex);
    }


}
