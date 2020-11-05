using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MoveTrap : MonoBehaviourPun
{
    public float start;
    public Material r1;
    public Material r2;
    public GameObject center;


    float term = 30;
    float moveTime = 4;

    void Start()
    {
        
        StartCoroutine(this.moveTrap());
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if(gameObject.GetComponent<Renderer>().material == r2)
            other.transform.position = center.transform.position;
    }
    IEnumerator moveTrap()
    {
        while(true)
        {
            yield return new WaitForSeconds(start);
            
            gameObject.GetComponent<Renderer>().material.Lerp(r2,r1,1);

            yield return new WaitForSeconds(moveTime);

            gameObject.GetComponent<Renderer>().material.Lerp(r1,r2,1);

            yield return new WaitForSeconds(term);
        }
    }
}
