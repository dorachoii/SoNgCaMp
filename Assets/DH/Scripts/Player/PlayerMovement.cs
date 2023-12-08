using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviourPun
{
    [SerializeField]
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        //터치를 받고 

        //그 위치로 이동하자.
        //Move();
        TouchPosition();
        TouchMoving();
    }

    [SerializeField]
    float speed = 10;
    [SerializeField]
    float rotspeed = 30;
    public void Move() {
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //이동은 카메라의 방향을 참고하여서 이동하기
        Vector3 dir = new Vector3(h * Camera.main.transform.right.x, 0, v * Camera.main.transform.forward.z).normalized;
        transform.position += dir * speed *  Time.deltaTime;

        anim.SetFloat("Speed",dir.sqrMagnitude); //어차피 크기는 

        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up),Time.deltaTime * rotspeed);
    }
    Vector3 target;

    public void TouchPosition() {
        if (Input.GetMouseButtonDown(0)) {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI를 클릭한 경우 무시
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo)) {
                target = hitInfo.point;
            }
        }

    }

    public void TouchMoving() {
        if (Vector3.Distance(transform.position, target) > 0.1)
        {
            Vector3 dir = (target - transform.position).normalized;
            transform.position += dir * speed * Time.deltaTime;
            photonView.RPC(nameof(PlayAnim), RpcTarget.All, "Speed", 1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), Time.deltaTime * rotspeed);
        }
        else {
            //anim.SetFloat("Speed", 0); //어차피 크기는 

            photonView.RPC(nameof(PlayAnim),RpcTarget.All,"Speed",0f);
        }
    }

    [PunRPC]
    public void PlayAnim(string name,float param) {
        anim.SetFloat(name,param);
    }
}
