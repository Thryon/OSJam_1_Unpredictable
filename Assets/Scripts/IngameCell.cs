using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
public class IngameCell : MonoBehaviour
{
    public enum EType
    {
        Ground,
        Wall,
        Teleporter,
    }
    public EType Type;
    public GameObject GroundModel;
    public GameObject WallModel;
    public GameObject TeleporterModel;
    [FormerlySerializedAs("renderer")] public MeshRenderer groundRenderer;
    [SerializeField] Vector2Int positionInGrid;
    public Vector2Int PositionInGrid => positionInGrid;
    [SerializeField] bool __refreshVisuals;

    private void Start()
    {
        RefreshVisuals();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (__refreshVisuals)
        {
            __refreshVisuals = false;
            RefreshVisuals();
        }
    }
#endif

    void RefreshVisuals()
    {
        GroundModel.SetActive(Type == EType.Ground);
        WallModel.SetActive(Type == EType.Wall);
        TeleporterModel.SetActive(Type == EType.Teleporter);
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
    public void SetPositionInGrid(int x, int y)
    {
        positionInGrid = new Vector2Int(x, y);
    }

    public void ApplyMaterial(Material mat)
    {
        if(groundRenderer == null)
            return;
        groundRenderer.sharedMaterial = mat;
    }
}
