using UnityEngine;

namespace Modules.GameModule.Scripts.Character.StateMachine
{
    [CreateAssetMenu(menuName = "Scriptables/State", fileName = "State", order = 0)]
    public abstract class State<T> : ScriptableObject where T : MonoBehaviour
    {
        protected T runner;

        public virtual void Init(T parent)
        {
            runner = parent;
        }

        public abstract void Update();
        
        public abstract void FixedUpdate();
        

        public abstract void CheckState();
        
        public abstract void ChangeState();
        
        public abstract void Exit();
    }
}