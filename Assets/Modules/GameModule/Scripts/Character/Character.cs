using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.GameModule.Scripts.Character.StateMachine;
using Modules.GameModule.Scripts.Ragdoll;
using UnityEngine;
using UnityEngine.AI;

namespace Modules.GameModule.Scripts.Character
{
    public enum CharacterState
    {
        Active,
        GettingUp,
        Moving,
        Ragdoll,
    }
    
    public class Character : MonoBehaviour
    {
        [SerializeField] private RagdollSystem ragdollSystem;
        [SerializeField] private AnimationController animationController;
        [SerializeField] private Transform rootBone;
        [SerializeField] private CharacterHead head;
        [SerializeField] private CharacterEyes eyes;
        [SerializeField] private CharacterHands hands;
        [SerializeField] private Transform hitPositionFollower;
        [SerializeField] private NavMeshAgent navMeshAgent;
        
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 hipsStartPosition;

        private BoneTransform[] standUpBoneTransforms;
        private BoneTransform[] ragdollBoneTransforms;
        private Transform[] bones;

        private UniTask currentTask;

        private float timeToResetBones = 2f;

        // private State<Character> state;

        private CharacterState state;

        private void Awake()
        {
            
            bones = rootBone.GetComponentsInChildren<Transform>();
            standUpBoneTransforms = new BoneTransform[bones.Length];
            ragdollBoneTransforms = new BoneTransform[bones.Length];

            for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
            {
                standUpBoneTransforms[boneIndex] = new BoneTransform();
                ragdollBoneTransforms[boneIndex] = new BoneTransform();
            }
            
            PopulateAnimationStartBoneTransforms(AnimationActionType.StandUp, standUpBoneTransforms);
        }

        private void Start()
        {
            animationController.PlayAnimation(AnimationActionType.Idle);
            state = CharacterState.Active;

            startPosition = transform.position;
            hipsStartPosition = rootBone.localPosition;

            ragdollSystem.OnTriggerEnterEvent += MoveHandsToHit;
            ragdollSystem.OnSystemActivityChanged += OnRagdollActive;
        }

        private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
        {
            for (int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
            {
                boneTransforms[boneIndex].Position = bones[boneIndex].localPosition;
                boneTransforms[boneIndex].Rotation = bones[boneIndex].localRotation;
            }
        }
        
        private void PopulateAnimationStartBoneTransforms(AnimationActionType type, BoneTransform[] boneTransforms)
        {
            Vector3 positionBeforeSampling = transform.position;
            Quaternion rotationBeforeSampling = transform.rotation;

            var clip = animationController.GetClip(type);
            clip.SampleAnimation(gameObject, 0);
            PopulateBoneTransforms(boneTransforms);

            transform.position = positionBeforeSampling;
            transform.rotation = rotationBeforeSampling;
        }

        private void OnRagdollActive(bool isActive)
        {
            state = isActive ? CharacterState.Ragdoll : CharacterState.Active;

            if (isActive)
            {
                animationController.SetActive(false);
                navMeshAgent.enabled = false;

                currentTask = StandUp(5);
            }
        }
        
        async UniTask StandUp(int delay)
        {
            await UniTask.Delay(delay * 1000);

            state = CharacterState.GettingUp;

            // AlignRotationToHips();
            AlignPositionToHips();
            
            PopulateBoneTransforms(ragdollBoneTransforms);

            currentTask = ResetBones();
        }
        
        private async UniTask ResetBones()
        {
            if(state == CharacterState.Ragdoll) return;
            
            var elapsedResetBonesTime = 0f;
            var elapsedPercentage = elapsedResetBonesTime / timeToResetBones;
            
            while (elapsedPercentage < 1f)
            {
                elapsedResetBonesTime += Time.deltaTime * 5f;

                for (int boneIndex = 0; boneIndex < bones.Length; boneIndex ++)
                {
                    bones[boneIndex].localPosition = Vector3.Lerp(
                        ragdollBoneTransforms[boneIndex].Position,
                        standUpBoneTransforms[boneIndex].Position,
                        elapsedPercentage);

                    bones[boneIndex].localRotation = Quaternion.Lerp(
                        ragdollBoneTransforms[boneIndex].Rotation,
                        standUpBoneTransforms[boneIndex].Rotation,
                        elapsedPercentage);
                }

                elapsedPercentage = elapsedResetBonesTime / timeToResetBones;

                await UniTask.WaitForEndOfFrame(this);
                if(state == CharacterState.Ragdoll) return;
            }
            
            await UniTask.Delay(10);
            if(state == CharacterState.Ragdoll) return;

            ragdollSystem.Disable();
            animationController.SetActive(true);
            animationController.PlayAnimation(AnimationActionType.StandUp);

            var standUpDuration = animationController.CurrentStateInfo().length;
            currentTask = GoToStartPosition(standUpDuration + 1f);
        }

        private async UniTask GoToStartPosition(float delay)
        {
            await UniTask.Delay((int) delay * 1000);
            if(state == CharacterState.Ragdoll) return;

            animationController.SetActive(true);
            animationController.PlayAnimation(AnimationActionType.Walk);

            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(Vector3.zero);
            var currentPosition = transform.position;
            currentPosition.y = 0f;
            while (Vector3.Distance(currentPosition, Vector3.zero) > 0.1f)
            {
                await UniTask.WaitForEndOfFrame(this);
                if(state == CharacterState.Ragdoll) return;

                currentPosition = transform.position;
                currentPosition.y = 0f;
            }

            transform.position = Vector3.zero;

            animationController.SetTrigger(AnimationActionType.Idle);
        }
        

        private void AlignPositionToHips()
        {
            Vector3 originalHipsPosition = hipsStartPosition; 
            transform.position = rootBone.position;

            Vector3 positionOffset = standUpBoneTransforms[0].Position;
            positionOffset.y = 0;
            positionOffset = transform.rotation * positionOffset;
            transform.position -= positionOffset;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
        
            rootBone.localPosition = originalHipsPosition;
        }

        private void MoveHandsToHit(Vector3 hitPosition, RagdollNode ragdollNode)
        {
            hitPositionFollower.position = hitPosition;
            
            // hands.StopReset();
            // hands.MoveHandsToHit();
            
            ragdollSystem.Enable();
            // ResetRagdoll(5f);
            // hands.ResetHands(3f);
        }
        
        public void ResetRagdoll(float delay)
        {
            StartCoroutine(ResetRagdollCoroutine(delay));
        }

        IEnumerator ResetRagdollCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            ragdollSystem.Disable();
        }
        
        
    }
}