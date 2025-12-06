using System;
using Obvious.Soap;
using UnityEngine;

namespace DefaultNamespace.Input
{
    [CreateAssetMenu(fileName = "UIInputListener", menuName = "ScriptableObjects/Input/UIInputListener")]
    public class UIInputListener : InputListenerBase
    {
        [SerializeField] private ScriptableEventInt onValueInput;
        [SerializeField] private ScriptableEventVector2Int onSelectInput;
        [SerializeField] private ScriptableEventNoParam onErase;
        [SerializeField] private ScriptableEventNoParam onHint;

        private void OnEnable()
        {
            onValueInput.OnRaised += OnValueInputRaised;
            onErase.OnRaised += OnEraseRaised;
            onHint.OnRaised += OnHintRaised;
            onSelectInput.OnRaised += OnSelectInputRaised;
        }

        private void OnSelectInputRaised(Vector2Int position)
        {
            OnSelectInput?.Invoke(position);
        }

        private void OnHintRaised()
        {
            OnHintInput?.Invoke();
        }

        private void OnEraseRaised()
        {
            OnEraseInput?.Invoke();
        }

        private void OnValueInputRaised(int value)
        {
            OnValueInput?.Invoke(value);
        }
    }
}
