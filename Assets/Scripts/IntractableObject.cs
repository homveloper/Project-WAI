using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : InteractableScreen
{

    //초기화 함수
    public override void Initialize()
    {
        base.Initialize();

    }

    //상호 작용 함수
    public override void Interact(KeyCode _KeyCode)
    {
        if(_KeyCode == KeyCode.E){

        }
    }
}
