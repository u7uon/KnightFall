using System.Collections;
using TMPro;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }


    [SerializeField]private TextMeshProUGUI stageText;
    [SerializeField]private Animator transitionAnimator;


    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            stageText.enabled = false;
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    void OnDisable()
    {
 
    }

    // void OnDestroy()
    // {
    //     StopAllCoroutines() ;
    // }

    // /// <summary>
    // /// Hiệu ứng chuyển stage với text
    // /// </summary>
    // public void PlayStageTransition(string text)
    // {
    //     StartCoroutine(StageTransitionCoroutine(text));
    // }

    // IEnumerator StageTransitionCoroutine(string text)
    // {
    //     stageText.text = text;
    //     transitionAnimator.SetTrigger("Start");
    //     yield return new WaitForSeconds(0.5f);
    //     stageText.enabled = true;
    //     yield return new WaitForSeconds(2f);
    //     transitionAnimator.SetTrigger("End");
    //     stageText.enabled = false;

    // }

    // public void PlayFadeTransition()
    // {
    //     StartCoroutine(Fade());
    // }

    // IEnumerator Fade()
    // {
    //     transitionAnimator.SetTrigger("Start");
    //     yield return new WaitForSeconds(1f);
    //     transitionAnimator.SetTrigger("End");
    // }


    // public void FadeIn()
    // {
    //     transitionAnimator.SetTrigger("Start");
    // }

    // public void FadeOut()
    // {
    //     transitionAnimator.SetTrigger("End");
    // }



}   