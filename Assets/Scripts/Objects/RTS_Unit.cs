using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class RTS_Unit : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1f;

    [SerializeField]
    private MeshRenderer _renderer;

    [SerializeField]
    private Color _selectedColor;

    [SerializeField]
    private int _maxDistance = 16;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;

                if (_renderer != null)
                    _renderer.material.color = value ? _selectedColor : Color.white;
            }
        }
    }

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public RTS_Tile CurrentTile
    {
        get => _currentTile;
        set
        {
            if (_currentTile != value)
            {
                _currentTile = value;

                // move immediately
                if (_path == null || _path.Count == 0)
                {
                    transform.position = new Vector3(_currentTile.Location.x,
                                                     _currentTile.Location.y);
                }
            }    
        }
    }

    public RTS_Tile[,] TileGrid { get; set; }

    private bool _isSelected = false;

    private bool _isFindingPath = false;

    private float _timeout = 0.5f;

    private RTS_Tile _currentTile;

    private List<Vector2Int> _path = new List<Vector2Int>();

    private List<RTS_Tile> _targets = new List<RTS_Tile>();

    public void MoveTo(RTS_Tile targetTile)
    {
        if (_path != null)
            _path.Clear();

        if (CurrentTile == targetTile)
            return;

        if (_targets.Count == 0)
        {
            var dist = Vector2Int.Distance(CurrentTile.Location, targetTile.Location);

            //Debug.Log("dist = " + dist);

            var amount = Mathf.Max(1, dist / _maxDistance);

            for (int i = 1; i <= amount; i++)
            {
                var p = CurrentTile.Location;

                if (p.x < targetTile.Location.x)
                    p.x = Mathf.Min(p.x + _maxDistance * i, targetTile.Location.x);
                else if (p.x > targetTile.Location.x)
                    p.x = Mathf.Max(p.x - _maxDistance * i, targetTile.Location.x);

                if (p.y < targetTile.Location.y)
                    p.y = Mathf.Min(p.y + _maxDistance * i, targetTile.Location.y);
                else if (p.y > targetTile.Location.y)
                    p.y = Mathf.Max(p.y - _maxDistance * i, targetTile.Location.y);

                var target = TileGrid[p.x, p.y];
                _targets.Add(target);

                //Debug.Log($"target {i} = {p.x}; {p.y}");
            }

            if (_targets.Count > 0)
                _targets.Add(targetTile);
        }

        FindPath(_targets.FirstOrDefault());
        //FindPath2(targetTile);
    }

    private void Update()
    {
        if (!_isFindingPath)
            MoveUpdate();
    }

    private void MoveUpdate()
    {
        if (_path != null && _path.Count > 0)
        {
            if (Vector2.Distance(transform.position, CurrentTile.Location) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(transform.position, CurrentTile.Location, Speed * Time.deltaTime);
            }
            else
            {
                transform.position = new Vector3(CurrentTile.Location.x, CurrentTile.Location.y);

                _path.Remove(CurrentTile.Location);

                if (_path.Count > 0)
                {
                    var nextLocation = _path.FirstOrDefault();
                    var nextTile = TileGrid[nextLocation.x, nextLocation.y];

                    if (nextTile.IsFree && nextTile.IsEnabled)
                    {
                        nextTile.IsFree = false;
                        CurrentTile.IsFree = true;
                        CurrentTile = nextTile;
                    }
                    else
                    {
                        _path.Clear();
                    }
                }
                else
                {
                    var currentTarget = _targets.FirstOrDefault();
                    _targets.Remove(currentTarget);

                    if (_targets.Count > 0)
                        MoveTo(_targets.FirstOrDefault());
                }
            }
        }
        else if (_targets.Count > 0 && CurrentTile != _targets.FirstOrDefault())
        {
            if (_timeout <= 0f)
            {
                _timeout = 0.5f;

                MoveTo(_targets.FirstOrDefault());
            }
            else
            {
                _timeout -= Time.deltaTime;
            }
        }
    }

    private int[,] GetBinGrid(RTS_Tile[,] grid)
    {
        var width = TileGrid.GetLength(0);
        var height = TileGrid.GetLength(1);

        var binGrid = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                binGrid[i, j] = (TileGrid[i, j].IsEnabled && TileGrid[i, j].IsFree) ? 0 : 1;
            }
        }

        // the first cell should be always free
        binGrid[CurrentTile.Location.x, CurrentTile.Location.y] = 0;

        return binGrid;
    }

    private async Task<List<Vector2Int>> CalcPath(RTS_Tile targetTile)
    {
        var path = new List<Vector2Int>();

        await Task.Run(() =>
        {
            var pathfinding = new RTS_AStar(GetBinGrid(TileGrid));
            path = pathfinding.FindPath(_currentTile.Location, targetTile.Location);
        });

        return path;
    }

    private async void FindPath(RTS_Tile targetTile)
    {
        _isFindingPath = true;

        var task = CalcPath(targetTile);
        _path = await task;

        _isFindingPath = false;

        if (_path != null)
        {
            var nextLocation = _path.FirstOrDefault();
            var nextTile = TileGrid[nextLocation.x, nextLocation.y];

            if (nextTile == _currentTile)
            {
                _path.Remove(nextLocation);

                if (_path.Count > 0)
                {
                    nextLocation = _path.FirstOrDefault();
                    nextTile = TileGrid[nextLocation.x, nextLocation.y];

                    if (nextTile.IsFree && nextTile.IsEnabled)
                    {
                        nextTile.IsFree = false;
                        CurrentTile.IsFree = true;
                        CurrentTile = nextTile;
                    }
                }
            }
        }
    }

    private void FindPath2(RTS_Tile targetTile)
    {
        _isFindingPath = true;

        var pathfinding = new RTS_AStar(GetBinGrid(TileGrid));
        _path = pathfinding.FindPath(_currentTile.Location, targetTile.Location);

        _isFindingPath = false;

        if (_path != null)
        {
            var nextLocation = _path.FirstOrDefault();
            var nextTile = TileGrid[nextLocation.x, nextLocation.y];

            if (nextTile == _currentTile)
            {
                _path.Remove(nextLocation);

                if (_path.Count > 0)
                {
                    nextLocation = _path.FirstOrDefault();
                    nextTile = TileGrid[nextLocation.x, nextLocation.y];

                    if (nextTile.IsFree && nextTile.IsEnabled)
                    {
                        nextTile.IsFree = false;
                        CurrentTile.IsFree = true;
                        CurrentTile = nextTile;
                    }
                }
            }
        }
    }
}
