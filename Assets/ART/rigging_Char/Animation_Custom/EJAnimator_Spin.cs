using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJAnimator_Spin : MonoBehaviour
{
    public Animator[] animator;
    public GameObject[] chars;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spin()
    {
        print("spin이 실행되었다");
        if (chars[0].activeSelf)
        {
            animator[0].SetTrigger("Spin");
        }else if (chars[1].activeSelf)
        {
            animator[1].SetTrigger("Spin");
        }else if (chars[2].activeSelf)
        {
            animator[2].SetTrigger("Spin");
        }
    }
}
