using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _bannerRoundInfo;
    [SerializeField] private TextMeshProUGUI _textRound;
    [SerializeField] private GameObject _tutorialInfo;
    [FormerlySerializedAs("_winScreen")] [SerializeField] private WinScreen gameWinScreen;
    [SerializeField] private WinScreen roundWinScreen;
    [SerializeField] private TextMeshProUGUI _textTimer;

    private GameManager _gameManager;
    private int _roundNumber = 1;
    
    private void Awake()
    {
        GlobalEvents.OnGameWin.AddListener(OnWin);
        GlobalEvents.OnRoundWin.AddListener(OnRoundWin);
        GlobalEvents.ResetForNewRound.AddListener(ResetForNewRound);
        _textRound.text = "Round N°1";
    }

    private void OnDestroy()
    {
        GlobalEvents.OnGameWin.RemoveListener(OnWin);
        GlobalEvents.OnRoundWin.RemoveListener(OnRoundWin);
        GlobalEvents.ResetForNewRound.RemoveListener(ResetForNewRound);
        GlobalEvents.OnBufferPhaseStarted.RemoveListener(ShowTimer);
    }

    private void ResetForNewRound()
    {
        roundWinScreen.gameObject.SetActive(false);
        _roundNumber++;
        _textRound.text = ("Round N°" + _roundNumber);
    }

    private void OnRoundWin()
    {
        roundWinScreen.gameObject.SetActive(true);
        roundWinScreen.Setup(GameManager.Instance.LastPlayPhaseResult);
    }

    private void OnWin()
    {
        gameWinScreen.gameObject.SetActive(true);
        gameWinScreen.Setup(GameManager.Instance.EndResult);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        _roundNumber = 1;
    }

    public void GoToMainMenu()
    {
        //Go to main menu
        SceneManager.LoadScene(0);
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
        gameWinScreen.gameObject.SetActive(false);
        roundWinScreen.gameObject.SetActive(false);
        
        _bannerRoundInfo.transform.localScale = Vector3.zero;
        _gameManager = GameManager.Instance;
        GlobalEvents.OnBufferPhaseDone.AddListener(HideTimer);
        GlobalEvents.OnBufferPhaseStarted.AddListener(ShowTimer);
    }

    private void HideTimer()
    {
        _textTimer.GetComponentInParent<Image>()?.DOFade(0f, 0.5f);
        _textTimer.DOFade(0f, 0.5f);
    }

    private void ShowTimer()
    {
        _textTimer.GetComponentInParent<Image>()?.DOFade(1f, 0.5f);
        _textTimer.DOFade(1f, 0.5f);
    }

    private void DisableTutorialText((int, InputManager.EInputType) arg0)
    {
        _tutorialInfo.GetComponent<CanvasGroup>()?.DOFade(0f, 0.5f);
    }
}
