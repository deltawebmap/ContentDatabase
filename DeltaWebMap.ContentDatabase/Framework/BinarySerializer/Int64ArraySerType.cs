using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int64ArraySerType : IBinarySerType
    {
        private long[] value;

        public Int64ArraySerType(long[] value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT64_ARRAY;

        public override int GetLength()
        {
            return 8 * value.Length;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            for (int i = 0; i < value.Length; i++)
            {
                BitConverter.GetBytes(value[i]).CopyTo(buffer, bufferPos + (8 * i));
            }
        }
    }
}
