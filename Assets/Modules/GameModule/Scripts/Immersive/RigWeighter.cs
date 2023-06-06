using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Modules.GameModule.Scripts.Immersive
{
    public class RigWeighter : MonoBehaviour
    {
        [SerializeField] private Rig rig;
        [SerializeField] private float rigSetSpeed;

        public float CurrentWeight => rig.weight;
        
        public void SetRigWeight(float targetWeight)
        {
            var rigWeight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * rigSetSpeed);
            rig.weight = rigWeight;
        }
        
        public void ResetRigWeight()
        {
            rig.weight = 0;
        }
    }
}