using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearShip : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
       if(other.gameObject.tag == "Mission")
            GameManager.GetInstance().SetFinish(true);
    }
}
