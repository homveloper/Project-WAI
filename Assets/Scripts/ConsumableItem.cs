using System.Collections;
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

    public override Progress Use(Player playerStat)
    {
        PlayerAnimation playerAnimation = playerStat.researcher.GetComponent<PlayerAnimation>();
        coroutine = OnStartAfterTime(playerStat,playerAnimation);

        Debug.Log(name + "을 사용하였습니다.");
        // 캐스팅
        // 아이템 섭취중 움직일 수 없으며, 맞으면 해당 기능이 취소된다.

        if(Inventory.instance != null){
            playerAnimation.animator.SetTrigger("casting");
            Inventory.instance.StartCoroutine(coroutine);

            // 캐스팅 시간 동안 캐릭터가 움직이거나 공격을 받거나 어떠한 행동을 했을 때;
            // Inventory.instance.StartCoroutine(OnInteracting(playerAnimation,coroutine));
        }

        return progress;
    }

    IEnumerator OnInteracting(PlayerAnimation playerAnimation, IEnumerator coroutine){
        float currentTime = Time.time;

        while(Time.time <= currentTime + castingTime){
            if(playerAnimation.IsRun || playerAnimation.IsWalk){
                Inventory.instance.StopCoroutine(coroutine);
                progress = Progress.INCOMPLETE_END;
            }
            yield return null;
        }
    }

    IEnumerator OnStartAfterTime(Player playerStat, PlayerAnimation playerAnimation){
        // yield return new WaitForSeconds(delayTime);

        float currentTime = Time.time;

        while(Time.time <= currentTime + castingTime){
            if(playerAnimation.IsRun || playerAnimation.IsWalk){
                if(coroutine != null)
                    Inventory.instance.StopCoroutine(coroutine);
                progress = Progress.INCOMPLETE_END;
            }
            yield return null;
        }

        Debug.Log("효과가 발동합니다.");

        Inventory.instance.StartCoroutine(TakeEffect(playerStat, hpModifier,Stat.HP));
        Inventory.instance.StartCoroutine(TakeEffect(playerStat, o2Modifier,Stat.O2));
        Inventory.instance.StartCoroutine(TakeEffect(playerStat, batteryModifier,Stat.BATTERY));
    }

    IEnumerator TakeEffect(Player playerStat, int modifier, Stat stat){
        Debug.Log("수치가 변동됩니다.");

        if(Stat.HP == stat){
            for(float i=0; i<modifier; i += modifier/overTime/smoothness){
                playerStat.SetHP(playerStat.GetHP() + modifier/overTime/smoothness);
                yield return new WaitForSeconds(1/overTime/smoothness);
            }   
        }

        if(Stat.O2 == stat){
            for(float i=0; i<modifier; i += modifier/overTime){
                playerStat.SetO2(playerStat.GetO2() + modifier/overTime);
                yield return new WaitForSeconds(1/overTime);
            }
        }

        if(Stat.BATTERY == stat){
            for(float i=0; i<modifier; i += modifier/overTime){
                playerStat.SetBt(playerStat.GetBt() + modifier/overTime);
                yield return new WaitForSeconds(1/overTime/smoothness);
            }
        }

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