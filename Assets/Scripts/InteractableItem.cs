using UnityEngine;

public enum Itemtype{
    WEAPHONE,OBJECT
}

[CreateAssetMenu(fileName = "new InteractableItem", menuName = "InteractableItem")]
public class InteractableItem : Item
{
    Itemtype itemtype;

    int damageModifier;

    public override void Use(Player playerStat)
    {
        
    }

    public override void Continue(Player playerStat)
    {

    }

    public Itemtype Itemtype{
        get => itemtype;
    }

    public int DamageModifier{
        get => damageModifier;
    }
}