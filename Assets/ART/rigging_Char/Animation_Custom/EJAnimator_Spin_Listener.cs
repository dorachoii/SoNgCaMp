using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJAnimator_Spin_Listener : MonoBehaviour
{
    public Animator animator;
    public GameObject character;


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
        if (character.activeSelf)
        {
            animator.SetTrigger("Spin");
        }
    }
}
