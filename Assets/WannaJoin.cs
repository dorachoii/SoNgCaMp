using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WannaJoin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Create�� �� �̵�
    public void create() {
        SceneController.StartLoadSceneAsync(this,false,6,null);
        //�̵� ����
        string path = Application.persistentDataPath + "/files/compose.mid";
        if (File.Exists(path)) {
            File.Delete(path);
        }

    }
    //Join�� �׳� â �ݱ�

    public GameObject select;
    public void join() {
        select.gameObject.SetActive(false);
    
    }
}
