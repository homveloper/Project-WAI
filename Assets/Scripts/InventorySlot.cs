using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public Image dropIcon;
    public string useButton;

    Item item;

    private void Start()
    {
        itemIcon.enabled = false;
        dropIcon.enabled = false;
    }

    private void Update() {
        if(Input.GetButtonDown(useButton))
            if(Inventory.instance.isDroppable)
                DropItem();
            else
                UseItem();

        if (item != null && item.isContinuable)
            item.Continue(Inventory.instance.playerStat);
    }

    public void AddItem(Item item)
    {
        this.item = item;

        if(item is ConsumableItem)
            itemIcon.sprite = ((ConsumableItem)item).icon;
        else if (item is InteractableItem)
            itemIcon.sprite = ((InteractableItem)item).icon;
        else if (item is ContinuableItem)
            itemIcon.sprite = ((ContinuableItem)item).icon;

        itemIcon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public void UseItem()
    {
        if (item != null && item.isUsable)
            item.Use(Inventory.instance.playerStat);
    }

    public void SetDropIcon(bool status){
        dropIcon.enabled = status;
    }

    public void DropItem(){
        if(item != null && item.isDroppable)
            Inventory.instance.Drop(item);
    }
}
