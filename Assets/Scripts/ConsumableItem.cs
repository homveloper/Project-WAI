using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ConsumableItem", menuName = "ConsumableItem", order = 0)]
public class ConsumableItem : Item{
    
    public int HPModifier{set;get;}
    public int O2Modifier{set;get;}

    public override void Use(Player playerStat){
        playerStat.SetHP(playerStat.GetHP() + HPModifier);
        playerStat.SetO2(playerStat.GetO2() + O2Modifier);
    }
}
