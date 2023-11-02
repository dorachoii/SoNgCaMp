using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EJTouchPad : MonoBehaviour
{
    public GameObject[] scoreTexts;
    public GameObject[] touchpads;
    public Canvas canvas;
    float currentTime = 0;

    float zoneBad = 3;
    float zoneGood = 1.5f;
    float zoneGreat = 0.75f;
    float zoneExcellent = 0.25f;

    public bool isTriggered;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        isTriggered = true;
        //print("touchpad�� triggerEnter�� ����" + other.gameObject);
        //touchpad ������?
    }
    private void OnTriggerStay(Collider other)
    {
        #region �⺻touch
        if (other.gameObject.CompareTag("Note"))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                print("triggerEnter�Ǿ��� space�ٸ� ������");

                Destroy(other.gameObject);

                if (Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) < zoneGood && Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) >= zoneGreat)
                {
                    GameObject good = Instantiate(scoreTexts[0], canvas.transform.position - Vector3.forward, Quaternion.identity);
                    good.transform.SetParent(canvas.transform);

                    Destroy(good, 0.5f);
                }
                else if (Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) < zoneGreat && Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) >= zoneExcellent)
                {
                    GameObject great = Instantiate(scoreTexts[1], canvas.transform.position - Vector3.forward, Quaternion.identity);
                    great.transform.SetParent(canvas.transform);

                    Destroy(great, 0.5f);
                }
                else if (Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) < zoneExcellent && Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) >= 0f)
                {
                    GameObject excellent = Instantiate(scoreTexts[2], canvas.transform.position - Vector3.forward, Quaternion.identity);
                    excellent.transform.SetParent(canvas.transform);

                    Destroy(excellent, 0.5f);
                }
                else if (Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) < zoneBad && Vector3.Distance(other.gameObject.transform.position, touchpads[0].transform.position) >= zoneGood)
                {
                    GameObject bad = Instantiate(scoreTexts[3], canvas.transform.position - Vector3.forward, Quaternion.identity);
                    bad.transform.SetParent(canvas.transform);

                    Destroy(bad, 0.5f);
                }
            }
        }
        #endregion

        #region keepTouch

        //if (other.gameObject.CompareTag("linkNote"))
        //{
        //    float length = touchpads[0].transform.position.y - other.gameObject.transform.position.y;
        //    other.gameObject.transform.localScale -= new Vector3(0, length, 0);
        //    //frame������ ������ �Ǵϱ� �����൵ �Ǵ� �κ��� �� ���� ��
        //}

        #endregion
    }

    private void OnTriggerExit(Collider other)
    {
        isTriggered = false;
        //print("touchpad�� triggerExit�� ����"+ other.gameObject);

        if (other.CompareTag("Note"))
        {
            MeshRenderer[] mesh = other.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < mesh.Length; i++) 
            {
                mesh[i].enabled = false;
            }
        }

        GameObject miss = Instantiate(scoreTexts[4], canvas.transform.position - Vector3.forward, Quaternion.identity);
        miss.transform.SetParent(canvas.transform);

        Destroy(miss, 0.5f);

    }
}
