using Nav3D.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Nav3D.Obstacles
{
    public class SpatialGrid
    {
        #region Attributes

        int m_LayersCount;
        float m_MinBucketSize;
        //0 is the lowest resolution, resp. the largest bucket size
        //(m_LayersCount - 1) is m_MinBucketSize resolution
        List<ConcurrentDictionary<Vector3Int, Leaf>> m_Layers;

        GraphConstructionProgress m_ConstructionProgress;

        #endregion

        #region Properties

        public int MinLayer => 0;
        public int MaxLayer => m_LayersCount - 1;

        #endregion

        #region Constructors

        public SpatialGrid(int _LayersCount, float _MinBucketSize, GraphConstructionProgress _ConstructionProgress)
        {
            m_LayersCount = _LayersCount;
            m_MinBucketSize = _MinBucketSize;
            m_Layers = new List<ConcurrentDictionary<Vector3Int, Leaf>>(m_LayersCount);

            m_ConstructionProgress = _ConstructionProgress;

            for (int i = 0; i < m_LayersCount; i++)
                m_Layers.Add(new ConcurrentDictionary<Vector3Int, Leaf>());
        }

        #endregion

        #region Public methods

        public void SubstractLeafFromProgress(int _GridLayer, bool _Processed)
        {
            int prunedLeavesCount = GetPrunedPower(_GridLayer, MaxLayer);

            m_ConstructionProgress.AddTotalLeavesCount(-prunedLeavesCount + (_Processed ? 1 : 0));

            if (_Processed)
                m_ConstructionProgress.AddProcessedLeavesCount(1);
        }

        int GetPrunedPower(int _Layer, int _MaxLayer)
        {
            return (int)Mathf.Pow(8, _MaxLayer - _Layer);
        }

        /// <summary>
        /// Inserts leaf at buckets grid on layer.
        /// </summary>
        /// <param name="_Layer">The layer to insert in.</param>
        /// <param name="_Leaf">The leaf.</param>
        /// <returns>The index on layer.</returns>
        public Vector3Int AddLeafOnLayer(int _Layer, Leaf _Leaf)
        {
            Vector3Int index = GetBucketIndexOnLayer(_Layer, _Leaf);

            m_Layers[_Layer].TryAdd(index, _Leaf);

            return index;
        }

        public Vector3Int GetBucketIndexOnLayer(int _Layer, Leaf _Leaf)
        {
            return UtilsMath.GetBucketIndex(_Leaf.Bounds.center, GetBucketSizeOnLayer(_Layer));
        }

        public void FormLeavesConnections(GraphConstructionProgress _Progress)
        {
            if (m_Layers.IsNullOrEmpty())
                return;

            //for each layer
            for (int i = 0; i < m_Layers.Count; i++)
            {
                ConcurrentDictionary<Vector3Int, Leaf> layer = m_Layers[i];

                //for each leaf in layer
                foreach (KeyValuePair<Vector3Int, Leaf> kvp in layer)
                {
                    Vector3Int index = kvp.Key;
                    Leaf leaf = kvp.Value;

                    Vector3 cellCenter = leaf.Bounds.center;

                    foreach (Vector3Int direction in leaf.FacesDirections)
                    {
                        Vector3Int adjacentIndex = index + direction;

                        Leaf adjacentLeaf = null;

                        //try to find a free adjacent leaf on the same layer
                        if (layer.TryGetValue(adjacentIndex, out adjacentLeaf))
                        {
                            if (!adjacentLeaf.Occupied)
                            {
                                leaf.AddFreeAdjacent(adjacentLeaf);

                                if (leaf.Occupied)
                                    continue;

                                adjacentLeaf.AddFreeAdjacent(leaf);
                                //no need to look for adjacent in this direction
                                adjacentLeaf.RemoveFaceConsideration(-direction);
                            }

                            continue;
                        }
                        else
                        {
                            Vector3 adjacentCellCenter = leaf.Bounds.center + new Vector3(
                            direction.x * leaf.Size,
                            direction.y * leaf.Size,
                            direction.z * leaf.Size);

                            float curLeafSize = leaf.Size * 2f;
                            int curLayerIndex = i - 1;

                            Vector3Int curLeafIndex = UtilsMath.GetBucketIndex(cellCenter, curLeafSize);
                            adjacentIndex = UtilsMath.GetBucketIndex(adjacentCellCenter, curLeafSize);

                            while (curLayerIndex >= 0 && curLeafIndex != adjacentIndex)
                            {
                                if (m_Layers[curLayerIndex].TryGetValue(adjacentIndex, out adjacentLeaf))
                                    break;

                                //go a higher level
                                curLayerIndex--;
                                curLeafSize *= 2f;

                                curLeafIndex = UtilsMath.GetBucketIndex(cellCenter, curLeafSize);
                                adjacentIndex = UtilsMath.GetBucketIndex(adjacentCellCenter, curLeafSize);
                            }

                            if (adjacentLeaf is { Occupied: false })
                            {
                                leaf.AddFreeAdjacent(adjacentLeaf);

                                if (!leaf.Occupied)
                                    adjacentLeaf.AddFreeAdjacent(leaf);
                            }
                        }
                    }
                }

                _Progress.SetLayersProcessignProgress(i, m_LayersCount);
            }
        }

        public static Vector3Int GetLevelUpIndex(Vector3Int _Index)
        {
            return new Vector3Int(
                                    Mathf.FloorToInt(_Index.x * 0.5f),
                                    Mathf.FloorToInt(_Index.y * 0.5f),
                                    Mathf.FloorToInt(_Index.z * 0.5f)
                                    );
        }

        #endregion

        #region Service methods

        float GetBucketSizeOnLayer(int _Layer)
        {
            return m_MinBucketSize * Mathf.Pow(2, m_LayersCount - _Layer - 1);
        }

        #endregion
    }
}
