using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class Int32SerType : IBinarySerType
    {
        private int value;

        public Int32SerType(int value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_INT32;

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
