using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int32ArraySerType : IBinarySerType
    {
        private int[] value;

        public Int32ArraySerType(int[] value)
        {
            this.value = value;
        }
        
        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT32_ARRAY;

        public override int GetLength()
        {
            return 4 * value.Length;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            for(int i = 0; i<value.Length; i++)
            {
                BitConverter.GetBytes(value[i]).CopyTo(buffer, bufferPos + (4 * i));
            }
        }
    }
}
