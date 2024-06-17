
using UnityEngine;
using Unity.Mathematics;


namespace Aplem.Common
{
    public static class CollisionUtil
    {
        public static bool CheckCollideCircleLine(Vector2 center, float radius, Vector2 start, Vector2 end)
        {
            var line = end - start;
            var lineDir = line.normalized;
            var startToCenter = center - start;
            var lineDistance = math.abs(startToCenter.x * lineDir.y - startToCenter.y * lineDir.x);
            if (lineDistance > radius)
                return false;

            var endToCenter = center - end;
            if (Vector2.Dot(startToCenter, line) * Vector2.Dot(endToCenter, line) <= 0)
            {
                return true;
            }

            var sqrRadius = radius * radius;
            if(sqrRadius >= startToCenter.sqrMagnitude || sqrRadius >= endToCenter.sqrMagnitude)
            {
                return true;
            }

            return false;
        }
    }
}