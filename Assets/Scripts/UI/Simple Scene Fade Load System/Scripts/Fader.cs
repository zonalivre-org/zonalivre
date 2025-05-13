using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour
{
    [HideInInspector]
    public bool start = false;
    [HideInInspector]
    public float fadeDamp = 0.0f;
    [HideInInspector]
    public string fadeScene;
    [HideInInspector]
    public float alpha = 0.0f;
    [HideInInspector]
    public Color fadeColor;
    [HideInInspector]
    public bool isFadeIn = false;
    CanvasGroup myCanvas;
    Image bg;
    bool startedLoading = false;
    //Set callback
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    //Remove callback
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitiateFader()
    {

        DontDestroyOnLoad(gameObject);

        //Getting the visual elements
        if (transform.GetComponent<CanvasGroup>())
            myCanvas = transform.GetComponent<CanvasGroup>();

        if (transform.GetComponentInChildren<Image>())
        {
            bg = transform.GetComponent<Image>();
            bg.color = fadeColor;
        }
        //Checking and starting the coroutine
        if (myCanvas && bg)
        {
            myCanvas.alpha = 0.0f;
            StartCoroutine(FadeIt());
        }
        else
            Debug.LogWarning("Something is missing please reimport the package.");
    }

    IEnumerator FadeIt()
    {
        while (!start)
        {
            yield return null;
        }

        float fadeDuration = 1f / fadeDamp; // Calculate fade duration based on fadeDamp
        float elapsedTime = 0f;
        bool hasFadedIn = false;

        while (!hasFadedIn)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time to make it independent of Time.deltaTime

            if (!isFadeIn)
            {
                // Fade in
                alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                if (alpha == 1 && !startedLoading)
                {
                    startedLoading = true;
                    DOTween.KillAll();
                    SceneManager.LoadScene(fadeScene);
                }
            }
            else
            {
                // Fade out
                alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
                if (alpha == 0)
                {
                    hasFadedIn = true;
                }
            }

            myCanvas.alpha = alpha;
            yield return null;
        }

        Initiate.DoneFading();
        Debug.Log("Your scene has been loaded, and fading in has just ended");
        Destroy(gameObject);
    }

    float newAlpha(float delta, int to, float currAlpha)
    {

        switch (to)
        {
            case 0:
                currAlpha -= fadeDamp * delta;
                if (currAlpha <= 0)
                    currAlpha = 0;

                break;
            case 1:
                currAlpha += fadeDamp * delta;
                if (currAlpha >= 1)
                    currAlpha = 1;

                break;
        }

        return currAlpha;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeIt());
        //We can now fade in
        isFadeIn = true;
    }
}
