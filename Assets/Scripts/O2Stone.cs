using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2Stone : MonoBehaviour
{
    public GameObject swichFlash;
    // Start is called before the first frame update
    void Start()
    {
        swichFlash.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
     void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag != "HitBox")
            return ;
        
        other.GetComponentInParent<Player>().statO2++;
        swichFlash.SetActive(true);
    }
}
