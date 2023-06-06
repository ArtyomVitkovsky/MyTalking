using UnityEngine;

namespace Modules.MainModule.Scripts.InputServices
{
    public class MobileInputService : InputService
    { 
        private Touch touch;
        
        private void Update()
        {
            GetInput();
        }

        public override void GetInput()
        {
            switch (Input.touchCount)
            {
                case 0:
                {
                    if (isTouched)
                    {
                        isTouched = false;
                        OnTouchRelease?.Invoke(touch.position);
                    }
                    break;
                }
                case 1:
                {
                    touch = Input.GetTouch(0);
                    isTouched = true;

                    if (touch.phase == TouchPhase.Began)
                    {
                        OnTouchBegan?.Invoke(touch.position);
                    }
                    
                    OnTouch?.Invoke(touch.position);
                    
                    break;
                }
            }
        }
    }
}