using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.GameModule.Scripts.Environment
{
    public class LightSwitcher : MonoBehaviour
    {
        [SerializeField] private List<Light> lights;
        [SerializeField] private Transform switcher;
        [SerializeField] private Vector3 enabledRotation;
        [SerializeField] private Vector3 disabledRotation;

        private Coroutine switcherAnimation;
        
        public bool isActive;

        private void Awake()
        {
            SetLightActive(isActive);
        }

        private void OnMouseDown()
        {
            if(switcherAnimation != null) StopCoroutine(switcherAnimation);
            
            isActive = !isActive;
            SetLightActive(isActive);

            switcherAnimation = StartCoroutine(SwitcherAnimation());
            
            Debug.Log($"Light is {isActive}");
        }

        private void SetLightActive(bool isActive)
        {
            foreach (var light in lights)
            {
                light.enabled = isActive;
            }
        }

        IEnumerator SwitcherAnimation()
        {
            Vector3 rotation = isActive ? enabledRotation : disabledRotation;

            var targetRotation =
                Quaternion.Lerp(switcher.localRotation, Quaternion.Euler(rotation), Time.deltaTime * 10f);

            switcher.localRotation = targetRotation;

            yield return new WaitForEndOfFrame();
            
            if(switcher.localRotation == Quaternion.Euler(rotation)) yield break;
            
            switcherAnimation = StartCoroutine(SwitcherAnimation());
        }
    }
}