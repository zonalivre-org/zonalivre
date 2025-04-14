using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    private void Awake() => Time.timeScale = 1;

    public void PlaySceneMusic(int index)
    {
        SoundManager.Instance.PlayMusicWithFade(index, 2f);
    }

    public void ChangeToSceneMusic(int index)
    {
        SoundManager.Instance.ChangeMusicWithFade(index, 0.5f);
    }

    public void StopSceneMusic()
    {
        SoundManager.Instance.StopMusicWithFade(1.25f);
    }

    public void LoadSceneDelay(int index)
    {
        string sceneIndex = GetSceneNameByIndex(index);

        Debug.Log("Loading scene: " + sceneIndex);

        Initiate.Fade(sceneIndex, Color.black, 1f); // Initiate a fade effect

    }
    
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadLastScene()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    private string GetSceneNameByIndex(int index)
    {
        // Get the full path of the scene using the build index
        string scenePath = SceneUtility.GetScenePathByBuildIndex(index);

        // Extract the scene name from the path
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        return sceneName;
    }
}