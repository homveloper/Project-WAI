using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSelect : MonoBehaviour
{
    public AudioSource mainBGM;
    public AudioSource temple1_BGM;
    public AudioSource temple2_BGM;
    public AudioSource temple3_BGM;    

    public AudioSource curBGM;

    public int stat = 0;
    public int cur = 0;
    // Start is called before the first frame update
    void Start()
    {
        mainBGM.Pause();
        temple1_BGM.Pause();
        temple2_BGM.Pause();
        temple3_BGM.Pause();
        curBGM.Pause();
    }
    // Update is called once per frame
    void Update()
    {
        ChangeBGM(stat);
        if(!curBGM.isPlaying)
            curBGM.Play();
    }

    public void ChangeBGM(int stat)
    {
        //Debug.Log(cur+" , "+stat);

        if(cur == stat)
            return;
        else
            cur = stat;

        if(curBGM.isPlaying)
            curBGM.Pause();

        switch(stat)
        {
            case 0 :
                curBGM = mainBGM;
                break;
            case 1 : 
                curBGM = temple1_BGM;
                break;
            case 2 : 
                curBGM = temple2_BGM;
                break;
            case 3 : 
                curBGM = temple3_BGM;
                break;    
        }
    }
}
