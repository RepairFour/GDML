using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public float inputGravity;
    Vector2 currentInput = new Vector2();

    public InputActionMap movementMap;

    [SerializeField] Vector3 moveDirection;
    [SerializeField] bool jump;

    public Vector3 getMoveDirection { get => moveDirection;}
    public bool getJump { get => jump; set => jump = value; }

    // Start is called before the first frame update
    void Start()
    {
        movementMap.Enable();
        jump = false;
    }

    // Update is called once per frame
    void Update()
    {
        var inputDirection = movementMap.FindAction("Move").ReadValue<Vector2>();
        currentInput.x = Mathf.MoveTowards(currentInput.x, inputDirection.x, inputGravity * Time.deltaTime);
        currentInput.y = Mathf.MoveTowards(currentInput.y, inputDirection.y, inputGravity * Time.deltaTime);

        moveDirection = new Vector3(currentInput.x, 0, currentInput.y);

        //if (movementMap.FindAction("Jump").triggered)
        //{
        //    jump = true;
        //}
    }
}
