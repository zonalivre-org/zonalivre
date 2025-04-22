using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public List<ObjectiveInteract> objectives;
    [SerializeField] private List<GameObject> taskList;
    [SerializeField] private GameObject taskPrefab;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        Debug.Log("Updating Task List");


        // Cleating the old task list items
        foreach (GameObject task in taskList)
        {
            Destroy(task);
        }

        taskList.Clear();

        // Add the new ones :D
        foreach (ObjectiveInteract task in objectives)
        {
            GameObject newTask = Instantiate(taskPrefab, transform);
            newTask.name = task.gameObject.name + " Task";
            newTask.GetComponentInChildren<TMP_Text>().text = task.objectiveDescription;
            newTask.transform.localScale = Vector3.one;
            newTask.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            newTask.GetComponent<RectTransform>().localRotation = Quaternion.identity;
            taskList.Add(newTask);
        }

        SortByShortestTime();
    }

    public List<GameObject> GetTaskList()
    {
        return taskList;
    }

    private void SortByShortestTime()
    {
        // Bones, this is where you write the sort algorithm!!! :OOOO
    }
}
