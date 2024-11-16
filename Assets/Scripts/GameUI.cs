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

    public void AnimateBanner()
    {
        _bannerRoundInfo.transform.DOScale(Vector3.one, 1f);
    }
}
