using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    public List<PlayerController> players = new();
    public float bufferPhaseDuration = 15f;
    public enum EGamePhase
    {
        BufferInputs,
        Play
    }
    
    public EGamePhase currentGamePhase;
    private float phaseTimer = 0f;

    private void Awake()
    {
        if(!instance)
            instance = this;
        GlobalEvents.OnInputBuffered.AddListener(OnInputBuffered);
    }

    private void OnInputBuffered((int, InputManager.EInputType) arg0)
    {
        if(!IsInPhase(EGamePhase.BufferInputs))
            return;
    }

    public bool IsInPhase(EGamePhase phase) => currentGamePhase == phase;

    private void Update()
    {
        phaseTimer += Time.deltaTime;
        switch (currentGamePhase)
        {
            case EGamePhase.BufferInputs:
            {
                if (phaseTimer >= bufferPhaseDuration || 
                    (InputManager.Instance.IsBufferFull(0) && InputManager.Instance.IsBufferFull(1)))
                {
                    GoToPhase(EGamePhase.Play, EGamePhase.BufferInputs);
                    GlobalEvents.OnBufferPhaseDone.Invoke();
                    return;
                }
            }
            break;
            case EGamePhase.Play:
            {
                
            }
            break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void GoToPhase(EGamePhase to, EGamePhase from)
    {
        phaseTimer = 0f;
        currentGamePhase = to;

        if (to == EGamePhase.Play)
        {
            StartCoroutine(PlayCoroutine());
        }
    }

    private void ProcessGameLoopOnce()
    {
        var player1Pawn = GridManager.Instance.GetPlayer(0);
        var player2Pawn = GridManager.Instance.GetPlayer(1);
        var player1Input = InputManager.Instance.PopInputFromBuffer(0);
        var player2Input = InputManager.Instance.PopInputFromBuffer(1);

        var player1Dir = GetDirectionFromInput(player1Input);
        var player2Dir = GetDirectionFromInput(player2Input);

        GridManager.ShootResult player1ShootResult = null;
        GridManager.ShootResult player2ShootResult = null;
        
        if (player1Input.IsMovementInput())
        {
            GridManager.Instance.MovePlayer(player1Pawn, player1Dir);
        }

        if (player2Input.IsMovementInput())
        {
            GridManager.Instance.MovePlayer(player2Pawn, player2Dir);
        }

        if (player1Input.IsShootInput())
        {
            player1ShootResult = GridManager.Instance.ShootInDirection(player1Pawn.position, player1Dir);
        }

        if (player2Input.IsShootInput())
        {
            player2ShootResult = GridManager.Instance.ShootInDirection(player2Pawn.position, player2Dir);
        }

        if (player1ShootResult != null)
        {
            if (player2ShootResult != null)
            {
                if ((player2ShootResult.hitPlayer && player2ShootResult.playerHitID == 0) && (player1ShootResult.hitPlayer && player1ShootResult.playerHitID == 1))
                {
                    // Both players shoot themselves, it's a draw
                }
            }
        }
    }

    public enum EPhaseResult
    {
        Continue,
        Player1Win,
        Player2Win,
        Draw
    }

    public void EndPlay(EPhaseResult result)
    {
        
    }
    
    public Vector2Int GetDirectionFromInput(InputManager.EInputType inputType)
    {
        switch (inputType)
        {
            case InputManager.EInputType.MoveUp:
                return Vector2Int.up;
            case InputManager.EInputType.MoveDown:
                return Vector2Int.down;
            case InputManager.EInputType.MoveLeft:
                return Vector2Int.left;
            case InputManager.EInputType.MoveRight:
                return Vector2Int.right;
            case InputManager.EInputType.ShootUp:
                return Vector2Int.up;
            case InputManager.EInputType.ShootDown:
                return Vector2Int.down;
            case InputManager.EInputType.ShootLeft:
                return Vector2Int.left;
            case InputManager.EInputType.ShootRight:
                return Vector2Int.right;
        }
        return Vector2Int.zero;
    }
    
    private IEnumerator PlayCoroutine()
    {
        
        yield break;
    }
}
