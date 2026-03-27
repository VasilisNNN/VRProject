using UnityEngine;
using Nav3D.Common;
using System.Collections.Generic;

namespace Nav3D.Obstacles
{
    public abstract class ObstacleInfoBase
    {
        #region Constructors

        public ObstacleInfoBase() { }

        #endregion

        #region Properties

        /// <summary>
        /// Common bounds embracing all obstacle generative geometries.
        /// </summary>
        public Bounds Bounds { get; protected set; }
        /// <summary>
        /// All triangles compositing obstacle geometries.
        /// </summary>
        public List<Triangle> Triangles { get; protected set; }
        public abstract List<int> IDs { get; }

        #endregion

        #region Public methods

        public abstract void ReplaceID(int _OldID, int _NewID);

        public override string ToString()
        {
            return $"\t{GetType().Name}: IDs: {string.Join(", ", IDs)}\n";
        }

        public static List<ObstacleInfoBase> GroupInfos(List<ObstacleInfoBase> _ProcessingInfo)
        {
            List<ObstacleInfoBase> processingInfos = _ProcessingInfo.Copy();

            bool hasIntersections;

            do
            {
                hasIntersections = false;

                for (int i = 0; i < processingInfos.Count; i++)
                {
                    ObstacleInfoBase currentObstacleInfo = processingInfos[i];

                    for (int j = i + 1; j < processingInfos.Count; j++)
                    {
                        ObstacleInfoBase otherObstacleInfo = processingInfos[j];

                        if (currentObstacleInfo.Intersects(otherObstacleInfo))
                        {
                            processingInfos[i] = currentObstacleInfo.CombineWith(otherObstacleInfo);
                            processingInfos.RemoveAt(j);

                            hasIntersections = true;

                            break;
                        }
                    }

                    if (hasIntersections)
                        break;
                }
            } while (hasIntersections);

            return processingInfos;
        }

        public bool Intersects(ObstacleInfoBase _Other)
        {
            return Bounds.Intersects(_Other.Bounds);
        }

        public ObstacleInfoBase CombineWith(ObstacleInfoBase _OtherObstacleInfo)
        {
            return ObstacleInfoGrouped.CombineObstacleInfos(this, _OtherObstacleInfo);
        }

        #endregion

        #region Service methods

        protected void ComputeBounds()
        {
            Bounds = ExtensionBounds.TrianglesBounds(Triangles).CeilSizeToMultiple(ObstacleManager.Instance.MinBucketSize);
        }

        #endregion
    }
}