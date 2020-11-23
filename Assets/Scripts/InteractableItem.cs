using UnityEngine;

public enum Itemtype{
    WEAPONE,OBJECT
}

[CreateAssetMenu(fileName = "new InteractableItem", menuName = "Item/InteractableItem")]
public class InteractableItem : Item
{
    public Itemtype itemtype;

    public int damageModifier;

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