using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrap : MonoBehaviour
{
    public float start;
    public Material r1;
    public Material r2;


    float term = 30;
    float moveTime = 4;

    void Start()
    {
        
        StartCoroutine(this.moveTrap());
        
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
