using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerID = 0;
    public PlayerPawn pawn;

    public void MoveTo(Vector3 transformPosition, float f)
    {
        transform.DOMove(transformPosition , 0.5f);
    }

    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
}
