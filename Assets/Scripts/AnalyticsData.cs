using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AnalyticsData
{
    public float time; // Tempo decorrido de jogo
    public string sender; // Quem enviou (remetente)
    public string track; // Evento ou caminho que se quer rastrear
    public string val; // Valor que está guardando

    public AnalyticsData(float time, string sender, string track, string val)
    {
        this.time = time;
        this.sender = sender;
        this.track = track;
        this.val = val;
    }
}
[Serializable]
public class AnalyticsFile
{
    public AnalyticsData[] data;
}
