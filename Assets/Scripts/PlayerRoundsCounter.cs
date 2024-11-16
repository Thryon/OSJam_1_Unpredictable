using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRoundsCounter : MonoBehaviour
{
    public int playerID;
    public List<Image> roundImages = new();
    public Sprite uncheckedImage;
    public Sprite checkedImage;
    
    private void Awake()
    {
        GlobalEvents.OnRoundWin.AddListener(OnRoundWin);
        RefreshImages();
    }

    private void OnRoundWin()
    {
        RefreshImages();
    }

    private void RefreshImages()
    {
        int roundsWon = GameManager.Instance.GetPlayerWonRounds(playerID);
        for (int i = 0; i < roundImages.Count; i++)
        {
            roundImages[i].sprite = i < roundsWon ? checkedImage : uncheckedImage;
        }
    }
}
