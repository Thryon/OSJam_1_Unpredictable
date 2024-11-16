using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Image> _inputImageList;

    [Header("Sprites Ref")] 
    [SerializeField] private Sprite _emptySprite;
    [SerializeField] private Sprite _fullSprite;

    private int _currentEmptySprite = 0;

    //Probably a better way to do this
    public void SetNextInputSprite()
    {
        _inputImageList[_currentEmptySprite].sprite = _fullSprite;
        _currentEmptySprite++;
    }

    //Reset each round ?
    public void ResetSprites()
    {
        foreach (var image in _inputImageList)
        {
            image.sprite = _emptySprite;
        }
    }
}
