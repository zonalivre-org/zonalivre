using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    public void CompleteLevel()
    {
        GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
        gameManager.CompleteLevel();
    }
}
