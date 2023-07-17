using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action MainInputAction;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            MainInputAction?.Invoke();
        }
    }
}