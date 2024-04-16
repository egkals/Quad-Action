using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;    // 따라갈 목표와 위치 오프셋을 public 변수로 선언
    public Vector3 offset;      // offset = 보정값, 살짝 손 본 위치값을 그대로 사용하기 위한

    void Update()
    {
       transform.position = target.position + offset;
    }
}
