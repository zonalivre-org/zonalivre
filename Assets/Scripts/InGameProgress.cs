using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InGameProgress : MonoBehaviour
{
    [SerializeField] private Slider clockSlider;
    [SerializeField] private TextMeshProUGUI clockNumber;
    [SerializeField] private int levelTime;
    [SerializeField] private int pointsToWin;
    private float currentTime;
    private int scoreProgress = 0;
    private bool win = false;
    private void Awake()
    {
        currentTime = levelTime;
        clockSlider.maxValue = levelTime;
        clockNumber.text = levelTime.ToString();
    }
    private void Update()
    {
        currentTime -= Time.deltaTime;
        clockSlider.value = currentTime;
        clockNumber.text = currentTime.ToString();
        if(currentTime <= 0)
        {
            if(win)
            {
                Debug.Log("Venceu! Parabens!");
            }
            else
            {
                Debug.Log("Oh não! Perdeu!");
            }
        }
    }
    public void AddScore(int score)
    {
        scoreProgress += score;
        if(scoreProgress >= pointsToWin) win = true;
    }
}
