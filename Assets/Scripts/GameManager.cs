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
                // DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    // public List<PlayerController> players = new();
    public PlayerController player1;
    public PlayerController player2;
    public float bufferPhaseDuration = 15f;
    public float roundWinPhaseDuration = 2f;
    public float playerMoveDuration = .5f;
    public float playerTeleportCooldown = .25f;
    public float playerTeleportDelay = .25f;
    public int RoundsWonBeforeVictory = 3;
    public float PhaseTimer => phaseTimer;
    
    public enum EGamePhase
    {
        BufferInputs,
        Play,
        WinRound,
        WinGame
    }
    
    public EGamePhase currentGamePhase;
    private float phaseTimer = 0f;
    private int player1WonRounds = 0;
    private int player2WonRounds = 0;
    
    public int Player1WonRounds => player1WonRounds;
    public int Player2WonRounds => player2WonRounds;

    private void Awake()
    {
        if(!instance)
            instance = this;
        GlobalEvents.OnInputBuffered.AddListener(OnInputBuffered);
        player1.pawn = GridManager.Instance.player1;
        player2.pawn = GridManager.Instance.player2;
        GoToPhase(EGamePhase.BufferInputs, EGamePhase.WinRound);
        
        PrepareRound();
    }

    public void PrepareRound()
    {
        player1.Reset();
        player2.Reset();
        
        GridManager.Instance.PlacePlayersOnSpawnPoints();
        Vector3 player1Pos = GridManager.Instance.GetCellWorldPosition(player1.pawn.position);
        player1.TeleportTo(player1Pos);
        Vector3 player2Pos = GridManager.Instance.GetCellWorldPosition(player2.pawn.position);
        player2.TeleportTo(player2Pos);
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
            case EGamePhase.WinRound:
            {
                if (phaseTimer >= roundWinPhaseDuration)
                {
                    GoToPhase(EGamePhase.BufferInputs, EGamePhase.WinRound);
                    return;
                }
            }
            break;
            case EGamePhase.WinGame:
            {
                
            }
            break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void GoToPhase(EGamePhase to, EGamePhase from)
    {
        phaseTimer = 0f;
        currentGamePhase = to;
        if (from == EGamePhase.BufferInputs)
        {
            GlobalEvents.OnBufferPhaseDone.Invoke();
            InputManager.Instance.ListenForInputs = false;
        }
        else if (from == EGamePhase.WinRound)
        {
            GlobalEvents.ResetForNewRound.Invoke();
            PrepareRound();
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
            GlobalEvents.OnPlayPhaseStarted.Invoke();
        }
        else if (to == EGamePhase.WinRound)
        {
            GlobalEvents.OnRoundWin.Invoke();
        }
        else if (to == EGamePhase.WinGame)
        {
            GlobalEvents.OnGameWin.Invoke();
        }
    }

    public enum EAction
    {
        Move,
        Shoot
    }
    public class GameLoopIteration
    {
        public List<EAction> player1Actions = new();
        public List<EAction> player2Actions = new();
        public GridManager.ShootResult Player1ShootResult;
        public GridManager.ShootResult Player2ShootResult;
        public GridManager.MoveResult Player1MoveResult;
        public GridManager.MoveResult Player2MoveResult;
        public EPlayPhaseResult playPhaseResult;
    }
    
    private int currentInputIndex = 0;
    
    private GameLoopIteration ProcessGameLoopOnce()
    {
        GameLoopIteration result = new();
        var player1Pawn = GridManager.Instance.GetPlayer(0);
        var player2Pawn = GridManager.Instance.GetPlayer(1);
        var player1Input = InputManager.Instance.PopInputFromBuffer(0);
        var player2Input = InputManager.Instance.PopInputFromBuffer(1);

        var player1Dir = GetDirectionFromInput(player1Input);
        var player2Dir = GetDirectionFromInput(player2Input);

        GridManager.ShootResult player1ShootResult = null;
        GridManager.ShootResult player2ShootResult = null;

        GridManager.MoveResult player1MoveResult = null;
        GridManager.MoveResult player2MoveResult = null;

        if (player1Input == InputManager.EInputType.None && player2Input == InputManager.EInputType.None)
        {
            result.playPhaseResult = EPlayPhaseResult.Continue;
            return result;
        }
        
        if (player1Input.IsMovementInput())
        {
            player1MoveResult = GridManager.Instance.MovePlayer(player1Pawn, player1Dir);
            result.Player1MoveResult = player1MoveResult;
            result.player1Actions.Add(EAction.Move);
        }

        if (player2Input.IsMovementInput())
        {
            player2MoveResult = GridManager.Instance.MovePlayer(player2Pawn, player2Dir);
            result.Player2MoveResult = player2MoveResult;
            result.player2Actions.Add(EAction.Move);
        }

        if (player1Input.IsShootInput())
        {
            player1ShootResult = GridManager.Instance.ShootInDirection(player1Pawn.playerID, player1Pawn.position, player1Dir);
            result.Player1ShootResult = player1ShootResult;
            result.player1Actions.Add(EAction.Shoot);
        }

        if (player2Input.IsShootInput())
        {
            player2ShootResult = GridManager.Instance.ShootInDirection(player2Pawn.playerID, player2Pawn.position, player2Dir);
            result.Player2ShootResult = player2ShootResult;
            result.player2Actions.Add( EAction.Shoot);
        }

        if (player1ShootResult != null)
        {
            if (player2ShootResult != null)
            {
                if ((player2ShootResult.hitPlayer && player2ShootResult.playerHitID == 0) &&
                    (player1ShootResult.hitPlayer && player1ShootResult.playerHitID == 1))
                {
                    // Both players shoot each other, it's a draw
                    result.playPhaseResult = EPlayPhaseResult.Draw;
                    return result;
                }
            }

            if (player1ShootResult.hitPlayer && player1ShootResult.playerHitID == 1)
            {
                result.playPhaseResult = EPlayPhaseResult.Player1Win;
                return result;
            }
        }

        if (player2ShootResult != null)
        {
            if (player2ShootResult.hitPlayer && player2ShootResult.playerHitID == 0)
            {
                result.playPhaseResult = EPlayPhaseResult.Player2Win;
                return result;
            }
        }

        result.playPhaseResult = EPlayPhaseResult.Continue;
        return result;
    }

    public enum EPlayPhaseResult
    {
        Continue,
        Player1Win,
        Player2Win,
        Draw
    }

    public EPlayPhaseResult LastPlayPhaseResult;
    public EPlayPhaseResult EndResult;
    public void EndPlay(EPlayPhaseResult result)
    {
        LastPlayPhaseResult = result;
        GlobalEvents.OnPlayPhaseDone.Invoke(result);
        if (result == EPlayPhaseResult.Continue)
        {
            GoToPhase(EGamePhase.BufferInputs, EGamePhase.Play);
        }
        else
        {
            if (result == EPlayPhaseResult.Player1Win)
            {
                player1WonRounds++;
            }
            else if (result == EPlayPhaseResult.Player2Win)
            {
                player2WonRounds++;
            }
            else if (result == EPlayPhaseResult.Draw)
            {
                player1WonRounds++;
                player2WonRounds++;
            }

            bool endGame = false;
            if (player1WonRounds >= RoundsWonBeforeVictory)
            {
                if (player2WonRounds >= RoundsWonBeforeVictory)
                {
                    EndResult = EPlayPhaseResult.Draw;
                }
                else
                {
                    EndResult = EPlayPhaseResult.Player1Win;
                }
                endGame = true;
            }
            else if(player2WonRounds >= RoundsWonBeforeVictory)
            {
                EndResult = EPlayPhaseResult.Player2Win;
                endGame = true;
            }

            if (endGame)
            {
                GoToPhase(EGamePhase.WinGame, EGamePhase.Play);
            }
            else
            {
                GoToPhase(EGamePhase.WinRound, EGamePhase.Play);
            }
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
            GlobalEvents.OnNewGameLoopIteration.Invoke();
            var result = ProcessGameLoopOnce();
            processedInputs++;
            bool hasAnyMovement = false;
            if (result.player1Actions.Contains(EAction.Move))
            {
                hasAnyMovement = true;
                IngameCell player1cell = GridManager.Instance.IngameGrid.GetCellAtPos(result.Player1MoveResult.toPosition);
                player1.MoveTo(player1cell.transform.position, 0.5f);
            }
            
            if (result.player2Actions.Contains(EAction.Move))
            {
                hasAnyMovement = true;
                IngameCell player2cell = GridManager.Instance.IngameGrid.GetCellAtPos(result.Player2MoveResult.toPosition);
                player2.MoveTo(player2cell.transform.position , 0.5f);
            }

            if (hasAnyMovement)
            {
                yield return new WaitForSeconds(playerMoveDuration);
            }

            bool hasAnyTp = false;
            
            if (result.player1Actions.Contains(EAction.Move) 
                && result.Player1MoveResult.teleported)
            {
                hasAnyTp = true;
                IngameCell player1cell = GridManager.Instance.IngameGrid.GetCellAtPos(result.Player1MoveResult.teleportDestination);
                player1.TeleportToWithVisuals(player1cell.transform.position, playerTeleportDelay);
            }
            
            if (result.player2Actions.Contains(EAction.Move) 
                && result.Player2MoveResult.teleported)
            {
                hasAnyTp = true;
                IngameCell player2cell = GridManager.Instance.IngameGrid.GetCellAtPos(result.Player2MoveResult.teleportDestination);
                player2.TeleportToWithVisuals(player2cell.transform.position, playerTeleportDelay);
            }

            if (hasAnyTp)
            {
                yield return new WaitForSeconds(playerTeleportDelay + playerTeleportCooldown);
            }
            
            bool hasAnyShots = false;
            bool player1Killed = false;
            bool player2Killed = false;
            float shootDuration = 0f;
            if (result.player1Actions.Contains(EAction.Shoot) && result.Player1ShootResult != null)
            {
                hasAnyShots = true;
                shootDuration = player1.GetShootDuration();
                if (result.Player1ShootResult.hitPlayer)
                {
                    if (result.Player1ShootResult.playerHitID == 0)
                    {
                        player1Killed = true;
                    }
                    else if (result.Player1ShootResult.playerHitID == 1)
                    {
                        player2Killed = true;
                    }
                }
                player1.Shoot(result.Player1ShootResult);
            }

            if (result.player2Actions.Contains(EAction.Shoot) && result.Player2ShootResult != null)
            {
                hasAnyShots = true;
                float duration = player2.GetShootDuration();
                if (duration > shootDuration)
                {
                    shootDuration = duration;
                }
                
                if (result.Player2ShootResult.hitPlayer)
                {
                    if (result.Player2ShootResult.playerHitID == 0)
                    {
                        player1Killed = true;
                    }
                    else if (result.Player2ShootResult.playerHitID == 1)
                    {
                        player2Killed = true;
                    }
                }

                player2.Shoot(result.Player2ShootResult);
            }

            if (hasAnyShots)
            {
                yield return new WaitForSeconds(shootDuration);
            }

            bool anyPlayerDied = player1Killed || player2Killed;
            float deathDuration = 0f;
            if (player1Killed)
            {
                deathDuration = player1.GetDeathDuration();
                player1.Kill();
            }

            if (player2Killed)
            {
                float duration = player1.GetDeathDuration();
                if (duration > deathDuration)
                {
                    deathDuration = duration;
                }
                player2.Kill();
            }

            if (anyPlayerDied)
            {
                yield return new WaitForSeconds(deathDuration);
            }
            
            if(result.playPhaseResult.IsEndingResult())
            {
                EndPlay(result.playPhaseResult);
                yield break;
            }
        }
        EndPlay(EPlayPhaseResult.Continue);
    }

    public int GetPlayerWonRounds(int playerID)
    {
        return playerID == 0 ? player1WonRounds : player2WonRounds;
    }
}
public static class PlayPhaseResultExtension
{
    public static bool IsEndingResult(this GameManager.EPlayPhaseResult result)
    {
        return (result == GameManager.EPlayPhaseResult.Draw || result == GameManager.EPlayPhaseResult.Player1Win || result == GameManager.EPlayPhaseResult.Player2Win);
    }
}
