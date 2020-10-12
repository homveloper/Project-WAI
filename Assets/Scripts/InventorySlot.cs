using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public string useButton;

    Item item;

    private void Start()
    {
        icon.enabled = false;
    }

    private void Update() {
        if(Input.GetButtonDown(useButton)){
            UseItem();
        }
    }

    public void AddItem(Item item)
    {
        this.item = item;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            Inventory.instance.Remove(item);
        }
    }

}
