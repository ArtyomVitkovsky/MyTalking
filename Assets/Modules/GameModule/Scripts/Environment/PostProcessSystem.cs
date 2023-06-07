using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Modules.GameModule.Scripts.Environment
{
    public class PostProcessSystem : MonoBehaviour
    {
        [SerializeField] private Volume volume; 
        
        [Header("BLOOM")]
        [SerializeField] private float bloomThreshold;
        [SerializeField] private AnimationCurve bloomThresholdCurve;

        [Header("FILM GRAIN")] 
        [SerializeField] private float grainIntensity;
        [SerializeField] private FilmGrainLookup defaultType;
        [SerializeField] private AnimationCurve grainThresholdCurve;
        [SerializeField] private Vector2 referenceResolution;
        
        [Header("CHROMATIC ABERRATION")]
        [SerializeField] private float aberrationIntensity;
        [SerializeField] private AnimationCurve aberrationIntensityCurve;
        
        [Header("LENS DISTORTION")]
        [SerializeField] private float lensDistortionIntensity;
        [SerializeField] private AnimationCurve lensDistortionCurve;


        private Bloom bloom;
        private FilmGrain filmGrain;
        private ChromaticAberration chromaticAberration;
        private LensDistortion lensDistortion;

        
        private void Start()
        {
            if (!volume.profile.TryGet(out bloom))
            {
                Debug.LogError("Volume don't have a bloom override");
            }

            if (!volume.profile.TryGet(out filmGrain))
            {
                Debug.LogError("Volume don't have a film Grain override");
            }
            
            if (!volume.profile.TryGet(out lensDistortion))
            {
                Debug.LogError("Volume don't have a lens Distortion override");
            }
            
            if (!volume.profile.TryGet(out chromaticAberration))
            {
                Debug.LogError("Volume don't have a chromatic Aberration override");
            }
        }

        public void UpdateEffectsByDayTime(float timePercent)
        {
            if (bloom != null)
            {
                bloomThreshold = bloomThresholdCurve.Evaluate(timePercent);
                bloom.threshold.value = bloomThreshold;
            }
            
            if (chromaticAberration != null)
            {
                aberrationIntensity = aberrationIntensityCurve.Evaluate(timePercent);
                chromaticAberration.intensity.value = aberrationIntensity;
            }
            
            if (lensDistortion != null)
            {
                lensDistortionIntensity = lensDistortionCurve.Evaluate(timePercent);
                lensDistortion.intensity.value = lensDistortionIntensity;
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
                if (grainIntensity > 1 && filmGrain.type.GetValue<FilmGrainLookup>() != defaultType)
                {
                    filmGrain.type.SetValue(new IntParameter(filmGrain.type.GetValue<int>() + 1));
                }
                filmGrain.intensity.value = grainIntensity;
            }
        }
    }
}