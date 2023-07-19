using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action PlayerClickAction;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlayerClickAction?.Invoke();
        }
    }
}