using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Interaction{
    [SerializeField]
    String key = "";

    [SerializeField]
    KeyCode keyCode;

    [SerializeField]
    string description;

    [SerializeField]
    Color color;

    public String Key{
        get=>key;
    }

    public KeyCode _KeyCode{
        get=>keyCode;
    }

    public String Description{
        get=>description;
        set=>description = value;
    }

    public Color _Color{
        get=>color;
        set=>color = value;
    }
}