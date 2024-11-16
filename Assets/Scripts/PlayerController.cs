
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    public PlayerPawn pawn;
    public ShootFX shootFX;

    public Animator animator;

    public void MoveTo(Vector3 transformPosition, float f)
    {
        transform.LookAt(transformPosition);
        transform.DOMove(transformPosition , 0.5f);
        animator.SetTrigger("Walk");
    }

    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }

    public void Shoot(GlobalEvents.Shot shot, float duration)
    {
        shootFX.gameObject.SetActive(true);
        var from = GridManager.Instance.GetCellWorldPosition(shot.from);
        var to = GridManager.Instance.GetCellWorldPosition(shot.to);
        shootFX.SetFromPoint(from);
        shootFX.SetToPoint(to);
        shootFX.Play(duration);
        animator.SetTrigger("Shoot");
    }
}
