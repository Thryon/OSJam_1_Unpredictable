using System;
using DG.Tweening;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _bannerRoundInfo;
    [SerializeField] private GameObject _tutorialInfo;
    
    public void Start()
    {
        GlobalEvents.OnInputBuffered.AddListener(DisableTutorialText);
        _bannerRoundInfo.transform.localScale = Vector3.zero;
    }

    private void DisableTutorialText((int, InputManager.EInputType) arg0)
    {
        _tutorialInfo.GetComponent<CanvasGroup>()?.DOFade(0f, 0.5f);
    }
}
