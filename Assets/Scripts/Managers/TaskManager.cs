using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;
    public List<ObjectiveInteract> objectives;
    [SerializeField] private List<TaskItem> taskList;
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
        foreach (TaskItem task in taskList)
        {
            Destroy(task.gameObject);
        }

        taskList.Clear();

        // Add the new ones :D
        foreach (ObjectiveInteract task in objectives)
        {
            GameObject newTask = Instantiate(taskPrefab, transform);
            newTask.name = task.gameObject.name + " Task";

            newTask.GetComponent<TaskItem>().SetTaskDescription(task.objectiveDescription);
            newTask.GetComponent<TaskItem>().SetAverageTime(task.averageTimeToComplete);

            newTask.transform.localScale = Vector3.one;

            newTask.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            newTask.GetComponent<RectTransform>().localRotation = Quaternion.identity;

            taskList.Add(newTask.GetComponent<TaskItem>());
        }

        SortByShortestTime(taskList);
    }

    public List<TaskItem> GetTaskList()
    {
        return taskList;
    }

    private void SortByShortestTime(List<TaskItem> levelTasks)
    {
        // Insertion sort algorithm
        // This is a simple and efficient algorithm for small lists, perfect for our needs!
        
        TaskItem taskToInsert;
        int HelperIndex;
        int listLenght = levelTasks.Count;
        for (int i = 1; i < listLenght; i++)
        {
            taskToInsert = levelTasks[i];
            HelperIndex = i - 1;
            while (HelperIndex >= 0 && levelTasks[HelperIndex].GetAverageTime() > taskToInsert.GetAverageTime())
            {
                levelTasks[HelperIndex + 1] = levelTasks[HelperIndex];
                HelperIndex--;
            }
            levelTasks[HelperIndex + 1] = taskToInsert;
        }
    }
}
