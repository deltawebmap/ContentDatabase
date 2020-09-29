using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int16SerType : IBinarySerType
    {
        private short value;
        
        public Int16SerType(short value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT16;

        public override int GetLength()
        {
            return 2;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, bufferPos);
        }
    }
}
