using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _bannerRoundInfo;
    [SerializeField] private GameObject _tutorialInfo;
    [SerializeField] private WinScreen _winScreen;
    [SerializeField] private TextMeshProUGUI _textTimer;

    private GameManager _gameManager;
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
        _winScreen.gameObject.SetActive(true);
        _winScreen.Setup(GameManager.Instance.LastPlayPhaseResult);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        int timer = Mathf.CeilToInt(_gameManager.bufferPhaseDuration) - Mathf.CeilToInt(_gameManager.PhaseTimer);
        if (_gameManager.currentGamePhase == GameManager.EGamePhase.BufferInputs)
        {
            _textTimer.text = timer.ToString();
        }
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
        _winScreen.gameObject.SetActive(false);
        _bannerRoundInfo.transform.localScale = Vector3.zero;
        _gameManager = GameManager.Instance;
        GlobalEvents.OnBufferPhaseDone.AddListener(HideTimer);
    }

    private void HideTimer()
    {
        _textTimer.GetComponentInParent<Image>()?.DOFade(0f, 0.5f);
        _textTimer.DOFade(0f, 0.5f);
    }

    private void DisableTutorialText((int, InputManager.EInputType) arg0)
    {
        _tutorialInfo.GetComponent<CanvasGroup>()?.DOFade(0f, 0.5f);
    }
}
