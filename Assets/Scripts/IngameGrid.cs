using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class IngameGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2Int player1SpawnPosition;
    [SerializeField] private Vector2Int player2SpawnPosition;
    [SerializeField] private float cellsDistance = 1f;
    [SerializeField] private IngameCell _ingameCellPrefab;
    [SerializeField] private bool _regenerateGrid;
    [FormerlySerializedAs("_pairGroundMat")] [SerializeField] private Material _evenGroundMat;
    [SerializeField] private Material _oddGroundMat;
    [SerializeField] private bool _applyMats;
    public List<IngameCell> cells = new List<IngameCell>();
    public Vector2Int Size => gridSize;
    public Vector2Int Player1SpawnPosition => player1SpawnPosition;
    public Vector2Int Player2SpawnPosition => player2SpawnPosition;
    
    void Update()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (_regenerateGrid)
            {
                _regenerateGrid = false;
                EditorUtility.SetDirty(this);
                GenerateGrid();
            }
            if (_applyMats)
            {
                _applyMats = false;
                foreach (var cell in cells)
                {
                    cell.ApplyMaterial((((cell.PositionInGrid.x + cell.PositionInGrid.y) % 2) == 0)
                        ? _evenGroundMat
                        : _oddGroundMat);
                }
                EditorUtility.SetDirty(this);
            }
        }
        #endif
    }

    private void ClearGrid()
    {
        Undo.RecordObject(this, "Clear grid");
        while (transform.childCount > 0)
        {
            GameObject child = transform.GetChild(0).gameObject;
            Undo.DestroyObjectImmediate(child);
        }
        cells.Clear();
    }
    
    private void GenerateGrid()
    {
        if (_ingameCellPrefab == null)
        {
            Debug.LogError("No cell prefab assigned to IngameGrid");
            return;
        }
        ClearGrid();
#if UNITY_EDITOR
        Undo.RecordObject(this, "Generate Grid");
#endif
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                IngameCell cell = SpawnCell(x, y);
                cells.Add(cell);
            }
        }
    }

    public IngameCell GetCellAtPos(Vector2Int position)
    {
        return GetCellAtPos(position.x, position.y);
    }

    public IngameCell GetCellAtPos(int x, int y)
    {
        return cells[x + y * gridSize.x];
    }

    private IngameCell SpawnCell(int x, int y)
    {
        IngameCell cell = (IngameCell)PrefabUtility.InstantiatePrefab(_ingameCellPrefab, transform);
        cell.transform.localPosition = new Vector3(x * cellsDistance, 0f, y * cellsDistance);
        cell.SetPositionInGrid(x, y);
        cell.name = "Cell_" + x + "_" + y;
        #if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(cell.gameObject, "Generate Grid");
            EditorUtility.SetDirty(cell);
        #endif
        return cell;
    }
}
