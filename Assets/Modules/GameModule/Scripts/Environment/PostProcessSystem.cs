using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Modules.GameModule.Scripts.Environment
{
    public class PostProcessSystem : MonoBehaviour
    {
        [SerializeField] private Volume volume; 
        
        [Header("Bloom")]
        private Bloom bloom;
        [SerializeField] private float bloomThreshold;
        [SerializeField] private AnimationCurve bloomThresholdCurve;

        [Header("Film grain")] 
        private FilmGrain filmGrain;
        [SerializeField] private float grainIntensity;
        [SerializeField] private FilmGrainLookup defaultType;
        [SerializeField] private AnimationCurve grainThresholdCurve;
        [SerializeField] private Vector2 referenceResolution;

        private void Start()
        {
            if (!volume.profile.TryGet(out bloom))
            {
                Debug.LogError("Volume don't have a bloom override");
            }

            if (!volume.profile.TryGet(out filmGrain))
            {
                Debug.LogError("Volume don't have a filmGrain override");
            }
        }

        public void UpdateEffectsByDayTime(float timePercent)
        {
            if (bloom != null)
            {
                bloomThreshold = bloomThresholdCurve.Evaluate(timePercent);
                bloom.threshold.value = bloomThreshold;
            }
            
            if (filmGrain != null)
            {
                var screenCoefficientX = Screen.width / referenceResolution.x;
                var screenCoefficientY = Screen.height / referenceResolution.y;
                var screenCoefficient =
                    screenCoefficientX > screenCoefficientY 
                        ? screenCoefficientX 
                        : screenCoefficientY;

                var typeDifference = filmGrain.type.value - defaultType;
                grainIntensity = 
                    grainThresholdCurve.Evaluate(timePercent) * screenCoefficient
                    - typeDifference / 10f;
                if (grainIntensity > 1 && filmGrain.type.value != defaultType)
                {
                    filmGrain.type.value++;
                }
                filmGrain.intensity.value = grainIntensity;
            }
        }
    }
}