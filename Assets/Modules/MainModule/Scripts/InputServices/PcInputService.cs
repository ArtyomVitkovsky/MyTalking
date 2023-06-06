using UnityEngine;

namespace Modules.MainModule.Scripts.InputServices
{
    public class PcInputService : InputService
    {
        private void Update()
        {
            GetInput();
        }

        public override void GetInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTouchBegan?.Invoke(Input.mousePosition);
                isTouched = true;
            }
            if (Input.GetMouseButton(0))
            {
                OnTouch?.Invoke(Input.mousePosition);
                isTouched = true;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                OnTouchRelease?.Invoke(Input.mousePosition);
                isTouched = false;
            }
        }
    }
}