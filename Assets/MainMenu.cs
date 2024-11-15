using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _playButton.onClick.AddListener(PlayButtonPressed);
        _quitButton.onClick.AddListener(QuitButtonPressed);
    }

    private void PlayButtonPressed()
    {
        //StartGame
    }

    private void QuitButtonPressed()
    {
        Application.Quit();
    }
}
