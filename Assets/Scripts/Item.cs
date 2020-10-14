using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Item", menuName = "Item")]

public class Item : ScriptableObject
{
    //when you wan't initialize use 'new'
    new public string name = "new Item";
    public int cost;
    public int HPModifier;
    public int O2Modifier;

    [TextArea(15,20)]
    public string description;
    public Sprite icon;

    public virtual void Use(Player playerStat){
        playerStat.SetHP(playerStat.GetHP() + HPModifier);
        playerStat.SetO2(playerStat.GetO2() + O2Modifier);
    }
}
