using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerToolBar : MonoBehaviourPun
{

    bool isClick;
    //만약 플레이어를 클릭했다면
    //툴 바를 보여준다.
    [SerializeField]
    GameObject tollBar;
    public void OnTollBar() {
        Debug.Log("Ontoolbar");
        tollBar.SetActive(!tollBar.activeSelf);
        chatBar.SetActive(false);
        animationBar.SetActive(false);
    }

    //툴 바 2개
    [SerializeField]
    GameObject chatBar;
    //하나는 채팅바 

    public void OnChatBar() {
        tollBar.SetActive(false);
        chatBar.SetActive(true);
    }

    //하나는 애니메이션 바
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
                return; // UI를 클릭한 경우 무시
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
