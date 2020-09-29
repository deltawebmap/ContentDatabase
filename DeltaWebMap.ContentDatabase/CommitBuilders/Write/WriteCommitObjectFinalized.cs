using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.CommitBuilders.Write
{
    /// <summary>
    /// Serialized version of a WriteCommitObject
    /// </summary>
    public class WriteCommitObjectFinalized
    {
        public readonly ulong objectId;
        public readonly int groupId;
        public readonly byte[] blob;

        public WriteCommitObjectFinalized(WriteCommitObject obj, byte[] ser)
        {
            objectId = obj.objectId;
            groupId = obj.groupId;
            blob = ser;
        }
    }
}
