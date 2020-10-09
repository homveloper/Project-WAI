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

    void Awake(){
        inventory = Inventory.instance;
        inventory.onItemChangedCallback +=  UpdateUI;
    
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void Update(){

    }

    void UpdateUI(){
        Debug.Log("인벤토리 새로고침");

        for(int i=0; i<slots.Length; i++){
            if( i< inventory.items.Count){
                slots[i].AddItem(inventory.items[i]);
            }else{
                slots[i].ClearSlot();
            }
        }
    }
}
