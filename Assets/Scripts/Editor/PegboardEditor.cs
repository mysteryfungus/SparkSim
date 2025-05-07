using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PegboardCreator))]
public class PegboardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PegboardCreator data = (PegboardCreator)target;

        base.OnInspectorGUI();

        if(data.size.x <= 0 || data.size.y <= 0)
        {
            EditorGUILayout.HelpBox("Size must be greater than 0", MessageType.Error);
        }

        if(GUILayout.Button("Create"))
        {
            data.Create(data.size);
        }
    }
}
