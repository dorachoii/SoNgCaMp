using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleRotate : MonoBehaviour
{

    float speed = 60;
    public GameObject circles;
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform  = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {

        rectTransform.
            localEulerAngles += new Vector3(0, 1, 0) * Time.deltaTime * speed;   
    }
}
