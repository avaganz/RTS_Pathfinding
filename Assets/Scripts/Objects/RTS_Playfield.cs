using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RTS_Playfield : MonoBehaviour
{
    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    private TileBase _enabledTile;

    [SerializeField]
    private TileBase _disabledTile;

    [SerializeField]
    private TileBase _highlightedTile;

    [SerializeField]
    private Vector2Int _size;

    public System.Action<RTS_Tile> TileClicked;

    public Vector2Int Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                Resize(_size);
            }
        }
    }

    public RTS_Tile[,] TileGrid { get; private set; }

    public void UpdateTile(RTS_Tile tile)
    {
        if (tile.IsEnabled)
            _tilemap.SetTile(new Vector3Int(tile.Location.x, tile.Location.y, 0), _enabledTile);
        else
            _tilemap.SetTile(new Vector3Int(tile.Location.x, tile.Location.y, 0), _disabledTile);


        if (tile.IsHighlighted)
            _tilemap.SetTile(new Vector3Int(tile.Location.x, tile.Location.y, 0), _highlightedTile);
        else
        {
            if (tile.IsEnabled)
                _tilemap.SetTile(new Vector3Int(tile.Location.x, tile.Location.y, 0), _enabledTile);
            else
                _tilemap.SetTile(new Vector3Int(tile.Location.x, tile.Location.y, 0), _disabledTile);
        }
    }

    public void SetHighlighted(RTS_Tile tileToHighlight, bool value)
    {
        foreach (var tile in TileGrid)
        {
            if (tile.IsHighlighted)
            {
                tile.IsHighlighted = false;
                UpdateTile(tile);
            }
        }

        tileToHighlight.IsHighlighted = value;
        UpdateTile(tileToHighlight);
    }

    public bool AddToGrid(RTS_Unit unit)
    {
        var tile = GetFreeTile();

        if (tile != null)
        {
            unit.CurrentTile = tile;
            unit.TileGrid = TileGrid;
            tile.IsFree = false;

            return true;
        }
        return false;
    }

    public bool AddToGrid(RTS_Unit unit, Vector2Int location)
    {
        var tile = TileGrid[location.x, location.y];

        if (tile != null && tile.IsFree && tile.IsEnabled)
        {
            unit.CurrentTile = tile;
            unit.TileGrid = TileGrid;
            tile.IsFree = false;

            return true;
        }
        return false;
    }


    public void InitGrid()
    {
        Resize(_size);
    }

    public void InitGrid(Vector2Int size)
    {
        _size = size;
        Resize(_size);
    }

    public void CleanUp()
    {
        foreach (var tile in TileGrid)
        {
            tile.IsFree = true;
            tile.IsHighlighted = false;
            tile.IsEnabled = true;
        }
    }

    private void Update()
    {
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var cellLocation = _tilemap.WorldToCell(p);

            if (cellLocation.x >= 0 && cellLocation.x < Size.x &&
                cellLocation.y >= 0 && cellLocation.y < Size.y)
            {
                var tile = TileGrid[cellLocation.x, cellLocation.y];

                if (TileClicked != null)
                    TileClicked.Invoke(tile);
            }
        }
    }

    private void Resize(Vector2Int size)
    {
        TileGrid = new RTS_Tile[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                TileGrid[x,y] = new RTS_Tile();
                TileGrid[x, y].IsFree = true;
                TileGrid[x, y].IsEnabled = true;
                TileGrid[x, y].Location = new Vector2Int(x, y);
            }
        }

        _tilemap.ClearAllTiles();
        _tilemap.BoxFill(new Vector3Int(size.x - 1, size.y - 1, 0), _enabledTile, 0, 0, size.y, size.y);
    }

    private RTS_Tile GetFreeTile()
    {
        foreach (var tile in TileGrid)
            if (tile.IsFree && tile.IsEnabled)
                return tile;

        return null;
    } 
}
