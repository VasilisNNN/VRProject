using System.IO;
using UnityEngine;

namespace Nav3D.Obstacles.Serialization
{
    class LeafSerializable : NodeSerializable
    {
        #region Properties

        public int[] FreeAdjacentIDs { get; set; }
        protected override NodeSerializableType NodeType => NodeSerializableType.LEAF;

        #endregion

        #region Constructors

        public LeafSerializable(Bounds _Bounds, byte _GridLayer, bool _Occupied, int[] _FreeAdjacentIDs, int _ID) : base(_Bounds, _GridLayer, _Occupied, _ID)
        {
            FreeAdjacentIDs = _FreeAdjacentIDs;
        }

        public LeafSerializable(BinaryReader _Reader) : base(_Reader)
        {
            int freeAdjacentIDsCount = _Reader.ReadInt32();

            FreeAdjacentIDs = new int[freeAdjacentIDsCount];

            for (int i = 0; i < freeAdjacentIDsCount; i++)
            {
                FreeAdjacentIDs[i] = _Reader.ReadInt32();
            }
        }

        #endregion

        #region Public methods

        public override void WriteIntoBinary(BinaryWriter _Writer)
        {
            base.WriteIntoBinary(_Writer);

            _Writer.Write(FreeAdjacentIDs.Length);

            foreach (int childID in FreeAdjacentIDs)
            {
                _Writer.Write(childID);
            }
        }

        public override Node GetDeserializedInstance()
        {
            return new Leaf(GetDeserializedBounds(), GridLayer, Occupied > 0, ID);
        }

        #endregion
    }
}