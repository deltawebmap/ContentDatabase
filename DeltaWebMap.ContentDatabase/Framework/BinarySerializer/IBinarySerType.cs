using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public abstract class IBinarySerType
    {
        public abstract short SerTypeId { get; }
        public abstract int GetLength();
        public abstract void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable);
    }
}
