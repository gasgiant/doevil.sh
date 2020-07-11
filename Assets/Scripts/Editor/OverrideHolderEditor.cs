using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OverrideHolder))]
public class OverrideHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        OverrideHolder holder = target as OverrideHolder;
        holder.Validate();
        serializedObject.ApplyModifiedProperties();
    }
}
