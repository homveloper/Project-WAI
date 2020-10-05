using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    Inventory inventory;

    [SerializeField]
    private List<Item> items;

    void Start(){
        inventory = Inventory.instance;
        inventory.onItemChangedCallback +=  UpdateUI;
    }

    void Update(){

    }

    void UpdateUI(){
        Debug.Log("인벤토리 새로고침");
    }
}
