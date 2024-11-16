using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterface : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [Header("References")]
    [SerializeField] private List<Image> _inputImageList;

    [Header("Sprites Ref")] 
    [SerializeField] private Sprite _emptySprite;
    [SerializeField] private Sprite _fullSprite;

    private int _currentEmptySprite = 0;

    private void Awake()
    {
        GlobalEvents.OnBufferPhaseDone.AddListener(OnBufferPhaseDone);
        GlobalEvents.OnBufferPhaseStarted.AddListener(OnBufferPhaseStarted);
        GlobalEvents.OnInputBuffered.AddListener(OnInputBuffered);
    }

    private void OnDestroy()
    {
        GlobalEvents.OnBufferPhaseDone.RemoveListener(OnBufferPhaseDone);
        GlobalEvents.OnBufferPhaseStarted.RemoveListener(OnBufferPhaseStarted);
        GlobalEvents.OnInputBuffered.RemoveListener(OnInputBuffered);
    }

    private void OnInputBuffered((int, InputManager.EInputType) inputInfo)
    {
        if (inputInfo.Item1 == playerID)
        {
            SetNextInputSprite();
        }
    }

    private void OnBufferPhaseStarted()
    {
        ResetSprites();
    }

    private void OnBufferPhaseDone()
    {
        
    }

    //Probably a better way to do this
    public void SetNextInputSprite()
    {
        _inputImageList[_currentEmptySprite].sprite = _fullSprite;
        _currentEmptySprite++;
    }

    //Reset each round ?
    public void ResetSprites()
    {
        _currentEmptySprite = 0;
        foreach (var image in _inputImageList)
        {
            image.sprite = _emptySprite;
        }
    }
}
