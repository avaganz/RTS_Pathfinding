using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RTS_TileContextMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Button _enableButton;

    [SerializeField]
    private Button _moveButton;

    [SerializeField]
    private Vector2Int _offset;

    private bool _isMouseOver = false;

    public System.Action<RTS_Tile> ToogleEnableAction { get; set; }

    public System.Action<RTS_Tile> MoveHereAction { get; set; }

    public bool IsMoveButtonVisible
    {
        get => _moveButton.gameObject.activeSelf;
        set => _moveButton.gameObject.SetActive(value);
    }

    public Vector3 Offset => new Vector3(_offset.x, _offset.y);

    public RTS_Tile SelectedTile { get; set; }

    public void ToogleEnable()
    {
        SelectedTile.IsEnabled = !SelectedTile.IsEnabled;

        if (ToogleEnableAction != null)
            ToogleEnableAction.Invoke(SelectedTile);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
    }

    public void MoveHere()
    {
        if (MoveHereAction != null)
            MoveHereAction.Invoke(SelectedTile);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isMouseOver)
            gameObject.SetActive(false);
    }
}
