using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectiveInteract))]
public class MiniGameSelector : Editor 
{
    SerializedProperty miniGameType;
    
    void OnEnable()
    {
        miniGameType = serializedObject.FindProperty("miniGameType");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Draw all properties except the ones you want to customize
        DrawPropertiesExcluding(serializedObject, "miniGameType", "mangoGoal", "mangoFallSpeed", "coolDownBetweenMangos",
         "QTEGoal", "QTEMoveSpeed", "QTESafeZoneSizePercentage", "cleanSpeed", "trashAmount",
         "scoreToWin", "spawnInterval", "growthSpeed", "maxDirtCount", "dogCleanSpeed", "moskitoGoal", "moskitoSpeed", "moskitoSpawnInterval");

        // Draw the custom property field for miniGameType
        EditorGUILayout.PropertyField(miniGameType);

        // Custom logic for specific mini-game settings
        if (miniGameType.enumValueIndex == 0) // Mango Catch
        {
            EditorGUILayout.LabelField("Mango Catch Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mangoGoal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mangoFallSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("coolDownBetweenMangos"));
        }
        else if (miniGameType.enumValueIndex == 1) // Quick Time Event
        {
            EditorGUILayout.LabelField("Quick Time Event Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("QTEGoal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("QTEMoveSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("QTESafeZoneSizePercentage"));
        }
        else if (miniGameType.enumValueIndex == 2) // Clean Minigame
        {
            EditorGUILayout.LabelField("Clean Minigame Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cleanSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trashAmount"));
        }
        else if (miniGameType.enumValueIndex == 3) // Whack A Mole
        {
            EditorGUILayout.LabelField("Whack A Mole Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreToWin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("spawnInterval"));
        }
        else if (miniGameType.enumValueIndex == 4) // Plant The Citronela
        {
            EditorGUILayout.LabelField("Plant The Citronela Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("growthSpeed"));
        }
        else if (miniGameType.enumValueIndex == 5) // Dog Clean MiniGame
        {
            EditorGUILayout.LabelField("Dog Clean MiniGame Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDirtCount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cleanSpeed"));
        }
        else if (miniGameType.enumValueIndex == 6) // Moskito Slayer
        {
            EditorGUILayout.LabelField("Moskito Slayer Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moskitoGoal"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moskitoSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moskitoSpawnInterval"));
        }

        // Apply any modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
