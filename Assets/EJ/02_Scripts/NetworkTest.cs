using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UnityWebRequestGET());
        StartCoroutine(UnityWebRequestGET_texture());

        StartCoroutine(UnityWebRequestPOST());
    }

    //01. GET: API�� ����Ǿ� �ִ� ������ �޾ƿ��� ��
    IEnumerator UnityWebRequestGET()
    {
        
        string apikey = "�߱޹��� APIŰ�� �ִ´�.";
        //string url = "http://api.neople.co.kr/df/servers?apikey=" + apikey;
        string url = "GET ����� �� API �ּ�" + apikey;

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text);
        }else
        {
            Debug.Log("GET ERROR");
        }
    }

    // GET: �̹��� �ٿ�ε�
    IEnumerator UnityWebRequestGET_texture()
    {
        string url = "GET ����� �� API �ּ�";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        if (www.error == null) 
        {
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        else
        {
            Debug.Log("GET_texture ERROR");
        }
    }

    //02. POST: API�� ������ �����ϴ� ��

    IEnumerator UnityWebRequestPOST()
    {
        string url = "POST ����� ����� ���� �ּҸ� �Է�";
        WWWForm form = new WWWForm();

        string id = "���̵�";
        string pw = "��й�ȣ";

        string nickName = "�ź���";
        int sessionType = 0;
        int genre = 0;
        int mood = 0;
        
        form.AddField("Username", id);
        form.AddField ("Password", pw);

        form.AddField("nickName", nickName);
        form.AddField("mainPosition", sessionType);
        form.AddField("genre", genre);
        form.AddField("mood", mood);
                
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.error == null)
        {
            Debug.Log(www.downloadHandler.text);
        }else
        {
            Debug.Log("POST ERROR");
        }
    }

    // POST �̹���

    IEnumerator UnityWebRequestPOST_multi()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        //�����Ϳ� ���� ����?
        //�̹��� �ּҰ� �;� ��? ���� �ּҰ� ���� �ʰ������� �𸣰ڴ�.
        formData.Add(new MultipartFormDataSection("???"));

        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post("�ּ�", formData);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }else
        {
            Debug.Log("Form upload complete!");
        }
    }

}
