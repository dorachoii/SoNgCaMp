using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //로딩바 프리팹 이름
    public static string loadingBarPath = "Loading";

    private void Start()
    {

    }



    //비동기 씬 처리. //모든 작업을 완료한 뒤 씬 이동
    public static IEnumerator LoadSceneAsync(bool isNetwork,int index, System.Action loadAction) {
        //기본적으로 그냥 넣어두자.
        //로딩 바 띄우기.
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


    public void Load(int index) {
        StartCoroutine(LoadSceneAsync(false, index, null));
    }
}
