using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static HttpController;

public class TokenCheck : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Checking();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //��ū üŷ
    public void Checking() {
        string token = TokenManager.Token;
        Debug.Log(token);
        //��û�ϱ�.
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
        //��ū ����.. ���� ����...
        ResponseDTO<LoginResponseDTO> responce = JsonUtility.FromJson<ResponseDTO<LoginResponseDTO>>(download.text);
        SceneManager.LoadScene(accesIndex);
    }

    [SerializeField]
    int backIndex;
    public void GoBack(DownloadHandler download) {
        Debug.Log("��ū ����..");
        //�α��� �������� �̵�.
        SceneManager.LoadScene(backIndex);
    }
}
