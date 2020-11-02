using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrap : MonoBehaviour
{
    public float start;
    public Material r1;
    public Material r2;

    float term = 30;
    float moveTime = 3;

    void Start()
    {
        StartCoroutine(this.moveTrap());
        
    }

    IEnumerator moveTrap()
    {
        while(true)
        {
            yield return new WaitForSeconds(start);
           
            gameObject.

            yield return new WaitForSeconds(moveTime);

           

            yield return new WaitForSeconds(term);
        }
    }
}
