using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField]
    float time;

    //Alpha...
    [SerializeField]Image img;

    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {

        //�����ֱ⸦ ��ġ�ٰ�.
        yield return new WaitForSeconds(time);

        yield return FadeOut(1);

        //destroy;
        Destroy(gameObject);
    
    }
    IEnumerator FadeOut(float speed) {
        Color target = new Color(img.color.r,img.color.g,img.color.b,0);

        for (float a = img.color.a; a > 0; a -= (Time.deltaTime * speed)) {
            yield return null;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            Debug.Log("����Ǵ�?");
        }
        Debug.Log("�ى��");
        DestroyImmediate(gameObject);
    }
}
