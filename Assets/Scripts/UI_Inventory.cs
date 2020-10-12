using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UI_Inventory : MonoBehaviourPunCallbacks
{

    public Transform itemsParent;

    Inventory inventory;

    [SerializeField]
    private InventorySlot[] slots;


    void Start()
    {
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    public void UpdateInventory(){
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
    }

    private void Update() {
        if(Input.GetButtonDown("SetDrop") && !inventory.isEmpty()){
            inventory.isDroppable = !inventory.isDroppable;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        Debug.Log("인벤토리 새로고침");

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
                if(inventory.isDroppable){
                    slots[i].SetDropIcon(true);
                }else{
                    slots[i].SetDropIcon(false);
                }
            }
            else
            {
                slots[i].ClearSlot();
                slots[i].SetDropIcon(false);
            }
        }
    }
}
