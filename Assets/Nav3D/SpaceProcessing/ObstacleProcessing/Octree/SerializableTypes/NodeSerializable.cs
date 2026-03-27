using UnityEngine;
using System;
using System.IO;

namespace Nav3D.Obstacles.Serialization
{
    public abstract class NodeSerializable
    {
        #region Constants

        readonly static string UNKNOWN_NODE_TYPE = $"Unknown {nameof(NodeSerializableType)} value: {{0}}";

        #endregion

        #region Nested type

        public enum NodeSerializableType
        {
            FORK = 0,
            LEAF = 1
        }

        #endregion

        #region Factory methods

        public static NodeSerializable ReadFromBytes(BinaryReader _Reader)
        {
            NodeSerializableType nodeType = (NodeSerializableType)_Reader.ReadInt32();

            if (nodeType == NodeSerializableType.FORK)
                return new ForkSerializable(_Reader);
            else if
                (nodeType == NodeSerializableType.LEAF)
                return new LeafSerializable(_Reader);

            throw new Exception(string.Format(UNKNOWN_NODE_TYPE, nodeType));
        }

        #endregion

        #region Properties

        public int ID { get; set; }
        public byte GridLayer { get; set; }
        public byte Occupied { get; set; }
        public float BoundsSizeX { get; set; }
        public float BoundsSizeY { get; set; }
        public float BoundsSizeZ { get; set; }
        public float BoundsCenterX { get; set; }
        public float BoundsCenterY { get; set; }
        public float BoundsCenterZ { get; set; }

        protected abstract NodeSerializableType NodeType { get; }

        #endregion

        #region Constructors

        public NodeSerializable(Bounds _Bounds, byte _GridLayer, bool _Occupied, int _ID)
        {
            ID = _ID;
            GridLayer = _GridLayer;
            Occupied = (byte)(_Occupied ? 1 : 0);

            BoundsSizeX = _Bounds.size.x;
            BoundsSizeY = _Bounds.size.y;
            BoundsSizeZ = _Bounds.size.z;

            BoundsCenterX = _Bounds.center.x;
            BoundsCenterY = _Bounds.center.y;
            BoundsCenterZ = _Bounds.center.z;
        }

        protected NodeSerializable(BinaryReader _Reader)
        {
            ID = _Reader.ReadInt32();
            GridLayer = _Reader.ReadByte();
            Occupied = _Reader.ReadByte();
            BoundsSizeX = _Reader.ReadSingle();
            BoundsSizeY = _Reader.ReadSingle();
            BoundsSizeZ = _Reader.ReadSingle();
            BoundsCenterX = _Reader.ReadSingle();
            BoundsCenterY = _Reader.ReadSingle();
            BoundsCenterZ = _Reader.ReadSingle();
        }

        #endregion

        #region Public methods

        public virtual void WriteIntoBinary(BinaryWriter _Writer)
        {
            _Writer.Write((int)NodeType);
            _Writer.Write(ID);
            _Writer.Write(GridLayer);
            _Writer.Write(Occupied);
            _Writer.Write(BoundsSizeX);
            _Writer.Write(BoundsSizeY);
            _Writer.Write(BoundsSizeZ);
            _Writer.Write(BoundsCenterX);
            _Writer.Write(BoundsCenterY);
            _Writer.Write(BoundsCenterZ);
        }

        public abstract Node GetDeserializedInstance();

        protected Bounds GetDeserializedBounds()
        {
            return new Bounds(new Vector3(BoundsCenterX, BoundsCenterY, BoundsCenterZ), new Vector3(BoundsSizeX, BoundsSizeY, BoundsSizeZ));
        }

        #endregion
    }
}