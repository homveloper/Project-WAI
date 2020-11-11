using UnityEngine;

public enum Progress{
    END, INCOMPLETE_END, ONGOING
}
[CreateAssetMenu(fileName = "new Item", menuName = "Item")]
public abstract class Item : ScriptableObject{
    new public string name = "new Item";

    public int meterialWood;
    public int meterialIron;
    public int meterialPart;

    [TextArea(15,20)]
    public string description;
    public Sprite icon;

    public abstract Progress Use(Player playerStat);

}

/*
    Item

        소비아이템

        일반아이템
*/