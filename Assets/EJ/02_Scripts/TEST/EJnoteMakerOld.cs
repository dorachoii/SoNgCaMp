using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EJnoteMakerOld : MonoBehaviour
{
    public Transform[] noteFactories;
    public GameObject[] notes;

    public int bpm = 72;
    double currentTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        // bpm(beats per minute): �д� �� ��Ʈ�� ������ ���ֵǴ°�.
        // 60 / bpm : �� ��Ʈ �� �ɸ��� �ð�?

        if (currentTime >= 3)
        {
            GameObject note = Instantiate(notes[0], noteFactories[0].position + Vector3.forward * -0.5f, Quaternion.identity);

            note.transform.forward = notes[0].transform.forward;    
            note.transform.SetParent(noteFactories[0].transform);

            currentTime -= 3;
        }
    }


    void LongNote()
    {
        //01. �� ���̸� ���η������� �մ´�.
        //02. rectangle�� ���� ���̿� ���� Length�� �����Ѵ�.
    }
}
