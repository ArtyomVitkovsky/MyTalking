using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Modules.GameModule.Scripts.Character
{
    [Serializable]
    public class AnimationAction
    {
        public string trigger;
        public AnimationActionType type;
        public string clipName;
    }

    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationAction[] animationActions;
        [SerializeField] private AnimationAction[] animationActionScenario;

        public Animator Animator => animator;

        public AnimatorStateInfo CurrentStateInfo()
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            return state;
        }
        
        public AnimationClip GetClip(AnimationActionType type)
        {
            var animationAction = animationActions.FirstOrDefault(a => a.type == type);

            var clip = animator
                .runtimeAnimatorController
                .animationClips.
                FirstOrDefault(c => c.name == animationAction.clipName);
            
            return clip;
        }

        public void SetActive(bool isActive)
        {
            animator.enabled = isActive;
        }
        
        public void PlayFullCycle(bool loop)
        {
            ResetTriggers();

            StartCoroutine(PlayFullCycleCoroutine(loop));
        }

        private void ResetTriggers()
        {
            foreach (var action in animationActions)
            {
                animator.ResetTrigger(action.trigger);
            }
        }

        public void SetTrigger(AnimationActionType type)
        {
            var action = animationActions.FirstOrDefault(a => a.type == type);

            if (action != null)
            {
                ResetTriggers();
                animator.SetTrigger(action.trigger);
                animator.Update(0);
            }
        }
        
        public void PlayAnimation(AnimationActionType type)
        {
            var action = animationActions.FirstOrDefault(a => a.type == type);

            if (action != null)
            {
                ResetTriggers();
                animator.Play(action.clipName);
                animator.Update(0);
            }
        }


        IEnumerator PlayFullCycleCoroutine(bool loop)
        {
            foreach (var action in animationActionScenario)
            {
                yield return StartCoroutine(PlayAnimationCoroutine(action.trigger));
            }

            if (loop)
            {
                PlayFullCycle(true);
            }
        }

        IEnumerator PlayAnimationCoroutine(string trigger)
        {
            animator.SetTrigger(trigger);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        }
    }
}