using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class ObjectSerType : IBinarySerType
    {
        private WriteObject value;

        public ObjectSerType(WriteObject value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_OBJECT;

        public override int GetLength()
        {
            return value.GetLength();
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            value.SerializeTo(nameTable, buffer, bufferPos);
        }
    }
}
