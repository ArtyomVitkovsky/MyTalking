using UnityEngine;
using UnityEngine.Events;

namespace Modules.MainModule.Scripts.InputServices
{
    public abstract class InputService : MonoBehaviour
    {
        public UnityAction<Vector2> OnTouchBegan;
        public UnityAction<Vector2> OnTouch;
        public UnityAction<Vector2> OnTouchRelease;

        protected bool isTouched;

        public bool IsTouched => isTouched;

        public abstract void GetInput();
    }
}