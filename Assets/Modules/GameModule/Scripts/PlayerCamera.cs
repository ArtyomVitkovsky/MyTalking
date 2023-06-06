using UnityEngine;

namespace Modules.GameModule.Scripts
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Camera camera;

        public Camera Camera => camera;
    }
}
