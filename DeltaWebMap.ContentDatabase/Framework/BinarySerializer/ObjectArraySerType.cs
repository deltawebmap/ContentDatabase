using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class ObjectArraySerType : IBinarySerType
    {
        private WriteObject[] value;

        public ObjectArraySerType(WriteObject[] value)
        {
            this.value = value;
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_OBJECT_ARRAY;

        public override int GetLength()
        {
            //Total
            int length = 4;
            foreach (var v in value)
                length += v.GetLength();
            return length;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            //Write total length
            BitConverter.GetBytes((int)value.Length).CopyTo(buffer, bufferPos);
            int offset = 4;

            //Write each
            foreach(var v in value)
            {
                //Serialize
                v.SerializeTo(nameTable, buffer, bufferPos + offset);

                //Write
                offset += v.GetLength();
            }

            //Raise exception if this failed
            if (offset != GetLength())
                throw new Exception("Fatal error serializing OBJECT ARRAY data. The length expected did not match the length used.");
        }
    }
}
