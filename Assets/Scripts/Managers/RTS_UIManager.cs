using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RTS_UIManager : MonoBehaviour
{
    [SerializeField]
    private RTS_TileContextMenu _tileContextMenu;

    [SerializeField]
    private RTS_MapSizeDropdown _mapSizeDropdown;

    [SerializeField]
    private RTS_UnitsAmountInputField _unitsAmountInputField;

    [SerializeField]
    private RTS_Playfield _playfield;

    private void Start()
    {
        _playfield.TileClicked += OnPlayfieldTileClicked;

        _tileContextMenu.ToogleEnableAction += OnMenuToogleEnable;
        _tileContextMenu.MoveHereAction += OnMenuMoveHere;

        _mapSizeDropdown.Changed += OnMapSizeChanged;

        _unitsAmountInputField.Changed += OnUnitsAmountChanged;
    }

    private void OnPlayfieldTileClicked(RTS_Tile tile)
    {
        if (tile != null)
        {
            _tileContextMenu.transform.position = Input.mousePosition + _tileContextMenu.Offset;
            _tileContextMenu.SelectedTile = tile;
            _tileContextMenu.IsMoveButtonVisible = RTS_UnitManager.Instance.SelectedUnits.Count() > 0;
            _tileContextMenu.gameObject.SetActive(true);
        }
        else
        {
            _tileContextMenu.gameObject.SetActive(false);
        }
    }

    private void OnMenuToogleEnable(RTS_Tile tile)
    {
        _playfield.UpdateTile(tile);
    }

    private void OnMenuMoveHere(RTS_Tile tile)
    {
        RTS_UnitManager.Instance.MoveTo(tile);
    }

    private void OnMapSizeChanged(Vector2Int size)
    {
        RTS_GameManager.Instance.ResetMap(size);
    }

    private void OnUnitsAmountChanged(int amount)
    {
        RTS_UnitManager.Instance.InitUnits(amount);
    }
}
