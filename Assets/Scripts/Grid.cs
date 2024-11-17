using System.Collections.Generic;
using UnityEngine;

public class CellContent
{
    public bool isPlayerObstacle;
    public bool isShootObstacle;
}

public class Cell
{
    public Vector2Int position;
    public Grid grid;
    public bool isWall;
    public bool isTeleporter;
    public string teleporterID;

    public List<CellContent> CellContents = new List<CellContent>();
    
    public bool IsPlayerObstacle()
    {
        if (isWall)
            return true;
        
        return CellContents.FindIndex(x => x.isPlayerObstacle) != -1;
    }
    
    public bool IsShootObstacle()
    {
        if (isWall)
            return true;
        
        return CellContents.FindIndex(x => x.isShootObstacle) != -1;
    }

    public CellContent GetFirstShootableContent()
    {
        return CellContents.Find(x => x.isShootObstacle);
    }
}
public class Grid
{
    public Cell[,] cells;
    public int Width => cells.GetLength(0);
    public int Height => cells.GetLength(0);
    
    public Grid(int width, int height)
    {
        cells = new Cell[width,height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[x,y] = new Cell() { position = new Vector2Int(x, y), grid = this };
            }
        }
    }
    
    public bool IsPosInGrid(Vector2Int position)
    {
        return !(position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height);
    }
    
    public Cell GetCellAtPosition(Vector2Int position)
    {
        if (!IsPosInGrid(position))
            return null;
        
        return cells[(int)position.x, (int)position.y];
    }

    public bool IsValidPos(Vector2Int position)
    {
        var cell = GetCellAtPosition(position);
        return cell != null && !cell.IsPlayerObstacle();
    }
}

public class PlayerPawn
{
    public int playerID;
    public Vector2Int position;
}

