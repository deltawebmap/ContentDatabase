using DeltaWebMap.ContentDatabase.Framework;
using DeltaWebMap.ContentDatabase.Framework.BinarySerializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.CommitBuilders.Write
{
    /// <summary>
    /// Serializes writeable data
    /// </summary>
    public class WriteCommitObject : WriteObject
    {
        public readonly ulong objectId;
        public readonly int groupId;

        public WriteCommitObject(ulong objectId, int groupId)
        {
            this.objectId = objectId;
            this.groupId = groupId;
        }

        public const short SER_TYPE_INT8 = 0;
        public const short SER_TYPE_INT16 = 1;
        public const short SER_TYPE_INT32 = 2;
        public const short SER_TYPE_INT64 = 3;
        public const short SER_TYPE_FLOAT = 4;

        public const short SER_TYPE_INT8_ARRAY = 5;
        public const short SER_TYPE_INT16_ARRAY = 6;
        public const short SER_TYPE_INT32_ARRAY = 7;
        public const short SER_TYPE_INT64_ARRAY = 8;
        public const short SER_TYPE_FLOAT_ARRAY = 9;

        public const short SER_TYPE_STRING = 10;
        public const short SER_TYPE_STRING_ARRAY = 11;

        public const short SER_TYPE_OBJECT = 12;
        public const short SER_TYPE_OBJECT_ARRAY = 13;

        public const short SER_TYPE_BOOL = 14;
        public const short SER_TYPE_BOOL_ARRAY = 15; //not currently implimented

        public const short SER_TYPE_DOUBLE = 16;
        public const short SER_TYPE_DOUBLE_ARRAY = 17;

        public const short SER_TYPE_STRING_ENUM = 18;
    }
}
