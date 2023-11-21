using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class EJAnimator_CompleteSpin : MonoBehaviour
{
    public Animator[] animator;
    public GameObject[] chars;

    CharacterInfo characterInfo;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CompleteSpin()
    {
        print("spin이 실행되었다");

        if (chars[0].activeSelf)
        {
            animator[0].SetTrigger("Spin");
            characterInfo.characterType = 0;
        }
        else if (chars[1].activeSelf)
        {
            animator[1].SetTrigger("Spin");
            characterInfo.characterType = 1;

        }
        else if (chars[2].activeSelf)
        {
            animator[2].SetTrigger("Spin");
            characterInfo.characterType = 2;

        }
    }
}
