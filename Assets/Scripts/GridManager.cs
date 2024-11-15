using System;
using UnityEngine;

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
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    public int gridWidth = 10;
    public int gridHeight = 10;

    public Grid grid;
    public PlayerPawn player1 = new() {playerID = 0};
    public PlayerPawn player2 = new() {playerID = 1};

    public PlayerPawn GetPlayer(int playerID)
    {
        return playerID == player1.playerID ? player1 : player2;
    }
    
    private void Awake()
    {
        if(!instance)
            instance = this;
        grid = new Grid(gridWidth, gridHeight);
        
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
        if (curPlayerCell != null)
        {
            curPlayerCell.RemovePlayer();
        }
        pawn.position = position;
        Cell cell = grid.GetCellAtPosition(position);
        cell.playerID = pawn.playerID;
    }

    public void MovePlayer(PlayerPawn pawn, Vector2Int direction)
    {
        Vector2Int targetPos = pawn.position + direction;
        if (grid.IsValidPos(targetPos))
        {
            PlacePlayer(pawn, targetPos);
        }
    }
    public void MovePlayer(PlayerPawn pawn, Vector2Int direction, int times)
    {
        for (int i = 0; i < times; i++)
        {
            MovePlayer(pawn, direction);
        }
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
        public bool HitNothing => !hitPlayer && !hitCellContent && !hitWall; 
    }

    public ShootResult ShootInDirection(Vector2Int startPos, Vector2Int direction)
    {
        ShootResult result = new ShootResult();
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

            if (cell.HasPlayer)
            {
                result.hitPlayer = true;
                result.playerHitID = cell.playerID;
                result.hitPosition = targetPos;
                result.cell = cell;
            }
            else if (cell.IsShootObstacle())
            {
                if (cell.isWall)
                {
                    result.hitWall = true;
                    result.hitPosition = targetPos;
                    result.cell = cell;
                }
                else
                {
                    result.hitCellContent = true;
                    result.cellContent = cell.GetFirstShootableContent();
                    result.hitPosition = targetPos;
                    result.cell = cell;
                }
            }
        }
        
        return result;
    }
}
