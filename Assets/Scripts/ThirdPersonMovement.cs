using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviourPunCallbacks
{

    public CharacterController controller;

    public float ResearcherWalk; // 연구원 걷기 속도
    public float ResearcherRun; // 연구원 달리기
    public float AlienWalk; // 외계인 걷기 속도
    public float turnSmoothTime; // 회전 시간
    public float turnSmoothVelocity; // 회전 속도
    public float gravity; // 중력 가속도

    private Vector3 velocity; // 연산 좌표

    public bool controllable; // 이동 가능 여부
    public bool alien; // 에일리언 여부

    void Update()
    {
        if(PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if( velocity.y < 0){
            velocity.y = -2f;
        }

        // 캐릭터 이동
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool hasHorizontalInput = !Mathf.Approximately(horizontal,0f);
        bool hasVeritcalInput = !Mathf.Approximately(vertical,0f);

        bool isWalk = hasHorizontalInput || hasVeritcalInput;
        bool isRun = Input.GetButton("Run") && isWalk && !alien;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 중력에 의한 이동 처리
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity);

        // 사망 사태일 때 이동할 수 없음
        if (GetComponent<Player>().IsDead())
            return;

        // 이동 제한 상태일때 이동할 수 없음
        if (controllable == false)
            return;

        // 유저 조작에 의한 이동 처리
        if (direction.magnitude >= 0.1f){

            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f,angle,0f);
            controller.Move(direction * (alien ? AlienWalk : (isRun ? ResearcherRun : ResearcherWalk)) * Time.deltaTime);
        }
    }

    public bool IsRun()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVeritcalInput = !Mathf.Approximately(vertical, 0f);

        bool isWalk = hasHorizontalInput || hasVeritcalInput;
        bool isRun = Input.GetButton("Run") && isWalk && !alien;

        return isRun;
    }
}

