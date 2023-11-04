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

        //내거 지우고
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


    //활성화될때 Stack에 화면을 넣는다.
    private void OnEnable()
    {
        StackTest.instance.Test(Canva);
    }
}
