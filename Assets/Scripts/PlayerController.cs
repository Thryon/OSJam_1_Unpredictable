
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    public PlayerPawn pawn;
    public ShootFX shootFX;
    public float shootDuration = 0.5f;
    public float deathDuration = 0.5f;
    public Transform shootPos;

    public Animator animator;

    public void MoveTo(Vector3 transformPosition, float f)
    {
        transform.LookAt(transformPosition);
        transform.DOMove(transformPosition , 0.5f).SetEase(Ease.Linear);
        animator.SetTrigger("Walk");
    }

    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }

    public void Shoot(GridManager.ShootResult shot)
    {
        shootFX?.gameObject.SetActive(true);
        var from = GridManager.Instance.GetCellWorldPosition(shot.shootStartPos);
        var to = GridManager.Instance.GetCellWorldPosition(shot.hitPosition);
        shootFX?.SetFromPoint(shootPos.position);
        shootFX?.SetToPoint(to);
        Vector3 dir = to - from;
        transform.LookAt(transform.position + dir);
        animator.SetTrigger("Shoot");
        shootFX?.Play(shootDuration);
    }

    public float GetShootDuration()
    {
        return shootDuration;
    }

    public float GetDeathDuration()
    {
        return deathDuration;
    }

    public void Kill()
    {
        animator.SetTrigger("Death");
    }

    public void Reset()
    {
        animator.Rebind();
    }
}
