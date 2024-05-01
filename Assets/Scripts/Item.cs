using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };   // enum = 중괄호 안에 데이터 열거형
    public Type type;       // 아이템 종류를 저장할 변수
    public int value;       // 아이템 값을 저장할 변수

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime); // 아이템 회전
    }
}
