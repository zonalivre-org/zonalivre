using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VolumeSlider))]
public class VolumeSliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}