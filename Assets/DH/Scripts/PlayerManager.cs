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
    //��ųʸ�.
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
