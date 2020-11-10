using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float castingTime = 5f;

    public override void Use(Player playerStat)
    {
        float currentTime = 0f;

        // 캐스팅
        // 아이템 섭취중 움직일 수 없으며, 맞으면 해당 기능이 취소된다.

        if(Inventory.instance != null){
            while(currentTime <= castingTime){
                Inventory.instance.StartCoroutine(TakeEffecAfterTime(playerStat,1f));
                currentTime += 1;
            }
        }
    }

    IEnumerator TakeEffecAfterTime(Player playerStat, float delayTime){
        yield return new WaitForSeconds(delayTime);

        float timeCorrection = castingTime/Time.deltaTime;

        // is Researcher
        if(!playerStat.IsAlienPlayer()){
            playerStat.SetHP(playerStat.GetHP() + hpModifier/castingTime);
            playerStat.SetO2(playerStat.GetO2() + o2Modifier/castingTime);

        // is Alien
        }else{
            playerStat.SetHP(playerStat.GetHP() + (hpModifier/2)/castingTime);
            playerStat.SetO2(playerStat.GetO2() + (o2Modifier/2)/castingTime);
        }

        playerStat.SetBt(playerStat.GetBt() + batteryModifier/castingTime);

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
}