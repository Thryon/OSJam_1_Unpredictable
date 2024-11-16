using System;
using DG.Tweening;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject _bannerRoundInfo;
    
    public void Start()
    {
        _bannerRoundInfo.transform.localScale = Vector3.zero;
    }
}
