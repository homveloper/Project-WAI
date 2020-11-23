using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public void SetMove(int value){
        GetComponentInParent<Player>().SetMove(value == 1);
    }
}
