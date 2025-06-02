using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.IO;
public class AnalyticsTest : MonoBehaviour
{
    public List<AnalyticsData> data;
    public static AnalyticsTest instance {  get; private set; }
    private void Awake()
    {
        instance = this;
        data = new List<AnalyticsData>();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S)) { 
        
        }   
    }

    public void AddAnalytics(string sender, string track, string val)
    {
        AnalyticsData d = new AnalyticsData(Time.time, sender, track, val);
        Debug.Log("Sender: " + d.sender + ", Track: " + d.track + ", Value: " + d.val);
        data.Add(d);
    }

    public void Save()
    {
        AnalyticsFile f = new AnalyticsFile();
        f.data = data.ToArray();
        string json = JsonUtility.ToJson(f, true);
        SaveFile(json);
        //SendEmail(json);
    }

    void SaveFile(string text)
    {
        string path = Application.dataPath + "/analytics.txt";
        Debug.Log("Arquivo salvo em: " + path);
        File.WriteAllText(path, text);
    }

    //void SendEmail(string text)
    //{
    //    var client = new SmtpClient();

    //    client.Send()

    //    Debug.Log("Email enviado");
    //}
}
