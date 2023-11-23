using Melanchall.DryWetMidi.Multimedia;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EJConcertCamchange : MonoBehaviour
{

    public GameObject[] cams;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(camChange());
    }

    float curTime = 0f;
    float waitTime = 2f;

    // Update is called once per frame
    void Update()
    {
      
    }


    int camIdx = 0;
    int repeat = 30;
    IEnumerator camChange()
    {
        for (int i = 0; i < repeat; i++)
        {
            if (camIdx == cams.Length-1)
            {
                camIdx = 0;
            }

            cams[camIdx].SetActive(false);

            camIdx ++;
            print("camIdx´Â" + camIdx);

            cams[camIdx].SetActive(true);


            yield return new WaitForSeconds(5f);

        }
    }
}
