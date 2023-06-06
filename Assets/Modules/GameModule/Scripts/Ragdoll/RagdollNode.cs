using System;
using System.Collections.Generic;
using Modules.GameModule.Scripts.Character;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.GameModule.Scripts.Ragdoll
{
    [Serializable]
    public class RagdollNode : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private RagdollCollision collision;
        [SerializeField] private float shockThreshold;
        
        private List<Hit> lastHits;

        
        public UnityAction<Vector3, RagdollNode> OnTriggerEnterEvent;

        public Rigidbody Rigidbody => rigidbody;
        
        public RagdollCollision Collision => collision;

        public float ShockThreshold => shockThreshold;

        public void Setup(Rigidbody rigidbody, RagdollCollision collision)
        {
            this.rigidbody = rigidbody;
            this.collision = collision;
        }

        private void Start()
        {
            collision.OnTriggerEnterEvent += OnCollisionTriggerEnter;
        }

        private void OnCollisionTriggerEnter(Vector3 hitPosition, RagdollCollision ragdollCollision)
        {
            OnTriggerEnterEvent?.Invoke(hitPosition, this);
        }

        public void AddHit(Vector3 hitDirection, float force)
        {
            lastHits ??= new List<Hit>();
            
            lastHits.Add(new Hit
            {
                hitedNode = this,
                hitDirection = hitDirection,
                force = force
            });
        }

        public void ReleaseHitForce()
        {
            if(lastHits == null) return;
            
            foreach (var hit in lastHits)
            {
                hit.hitedNode.Rigidbody.AddForce(hit.hitDirection * hit.force, ForceMode.Impulse);
            }
            
            lastHits.Clear();
        }
    }
}