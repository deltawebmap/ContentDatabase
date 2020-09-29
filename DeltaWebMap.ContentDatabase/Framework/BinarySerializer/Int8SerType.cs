using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int8SerType : IBinarySerType
    {
        private byte value;

        public Int8SerType(byte value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT8;

        public override int GetLength()
        {
            return 1;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            buffer[bufferPos] = value;
        }
    }
}
