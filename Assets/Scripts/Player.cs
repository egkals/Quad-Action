using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;     // public = 인스펙터 창에서 값 설정 가능
    float hAxis;
    float vAxis;
    bool wDown;

    Vector3 moveVec;

    Animator anim;

    void Awake()    // 초기화 함수 Awake()
    {
        anim = GetComponentInChildren<Animator>();  // Animator 변수를 GetComponentInChildren()으로 초기화
                                                    // 자식 오브젝트에 있는 컴포넌트 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");  //GetAxisRaw() = Axis 값을 정수로 반환하는 함수
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");                  // shift를 꾹 누르고 있을때만 작동되도록 GetButton
                                                          // 반대로 눌렀을 때만 작동하는건 GetDown?

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  // nomalized = 방향 값이 1로 보정된 벡터


        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;     // transform 이동은 꼭 Time.deltaTime까지 곱해야함
                                                                                          // bool 형태 조건 ? true일 때 값 : flase 일 때 값 (삼항연산자)


        anim.SetBool("isRun", moveVec != Vector3.zero);         // SetBool() 함수로 파라메터 값 설정
        anim.SetBool("isWalk", wDown);

        // 현 위치에서 나아가는 방향으로 바라보기
        transform.LookAt(transform.position + moveVec);  // LookAt() = 지정된 벡터를 향해서 회전시켜주는 함수
    }
}
