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

    public List<Item> items = new List<Item>();
    
    public Player playerStat;

    bool hasWeaphone;

    [SerializeField]
    bool isDroppable;



    private void Start() {
        playerStat = gameObject.GetComponent<Player>();
    }

    public bool Add(Item item){

        if(items.Count >= space){
            Debug.Log("인벤토리에 공간이 없습니다.");
            return false;
        }

        if(hasWeaphone && item is InteractableItem){
            return false;
        }

        if(item is InteractableItem && ((InteractableItem)item).Itemtype == Itemtype.WEAPHONE){
            hasWeaphone = true;
            EquipWeaphone(item.model);
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
        if(item.isDroppable){
            items.Remove(item);

            if(item is InteractableItem && ((InteractableItem)item).Itemtype == Itemtype.WEAPHONE){
                hasWeaphone = false;
                EquipWeaphone(null);
            }

            if(onItemChangedCallback != null)   
                onItemChangedCallback.Invoke();

            GameObject droppedItem = PhotonNetwork.Instantiate("Item/" + item.name, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        }
    }

    public void DropAll(){
        foreach(Item item in items){
            Drop(item);
        }

        if(onItemChangedCallback != null)   
            onItemChangedCallback.Invoke();
    }

    public bool isEmpty(){
        return items.Count == 0;
    }

    public bool IsDroppable{
        get=>isDroppable;
        set=>isDroppable = value;
    }

    public bool HasWeaphone{
        get=>hasWeaphone;
    }

    public void EquipWeaphone(GameObject weaphone){
        if(weaphone != null){
            GameObject newWeaphone = GameObject.Instantiate(weaphone);
            Transform rightHand = TransformExtention.FirstOrDefault(transform,x => x.name == "mixamorig:RightHand");
            newWeaphone.transform.SetParent(rightHand);
        }
    }

    public void UnEquipWeaphone(GameObject weaphone){
        if(weaphone != null){
            Transform rightHand = TransformExtention.FirstOrDefault(transform,x => x == weaphone);
            GameObject.Destroy(weaphone);
        }
    }
}