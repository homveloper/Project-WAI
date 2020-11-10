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
        if(Input.GetButtonDown(useButton)){
            if(Inventory.instance.isDroppable){
                DropItem();
            }else{
                UseItem();
            }
        }
    }

    public void AddItem(Item item)
    {
        this.item = item;

        if(item is ConsumableItem){
            itemIcon.sprite = ((ConsumableItem)item).icon;
        }else if (item is InteractableItem){
            itemIcon.sprite = ((InteractableItem)item).icon;
        }

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
        if (item != null)
        {
            item.Use(Inventory.instance.playerStat);
         
            if(item is ConsumableItem)
                Inventory.instance.Remove(item);
        }
    }

    public void SetDropIcon(bool status){
        dropIcon.enabled = status;
    }

    public void DropItem(){
        if(item != null){
            Inventory.instance.Drop(item);
        }
    }

}
