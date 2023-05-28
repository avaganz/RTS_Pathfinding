using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTS_UnitManager : MonoBehaviour
{
    [SerializeField]
    private RTS_Unit _unitPrefab;

    [SerializeField]
    private int _amount;

    [SerializeField]
    private float _unitSpeed = 1f;

    [SerializeField]
    private RTS_Playfield _playfield;

    public IEnumerable<RTS_Unit> SelectedUnits => _units.Where(e => e.IsSelected);

    private List<RTS_Unit> _units = new List<RTS_Unit>();

    public static RTS_UnitManager Instance { get; private set; }

    public void InitUnits(int amount = -1)
    {
        if (amount > -1)
            _amount = amount;

        // cleanup
        foreach (var unit in _units)
        {
            _playfield.CleanUp();
            Destroy(unit.gameObject);
        }

        _units.Clear();

        var p = Vector2Int.zero;

        // create new
        for (int i = 0; i < _amount; i++)
        {
            var unit = Instantiate(_unitPrefab, transform);
            unit.name = "Unit" + i;
            unit.Speed = _unitSpeed;

            _units.Add(unit);
            _playfield.AddToGrid(unit, p);

            p.x++;

            if (p.x >= Mathf.Sqrt(_amount))
            {
                p.x = 0;
                p.y++;
            }
        }
    }

    public void SelectUnitsAt(Rect rect)
    {
        var unitsToSelect = _units.Where(e => e.transform.position.x >= rect.x &&
                                              e.transform.position.x <= rect.width &&
                                              e.transform.position.y <= rect.y &&
                                              e.transform.position.y >= rect.height);

        foreach (var unit in unitsToSelect)
        {
            unit.IsSelected = true;
            //Debug.Log($"{unit.name} is selected");
        }
    }

    public void UnselectAll()
    {
        foreach (var unit in _units)
            unit.IsSelected = false;
    }

    public void MoveTo(RTS_Tile tile)
    {
        _playfield.SetHighlighted(tile, true);

        foreach (var unit in SelectedUnits)
            unit.MoveTo(tile);
    }

    private void Awake()
    {
        Instance = this;
    }
}
