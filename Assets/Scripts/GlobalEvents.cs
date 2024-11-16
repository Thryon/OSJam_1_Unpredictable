using UnityEngine;
using UnityEngine.Events;

public static class GlobalEvents
{
    public class InputEvent : UnityEvent<(int,InputManager.EInputType)>{}
    public static InputEvent OnInputBuffered = new ();
    
    public static UnityEvent OnBufferPhaseStarted = new ();
    public static UnityEvent OnBufferPhaseDone = new ();
    public static UnityEvent<GameManager.EPlayPhaseResult> OnPlayPhaseDone = new ();

    public struct Movement
    {
        public int playerID;
        public Vector2Int from;
        public Vector2Int direction;
        public Vector2Int to;
    }
    public class MovementEvent : UnityEvent<Movement>{}
    public static MovementEvent OnPlayerMoved = new ();
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
