using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup.alpha = 0f;
        GlobalEvents.PauseToggled.AddListener(OnPauseToggled);
    }

    private void OnDestroy()
    {
        GlobalEvents.PauseToggled.RemoveListener(OnPauseToggled);
    }

    private void OnPauseToggled(bool paused)
    {
        canvasGroup.alpha = paused ? 1 : 0;
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.TogglePause();
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        GameManager.Instance.TogglePause();
    }
}
