using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public bool firstTime = false;
    public bool[] levelsUnlocked = new bool[10];
    public bool[] levelsCompleted = new bool[10];
    public bool[] cutScenesUnlocked = new bool[10];
    public bool[] cutScenesWatched = new bool[10];
    public Vector3 spawnPosition;
}
