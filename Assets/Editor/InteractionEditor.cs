using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

[CustomEditor(typeof(Interaction))]
class InteractionEditor : Editor
{
    public override void OnInspectorGUI(){
        // Interaction interaction = (Interaction)target;

        if(GUILayout.Button("test")){
        }
    }
}