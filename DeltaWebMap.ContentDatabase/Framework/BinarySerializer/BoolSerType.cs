using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class BoolSerType : IBinarySerType
    {
        private bool value;

        public BoolSerType(bool value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_BOOL;

        public override int GetLength()
        {
            return 1;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            if (value)
                buffer[bufferPos] = 0x01;
            else
                buffer[bufferPos] = 0x00;
        }
    }
}
