using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        other.isTrigger = false;
        Debug.Log(other);
        if (other.tag == "Finish")
        {
            GameManager.GetInstance().ChangeMap("proto_field2");
        }
        else if (other.tag == "Respawn")
        {
            GameManager.GetInstance().ChangeMap("proto_field");
        }
    }
}
