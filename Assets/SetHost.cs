using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HttpController;

public class SetHost : MonoBehaviour
{
    public ChatService service;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public InputField field;
    public string defaultUri = "/swagger-ui/index.html";
    //요청
    public void RequestButton()
    {
        string host = field.text;
        if (host == "" || host == null)
            return;

        host = "http://" + host;
        HttpRequest rq = new HttpBuilder()
            .Host(host)
            .Uri(defaultUri)
            .Success((down) => {
                service.CreateChat("요청 성공 : " + down.text);


                PlayerPrefs.SetString("HOST", host);
                //Host 변경
                //HttpController.default_host = host;
                //HttpInfo.defaultHost = host + "/";
                service.CreateChat(HttpController.default_host + "로 변경이 되었습니다.");
            })
            .Failure((down) => {
                service.CreateChat("요청 실패 : " + down.error);
            })
            .build();

        StartCoroutine(SendRequest(rq));
    }

    public void JoinBtn()
    {
        SceneController.StartLoadSceneAsync(this, false, 0, null);

    }
}
