using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HttpController;

public class PhotonPlayerManager : MonoBehaviour
{
    [SerializeField]
    string playerObjectPath;

    [SerializeField]
    Vector3 spawnPostion;

    public enum CharType { 
        Ch_01,
        Ch_02,
        Ch_03
    }
    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            ConnectionManager.Get.ConnectToPhoton();
            ConnectionManager.Get.onJoinRoom = () =>
            {
                TokenLogin();
            };
        }
        else {
            TokenLogin();
        }

        //LoginResponseDTO dto = (LoginResponseDTO)PlayerManager.Get.GetValue("LoginInfo");

        //Ŭ���̾�Ʈ�� Infod������ �ٸ��� ����
        //Ÿ�Կ� ���� �ٸ��� ����
    }
    // Start is called before the first frame update


    public void TokenLogin() {
        //��ū���� ��û�ϰ�, ��û ������ �ϴ°� �°���.
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/users/byToken")
            .Data(TokenManager.Token)
            .Success((down) => {
                //�ڱ� ĳ���� �����ϱ�..
                ResponseDTO<LoginDTO2> responce = JsonUtility.FromJson<ResponseDTO<LoginDTO2>>(down.text);
                Vector3 pos = CloudRayCast();

                GameObject character = PhotonNetwork.Instantiate("Ch_0" + (responce.results.authority.characterType), pos != Vector3.one ? pos : spawnPostion, Quaternion.identity); //�̰� �����ʿ�
                Player player = character.GetComponent<Player>();
                player.Login = responce.results.authority;

            })
            .Failure((down) => {
                SceneController.StartLoadSceneAsync(this, false, 2, null);
            })
            .build();
        StartCoroutine(SendRequest(rq));
    }


    //����Ʈ �� �ϳ��� ��� �� ����Ʈ�� �Ʒ��������� Ray�� ���� �������� �����ϱ�.
    [SerializeField]
    Transform[] points;
    public Vector3 CloudRayCast() {
        int rand = UnityEngine.Random.Range(0,points.Length);
        Transform target = points[rand];
        Ray ray = new Ray(target.position ,target.up * -1); //�� ��ġ���� �Ʒ� ��������
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            //�¾Ҵ�?
            return hitInfo.point;
        }
        return Vector3.one;
/*        else
        {
            //���������� ����...��������
            return CloudRayCast();
        }*/


    }
}
