using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analytics : MonoBehaviour
{
    public List<AnalyticsData> data;
    public static Analytics Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            data = new List<AnalyticsData>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }

    public void AddAnalytics(float time, string sender, string track, string value)
    {
        AnalyticsData newData = new AnalyticsData(time, sender, track, value);
        Debug.Log($"Adding analytics data: Sender: {newData.sender}, Time: {newData.time}, Track: {newData.track}, Value: {newData.value}");
        data.Add(newData);

        Save();
    }

    public void Save()
    {
        AnalyticsData.AnalyticsFile file = new AnalyticsData.AnalyticsFile();
        file.data = data.ToArray();

        string json = JsonUtility.ToJson(file, true);
        SaveFile(json);
    }

    private void SaveFile(string text)
    {
        string path = Application.dataPath + "/analytics.txt";
        Debug.Log("Saving analytics data to " + path);
        System.IO.File.WriteAllText(path, text);
    }
}
