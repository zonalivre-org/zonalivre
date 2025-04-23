using TMPro;
using UnityEngine;

public class TaskItem : MonoBehaviour
{
    private string taskDescription;
    private float averageTime;
    [SerializeField] private TMP_Text textObject;

    public void SetTaskDescription(string description)
    {
        taskDescription = description;
        textObject.text = taskDescription;
    }

    public void SetAverageTime(float time)
    {
        averageTime = time;
        textObject.text = $"{taskDescription} - {time}";
    }

    public float GetAverageTime()
    {
        return averageTime;
    }
}
