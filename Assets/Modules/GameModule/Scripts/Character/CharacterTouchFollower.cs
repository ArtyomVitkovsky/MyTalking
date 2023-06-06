using Modules.MainModule.Scripts.InputServices;
using UnityEngine;
using Zenject;

namespace Modules.GameModule.Scripts.Character
{
    public class CharacterTouchFollower : MonoBehaviour
    {
        [SerializeField] private Character2Rigs rigs;

        private InputService inputService;
        [Inject]
        private void Construct(InputService inputService)
        {
            InjectInputService(inputService);
        }

        private void InjectInputService(InputService inputService)
        {
            this.inputService = inputService;
        }

        private void Update()
        {
            if (inputService.IsTouched)
            {
                rigs.ReweighingToSecondary();
            }
            else
            {
                rigs.ReweighingToMain();
            }
        }
    }
}