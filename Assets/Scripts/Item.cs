using UnityEngine;

public enum Progress{
    END, INCOMPLETE_END, ONGOING
}
[CreateAssetMenu(fileName = "new Item", menuName = "Item/Item")]
public abstract class Item : ScriptableObject{
    new public string name = "new Item";

    public bool isUsable;
    public bool isContinuable;
    public bool isDroppable;

    public int meterialWood;
    public int meterialIron;
    public int meterialPart;

    [TextArea(15,20)]
    public string description;
    public Sprite icon;

    public GameObject model;
    public abstract void Use(Player playerStat); // 사용

    public abstract void Continue(Player playerStat); // 지속 효과
}

/*
    Item

        소비아이템

        일반아이템
*/