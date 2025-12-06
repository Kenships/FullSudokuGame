using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public abstract class InputListenerBase : ScriptableObject
    {
        public UnityAction<int> OnValueInput;
        public UnityAction OnEraseInput;
        public UnityAction OnEscapeInput;
        public UnityAction OnDeselectInput;
        public UnityAction<Vector2> OnMoveInput;
        public UnityAction<Vector2Int> OnSelectInput;

        public UnityAction OnHintInput;
    }
}
