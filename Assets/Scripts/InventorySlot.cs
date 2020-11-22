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
        if(Input.GetButtonDown(useButton) && GameManager.GetInstance().mPlayer.GetComponent<Player>().IsControllable())
            if (Inventory.instance.IsDroppable)
                DropItem();
            else
                UseItem();

        if (item != null && item.isContinuable)
            item.Continue(Inventory.instance.playerStat);
    }

    public void AddItem(Item item)
    {
        this.item = item;
        itemIcon.sprite =item.icon;
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
