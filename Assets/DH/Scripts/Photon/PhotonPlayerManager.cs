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

        //클라이언트의 Infod에따라서 다르게 생성
        //타입에 따라 다르게 생성
    }
    // Start is called before the first frame update


    public void TokenLogin() {
        //토큰으로 요청하고, 요청 성공시 하는게 맞겠지.
        HttpRequest rq = new HttpBuilder()
            .Uri("/api/v1/users/byToken")
            .Data(TokenManager.Token)
            .Success((down) => {
                //자기 캐릭터 생성하기..
                ResponseDTO<LoginDTO2> responce = JsonUtility.FromJson<ResponseDTO<LoginDTO2>>(down.text);
                Vector3 pos = CloudRayCast();

                GameObject character = PhotonNetwork.Instantiate("Ch_0" + (responce.results.authority.characterType), pos != Vector3.one ? pos : spawnPostion, Quaternion.identity); //이거 수정필요
                Player player = character.GetComponent<Player>();
                player.Login = responce.results.authority;

            })
            .Failure((down) => {
                SceneController.StartLoadSceneAsync(this, false, 2, null);
            })
            .build();
        StartCoroutine(SendRequest(rq));
    }


    //포인트 중 하나를 골라서 그 포인트의 아랫방향으로 Ray를 쏴서 맞은곳에 생성하기.
    [SerializeField]
    Transform[] points;
    public Vector3 CloudRayCast() {
        int rand = UnityEngine.Random.Range(0,points.Length);
        Transform target = points[rand];
        Ray ray = new Ray(target.position ,target.up * -1); //그 위치에서 아래 방향으로
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            //맞았니?
            return hitInfo.point;
        }
        return Vector3.one;
/*        else
        {
            //맞을때까지 진행...하지말자
            return CloudRayCast();
        }*/


    }
}
