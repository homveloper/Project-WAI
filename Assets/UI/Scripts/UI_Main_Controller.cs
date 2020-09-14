using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main_Controller : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetPage(int page)
    {
        float timeOfTravel = 5; //time after object reach a target place 
        float currentTime = 0; // actual floting time 
        float normalizedValue;
        RectTransform rectTransform = GameObject.Find("UI_Main_Menu").GetComponent<RectTransform>(); //getting reference to this component 

        Vector3 startPosition = new Vector3(-1000, 0, 0);
        Vector3 endPosition = new Vector3(1000, 0, 0);

        IEnumerator LerpObject()
        {

            while (currentTime <= timeOfTravel)
            {
                currentTime += Time.deltaTime;
                normalizedValue = currentTime / timeOfTravel; // we normalize our time 

                rectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, normalizedValue);
                yield return null;
            }
        }
    }
}
