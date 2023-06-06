using System;
using System.Collections;
using Modules.GameModule.Scripts.Immersive;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Modules.GameModule.Scripts.Character
{
    public class CharacterHands : MonoBehaviour
    {
        [SerializeField] private Character2Rigs leftHand;
        [SerializeField] private Character2Rigs rightHand;

        private Coroutine resetCoroutine;

        private void Start()
        {
            leftHand.ResetWeights();
            rightHand.ResetWeights();
        }

        public void MoveHandsToHit()
        {
            leftHand.OnWeighingDone += OnLeftHandWeighingDone;
            rightHand.OnWeighingDone += OnRightHandWeighingDone;
            
            leftHand.ReweighingToSecondary();
            rightHand.ReweighingToSecondary();
        }
        

        public void MoveHandsToStart()
        {
            leftHand.OnWeighingDone += OnLeftHandWeighingDone;
            rightHand.OnWeighingDone += OnRightHandWeighingDone;
            
            leftHand.ReweighingToMain();
            rightHand.ReweighingToMain();
        }
        
        private void OnLeftHandWeighingDone()
        {
            leftHand.OnWeighingDone -= OnLeftHandWeighingDone;

            leftHand.ResetWeights();
        }
        
        private void OnRightHandWeighingDone()
        {
            rightHand.OnWeighingDone -= OnLeftHandWeighingDone;

            rightHand.ResetWeights();
        }
        
        public void ResetHands(float delay)
        {
            StopReset();
            resetCoroutine = StartCoroutine(ResetHandsCoroutine(delay));
        }

        IEnumerator ResetHandsCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            MoveHandsToStart();
        }

        public void StopReset()
        {
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);
        }
    }
}
