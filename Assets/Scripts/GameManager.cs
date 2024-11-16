using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    // public List<PlayerController> players = new();
    public PlayerController player1;
    public PlayerController player2;
    public float bufferPhaseDuration = 15f;
    public enum EGamePhase
    {
        BufferInputs,
        Play,
        Win
    }
    
    public EGamePhase currentGamePhase;
    private float phaseTimer = 0f;

    private void Awake()
    {
        if(!instance)
            instance = this;
        GlobalEvents.OnInputBuffered.AddListener(OnInputBuffered);
        GoToPhase(EGamePhase.BufferInputs, EGamePhase.BufferInputs);
        player1.pawn = GridManager.Instance.player1;
        player2.pawn = GridManager.Instance.player2;
        
        IngameCell player1cell = GridManager.Instance.IngameGrid.GetCellAtPos(player1.pawn.position);
        player1.TeleportTo(player1cell.transform.position);
        IngameCell player2cell = GridManager.Instance.IngameGrid.GetCellAtPos(player2.pawn.position);
        player2.TeleportTo(player2cell.transform.position);
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
                    
                    return;
                }
            }
            break;
            case EGamePhase.Play:
            {
                
            }
            break;
            case EGamePhase.Win:
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
        if (from == EGamePhase.BufferInputs)
        {
            GlobalEvents.OnBufferPhaseDone.Invoke();
            InputManager.Instance.ListenForInputs = false;
        }
        

        if (to == EGamePhase.BufferInputs)
        {
            InputManager.Instance.ClearInputs();
            InputManager.Instance.ListenForInputs = true;
            GlobalEvents.OnBufferPhaseStarted.Invoke();
        }
        else if (to == EGamePhase.Play)
        {
            StartCoroutine(PlayCoroutine());
        }
        else if (to == EGamePhase.Win)
        {
            
        }
    }

    private int currentInputIndex = 0;
    
    private EPlayPhaseResult ProcessGameLoopOnce()
    {
        var player1Pawn = GridManager.Instance.GetPlayer(0);
        var player2Pawn = GridManager.Instance.GetPlayer(1);
        var player1Input = InputManager.Instance.PopInputFromBuffer(0);
        var player2Input = InputManager.Instance.PopInputFromBuffer(1);

        var player1Dir = GetDirectionFromInput(player1Input);
        var player2Dir = GetDirectionFromInput(player2Input);

        GridManager.ShootResult player1ShootResult = null;
        GridManager.ShootResult player2ShootResult = null;

        if (player1Input == InputManager.EInputType.None && player2Input == InputManager.EInputType.None)
            return EPlayPhaseResult.Continue;
        
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
            player1ShootResult = GridManager.Instance.ShootInDirection(player1Pawn.playerID, player1Pawn.position, player1Dir);
        }

        if (player2Input.IsShootInput())
        {
            player2ShootResult = GridManager.Instance.ShootInDirection(player2Pawn.playerID, player2Pawn.position, player2Dir);
        }

        if (player1ShootResult != null)
        {
            if (player2ShootResult != null)
            {
                if ((player2ShootResult.hitPlayer && player2ShootResult.playerHitID == 0) &&
                    (player1ShootResult.hitPlayer && player1ShootResult.playerHitID == 1))
                {
                    // Both players shoot each other, it's a draw
                    return EPlayPhaseResult.Draw;
                }
            }

            if (player1ShootResult.hitPlayer && player1ShootResult.playerHitID == 1)
            {
                return EPlayPhaseResult.Player1Win;
            }
        }

        if (player2ShootResult != null)
        {
            if (player2ShootResult.hitPlayer && player2ShootResult.playerHitID == 0)
            {
                return EPlayPhaseResult.Player2Win;
            }
        }

        return EPlayPhaseResult.Continue;
    }

    public enum EPlayPhaseResult
    {
        Continue,
        Player1Win,
        Player2Win,
        Draw
    }

    public EPlayPhaseResult LastPlayPhaseResult;
    public void EndPlay(EPlayPhaseResult result)
    {
        LastPlayPhaseResult = result;
        GlobalEvents.OnPlayPhaseDone.Invoke(result);
        if (result == EPlayPhaseResult.Continue)
        {
            GoToPhase(EGamePhase.BufferInputs, EGamePhase.Play);
        }
        else if (result == EPlayPhaseResult.Player1Win || 
                 result == EPlayPhaseResult.Player2Win || 
                 result == EPlayPhaseResult.Draw)
        {
            GoToPhase(EGamePhase.Win, EGamePhase.Play);
        }
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
        int processedInputs = 0;
        int totalInputsCount = InputManager.Instance.maxInputsInBuffer;
        while (processedInputs < totalInputsCount)
        {
            EPlayPhaseResult result = ProcessGameLoopOnce();
            processedInputs++;
            
            IngameCell player1cell = GridManager.Instance.IngameGrid.GetCellAtPos(player1.pawn.position);
            
            player1.MoveTo(player1cell.transform.position, 0.5f);
            IngameCell player2cell = GridManager.Instance.IngameGrid.GetCellAtPos(player2.pawn.position);
            player2.MoveTo(player2cell.transform.position , 0.5f);
            
            yield return new WaitForSeconds(0.5f);
            if(result.IsEndingResult())
            {
                EndPlay(result);
                yield break;
            }
        }
        EndPlay(EPlayPhaseResult.Continue);
    }
}
public static class PlayPhaseResultExtension
{
    public static bool IsEndingResult(this GameManager.EPlayPhaseResult result)
    {
        return (result == GameManager.EPlayPhaseResult.Draw || result == GameManager.EPlayPhaseResult.Player1Win || result == GameManager.EPlayPhaseResult.Player2Win);
    }
}
