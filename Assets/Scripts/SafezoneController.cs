using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SafezoneController : MonoBehaviour
{
    public List<GameObject> survivorList;

    void Start()
    {
        survivorList = new List<GameObject>();
    }

    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameObject player = other.gameObject;
        bool valid = true;

        if (other.GetComponent<Player>().IsDead())
            valid = false;
        else if (other.GetComponent<Player>().IsAlienPlayer())
            valid = false;

        Debug.Log(valid);

        if (!survivorList.Contains(player) && valid)
            survivorList.Add(player);
        else if (survivorList.Contains(player) && !valid)
            survivorList.Remove(player);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameObject player = other.gameObject;

        if (survivorList.Contains(player))
            survivorList.Remove(player);
    }
}
