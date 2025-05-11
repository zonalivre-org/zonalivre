using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
    private string taskDescription;
    private float averageTime;
    [SerializeField] private Image checkIcon;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text textObject;
    [SerializeField] private GameObject stroke;

    public void SetTaskDescription(string description)
    {
        taskDescription = description;
        textObject.text = taskDescription;
    }

    public void SetAverageTime(float time)
    {
        averageTime = time;
        textObject.text = $"{taskDescription}"; // - {time}
    }

    public float GetAverageTime()
    {
        return averageTime;
    }

    public void MarkAsComplete()
    {
        checkIcon.color = Color.green;
        stroke.SetActive(true);
        stroke.GetComponent<Animator>().Play("Chalk stroke");
    }

    public void SetIcon(Sprite iconSprite)
    {
        icon.sprite = iconSprite;
    }
}
