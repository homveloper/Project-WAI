using UnityEngine;

public enum Itemtype{
    WEAPHONE,OBJECT
}

[CreateAssetMenu(fileName = "new InteractableItem", menuName = "InteractableItem")]
public class InteractableItem : Item
{
    public Itemtype itemtype;

    public int damageModifier;

    public override void Use(Player playerStat)
    {
        
    }

    public Itemtype Itemtype{
        get => itemtype;
    }

    public int DamageModifier{
        get => damageModifier;
    }
}