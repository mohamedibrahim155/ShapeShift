using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.Utilities
{
    public static class MathUtils
    {
        public static float GetAngleBetweenVectors(Vector2 from, Vector2 to)
        {
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
            return (angle + 360) % 360; // Normalize angle to [0, 360)
        }

        public static Vector2 GetDirectionFromAngle(float angleInDegrees)
        {
            float radians = angleInDegrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
        }

        /// <summary>
        ///  Remapped value of float to loat. For example, if you want to remap 0.5 from range 0-1 to range 0-100, the result will be 50.
        /// </summary>
        /// <param name="inMin"></param>
        /// <param name="inMax"></param>
        /// <param name="outMin"></param>
        /// <param name="outMax"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static float Remap(float val, float in1, float in2, float out1, float out2 )
        {
          float inverseLerp = Mathf.InverseLerp(in1, in2, val);
            return Mathf.Lerp(out1, out2, inverseLerp); ;
        }

        public static Vector3 GetDirection(Transform a, Transform b)
        {
            return GetDirection(a.position, b.position);
        }

        public static Vector3 GetDirection(Vector3 a, Vector3 b)
        {
            return (b - a).normalized;
        }

        public static float GetTranformDistance(Transform a, Transform b)
        {
            return Vector3.Distance(a.position, b.position);
        }



    }
}
