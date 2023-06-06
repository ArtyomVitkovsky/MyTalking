using UnityEngine;

namespace Modules.GameModule.Scripts.Ragdoll
{
    public class DragRigidbody : MonoBehaviour 
    { 
        [SerializeField] private float maxDistance = 100.0f; 
    
        [SerializeField] private float spring = 50.0f; 
        [SerializeField] private float damper = 5.0f; 
        [SerializeField] private float drag = 10.0f; 
        [SerializeField] private float angularDrag = 5.0f; 
        [SerializeField] private float distance = 0.2f; 
        [SerializeField] private bool attachToCenterOfMass = false; 
 
        [SerializeField] private SpringJoint springJoint;
        private float oldDrag;
        private float oldAngularDrag;

        public void DragObject(RaycastHit hit, Rigidbody grabbedRb) 
        {
            // if(!springJoint) 
            // { 
            //     GameObject go = new GameObject("Rigidbody dragger"); 
            //     Rigidbody body = go.AddComponent<Rigidbody>(); 
            //     body.isKinematic = true; 
            //     springJoint = go.AddComponent<SpringJoint>(); 
            // } 
        
            springJoint.transform.position = hit.point;
            if (springJoint.connectedBody != grabbedRb || springJoint.connectedBody == null)
            {
                oldDrag = grabbedRb.drag;
                oldAngularDrag = grabbedRb.angularDrag;

                springJoint.connectedBody = grabbedRb;
                
                if (attachToCenterOfMass)
                {
                    Vector3 anchor = transform.TransformDirection(grabbedRb.centerOfMass) +
                                     grabbedRb.transform.position;
                    anchor = springJoint.transform.InverseTransformPoint(anchor);
                    springJoint.anchor = anchor;
                }
                else
                {
                    springJoint.anchor = Vector3.zero;
                }

                springJoint.spring = spring;
                springJoint.damper = damper;
                springJoint.maxDistance = distance;
            }

            DragObject();
        } 
    
        private void DragObject() 
        {
            springJoint.connectedBody.drag = drag;
            springJoint.connectedBody.angularDrag = angularDrag;
        }

        public void Reset()
        {
            if (springJoint.connectedBody)
            {
                springJoint.connectedBody.drag = oldDrag;
                springJoint.connectedBody.angularDrag = oldAngularDrag;
                springJoint.connectedBody = null; 
            } 
        }
    }
}