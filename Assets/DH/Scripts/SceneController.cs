using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //�ε��� ������ �̸�
    public static string loadingBarPath = "Loading";

    private void Start()
    {
        StartCoroutine(LoadSceneAsync(false,2,()=> {
            Debug.Log("�ε��Ϸ�!!!!!");
        }));
    }



    //�񵿱� �� ó��. //��� �۾��� �Ϸ��� �� �� �̵�
    public static IEnumerator LoadSceneAsync(bool isNetwork,int index, System.Action loadAction) {
        //�⺻������ �׳� �־����.
        //�ε� �� ����.
        GameObject go =  (GameObject)Resources.Load(loadingBarPath);
        Instantiate(go);

        loadAction?.Invoke();

        if (!isNetwork)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);


            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }
        else {
            PhotonNetwork.LoadLevel(index);
        
        }

       

        //done.
    }

    public static void  PlayUI() {
        Debug.Log("Loading");
        GameObject go = (GameObject)Resources.Load(loadingBarPath);
        Instantiate(go);

    }

    public static void StartLoadSceneAsync(MonoBehaviour mono, bool isNetwork, int index, System.Action loadAction) {
        mono.StartCoroutine(LoadSceneAsync(isNetwork,index,loadAction));

    }
    
}
