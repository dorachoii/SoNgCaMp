using Melanchall.DryWetMidi.Multimedia;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EJConcertCamchange : MonoBehaviour
{

    public GameObject[] cams;


    // Start is called before the first frame update
    void Awake()
    {
        //StartCoroutine(camChange());
    }

    bool iscamChanged = false;
    // Update is called once per frame
    void Update()
    {
        if (!iscamChanged)
        {
            iscamChanged = true;
            StartCoroutine(camChange());
        }
    }

    
    int camIdx = 0;
    int repeat = 2000;
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


            yield return new WaitForSeconds(4f);

        }
    }
}
