using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{


    private void Start()
    {
        StartCoroutine(LoadSceneAsync(1,()=>{ },null));
    }

    //비동기 씬 처리. //모든 작업을 완료한 뒤 씬 이동
    public static IEnumerator LoadSceneAsync(int index, System.Action loadAction,System.Action completeAction) {
        //기본적으로 그냥 넣어두자.
        //로딩 바 띄우기.

        loadAction?.Invoke();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);


        while (!asyncOperation.isDone) {
            yield return null;
        }

        //done.
        completeAction?.Invoke();
    }

    
}
