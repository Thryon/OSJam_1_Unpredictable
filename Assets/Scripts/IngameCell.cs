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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPositionInGrid(int x, int y)
    {
        positionInGrid = new Vector2Int(x, y);
    }
}
