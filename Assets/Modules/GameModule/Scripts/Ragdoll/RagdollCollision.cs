using System;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.GameModule.Scripts.Ragdoll
{
    public class RagdollCollision : MonoBehaviour
    {
        [SerializeField] private Collider collider;
        
        public UnityAction<Vector3, RagdollCollision> OnTriggerEnterEvent;

        private void OnValidate()
        {
            if (collider == null) collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var hitPosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            OnTriggerEnterEvent?.Invoke(hitPosition, this);
        }

        // private void OnCollisionEnter(Collision collision)
        // {
        //     OnTriggerEnterEvent?.Invoke(collision.contacts[0].point, this);
        // }

    }
}