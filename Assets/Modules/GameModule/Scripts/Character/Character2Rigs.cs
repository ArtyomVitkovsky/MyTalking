using System;
using System.Collections;
using System.Collections.Generic;
using Modules.GameModule.Scripts.Immersive;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.GameModule.Scripts.Character
{
    public class Character2Rigs : MonoBehaviour
    {
        [SerializeField] private RigWeighter mainRig;
        [SerializeField] private RigWeighter secondaryRig;
        

        
        private Coroutine moveHandsCoroutine;

        public UnityAction OnWeighingDone;

        public void ReweighingToMain()
        {
            if(moveHandsCoroutine != null) StopCoroutine(moveHandsCoroutine);
            moveHandsCoroutine = StartCoroutine(ReweighingCoroutine(true));
        }
        
        public void ReweighingToSecondary()
        {
            if(moveHandsCoroutine != null) StopCoroutine(moveHandsCoroutine);
            moveHandsCoroutine = StartCoroutine(ReweighingCoroutine(false));
        }

        IEnumerator ReweighingCoroutine(bool isToMain)
        {
            float mainRigWeight = isToMain ? 1 : 0;
            float secondaryRigWeight = isToMain ? 0 : 1;

            var isDone = IsDone(mainRigWeight, secondaryRigWeight);
            
            while (!isDone)
            {
                mainRig.SetRigWeight(mainRigWeight);
                secondaryRig.SetRigWeight(secondaryRigWeight);

                isDone = IsDone(mainRigWeight, secondaryRigWeight);
                yield return null;
            }

            OnWeighingDone?.Invoke();
        }

        public void ResetWeights()
        {
            mainRig.ResetRigWeight();
            secondaryRig.ResetRigWeight();
        }

        private bool IsDone(float startRigWeight, float hitRigWeight)
        {
            bool isDone = Math.Abs(mainRig.CurrentWeight - startRigWeight) < 0.05f &&
                          Math.Abs(secondaryRig.CurrentWeight - hitRigWeight) < 0.05f;
            
            return isDone;
        }
    }
}