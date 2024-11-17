using UnityEngine;
using UnityEngine.Events;

public static class GlobalEvents
{
    public class InputEvent : UnityEvent<(int,InputManager.EInputType)>{}
    public static InputEvent OnInputBuffered = new ();
    
    public static UnityEvent OnBufferPhaseStarted = new ();
    public static UnityEvent OnBufferPhaseDone = new ();
    public static UnityEvent OnPlayPhaseStarted = new ();
    public static UnityEvent<GameManager.EPlayPhaseResult> OnPlayPhaseDone = new ();
    public static UnityEvent OnRoundWin = new ();
    public static UnityEvent OnGameWin = new ();
    public static UnityEvent ResetForNewRound = new ();
    public static UnityEvent OnNewGameLoopIteration = new ();

    public class MovementEvent : UnityEvent<GridManager.MoveResult>{}
    public static MovementEvent OnPlayerMoved = new ();
    public static MovementEvent OnPlayerFailedToMove = new ();
    public struct Shot
    {
        public int playerID;
        public Vector2Int from;
        public Vector2Int direction;
        public Vector2Int to;
        public GridManager.ShootResult result;
    }
    public class ShotEvent : UnityEvent<Shot>{}
    public static ShotEvent OnPlayerShot = new ();
}
