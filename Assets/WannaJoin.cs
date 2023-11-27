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

    //Create면 씬 이동
    public void create() {
        SceneController.StartLoadSceneAsync(this,false,6,null);
        //미디 삭제
        string path = Application.persistentDataPath + "/files/compose.mid";
        if (File.Exists(path)) {
            File.Delete(path);
        }

    }
    //Join면 그냥 창 닫기

    public GameObject select;
    public void join() {
        select.gameObject.SetActive(false);
    
    }
}
