using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Inventory : MonoBehaviourPun {
    
    #region Singleton

    public int space = 5;

    public static Inventory instance;

    private void Awake() {
        

        print("Awake Inventory");

        if(instance != null){
            Debug.LogWarning("하나 이상의 인벤토리가 발견되었습니다.");
            return;
        }

        if(photonView.IsMine){
            instance = this;    
        }
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public bool isDroppable;

    public List<Item> items = new List<Item>();

    public Player playerStat;

    private void Start() {
        playerStat = gameObject.GetComponent<Player>();
    }

    public bool Add(Item item){

        if(items.Count >= space){
            Debug.Log("인벤토리에 공간이 없습니다.");
            return false;
        }

        items.Add(item);

        if(onItemChangedCallback != null){
            onItemChangedCallback.Invoke();
        }

        return true;
    }

    public void Remove(Item item){
        items.Remove(item);

        if(onItemChangedCallback != null)   
            onItemChangedCallback.Invoke();
    }
    
    public void Drop(Item item){
        items.Remove(item);

        if(onItemChangedCallback != null)   
            onItemChangedCallback.Invoke();

        GameObject droppedItem = PhotonNetwork.InstantiateRoomObject("Item/" + item.name, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
    }

    public void DropAll(){
        foreach(Item item in items){
            GameObject droppedItem = PhotonNetwork.InstantiateRoomObject("Item/" + item.name, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        }

        if(onItemChangedCallback != null)   
            onItemChangedCallback.Invoke();
    }

    public bool isEmpty(){
        return items.Count == 0;
    }
}