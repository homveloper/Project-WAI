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

    bool hasWeapone;

    [SerializeField]
    bool isDroppable;

    Transform rightHand;

    private void Start() {
        playerStat = gameObject.GetComponent<Player>();

        if(photonView.IsMine){
            rightHand = TransformExtention.FirstOrDefault(transform,x => x.name == "mixamorig:RightHand");
        }
    }

    public bool Add(Item item){
        if(!playerStat.IsAlienObject()){
            if(items.Count >= space){
                Debug.Log("인벤토리에 공간이 없습니다.");
                return false;
            }

            if(hasWeapone && item is InteractableItem && ((InteractableItem)item).Itemtype == Itemtype.WEAPONE){
                return false;
            }

            if(item is InteractableItem && ((InteractableItem)item).Itemtype == Itemtype.WEAPONE){
                hasWeapone = true;
                EquipWeapone(item.model);
            }

            items.Add(item);

            if(onItemChangedCallback != null){
                onItemChangedCallback.Invoke();
            }

            return true;
        }else{
            return false;
        }
    }

    public void Remove(Item item){
        items.Remove(item);

        if(onItemChangedCallback != null)   
            onItemChangedCallback.Invoke();
    }
    
    public void Drop(Item item){
        if(item.isDroppable){
            items.Remove(item);

            if(item is InteractableItem && ((InteractableItem)item).Itemtype == Itemtype.WEAPONE){
                hasWeapone = false;
                UnEquipWeapone();
            }

            if(onItemChangedCallback != null)   
                onItemChangedCallback.Invoke();



            GameObject droppedItem = PhotonNetwork.Instantiate("Item/" + item.name, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        }
    }

    public void DropAll(){
        // 아이템을 뒤에서 부터 제거하지 않으면 리스트의 인덱스 접근 오류가 발생합니다.
        for(int i=items.Count -1; i>=0; i--){
            Drop(items[i]);
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

    public bool HasWeapone{
        get=>hasWeapone;
    }

    public void EquipWeapone(GameObject weapone){
        if(weapone != null){
            GameObject newWeapone = PhotonNetwork.Instantiate("Item/" + weapone.name, rightHand.position, Quaternion.identity);
            photonView.RPC("RPCSetParent", RpcTarget.AllBuffered,newWeapone.transform, rightHand);
        }
    }

    public void UnEquipWeapone(){
        photonView.RPC("DestroyWeapone", RpcTarget.AllBuffered, photonView.OwnerActorNr);
    }

    [PunRPC]
    void RPCSetParent(Transform child, Transform parent){
        child.SetParent(parent);
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
    }

    [PunRPC]
    void DestroyWeapone(int actorNumber){
        GameObject[] weapones = GameObject.FindGameObjectsWithTag("Weapone");

        for(int i=weapones.Length-1; i>=0; i++){
            if (photonView.OwnerActorNr == actorNumber){
                Destroy(weapones[i]);
                break;
            }
        }
    }
}