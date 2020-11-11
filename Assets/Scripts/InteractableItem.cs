using UnityEngine;

public enum Itemtype{
    WEAPHONE,OBJECT
}

[CreateAssetMenu(fileName = "new InteractableItem", menuName = "InteractableItem")]
public class InteractableItem : Item
{
    Itemtype itemtype;

    int damageModifier;

    public override Progress Use(Player playerStat)
    {
        return Progress.END;
    }

    public Itemtype Itemtype{
        get => itemtype;
    }

    public int DamageModifier{
        get => damageModifier;
    }
}