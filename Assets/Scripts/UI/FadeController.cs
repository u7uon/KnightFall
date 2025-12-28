using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    
    [Header("Fade Settings")]
    public Image fadeImage; // Image đen để fade
    public float fadeDuration = 1.5f; // Thời gian fade (giây)
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Fade in khi bắt đầu scene
        StartCoroutine(FadeIn());
    }
    
    // Fade Out rồi chuyển scene
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }
    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(FadeOutAndLoad(sceneIndex));
    }
    
    // Coroutine Fade In (từ đen sang trong suốt)
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = 0f;
        fadeImage.color = color;
    }
    
    // Coroutine Fade Out rồi load scene
    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        
        // Fade out (từ trong suốt sang đen)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = 1f;
        fadeImage.color = color;
        
        // Load scene
        SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator FadeOutAndLoad(int sceneIndex)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        
        color.a = 1f;
        fadeImage.color = color;
        
        SceneManager.LoadScene(sceneIndex);
    }
}