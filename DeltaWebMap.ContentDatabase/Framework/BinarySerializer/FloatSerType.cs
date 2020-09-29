using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class FloatSerType : IBinarySerType
    {
        private float value;

        public FloatSerType(float value)
        {
            this.value = value;
        }
        
        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_FLOAT;

        public override int GetLength()
        {
            return 4;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, bufferPos);
        }
    }
}
