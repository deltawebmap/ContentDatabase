using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int64SerType : IBinarySerType
    {
        private long value;

        public Int64SerType(long value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT64;

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
