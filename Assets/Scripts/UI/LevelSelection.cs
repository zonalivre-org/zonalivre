using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    public GameObject[] cutsceneButtons;
    public GameObject[] levelButtons;

    void Start()
    {
        // Initialize level buttons
        ShowUnlockedLevels();
        ShowUnlockedCutScenes();

    }

    public void ShowUnlockedLevels()
    {
        bool[] levelsCompleted = SaveManager.Instance.LoadGame().levelsUnlocked;

        // Show only unlocked levels
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelsCompleted[i])
            {
                levelButtons[i].GetComponent<ButtonAnimation>().SetClickable(true);
            }
            else
            {
                levelButtons[i].GetComponent<ButtonAnimation>().SetClickable(false);
            }
        }
    }

    public void ShowUnlockedCutScenes()
    {

        bool[] cutScenesUnlocked = SaveManager.Instance.LoadGame().cutScenesUnlocked;

        // Show only unlocked cutscenes
        for (int i = 0; i < cutsceneButtons.Length; i++)
        {
            cutsceneButtons[i].GetComponent<VideoButton>().cutSceneIndex = i;
            cutsceneButtons[i].GetComponent<VideoButton>().levelSelection = this;
            if (cutScenesUnlocked[i])
            {
                cutsceneButtons[i].GetComponent<ButtonAnimation>().SetClickable(true);
            }
            else
            {
                cutsceneButtons[i].GetComponent<ButtonAnimation>().SetClickable(false);
            }
        }
    }

}
