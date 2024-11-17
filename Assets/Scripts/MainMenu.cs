using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Image _imagePreview;

    [Header("Level Preview")] 
    [SerializeField] private Image _previewLvl1;
    [SerializeField] private Image _previewLvl2;
    [SerializeField] private Image _previewLvl3;

    [Header("SceneSelected")] 
    [SerializeField] private int _buildIndexFirstLevel;
    [SerializeField] private int _buildIndexSecondLevel;
    [SerializeField] private int _buildIndexThirdLevel;

    private int _indexOfPlaySceneSelected;

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

    public void SliderChangeLevelPreview(Single index)
    {
        switch (index)
        {
            case 0:
                _imagePreview = _previewLvl1;
                _indexOfPlaySceneSelected = _buildIndexFirstLevel;
                break;
            case 1:
                _imagePreview = _previewLvl2;
                _indexOfPlaySceneSelected = _buildIndexSecondLevel;
                break;
            case 2:
                _imagePreview = _previewLvl3;
                _indexOfPlaySceneSelected = _buildIndexThirdLevel;
                break;
        }
    }
}
