using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //������ �̱���
    static PlayerManager instance;
    public static PlayerManager Get
    {
        get
        {
            if (instance != null)
                return instance;
            //������
            GameObject conn = new GameObject("PlayerManager");
            DontDestroyOnLoad(conn);
            //�ν��Ͻ� �����ϱ�
            instance = conn.AddComponent<PlayerManager>();
            return instance;

        }
    }
    //�׳� �ʿ��� �����͸� ��Ƶ���.
    //��ųʸ�.
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
