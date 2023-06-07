using System;
using System.Collections;
using System.Collections.Generic;
using Modules.GameModule.Scripts.Ragdoll;
using Modules.MainModule.Scripts;
using Modules.MainModule.Scripts.InputServices;
using MoreMountains.NiceVibrations;
using UnityEngine;
using Zenject;

namespace Modules.GameModule.Scripts
{
    public class Hand : MonoBehaviour
    {
        [SerializeField] private float zPos;
        [SerializeField] private float punchForce;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float grabDelay;
        
        [SerializeField] private Transform debugTransform;

        [SerializeField] private DragRigidbody dragRigidbody;

        private Transform grabbedTransform;
        private Rigidbody grabbedRigidbody;
        private Collider grabbedCollider;

        private Ray ray;

        private Collider hittedCollider;

        private float grabDuration;
        
        private PlayerCamera playerCamera;
        private InputService inputService;

        [Inject]
        private void Construct(PlayerCamera playerCamera, InputService mobileInputService)
        {
            InjectCamera(playerCamera);
            InjectInput(mobileInputService);
        }

        private void InjectCamera(PlayerCamera playerCamera)
        {
            this.playerCamera = playerCamera;
        }
        
        private void InjectInput(InputService inputService)
        {
            this.inputService = inputService;
            this.inputService.OnTouchBegan += SetHittedCollider;
            this.inputService.OnTouchRelease += Punch;
            this.inputService.OnTouchRelease += Drop;
            this.inputService.OnTouch += MoveHand;
            this.inputService.OnTouch += Grab;
        }

        private void Awake()
        {
            debugTransform.gameObject.SetActive(Debug.isDebugBuild);
        }

        private void Punch(Vector2 position)
        {
            ray = playerCamera.Camera.ScreenPointToRay(
                new Vector3(
                    position.x,
                    position.y,
                    playerCamera.Camera.nearClipPlane));
            
            Physics.Raycast(ray, out var hit, 100);
            if (hit.collider != null)
            {
                debugTransform.position = hit.point;

                if (hit.collider.gameObject.TryGetComponent(out RagdollNode node))
                {
                    var heading = node.transform.position - hit.point;
                    var distance = heading.magnitude;
                    var direction = heading / distance;
                    
                    node.AddHit(direction, punchForce);
                    
                    node.OnTriggerEnterEvent?.Invoke(hit.point, node);
                    MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                }
                
                if (hit.collider.gameObject.TryGetComponent(out Rigidbody rb))
                {
                    Vector3 direction = (rb.transform.position - hit.point).normalized;
                    
                    rb.AddForce(direction * punchForce, ForceMode.Impulse);
                    MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                }
            }
        }
        
        private void Grab(Vector2 position)
        {
            ray = playerCamera.Camera.ScreenPointToRay(
                new Vector3(
                    position.x,
                    position.y,
                    playerCamera.Camera.nearClipPlane));

            Physics.Raycast(ray, out var hit, 100);
            if (hit.collider != null)
            {
                debugTransform.position = hit.point;

                grabDuration += Time.deltaTime;

                if (grabDuration >= grabDelay)
                {
                    if (hittedCollider != hit.collider && grabbedTransform == null)
                    {
                        grabDuration = 0;
                        return;
                    }

                    if (hit.transform.TryGetComponent(out Rigidbody rb))
                    {
                        if (grabbedTransform == null && !rb.isKinematic)
                        {
                            MMVibrationManager.Haptic(HapticTypes.SoftImpact);

                            grabbedTransform = hit.transform;
                            grabbedRigidbody = rb;
                            grabbedCollider = hit.collider;
                        }
                    }

                    Drag(hit);
                }
            }
        }

        private void SetHittedCollider(Vector2 position)
        {
            ray = playerCamera.Camera.ScreenPointToRay(
                new Vector3(
                    position.x,
                    position.y,
                    playerCamera.Camera.nearClipPlane));

            Physics.Raycast(ray, out var hit, 100);
            if (hit.collider != null)
            {
                debugTransform.position = hit.point;

                hittedCollider = hit.collider;
            }
        }

        private void Drag(RaycastHit hit)
        {
            if(grabbedTransform == null) return;
            
            dragRigidbody.DragObject(hit, grabbedRigidbody);
        }
        
        private void Drop(Vector2 position)
        {
            grabbedTransform = null;
            grabbedRigidbody = null;
            dragRigidbody.Reset();
            grabDuration = 0;
        }

        private void MoveHand(Vector2 position)
        {
            var targetPosition = playerCamera.Camera.ScreenToWorldPoint(
                new Vector3(
                position.x,
                position.y,
                zPos));

            // transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
            transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
        }
    }
}
