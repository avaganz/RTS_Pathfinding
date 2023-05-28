using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_SelectionTool : MonoBehaviour
{
    [SerializeField]
    private GameObject _rectPrefab;

    private GameObject _currentRect;

    private Vector3 _startPosition;

    public System.Action<Rect> SelectionDone { get; set; }

    private void Update()
    {
        InputUpdate();
    }

    private void InputUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = -1f;

            StartDraw(p);
        }
        else if (Input.GetMouseButton(0))
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = -1f;

            ContinueDraw(p);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = -1f;

            EndDraw(p);            
        }
    }

    private void StartDraw(Vector3 p)
    {
        _startPosition = p;
        _currentRect = Instantiate(_rectPrefab, _startPosition, Quaternion.identity);
        _currentRect.transform.localScale = Vector3.zero;
    }

    private void ContinueDraw(Vector3 p)
    {
        var size = p - _startPosition;
        _currentRect.transform.localScale = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), 1f);
        _currentRect.transform.position = _startPosition + size / 2f;
    }

    private void EndDraw(Vector3 p)
    {
        if (_startPosition.x > p.x)
        {
            var t = _startPosition.x;
            _startPosition.x = p.x;
            p.x = t;
        }
        if (_startPosition.y < p.y)
        {
            var t = _startPosition.y;
            _startPosition.y = p.y;
            p.y = t;
        }

        var rect = new Rect(_startPosition.x, _startPosition.y, p.x, p.y);

        if (SelectionDone != null)
            SelectionDone.Invoke(rect);

        Destroy(_currentRect);
    }
}
