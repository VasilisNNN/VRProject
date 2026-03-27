using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nav3D.Common
{
    public static class UtilsCommon
    {
        #region Nested types

        class RandomSeedInvariant : IDisposable
        {
            #region Atributes

            UnityEngine.Random.State m_RandomStateBuffer;

            #endregion

            #region Construction

            public RandomSeedInvariant()
            {
                m_RandomStateBuffer = UnityEngine.Random.state;
            }

            #endregion

            #region IDisposable methods

            public void Dispose() => UnityEngine.Random.state = m_RandomStateBuffer;

            #endregion
        }

        #endregion

        #region Properties

        public static IDisposable RandomSeedPermanence => new RandomSeedInvariant();

        #endregion

        #region Public methods

        public static Color[] GetNDistanColors(int _N)
        {
            Color[] result = new Color[_N];

            float step = (360f / _N) / 360f;
            float hue = 0;

            for (int i = 0; i < _N; i++)
            {
                result[i] = Color.HSVToRGB(hue, 1f, 1f);

                hue += step;
            }

            return result;
        }

        public static void SmartDestroy(UnityEngine.Object _Object)
        {
            if (_Object == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject gameObjectRef = null;

                if (_Object is Component component)
                    gameObjectRef = component.gameObject;

                Object.DestroyImmediate(_Object);

                if (gameObjectRef != null)
                    Object.DestroyImmediate(gameObjectRef);
            }
            else
#endif
            {
                Object.Destroy(_Object);
            }
        }

        #endregion
    }
}
