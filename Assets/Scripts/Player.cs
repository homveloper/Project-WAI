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
        
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
