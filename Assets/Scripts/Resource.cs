using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class Resource
{
    int wood;
    int iron;
    int part;

    public int Wood
    {
        get {return wood;}
        set {wood = value;}
    }

    public int Iron
    {
        get {return Iron;}
        set {Iron = value;}
    }

    public int Part
    {
        get {return part;}
        set {part = value;}
    }
}
