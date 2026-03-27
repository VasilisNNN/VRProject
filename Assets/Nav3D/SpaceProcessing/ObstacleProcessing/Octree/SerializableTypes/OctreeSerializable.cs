using System.IO;
using System.Collections.Generic;

namespace Nav3D.Obstacles.Serialization
{
    public class OctreeSerializable
    {
        #region Properties

        public int ID { get; set; }
        public int LayersCount { get; set; }
        public float MinBucketSizeBase { get; set; }
        public float MinBucketSizeReal { get; set; }
        public int[] RootIDs { get; set; }
        public NodeSerializable[] Nodes { get; set; }

        #endregion

        #region Constructors

        public OctreeSerializable(NodeSerializable[] _Nodes, List<int> _RootIDs, int _LayersCount, float _MinBucketSizeBase, float _MinBucketSizeReal, int _ID)
        {
            ID = _ID;
            Nodes = _Nodes;
            RootIDs = _RootIDs.ToArray();
            LayersCount = _LayersCount;
            MinBucketSizeBase = _MinBucketSizeBase;
            MinBucketSizeReal = _MinBucketSizeReal;
        }

        public OctreeSerializable(BinaryReader _Reader, ObstacleDeserializingProgress _Progress)
        {
            ID = _Reader.ReadInt32();
            LayersCount = _Reader.ReadInt32();
            MinBucketSizeBase = _Reader.ReadSingle();
            MinBucketSizeReal = _Reader.ReadSingle();

            int rootIDsCount = _Reader.ReadInt32();
            RootIDs = new int[rootIDsCount];

            for (int i = 0; i < rootIDsCount; i++)
            {
                RootIDs[i] = _Reader.ReadInt32();
            }

            int nodeCount = _Reader.ReadInt32();
            Nodes = new NodeSerializable[nodeCount];

            _Progress.SetProgress(1);

            for (int i = 0; i < nodeCount; i++)
            {
                Nodes[i] = NodeSerializable.ReadFromBytes(_Reader);

                _Progress.SetProgress(i / (float)nodeCount);
            }
        }

        #endregion

        #region Public methods

        public void WriteIntoBinary(BinaryWriter _Writer)
        {
            _Writer.Write(ID);
            _Writer.Write(LayersCount);
            _Writer.Write(MinBucketSizeBase);
            _Writer.Write(MinBucketSizeReal);
            _Writer.Write(RootIDs.Length);

            foreach (int rootID in RootIDs)
            {
                _Writer.Write(rootID);
            }

            _Writer.Write(Nodes.Length);

            foreach(NodeSerializable node in Nodes)
            {
                node.WriteIntoBinary(_Writer);
            }
        }

        public Octree GetDeserializedInstance(ObstacleInfoBase _ObstacleInfo, ObstacleDeserializingProgress _Progress)
        {
            Node[] roots = GetRoots(_Progress, out Dictionary<int, Node> deserializedNodes);

            Octree octree = new Octree(
                _ObstacleInfo,
                roots,
                LayersCount,
                MinBucketSizeBase,
                MinBucketSizeReal,
                deserializedNodes.Count
                );

            foreach (KeyValuePair<int, Node> nodeData in deserializedNodes)
                nodeData.Value.SetOctreeReference(octree);

            return octree;
        }

        #endregion

        #region Service methods

        Node[] GetRoots(ObstacleDeserializingProgress _Progress, out Dictionary<int, Node> _DeserializedNodes)
        {
            Dictionary<int, NodeSerializable> treeMap = new Dictionary<int, NodeSerializable>(Nodes.Length);

            foreach (NodeSerializable node in Nodes)
                treeMap.Add(node.ID, node);

            Dictionary<int, Node> deserializedNodes = new Dictionary<int, Node>(treeMap.Count);

            int deserializingCounter = 0;

            //deserialize base Nodes
            foreach (KeyValuePair<int, NodeSerializable> nodeInfo in treeMap)
            {
                deserializedNodes.Add(nodeInfo.Key, nodeInfo.Value.GetDeserializedInstance());

                deserializingCounter++;

                if (deserializingCounter == 100)
                {
                    deserializingCounter = 0;
                    _Progress.SetProgress(deserializingCounter / treeMap.Count);
                }
            }

            int relationsSetCounter = 0;

            //set relations
            foreach (KeyValuePair<int, Node> nodeInfo in deserializedNodes)
            {
                if (nodeInfo.Value is Fork fork)
                {
                    fork.SetChilds(deserializedNodes, ((ForkSerializable)treeMap[nodeInfo.Key]).ChildIDs);
                }
                else if (nodeInfo.Value is Leaf leaf)
                {
                    leaf.SetFreeAdjacents(deserializedNodes, ((LeafSerializable)treeMap[nodeInfo.Key]).FreeAdjacentIDs);
                }

                relationsSetCounter++;

                if (relationsSetCounter == 100)
                {
                    relationsSetCounter = 0;
                    _Progress.SetProgress(relationsSetCounter / treeMap.Count);
                }
            }

            _DeserializedNodes = deserializedNodes;

            List<Node> roots = new List<Node>(RootIDs.Length);

            foreach (int id in RootIDs)
            {
                roots.Add(deserializedNodes[id]);
            }

            return roots.ToArray();
        }

        #endregion
    }
}