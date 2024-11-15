using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance
    {
        get
        {
            if (!instance)
            {
                GameObject go = new GameObject("InputManager");
                instance = go.AddComponent<InputManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    
    public int maxInputsInBuffer = 5;
    public enum EInputType
    {
        None = 0,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        ShootUp,
        ShootDown,
        ShootLeft,
        ShootRight
    }

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    public void OnMoveUp(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.MoveUp);
    }

    public void OnMoveDown(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.MoveDown);
    }

    public void OnMoveLeft(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.MoveLeft);
    }

    public void OnMoveRight(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.MoveRight);
    }

    public void OnShootUp(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.ShootUp);
    }

    public void OnShootDown(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.ShootDown);
    }

    public void OnShootLeft(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.ShootLeft);
    }

    public void OnShootRight(int playerID, InputAction.CallbackContext context)
    {
        BufferInput(playerID, EInputType.ShootRight);
    }

    public void BufferInput(int playerID, EInputType inputType)
    {
        if (!ListenForInputs)
            return;
        List<EInputType> inputBuffer = GetBuffer(playerID);
        if (inputBuffer.Count >= maxInputsInBuffer)
            return;
        
        inputBuffer.Add(inputType);
        GlobalEvents.OnInputBuffered.Invoke((playerID, inputType));
    }

    public List<EInputType> player1InputBuffer = new();
    public List<EInputType> player2InputBuffer = new();

    public bool ListenForInputs;

    public List<EInputType> GetBuffer(int playerID)
    {
        return playerID == 0 ? player1InputBuffer : player2InputBuffer;
    }
    
    public EInputType PopInputFromBuffer(int playerID)
    {
        var buffer = GetBuffer(playerID);
        EInputType inputType = buffer[0];
        buffer.RemoveAt(0);
        return inputType;
    }

    public bool IsBufferFull(int playerID) => GetBuffer(playerID).Count >= maxInputsInBuffer;
    
    private void Start()
    {
        var controls = new InputSystem_Actions();
        controls.Player1.MoveUp.performed += ctx => OnMoveUp(0, ctx);
        controls.Player1.MoveDown.performed += ctx => OnMoveDown(0, ctx);
        controls.Player1.MoveLeft.performed += ctx => OnMoveLeft(0, ctx);
        controls.Player1.MoveRight.performed += ctx => OnMoveRight(0, ctx);
        controls.Player1.ShootUp.performed += ctx => OnShootUp(0, ctx);
        controls.Player1.ShootDown.performed += ctx => OnShootDown(0, ctx);
        controls.Player1.ShootLeft.performed += ctx => OnShootLeft(0, ctx);
        controls.Player1.ShootRight.performed += ctx => OnShootRight(0, ctx);
        
        controls.Player2.MoveUp.performed += ctx => OnMoveUp(1, ctx);
        controls.Player2.MoveDown.performed += ctx => OnMoveDown(1, ctx);
        controls.Player2.MoveLeft.performed += ctx => OnMoveLeft(1, ctx);
        controls.Player2.MoveRight.performed += ctx => OnMoveRight(1, ctx);
        controls.Player2.ShootUp.performed += ctx => OnShootUp(1, ctx);
        controls.Player2.ShootDown.performed += ctx => OnShootDown(1, ctx);
        controls.Player2.ShootLeft.performed += ctx => OnShootLeft(1, ctx);
        controls.Player2.ShootRight.performed += ctx => OnShootRight(1, ctx);
    }

    
}

public static class EInputTypeExtensions
{
    public static bool IsShootInput(this InputManager.EInputType playerInput)
    {
        return playerInput == InputManager.EInputType.ShootUp ||
               playerInput == InputManager.EInputType.ShootDown ||
               playerInput == InputManager.EInputType.ShootLeft ||
               playerInput == InputManager.EInputType.ShootRight;
    }

    public static bool IsMovementInput(this InputManager.EInputType playerInput)
    {
        return playerInput == InputManager.EInputType.MoveUp ||
               playerInput == InputManager.EInputType.MoveDown ||
               playerInput == InputManager.EInputType.MoveLeft ||
               playerInput == InputManager.EInputType.MoveRight;
    }
}
