using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class DoubleSerType : IBinarySerType
    {
        private double value;

        public DoubleSerType(double value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_DOUBLE;

        public override int GetLength()
        {
            return 8;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, bufferPos);
        }
    }
}
