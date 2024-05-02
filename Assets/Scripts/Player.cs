using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;     // public = 인스펙터 창에서 값 설정 가능
    public GameObject[] weapons;    // 무기 배열 함수
    public bool[ ] hasWeapons;      // 무기 가지고 있는지 상태 배열 함수


    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;     // 상호작용 키
    bool sDown1;    // 1번 무기 스왑
    bool sDown2;    // 2번

    bool isJump;
    bool isDodge;
    bool isSwap;    // 교체 시간차를 위한 플래그

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;      // ontrigger 사용위한 오브젝트
    GameObject equipWeapon;     // 현재 장착중인 무기 저장하는 변수
    int equipWeaponIndex = -1;  // 0이 망치라 -1로 초기화 해놓음

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
        Swap();     // 무기 교체 함수
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");  //GetAxisRaw() = Axis 값을 정수로 반환하는 함수
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");         // shift를 꾹 누르고 있을때만 작동되도록 GetButton
        jDown = Input.GetButtonDown("Jump");     // 눌렀을 때만 작동
        iDown = Input.GetButtonDown("Interation");  // 인터렉션 키
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;  // nomalized = 방향 값이 1로 보정된 벡터


        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap)
            moveVec = Vector3.zero;
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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
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
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
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

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))// 1번을 눌렀을때 이미 가지고 있는 것이면 수행 x
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;

        int weaponIndex = -1;   // 기본 인덱스 -1
        if (sDown1) weaponIndex = 0;    // 1번 무기
        if (sDown2) weaponIndex = 1;

        if ((sDown1 || sDown2) && !isJump && !isDodge) // 1,2,3 셋 중 아무거나 눌렸을 때
        {
            if(equipWeapon != null) // 빈손이 아닐때만 해당 로직을 실행
                equipWeapon.SetActive(false);   // 현재 무기를 없애버림


            equipWeaponIndex = weaponIndex;    // 현재 장착하고 있는 무기의 인덱스를 눌린 것으로 바꿈
            equipWeapon = weapons[weaponIndex]; // 현재 무기를 누른 무기로 바꿈
            equipWeapon.SetActive(true);   // 배열의 해당 무기를 활성화 시켜 보이게 하기

            anim.SetTrigger("doSwap");  // 스왑하는 애니메이션 추가

            isSwap = true;

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;       // 아이템 스크립트의 아이템들 벨류 값임
                hasWeapons[weaponIndex] = true;     // 그 아이템을 가진 상태로 바뀜

                Destroy(nearObject);    // 상호작용 성공하면 그 물체 없애기
            }
        }
    }

    void OnCollisionEnter(Collision collision)  //  충돌로 착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }

}
