﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Stat{
    HP,O2,BATTERY
}

[CreateAssetMenu(fileName = "new ConsumableItem", menuName = "ConsumableItem", order = 0)]
public class ConsumableItem : Item
{

    [SerializeField]
    int hpModifier;
    [SerializeField]
    int o2Modifier;
    [SerializeField]
    int batteryModifier;

    [SerializeField]
    float castingTime = 1f;

    [SerializeField]
    float overTime = 5f;
    float smoothness = 2f;

    Progress progress = Progress.END;
    IEnumerator coroutine;

    public override void Use(Player playerStat)
    {
        PlayerAnimation playerAnimation = playerStat.researcher.GetComponent<PlayerAnimation>();
        coroutine = OnStartAfterTime(playerStat,castingTime);

        Debug.Log(name + "을 사용하였습니다.");

        if(Inventory.instance != null){
            playerAnimation.animator.SetTrigger("casting");
            Inventory.instance.StartCoroutine(coroutine);
            Inventory.instance.StartCoroutine(IsCasting(coroutine,playerAnimation));
        }
    }

    IEnumerator IsCasting(IEnumerator coroutine,PlayerAnimation playerAnimation){

        float calibrationTime = 0.5f;

        yield return new WaitForSeconds(calibrationTime);

        float startTime = Time.time;

        while(Time.time <= startTime + castingTime){
            if(!playerAnimation.AnimatorIsPlaying("Nervously Look Around")){
                Inventory.instance.StopCoroutine(coroutine);
            }
            yield return null;  //1프레임 마다 체크합니다.
        }
    }

    IEnumerator OnStartAfterTime(Player playerStat, float delayTime){
        yield return new WaitForSeconds(delayTime);

        Debug.Log("효과가 발동합니다.");

        TakeEffect(playerStat, hpModifier,Stat.HP);
        TakeEffect(playerStat, o2Modifier,Stat.O2);
        TakeEffect(playerStat, batteryModifier,Stat.BATTERY);

        // IEnumerator[] coroutines = {TakeEffect(playerStat, hpModifier,Stat.HP),
        //                             TakeEffect(playerStat, o2Modifier,Stat.O2),
        //                             TakeEffect(playerStat, batteryModifier,Stat.BATTERY)};

        // for(int i=0; i<coroutines.Length; i++){
        //     Inventory.instance.StartCoroutine(coroutines[i]);
        // }

        Inventory.instance.Remove(this);
    }

    // IEnumerator TakeEffect(Player playerStat, int modifier, Stat stat){
    //     Debug.Log("수치가 변동됩니다.");

    //     if(Stat.HP == stat){
    //         for(float i=0; i<modifier; i += modifier/overTime){
    //             playerStat.SetHP(playerStat.GetHP() + modifier/overTime);
    //             yield return new WaitForSeconds(1/overTime);
    //         }
    //     }

    //     if(Stat.O2 == stat){
    //         for(float i=0; i<modifier; i += modifier/overTime){
    //             playerStat.SetO2(playerStat.GetO2() + modifier/overTime);
    //             yield return new WaitForSeconds(1/overTime);
    //         }
    //     }

    //     if(Stat.BATTERY == stat){
    //         for(float i=0; i<modifier; i += modifier/overTime){
    //             playerStat.SetBt(playerStat.GetBt() + modifier/overTime);
    //             yield return new WaitForSeconds(1/overTime);
    //         }
    //     }

    // }

    void TakeEffect(Player playerStat, int modifier, Stat stat){
        Debug.Log("수치가 변동됩니다.");

        if(Stat.HP == stat)
            playerStat.SetHP(playerStat.GetHP() + modifier);
        if(Stat.O2 == stat)
            playerStat.SetO2(playerStat.GetO2() + modifier);
        if(Stat.BATTERY == stat)
            playerStat.SetBt(playerStat.GetBt() + modifier);
    }

    public int HPModifier
    {
        get => hpModifier;
        set => hpModifier = value;
    }

    public int O2Modifier
    {
        get => o2Modifier;
        set => o2Modifier = value;
    }

    public int BatteryModifier{
        get => batteryModifier;
        set => batteryModifier = value;
    }

    public float CastingTime{
        get => castingTime;
    }

    public float OverTime{
        get => overTime;
    }
}