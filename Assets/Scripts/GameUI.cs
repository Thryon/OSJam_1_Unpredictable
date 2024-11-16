using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _bannerRoundInfo;
    [SerializeField] private GameObject _tutorialInfo;
    [SerializeField] private GameObject _winScreen;

    private void Awake()
    {
        GlobalEvents.OnWin.AddListener(OnWin);
    }

    private void OnDestroy()
    {
        GlobalEvents.OnWin.RemoveListener(OnWin);
    }

    private void OnWin()
    {
        _winScreen.SetActive(true);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // public void DisableWinScreen(bool goToBufferPhase)
    // {
    //     _winScreen.SetActive(false);
    //     if (goToBufferPhase)
    //     {
    //         GameManager.Instance.GoToPhase(GameManager.EGamePhase.BufferInputs, GameManager.EGamePhase.Win);
    //     }
    // }

    public void Start()
    {
        GlobalEvents.OnInputBuffered.AddListener(DisableTutorialText);
        _winScreen.SetActive(false);
        _bannerRoundInfo.transform.localScale = Vector3.zero;
    }

    private void DisableTutorialText((int, InputManager.EInputType) arg0)
    {
        _tutorialInfo.GetComponent<CanvasGroup>()?.DOFade(0f, 0.5f);
    }
}
