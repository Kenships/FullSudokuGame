using System;
using DefaultNamespace;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "IOInputListener", menuName = "ScriptableObjects/Input/IOInputListener")]
public class IOInputListener : InputListenerBase, InputSystem_Actions.INumberPadActions
{
    [SerializeField]
    private BoolVariable noteMode;
    private InputSystem_Actions m_inputAction;
    
    private void OnEnable()
    {
        if (m_inputAction == null)
        {
            m_inputAction = new InputSystem_Actions();
        }
        
        m_inputAction.NumberPad.SetCallbacks(this);
        m_inputAction.Enable();
    }

    private void OnDisable()
    {
        m_inputAction.NumberPad.RemoveCallbacks(this);
        m_inputAction.Disable();
    }

    public void On_1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(1);
        }
    }

    public void On_2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(2);
        }
        
    }

    public void On_3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(3);
        }
    }

    public void On_4(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(4);
        }
    }

    public void On_5(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(5);
        }
    }

    public void On_6(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(6);
        }
    }

    public void On_7(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(7);
        }
    }

    public void On_8(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(8);
        }
    }

    public void On_9(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnValueInput?.Invoke(9);
        }
    }

    public void OnErase(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnEraseInput?.Invoke();
        }
    }

    public void OnNoteMode(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            noteMode.Value = !noteMode.Value;
        }
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnEscapeInput?.Invoke();
            OnDeselectInput?.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnMoveInput?.Invoke(context.ReadValue<Vector2>());
        }
    }
}
