using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class StringSerType : IBinarySerType
    {
        private string value;

        public StringSerType(string value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_STRING;

        public override int GetLength()
        {
            return Encoding.UTF8.GetByteCount(value);
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            Encoding.UTF8.GetBytes(value).CopyTo(buffer, bufferPos);
        }
    }
}
