using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Mail;
using System.Net;

public class Analytics : MonoBehaviour
{
    public List<AnalyticsData> data;
    public static Analytics Instance;

    private void Start()
    {
        InvokeRepeating(nameof(Save), 1, 10);
    }

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
        SaveFile(json);
        SendToMail("                             ...,?77??!~~~~!???77?<~.... \r\n                        ..?7`                           `7!.. \r\n                    .,=`          ..~7^`   I                  ?1. \r\n       ........  ..^            ?`  ..?7!1 .               ...??7 \r\n      .        .7`        .,777.. .I.    . .!          .,7! \r\n      ..     .?         .^      .l   ?i. . .`       .,^ \r\n       b    .!        .= .?7???7~.     .>r .      .= \r\n       .,.?4         , .^         1        `     4... \r\n        J   ^         ,            5       `         ?<. \r\n       .%.7;         .`     .,     .;                   .=. \r\n       .+^ .,       .%      MML     F       .,             ?, \r\n        P   ,,      J      .MMN     F        6               4. \r\n        l    d,    ,       .MMM!   .t        ..               ,, \r\n        ,    JMa..`         MMM`   .         .!                .; \r\n         r   .M#            .M#   .%  .      .~                 ., \r\n       dMMMNJ..!                 .P7!  .>    .         .         ,, \r\n       .WMMMMMm  ?^..       ..,?! ..    ..   ,  Z7`        `?^..  ,, \r\n          ?THB3       ?77?!        .Yr  .   .!   ?,              ?^C \r\n            ?,                   .,^.` .%  .^      5. \r\n              7,          .....?7     .^  ,`        ?. \r\n                `<.                 .= .`'           1 \r\n                ....dn... ... ...,7..J=!7,           ., \r\n             ..=     G.,7  ..,o..  .?    J.           F \r\n           .J.  .^ ,,,t  ,^        ?^.  .^  `?~.      F \r\n          r %J. $    5r J             ,r.1      .=.  .% \r\n          r .77=?4.    ``,     l ., 1  .. <.       4., \r\n          .$..    .X..   .n..  ., J. r .`  J.       `' \r\n        .?`  .5        `` .%   .% .' L.'    t \r\n        ,. ..1JL          .,   J .$.?`      . \r\n                1.          .=` ` .J7??7<.. .; \r\n                 JS..    ..^      L        7.: \r\n                   `> ..       J.  4. \r\n                    +   r `t   r ~=..G. \r\n                    =   $  ,.  J \r\n                    2   r   t  .; \r\n              .,7!  r   t`7~..  j.. \r\n              j   7~L...$=.?7r   r ;?1. \r\n               8.      .=    j ..,^   .. \r\n              r        G              . \r\n            .,7,        j,           .>=. \r\n         .J??,  `T....... %             .. \r\n      ..^     <.  ~.    ,.             .D \r\n    .?`        1   L     .7.........?Ti..l \r\n   ,`           L  .    .%    .`!       `j, \r\n .^             .  ..   .`   .^  .?7!?7+. 1 \r\n.`              .  .`..`7.  .^  ,`      .i.; \r\n.7<..........~<<3?7!`    4. r  `          G% \r\n                          J.` .!           % \r\n                            JiJ           .` \r\n                              .1.         J \r\n                                 ?1.     .'         \r\n                                     7<..%");
    }

    private void SaveFile(string text)
    {
        string path = Application.dataPath + "/analytics.txt";
        Debug.Log("Saving analytics data to " + path);
        System.IO.File.WriteAllText(path, text);
    }

    public void SendToMail(string s)
    {
        string password = "uzrnhxxvwsnsunxb";
        string mail = "gabriellogand@gmail.com";
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(mail, password),
            EnableSsl = true
        };
        client.Send("ecparreiras@sga.pucminas.br", "ecparreiras@sga.pucminas.br", "Sonic",s);
        Debug.Log("!!!!! Mail sent !!!!!");

    }

    private void OnApplicationQuit()
    {
        AnalyticsData.AnalyticsFile file = new AnalyticsData.AnalyticsFile();
        file.data = data.ToArray();

        string json = JsonUtility.ToJson(file, true);

        SendToMail(json);
    }
}
