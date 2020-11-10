using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseLerp : MonoBehaviour
{
    // Start is called before the first frame update

    Player player;
    bool isCalled = false;
    public float modifier = 50;
    public float overTime = 5f;

    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCalled)
        {
            StartCoroutine(increase(player,overTime));
            isCalled = true;
        }
    }

    IEnumerator increase(Player playerStat, float overTime)
    {
        for(float i=0; i<modifier; i += modifier/overTime){
            playerStat.SetHP(playerStat.GetHP() + modifier/overTime);
            yield return new WaitForSeconds(1/overTime);
        }
    }
}
