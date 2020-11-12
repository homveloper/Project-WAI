using UnityEditor;

[CustomEditor(typeof(Resource))]
 public class ResourceEditor : Editor
 {
    //  SerializedObject sObj;
 
    //  void OnEnable()
    //  {
    //      this.sObj = new SerializedObject(target);
    //  }
    //  public override void OnInspectorGUI()
    //  {
    //      this.sObj.Update();

    //      SerializedProperty wood = sObj.FindProperty("wood");
    //      wood.intValue = EditorGUILayout.IntField("wood", wood.intValue);
 
    //      SerializedProperty iron = sObj.FindProperty("iron");
    //      iron.intValue = EditorGUILayout.IntField("iron", iron.intValue);
 
    //      SerializedProperty part = sObj.FindProperty("part");
    //      part.intValue = EditorGUILayout.IntField("part", part.intValue);
 
    //      this.sObj.ApplyModifiedProperties();
    //  }
 }