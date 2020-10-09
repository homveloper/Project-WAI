using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Inventory : MonoBehaviour {
    
    #region Singleton

    public int space = 5;

    public static Inventory instance;

    private void Awake() {
        if(instance != null){
            Debug.LogWarning("하나 이상의 인벤토리가 발견되었습니다.");
            return;
        }

        instance = this;    
    }

    #endregion

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public List<Item> items = new List<Item>();

    public bool Add(Item item){

        if(items.Count >= space){
            Debug.Log("인벤토리에 공간이 없습니다.");
            return false;
        }

        items.Add(item);

        if(onItemChangedCallback != null){
            onItemChangedCallback.Invoke();
        }
        print(onItemChangedCallback);

        return true;
    }

    public void Remove(Item item){
        items.Remove(item);

        if(onItemChangedCallback != null)   
            onItemChangedCallback.Invoke();
    }
}