using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_Camera : MonoBehaviour
{
    [SerializeField]
    private float _mouseSensity = 1f;

    private bool _isScrollButtonDown = false;

    private void Update()
    {
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (Camera.main.orthographicSize - Input.mouseScrollDelta.y < 1f)
            Camera.main.orthographicSize = 1f;
        else
            Camera.main.orthographicSize -= Input.mouseScrollDelta.y;

        if (Input.GetMouseButtonDown(2))
            _isScrollButtonDown = true;

        if (Input.GetMouseButtonUp(2))
            _isScrollButtonDown = false;

        if (_isScrollButtonDown)
        {
            transform.position += new Vector3(-Input.GetAxisRaw("Mouse X") * _mouseSensity * Camera.main.orthographicSize,
                                              -Input.GetAxisRaw("Mouse Y") * _mouseSensity * Camera.main.orthographicSize);
        }
    }
}
