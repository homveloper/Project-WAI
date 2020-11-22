using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
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

        print("UI inventory get player inventory");
    }

    private void Update() {
        if(inventory != null){
            if(Input.GetButtonDown("SetDrop") && !inventory.isEmpty() && GameManager.GetInstance().mPlayer.GetComponent<Player>().IsControllable()){
                inventory.IsDroppable = !inventory.IsDroppable;
                UpdateUI();
            }
            
            if(inventory.isEmpty() && inventory.IsDroppable){
                inventory.IsDroppable = false;
                UpdateUI();
            }
        }else{
            print("inventory is null");
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

                if(inventory.IsDroppable){
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
