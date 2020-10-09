using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Item", menuName = "Item")]

public class Item : ScriptableObject
{
    //when you wan't initialize use 'new'
    new public string name = "new Item";
    public int cost;

    [TextArea(15,20)]
    public string description;
    public Sprite icon;

    public virtual void Use(){
        Debug.Log("Using " + name);
    }
}
