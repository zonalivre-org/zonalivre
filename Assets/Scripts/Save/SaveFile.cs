using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public bool[] levelsUnlocked = new bool[10];
    public bool[] levelsCompleted = new bool[10];
    public bool[] cutScenesUnlocked = new bool[10];
    public bool[] cutScenesWatched = new bool[10];
    public int currentLevel = 0;
}
