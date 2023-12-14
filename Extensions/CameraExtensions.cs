using UnityEngine;

namespace Aplem.Common
{
    public static class CameraExtensions
    {
        public static Vector2 GetWorldPosition2D(this Camera camera, Vector2 screenPosition)
        {
            return camera.ScreenToWorldPoint(screenPosition);
        }
    }
}
