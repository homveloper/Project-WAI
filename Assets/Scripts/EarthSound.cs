using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthSound : MonoBehaviour
{
    public AudioSource earth1;
    public AudioSource earth2;
    public AudioSource earth3;

    public int cnt = 0;
    // Start is called before the first frame update
    void Start()
    {
        earth1.Pause();
        earth2.Pause();
        earth3.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if(cnt == 1)
        {
            earth1.Play();
            cnt++;
        }
        else if(cnt == 2 && !earth1.isPlaying)
        {
            earth2.Play();
            cnt++;
        }
        else if(cnt == 3 && !earth2.isPlaying)
        {
            earth3.Play();
        }
    }
}
