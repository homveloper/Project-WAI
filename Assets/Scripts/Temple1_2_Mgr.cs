using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple1_2_Mgr : MonoBehaviour
{
    public int cnt = 6;
    public GameObject Goal;
    public AudioSource wallSound;
    // Start is called before the first frame update
    void Start()
    {
        wallSound.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 0)
        {
            Goal.SetActive(false);
            wallSound.Play();
            cnt = 6;
        }
    }
}
