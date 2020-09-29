using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int8ArraySerType : IBinarySerType
    {
        private byte[] value;

        public Int8ArraySerType(byte[] value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT8_ARRAY;

        public override int GetLength()
        {
            return value.Length;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            Array.Copy(value, 0, buffer, bufferPos, value.Length);
        }
    }
}
