using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_GameManager : MonoBehaviour
{
    [SerializeField]
    private RTS_Playfield _playfield;

    [SerializeField]
    private RTS_UnitManager _unitManager;

    [SerializeField]
    private RTS_SelectionTool _selectionTool;

    public static RTS_GameManager Instance { get; private set; }

    public void ResetMap(Vector2Int size)
    {
        _playfield.InitGrid(size);
        _unitManager.InitUnits();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _playfield.InitGrid();
        _unitManager.InitUnits();

        _selectionTool.SelectionDone += OnSelectionDone;
    }

    private void OnSelectionDone(Rect rect)
    {
        _unitManager.UnselectAll();
        _unitManager.SelectUnitsAt(rect);
    }
}
