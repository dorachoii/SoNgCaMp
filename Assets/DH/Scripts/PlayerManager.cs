using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //개선된 싱글톤
    static PlayerManager instance;
    public static PlayerManager Get
    {
        get
        {
            if (instance != null)
                return instance;
            //없을시
            GameObject conn = new GameObject("PlayerManager");
            DontDestroyOnLoad(conn);
            //인스턴스 생성하기
            instance = conn.AddComponent<PlayerManager>();
            return instance;

        }
    }
    //딕셔너리.
    public Dictionary<string, System.Object> infoList = new Dictionary<string, System.Object>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
