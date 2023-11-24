using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJAnimatorLoop : MonoBehaviour
{
    public Animator vocalAnimator;



    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        StartCoroutine(loopingAnimation());
        
    }

    float curTime;
    IEnumerator loopingAnimation()
    {
        
        while (curTime <300f)
        {
            vocalAnimator.SetTrigger("Sing");
            yield return new WaitForSeconds(2f);
            vocalAnimator.SetTrigger("BellyDancing");
            yield return new WaitForSeconds(3f);
        }
    }
}
