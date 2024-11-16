using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class ShootFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _missile;
    [SerializeField] private ParticleSystem _explosion;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private ParticleSystem _missileObject;
    private ParticleSystem _explosionObject;
    
    public void Play(float duration)
    {
        _missileObject?.Play();
        _missileObject?.transform.DOMove(_endPos, 1f).SetEase(Ease.InCubic);
        StartCoroutine(WaitAndPlayExplosion(_missileObject, _explosionObject));
    }

    public void SetFromPoint(Vector3 from)
    {
        _startPos = from;
        _missileObject = Instantiate(_missile, from, quaternion.identity);
        _missileObject.Stop();
    }

    public void SetToPoint(Vector3 to)
    {
        _endPos = to;
        _explosionObject = Instantiate(_explosion, to, quaternion.identity);
        _explosionObject.Stop();
    }

    private IEnumerator WaitAndPlayExplosion (ParticleSystem missile, ParticleSystem explosion)
    {
        yield return new WaitForSeconds(1f);
        missile.Stop();
        explosion.Play();
    }
}