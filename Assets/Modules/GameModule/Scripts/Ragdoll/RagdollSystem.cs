using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.GameModule.Scripts.Ragdoll
{
    public class RagdollSystem : MonoBehaviour
    {
        [SerializeField] private List<RagdollNode> ragdollNodes;
        [SerializeField] private bool enableOnAwake;
        

        public UnityAction<Vector3, RagdollNode> OnTriggerEnterEvent;
        public UnityAction<bool> OnSystemActivityChanged;

        private void OnValidate()
        {
            ragdollNodes ??= new List<RagdollNode>();

            if (ragdollNodes.Count == 0)
            {
                var childRb = GetComponentsInChildren<Rigidbody>();
                ragdollNodes.Capacity = childRb.Length;
                
                foreach (var rigidbody in childRb)
                {
                    var nodeGO = rigidbody.gameObject;
                    if (!nodeGO.TryGetComponent(out RagdollNode node))
                    {
                        node = nodeGO.AddComponent<RagdollNode>();
                    }
                    if (!nodeGO.TryGetComponent(out RagdollCollision collision))
                    {
                        collision = nodeGO.AddComponent<RagdollCollision>();
                    }
                    
                    node.Setup(rigidbody, collision);
                    
                    ragdollNodes.Add(node);
                }
            }
        }

        private void Start()
        {
            foreach (var node in ragdollNodes)
            {
                node.OnTriggerEnterEvent += OnCollisionTriggerEnter;
            }
            
            SetActive(enableOnAwake);
        }

        private void OnCollisionTriggerEnter(Vector3 hitPosition, RagdollNode ragdollNode)
        {
            OnTriggerEnterEvent?.Invoke(hitPosition, ragdollNode);
        }

        public void Enable()
        {
            SetActive(true);
        }
        
        public void Disable()
        {
            SetActive(false);
        }
        
        private void SetActive(bool isActive)
        {
            OnSystemActivityChanged?.Invoke(isActive);

            foreach (var node in ragdollNodes)
            {
                node.Rigidbody.velocity = Vector3.zero;
                node.Rigidbody.angularVelocity = Vector3.zero;
                node.Rigidbody.isKinematic = !isActive;
                if(isActive) node.ReleaseHitForce();
            }
        }
    }
}
