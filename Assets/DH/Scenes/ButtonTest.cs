using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public GameObject Canva;
    public ButtonTest nextButton;
    public bool start;
    
    public void soigjs()
    {
        //StackTest.instance.Test(transform.parent.gameObject);
        //nextButton.gameObject.SetActive(true);
        //gameObject.SetActive(false); 

        //���� �����
        Canva.gameObject.SetActive(false);
        nextButton.Canva.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Ȱ��ȭ�ɶ� Stack�� ȭ���� �ִ´�.
    private void OnEnable()
    {
        StackTest.instance.Test(Canva);
    }
}
