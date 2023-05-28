using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_UnitsAmountInputField : MonoBehaviour
{
    public Action<int> Changed;

    public void ValueChanged(string value)
    {
        if (Changed != null)
            Changed.Invoke(Convert.ToInt32(value));
    }
}
