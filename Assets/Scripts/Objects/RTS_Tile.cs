using UnityEngine;

public class RTS_Tile
{
    public bool IsEnabled { get; set; }

    public bool IsFree { get; set; }

    public bool IsHighlighted { get; set; }

    public Vector2Int Location { get; set; }
}
