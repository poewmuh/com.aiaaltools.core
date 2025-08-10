using UnityEngine;

namespace AiaalTools
{
    public abstract class RareUpdateBehaviour : MonoBehaviour
    {
        protected virtual byte frameRate => 3;
        protected float rareTickDelta;
        private byte _stepHandlerFrameCounter;
        
        protected abstract void RareUpdate();

        private void Update()
        {
            if (_stepHandlerFrameCounter % frameRate > 0)
            {
                _stepHandlerFrameCounter++;
                return;
            }
            
            rareTickDelta = Time.deltaTime * frameRate;
            RareUpdate();
            
            _stepHandlerFrameCounter++;
        }
    }
}