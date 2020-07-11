using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.Update();
        Tile tile = target as Tile;
        tile.Validate();
        serializedObject.ApplyModifiedProperties();
    }
}
