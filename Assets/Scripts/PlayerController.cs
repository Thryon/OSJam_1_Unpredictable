
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    public PlayerPawn pawn;
    public ShootFX shootFX;

    public void MoveTo(Vector3 transformPosition, float f)
    {
        transform.DOMove(transformPosition , 0.5f);
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
    }
}
