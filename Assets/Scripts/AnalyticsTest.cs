using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.Net;
using System.IO;


public class AnalyticsTest : MonoBehaviour
{
    public List<AnalyticsData> data;
    public static AnalyticsTest Instance { get; private set; }
    void Awake()
    {
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            
            Save();
        }


    }

    private void AddAnalytics(string sender, string track, string value)
    {
        AnalyticsData data = new AnalyticsData(Time.time, sender, track, value);
        Debug.Log("Send: " + data.sender + ", Track: " + data.track + ", Value: " + data.value);
    }

    private void Save()
    {
        AnalyticsFile analyticsFile = new AnalyticsFile();
        analyticsFile.data = data.ToArray();
        string json = JsonUtility.ToJson(analyticsFile, true);
        SaveFile(json);
        SendEmail(json);

    }

    void SaveFile(string text)
    {
        string path = Application.dataPath + "/analytics.txt";
        Debug.Log("Arquivo salvo em:" + path);
        File.WriteAllText(path, text);
    }

    void SendEmail(string text)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("eric.parreiras@sga.pucminas.br", "senha"),
            EnableSsl = true
        };
        client.Send("remetente@gmail.com", "destinatario@gmail.com", "Assunto", text);
        Debug.Log("text");
    }
    




}




