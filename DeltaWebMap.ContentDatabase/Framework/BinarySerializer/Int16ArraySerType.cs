using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int16ArraySerType : IBinarySerType
    {
        private short[] value;

        public Int16ArraySerType(short[] value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT16_ARRAY;

        public override int GetLength()
        {
            return 2 * value.Length;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            for (int i = 0; i < value.Length; i++)
            {
                BitConverter.GetBytes(value[i]).CopyTo(buffer, bufferPos + (2 * i));
            }
        }
    }
}
