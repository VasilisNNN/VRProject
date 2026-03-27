using System.IO;
using UnityEngine;

namespace Nav3D.Obstacles.Serialization
{
    class ForkSerializable : NodeSerializable
    {
        #region Properties

        public int[] ChildIDs { get; set; }
        protected override NodeSerializableType NodeType => NodeSerializableType.FORK;

        #endregion

        #region Constructors

        public ForkSerializable(Bounds _Bounds, byte _GridLayer, bool _Occupied, int[] _ChildIDs, int _ID) : base(_Bounds, _GridLayer, _Occupied, _ID)
        {
            ChildIDs = _ChildIDs;
        }

        public ForkSerializable(BinaryReader _Reader) : base(_Reader)
        {
            int childIDsCount = _Reader.ReadInt32();

            ChildIDs = new int[childIDsCount];

            for (int i = 0; i < childIDsCount; i++)
            {
                ChildIDs[i] = _Reader.ReadInt32();
            }
        }

        #endregion

        #region Public methods

        public override void WriteIntoBinary(BinaryWriter _Writer)
        {
            base.WriteIntoBinary(_Writer);

            _Writer.Write(ChildIDs.Length);

            foreach (int childID in ChildIDs)
            {
                _Writer.Write(childID);
            }
        }

        public override Node GetDeserializedInstance()
        {
            return new Fork(GetDeserializedBounds(), GridLayer, Occupied > 0, ID);
        }

        #endregion
    }
}