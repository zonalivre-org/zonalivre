using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonAnimation))]
public class ButtonAnimationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}