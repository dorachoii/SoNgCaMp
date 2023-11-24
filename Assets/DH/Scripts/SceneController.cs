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

    //�񵿱� �� ó��. //��� �۾��� �Ϸ��� �� �� �̵�
    public static IEnumerator LoadSceneAsync(int index, System.Action loadAction,System.Action completeAction) {
        //�⺻������ �׳� �־����.
        //�ε� �� ����.

        loadAction?.Invoke();

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);


        while (!asyncOperation.isDone) {
            yield return null;
        }

        //done.
        completeAction?.Invoke();
    }

    
}
