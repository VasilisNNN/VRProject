using UnityEngine;
using System.Globalization;

namespace Nav3D.Common
{
    public static class ExtensionVector3
    {
        #region Static methods

        public static string ToStringExt(this Vector3 _Vector)
        {
            return $"({{{_Vector.x.ToString(CultureInfo.InvariantCulture)}}} " +
                   $"{{{_Vector.y.ToString(CultureInfo.InvariantCulture)}}} " +
                   $"{{{_Vector.z.ToString(CultureInfo.InvariantCulture)}}})";
        }

        #endregion
    }
}
