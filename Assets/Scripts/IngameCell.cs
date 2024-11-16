using UnityEngine;

public class IngameCell : MonoBehaviour
{
    public enum EType
    {
        Ground,
        Wall
    }
    public EType Type;
    Vector2Int positionInGrid;

    public Vector2Int PositionInGrid => positionInGrid;

    public void SetPositionInGrid(int x, int y)
    {
        positionInGrid = new Vector2Int(x, y);
    }
}
