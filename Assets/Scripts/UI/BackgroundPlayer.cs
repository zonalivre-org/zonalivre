using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BackgroundPlayer : MonoBehaviour
{
    public VideoPlayer _player;
    public string clipName;
    public RenderTexture renderTexture;

    private void Awake()
    {
        _player = GetComponent<VideoPlayer>();
        string clipPath = System.IO.Path.Combine(Application.streamingAssetsPath, clipName);
        _player.url = clipPath;
        _player.Play();
    }
}
