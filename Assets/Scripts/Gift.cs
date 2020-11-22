using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift : MonoBehaviour
{
    public GameObject gift; 
    // Start is called before the first frame update
    void Start()
    {
        gift.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "HitBox")
            return ;
        
        gift.SetActive(true);
    }
}
