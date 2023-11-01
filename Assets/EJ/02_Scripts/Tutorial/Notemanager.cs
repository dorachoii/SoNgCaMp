using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notemanager : MonoBehaviour
{
    public int bpm = 0;
    double currentTime = 0d;    //float���� ������ �۴�.

    [SerializeField] Transform ftNoteAppear = null;     //note�� ������ ��ġ
    [SerializeField] GameObject goNote = null;          //note Prefab

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= 60d / bpm) //1��
        {
            GameObject t_note = Instantiate(goNote, ftNoteAppear.position, Quaternion.identity);
            t_note.transform.SetParent(this.transform);
            currentTime -= 60d / bpm;   
            //currentTime = 0 �ʱ�ȭ���� �ʰ� ���ִ� ����
            //currentTime = 0.51005551~��ŭ�� ������ �սǵǱ� �����̴�.
            //���ڰ� �ٸ��� ������
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Note"))
        {
            Destroy(other.gameObject);
        }
    }
}
