using UnityEngine;
using UnityEngine.SceneManagement;
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
        SceneManager.LoadSceneAsync("Scenes/SampleScene");
    }

    private void QuitButtonPressed()
    {
        Application.Quit();
    }
}
