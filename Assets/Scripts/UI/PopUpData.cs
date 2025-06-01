using UnityEngine;

[System.Serializable]
public class PopUpData
{
    public string title;
    [TextArea(3, 10)]
    public string description;
    public string videoFileName;
}