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

    Transform rightHand;

    private void Start() {
        playerStat = gameObject.GetComponent<Player>();
        rightHand = TransformExtention.FirstOrDefault(transform,x => x.name == "mixamorig:RightHand");
    }

    public bool Add(Item item){

        if(items.Count >= space){
            Debug.Log("인벤토리에 공간이 없습니다.");
            return false;
        }

        if(hasWeaphone && item is InteractableItem && ((InteractableItem)item).Itemtype == Itemtype.WEAPHONE){
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
                UnEquipWeaphone();
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

    public void EquipWeaphone(GameObject weapone){
        if(weapone != null){
            GameObject newWeapone = PhotonNetwork.Instantiate("Item/" + weapone.name, rightHand.position, Quaternion.identity);
            newWeapone.transform.SetParent(rightHand);
            newWeapone.transform.localPosition = Vector3.zero;
            newWeapone.transform.localRotation = Quaternion.identity;
        }
    }

    public void UnEquipWeaphone(){
        foreach(Transform child in rightHand){
            photonView.RPC("DestroyWeapone", RpcTarget.AllBuffered, photonView.OwnerActorNr);
        }
    }

    [PunRPC]
    void DestroyWeapone(int actorNumber){
        GameObject[] weapones = GameObject.FindGameObjectsWithTag("Weapone");

        foreach(GameObject weapone in weapones){
            if (photonView.OwnerActorNr == actorNumber){
                Destroy(weapone);
                break;
            }
        }
    }
}