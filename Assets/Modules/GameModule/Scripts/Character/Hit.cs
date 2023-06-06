using Modules.GameModule.Scripts.Ragdoll;
using UnityEngine;

namespace Modules.GameModule.Scripts.Character
{
    public class Hit
    {
        public RagdollNode hitedNode;
        public Vector3 hitDirection;
        public float force;
    }
}