using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RTS_MapSizeDropdown : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _dropdown;

    public Action<Vector2Int> Changed;

    public void OnDropdownChanged()
    {
        var size = new Vector2Int(16, 16);

        switch (_dropdown.value)
        {
            case 0:
                size = new Vector2Int(16, 16);
                break;
            case 1:
                size = new Vector2Int(32, 32);
                break;
            case 2:
                size = new Vector2Int(64, 64);
                break;
            case 3:
                size = new Vector2Int(128, 128);
                break;
            case 4:
                size = new Vector2Int(256, 256);
                break;
        }

        if (Changed != null)
            Changed.Invoke(size);
    }
}
