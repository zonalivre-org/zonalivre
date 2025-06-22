using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.Net;

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

    public void AddAnalytics(float time, string sender, string track, string value, int sceneIndex)
    {
        AnalyticsData newData = new AnalyticsData(time, sender, track, value, sceneIndex);
        Debug.Log($"Adding analytics data: Sender: {newData.sender}, Time: {newData.time}, Track: {newData.track}, Value: {newData.value}");
        data.Add(newData);

        Save();
    }

    public void Save()
    {
        AnalyticsData.AnalyticsFile file = new AnalyticsData.AnalyticsFile();
        file.data = data.ToArray();

        string json = JsonUtility.ToJson(file, true);
        SaveJsonFile(json);
    }

    private void SaveJsonFile(string jsonContent)
    {
        // Get playerName from SaveFile
        SaveFile saveFile = SaveManager.Instance.LoadGame();
        string playerName = saveFile.playerName;

        // Format date and time
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        // Create filename
        string fileName = $"{playerName}_{timestamp}.json";

        // Save JSON file
        string path = System.IO.Path.Combine(Application.dataPath, fileName);
        Debug.Log("Saving analytics data to " + path);
        System.IO.File.WriteAllText(path, jsonContent);
    }

    public void SendMail()
    {
        AnalyticsData.AnalyticsFile file = new AnalyticsData.AnalyticsFile();
        file.data = data.ToArray();

        string json = JsonUtility.ToJson(file, true);

        // Get playerName and timestamp for filename
        SaveFile saveFile = SaveManager.Instance.LoadGame();
        string playerName = saveFile.playerName;
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{playerName}_{timestamp}.json";
        string path = System.IO.Path.Combine(Application.dataPath, fileName);

        // Save the JSON file
        System.IO.File.WriteAllText(path, json);

        // Send the file as attachment
        Mail(path, fileName);
    }

    private void Mail(string filePath, string fileName)
    {
        string password = "uzrnhxxvwsnsunxb";
        string mail = "gabriellogand@gmail.com";
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(mail, password),
            EnableSsl = true
        };

        SaveFile saveFile = SaveManager.Instance.LoadGame();

        MailMessage message = new MailMessage(mail, mail);
        message.Subject = saveFile.playerName + ": Zona Livre - Analytics";
        message.Body = "Analytics data attached as JSON file.";
        message.Attachments.Add(new Attachment(filePath));

        client.Send(message);
        Debug.Log("!!!!! Mail with attachment sent !!!!!");
    }
}
