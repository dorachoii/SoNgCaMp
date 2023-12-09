using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerToolBar : MonoBehaviourPun
{

    bool isClick;
    //���� �÷��̾ Ŭ���ߴٸ�
    //�� �ٸ� �����ش�.
    [SerializeField]
    GameObject tollBar;
    public void OnTollBar() {
        Debug.Log("Ontoolbar");
        tollBar.SetActive(!tollBar.activeSelf);
        chatBar.SetActive(false);
        animationBar.SetActive(false);
    }

    //�� �� 2��
    [SerializeField]
    GameObject chatBar;
    //�ϳ��� ä�ù� 

    public void OnChatBar() {
        tollBar.SetActive(false);
        chatBar.SetActive(true);
    }

    //�ϳ��� �ִϸ��̼� ��
    [SerializeField]
    GameObject animationBar;

    public void OnAnimationBar() {
        tollBar.SetActive(false);
        animationBar.SetActive(true);
    }

    [SerializeField] UnityEvent ev;



    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetMouseButtonUp(0)) {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI�� Ŭ���� ��� ����
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, 1 << LayerMask.NameToLayer("Player")) && hitInfo.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                if (isClick) {
                    ev?.Invoke();
                    isClick = false;
                    return;
                }
                    
                isClick = true;
                
            }
            else {
                isClick = false;
            }
        }
    }

}
