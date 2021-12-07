using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public PlayerMap input;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        input = new PlayerMap();
        input.Enable();
    }

    public void DisableInput() {
        input.Disable();
    }
    public void EnableInput()
    {
        input.Enable();
    }
}
