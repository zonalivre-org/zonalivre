using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region ChangeSaveValues
    public void SetLevelCompletion(int levelIndex, bool isCompleted)
    {
        SaveFile saveFile = LoadGame() ?? new SaveFile();
        saveFile.levelsCompleted[levelIndex] = isCompleted;
        SaveGame(saveFile);
    }

    public void SetLevelLock(int levelIndex, bool isUnlocked)
    {
        SaveFile saveFile = LoadGame() ?? new SaveFile();
        saveFile.levelsUnlocked[levelIndex] = isUnlocked;
        SaveGame(saveFile);
    }

    public void SetCutSceneCompletion(int cutSceneIndex, bool isWatched)
    {
        SaveFile saveFile = LoadGame() ?? new SaveFile();
        saveFile.cutScenesWatched[cutSceneIndex] = isWatched;
        SaveGame(saveFile);
    }

    public void SetCutSceneLock(int cutSceneIndex, bool isUnlocked)
    {
        SaveFile saveFile = LoadGame() ?? new SaveFile();
        saveFile.cutScenesUnlocked[cutSceneIndex] = isUnlocked;
        SaveGame(saveFile);
    }

    public void SetSpawnPosition(Vector3 position)
    {
        SaveFile saveFile = LoadGame() ?? new SaveFile();
        saveFile.spawnPosition = position;
        SaveGame(saveFile);
    }

    public void ToggleFirstTime(bool isFirstTime)
    {
        SaveFile saveFile = LoadGame() ?? new SaveFile();
        saveFile.firstTime = isFirstTime;
        SaveGame(saveFile);
    }

    #endregion

    #region Save and Load Methods
    public void SaveGame(SaveFile saveFile)
    {
        string json = JsonUtility.ToJson(saveFile);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Game saved to " + Application.persistentDataPath + "/savefile.json");
    }

    public SaveFile LoadGame()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            SaveFile saveFile = JsonUtility.FromJson<SaveFile>(json);
            Debug.Log("Game loaded from " + path);
            return saveFile;
        }
        else
        {
            Debug.LogWarning("Save file not found at " + path + ". Creating a new save file.");
            return CreateSaveFile();
        }
    }

    public SaveFile CreateSaveFile()
    {
        SaveFile newSaveFile = new SaveFile();
        newSaveFile.cutScenesUnlocked[0] = true; // Unlock the first cutscene by default
        newSaveFile.levelsUnlocked[0] = true; // Unlock the first level by default
        newSaveFile.firstTime = true; // Indicate that this is the first time playing
        newSaveFile.spawnPosition = new Vector3(-350, 0, 6.75f);
        SaveGame(newSaveFile);
        return newSaveFile;
    }

    #endregion
}
