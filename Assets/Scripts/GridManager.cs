using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;
    public static GridManager Instance
    {
        get
        {
            if (!instance)
            {
                GameObject go = new GameObject("GridManager");
                instance = go.AddComponent<GridManager>();
                // DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    public int gridWidth = 10;
    public int gridHeight = 10;

    public Grid grid;
    public PlayerPawn player1 = new() {playerID = 0};
    public PlayerPawn player2 = new() {playerID = 1};
    public IngameGrid IngameGrid;
    
    public Dictionary<string, List<Cell>> teleporterCellsDictionary = new();

    public void InitWithIngameGrid(IngameGrid ingameGrid)
    {
        IngameGrid = ingameGrid;
        gridWidth = ingameGrid.Size.x;
        gridHeight = ingameGrid.Size.y;
        grid = new Grid(gridWidth, gridHeight);
        teleporterCellsDictionary.Clear();
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Cell cell = grid.GetCellAtPosition(new Vector2Int(x, y));
                var ingameCell = ingameGrid.GetCellAtPos(x, y);
                cell.isWall = ingameCell.Type == IngameCell.EType.Wall;
                if (ingameCell.Type == IngameCell.EType.Teleporter)
                {
                    var teleporter = ingameCell.GetComponentInChildren<IngameTeleporter>(true);
                    if (teleporter != null)
                    {
                        cell.isTeleporter = true;
                        cell.teleporterID = teleporter.connectionID;
                        if (!teleporterCellsDictionary.ContainsKey(teleporter.connectionID))
                        {
                            teleporterCellsDictionary.Add(teleporter.connectionID, new List<Cell>());
                        }
                        teleporterCellsDictionary[teleporter.connectionID].Add(cell);
                    }
                }
            }
        }
    }

    public PlayerPawn GetPlayer(int playerID)
    {
        return playerID == player1.playerID ? player1 : player2;
    }

    private void Awake()
    {
        if(!instance)
            instance = this;
        var ingameGrid = FindFirstObjectByType<IngameGrid>();
        InitWithIngameGrid(ingameGrid);
        PlacePlayersOnSpawnPoints();
    }

    public void PlacePlayersOnSpawnPoints()
    {
        PlacePlayer(0, IngameGrid.Player1SpawnPosition);
        PlacePlayer(1, IngameGrid.Player2SpawnPosition);
    }

    public Vector3 GetCellWorldPosition(Vector2Int position)
    {
        var cell = IngameGrid.GetCellAtPos(position);
        if(cell == null)
            return Vector3.zero;

        return cell.transform.position;
    }
    
    public void PlacePlayer(int playerID, Vector2Int position)
    {
        PlayerPawn pawn = GetPlayer(playerID);
        PlacePlayer(pawn, position);
    }

    public bool IsPosInGrid(Vector2Int position)
    {
        return grid.IsPosInGrid(position);
    }
    
    public void PlacePlayer(PlayerPawn pawn, Vector2Int position)
    {
        if(!IsPosInGrid(position))
            return;
        Cell curPlayerCell = grid.GetCellAtPosition(position);
        // Do something on the cell the player just left ? 
        pawn.position = position;
        Cell cell = grid.GetCellAtPosition(position);
        // Do something on the cell the player just arrived to ?
    }

    public class MoveResult
    {
        public PlayerPawn playerPawn;
        public bool hitObstacle;
        public Cell cell;
        public Vector2Int fromPosition;
        public Vector2Int toPosition;
        public Vector2Int direction;
        public Vector2Int teleportDestination;
        public bool teleported;
    }
    
    public MoveResult MovePlayer(PlayerPawn pawn, Vector2Int direction)
    {
        MoveResult result = new()
        {
            playerPawn = pawn,
            direction = direction , 
            fromPosition = pawn.position,
            toPosition = pawn.position + direction,
            cell = grid.IsPosInGrid(pawn.position) ? grid.GetCellAtPosition(pawn.position): null
        };
        Vector2Int targetPos = pawn.position + direction;
        if (grid.IsValidPos(targetPos))
        {
            PlacePlayer(pawn, targetPos);
            var cell = grid.GetCellAtPosition(targetPos);
            if (cell.isTeleporter)
            {

                if (teleporterCellsDictionary.TryGetValue(cell.teleporterID, out var teleporterCells))
                {
                    List<Cell> targetCells = new List<Cell>(teleporterCells);
                    targetCells.Remove(cell);
                    if (targetCells.Count > 0)
                    {
                        int rndIndex = Random.Range(0, targetCells.Count);
                        var targetCell = targetCells[rndIndex];
                        PlacePlayer(pawn, targetCell.position);
                        result.teleported = true;
                        result.teleportDestination = targetCell.position;
                    }
                }
            }
            GlobalEvents.OnPlayerMoved.Invoke(result);
        }
        else
        {
            result.hitObstacle = true;
            GlobalEvents.OnPlayerFailedToMove.Invoke(result);
        }

        return result;
    }

    public class ShootResult
    {
        public bool hitPlayer;
        public int playerHitID;
        public bool hitCellContent;
        public CellContent cellContent;
        public bool hitWall;
        public Cell cell;
        public Vector2Int hitPosition;
        public Vector2Int shootDirection;
        public Vector2Int shootStartPos;
        public bool hitNothing => !hitPlayer && !hitCellContent && !hitWall;

        public bool HasHitPlayer(int playerID)
        {
            return hitPlayer && playerHitID == playerID;
        }
    }
    
    public ShootResult ShootInDirection(int playerID, Vector2Int startPos, Vector2Int direction)
    {
        ShootResult result = new ShootResult() {shootDirection = direction, shootStartPos = startPos};
        bool found = false;
        Vector2Int targetPos = startPos;
        while (!found)
        {
            targetPos += direction;
            Cell cell = grid.GetCellAtPosition(targetPos);
            if (cell == null)
            {
                found = true;
                result.hitPosition = targetPos;
                break;
            }

            if (player1.position == targetPos || player2.position == targetPos)
            {
                result.hitPlayer = true;
                result.playerHitID = player1.position == targetPos ? 0 : 1;
                result.hitPosition = targetPos;
                result.cell = cell;
                found = true;
            }
            else if (cell.IsShootObstacle())
            {
                if (cell.isWall)
                {
                    result.hitWall = true;
                    result.hitPosition = targetPos;
                    result.cell = cell;
                    found = true;
                }
                else
                {
                    result.hitCellContent = true;
                    result.cellContent = cell.GetFirstShootableContent();
                    result.hitPosition = targetPos;
                    result.cell = cell;
                    found = true;
                }
            }
        }
        
        GlobalEvents.OnPlayerShot.Invoke(new GlobalEvents.Shot()
        {
            playerID = playerID,
            direction = direction,
            from = startPos,
            to = targetPos,
            result = result
        });
        
        return result;
    }
}
