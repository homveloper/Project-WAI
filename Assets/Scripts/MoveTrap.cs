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

        Debug.Log(gameObject.GetComponent<Renderer>().material.color);
        Debug.Log(r2.color);
        Debug.Log(r1.color);
        
        if(gameObject.GetComponent<Renderer>().material.color == r2.color)
        {
            Debug.Log("돌아가");
            other.transform.position = center.transform.position;
        }
    }
    IEnumerator moveTrap()
    {
        while(true)
        {
            yield return new WaitForSeconds(start);
            
            gameObject.GetComponent<Renderer>().material.Lerp(r2,r1,1f);

            yield return new WaitForSeconds(moveTime);

            gameObject.GetComponent<Renderer>().material.Lerp(r1,r2,1f);

            yield return new WaitForSeconds(term);
        }
    }
}
