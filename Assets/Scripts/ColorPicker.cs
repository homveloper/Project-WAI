using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ColorPicker : MonoBehaviour
{
    public Color red;
    public List<Color> colors;
}

[CustomEditor(typeof(ColorPicker))]
class ColorPickerEditor : Editor
{
    public override void OnInspectorGUI(){
        ColorPicker colorPicker = (ColorPicker)target;

        if(colorPicker != null) return;

        if(GUILayout.Button("Color")){
            
        }
    }
}