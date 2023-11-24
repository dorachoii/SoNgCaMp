using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPun
{
    public LoginResponseDTO Login;
    //cloth,skin,ribbon,eye
    public SkinnedMeshRenderer renderer;
    public Material mat;


    public GameObject[] accessoriList;
    public enum Accessories { 
        Crown,
        Glasses,
        Cap,
        Bag
    }

    //���� ������ �Ǿ���..
    private void Start()
    {
        if (!photonView.IsMine)
            return;
        //�� ����� �ʹ� �����. �ǵ����̸� ��������
        //LoginResponseDTO dto = (LoginResponseDTO)PlayerManager.Get.GetValue("LoginInfo");
        photonView.RPC(nameof(ss),RpcTarget.AllBuffered,JsonUtility.ToJson(Login));
    }

    [PunRPC]
    public void ss(string dto) {
            Debug.Log(dto);
            //Ŀ���� ���� ������, Ŀ���� ������ ���� ��������
            Login = JsonUtility.FromJson<LoginResponseDTO>(dto);
            //dto �־ �����ϴ� �Լ� ����.


            string hexString = Login.hexStringCloth + ":" + Login.hexStringSkin + ":" + Login.hexStringRibbon + ":" + Login.hexStringFace;
            string[] colorString = hexString.Split(":"); 
            for(int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i] = new Material(mat);
                renderer.materials[i].color = hexstringToColor(colorString[i]); 
            }

            List<bool> checkList = new List<bool>();
            checkList.Add(Login.isCrownOn);
            checkList.Add(Login.isGlassOn);
            checkList.Add(Login.isCapOn);
            checkList.Add(Login.isBagOn);
            for (int i = 0; i < accessoriList.Length; i++) {
                accessoriList[i].SetActive(checkList[i]);
            }



            //��������();
       
        
    }


    public Color hexstringToColor(string rgbaString) {

        string[] rgbaValues = rgbaString.Replace("RGBA(", "").Replace(")", "").Split(',');

        // ����� ������ �Ǽ��� ��ȯ
        float r = float.Parse(rgbaValues[0]);
        float g = float.Parse(rgbaValues[1]);
        float b = float.Parse(rgbaValues[2]);
        float a = float.Parse(rgbaValues[3]);
        return new Color(r,g,b,a);
    }

    public void SetAccessories(Accessories accessori,LoginResponseDTO dto) {
        switch (accessori)
        {
            case Accessories.Crown:
                
                break;
            case Accessories.Glasses:

                break;
            case Accessories.Cap:

                break;
            case Accessories.Bag:
                
                break;
        }


    }
}
