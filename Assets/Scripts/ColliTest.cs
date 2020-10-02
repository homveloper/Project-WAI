﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ColliTest : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)

    {
        Debug.Log(other.name + "감지 시작!");
        SceneManager.LoadScene("Ruin 1-1 new ver.");

    }


    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌중일 때

    private void OnTriggerStay(Collider other)

    {

        Debug.Log(other.name + "감지 중!");

    }



    // Collider 컴포넌트의 is Trigger가 true인 상태로 충돌이 끝났을 때

    private void OnTriggerExit(Collider other)

    {

        Debug.Log(other.name + "감지 끝!");
    }

}
