using UnityEngine;

namespace Spectral.Core.Math
{
    public static class Vector3Extensions
    {
        public static float MaxComponent(this Vector3 a)
        {
            return Mathf.Max(a.x, Mathf.Max(a.y, a.z));
        }
    }
}