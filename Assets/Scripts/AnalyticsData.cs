using System;


[Serializable]
public class AnalyticsData
{
    public float time;
    public string sender;
    public string track;
    public string value;

    public AnalyticsData(float time, string sender, string track, string value)
    {
        this.time = time;
        this.sender = sender;
        this.track = track;
        this.value = value;
    }

    [Serializable]
    public class AnalyticsFile
    {
        public AnalyticsData[] data;
    }
}
