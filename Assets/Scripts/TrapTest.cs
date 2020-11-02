using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTest : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject center;
    void OnTriggerEnter(Collider other)
    {
       other.transform.position = center.transform.position;
    }
}
