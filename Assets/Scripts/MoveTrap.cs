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

    int cnt =1;

    public float term = 14;
    public float moveTime = 3;

    int frame = 0;

    void Awake()
    {
        StartCoroutine(this.moveTrap());
    }
    void Start()
    {
        // StartCoroutine(this.moveTrap());
    }
    void Update()
    {  
        frame++;
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
        yield return new AsyncOperation();

        WaitForSeconds startWait = new WaitForSeconds(start); 
        WaitForSeconds moveWait = new WaitForSeconds(moveTime); 
        WaitForSeconds termWait = new WaitForSeconds(term); 

        yield return startWait;
        
        while(true)
        {
            if(start == 0)
                Debug.Log(frame);
            
            gameObject.GetComponent<Renderer>().material.Lerp(r2,r1,1f);

            yield return moveWait;

            gameObject.GetComponent<Renderer>().material.Lerp(r1,r2,1f);

            yield return termWait;
        }
    }
}
