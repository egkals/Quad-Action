using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;     // public = 인스펙터 창에서 값 설정 가능
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    void Awake()    // 초기화 함수 Awake()
    {
        anim = GetComponentInChildren<Animator>();  // Animator 변수를 GetComponentInChildren()으로 초기화
                                                    // 자식 오브젝트에 있는 컴포넌트 가져오기
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");  //GetAxisRaw() = Axis 값을 정수로 반환하는 함수
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");         // shift를 꾹 누르고 있을때만 작동되도록 GetButton
        jDown = Input.GetButtonDown("Jump");     // 눌렀을 때만 작동
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  // nomalized = 방향 값이 1로 보정된 벡터


        if (isDodge)
            moveVec = dodgeVec;

        // transform 이동은 꼭 Time.deltaTime까지 곱해야함
        // bool 형태 조건 ? true일 때 값 : flase 일 때 값 (삼항연산자)
        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;    

        anim.SetBool("isRun", moveVec != Vector3.zero);     // SetBool() 함수로 파라메터 값 설정
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 현 위치에서 나아가는 방향으로 바라보기
        transform.LookAt(transform.position + moveVec);  // LookAt() = 지정된 벡터를 향해서 회전시켜주는 함수
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            // AddForce() = 물리적인 힘 가하기, 15 = 점프 파워, impulse = 즉발
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;
            speed *= 2;           
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);   // Invoke() = 시간차 함수, 함수 이름을 문자열로 집어 넣고, 시간차
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void OnCollisionEnter(Collision collision)  //  충돌로 착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }
}
