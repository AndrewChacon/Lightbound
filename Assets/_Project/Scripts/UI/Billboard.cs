using UnityEngine;

namespace Lightbound
{
    /// <summary>
    /// Keeps this object's rotation matched to the camera's, so world-space
    /// labels stay screen-aligned and readable regardless of viewing angle.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        private Transform _camera;

        private void Start()
        {
            _camera = Camera.main != null ? Camera.main.transform : null;
        }

        private void LateUpdate()
        {
            if (_camera == null) return;
            transform.rotation = _camera.rotation;
        }
    }
}
