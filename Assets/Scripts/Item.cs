using UnityEngine;

[CreateAssetMenu(fileName = "new Item", menuName = "Item")]
public class Item : ScriptableObject{
    new public string name = "new Item";

    public Resource cost;

    [TextArea(15,20)]
    public string description;
    public Sprite icon;

    public virtual void Use(Player playerStat){

    }

}

/*
    Item

        소비아이템

        일반아이템
*/