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
    //그냥 필요한 데이터를 담아두자.
    //딕셔너리.
    public Dictionary<string, System.Object> infoList = new Dictionary<string, System.Object>();
    public void Add(string key,System.Object value) {
        infoList.Add(key,value);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
