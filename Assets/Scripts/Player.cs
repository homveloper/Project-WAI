using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float statHp;
    public float statHpMax;
    public float statO2;
    public float statO2Max;
    public int meterialWood;
    public int meterialIron;
    public int meterialPart;

    void Start()
    {
        statHp = 100;
        statHpMax = 100;
        statO2 = 100;
        statO2Max = 100;
        meterialWood = 0;
        meterialIron = 0;
        meterialPart = 0;
    }

    void Update()
    {
        // 산소 차감
        statO2 -= Time.deltaTime;

        // 체력 차감
        if (statO2 <= 0)
            statHp -= (Time.deltaTime * 5);

        // TODO : get, set 메소드 만들어서 0미만이 되거나 최대치 이상이 되는 것을 필터해야함.
    }

    private void OnTriggerEnter(Collider other)
    {

    }

}
