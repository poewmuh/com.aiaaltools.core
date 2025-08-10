using UnityEngine;

namespace AiaalTools
{
    public class LookAtCamera : MonoBehaviour
    {
        private void Start()
        {
            var cameraTransform = Camera.main.transform;
            if (cameraTransform != null)
            {
                transform.LookAt(transform.position + cameraTransform.forward);
            }
        }
    }
}