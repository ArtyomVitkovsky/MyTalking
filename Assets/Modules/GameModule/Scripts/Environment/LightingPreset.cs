using UnityEngine;

namespace Modules.GameModule.Scripts.Environment
{
    [System.Serializable]
    [CreateAssetMenu(fileName ="Lighting Preset", menuName ="Scriptables/Lighting Preset",order =1)]
    public class LightingPreset : ScriptableObject
    {
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public Gradient FogColor;
    }
}