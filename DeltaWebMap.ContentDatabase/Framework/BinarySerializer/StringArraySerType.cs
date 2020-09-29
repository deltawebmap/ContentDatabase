using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class StringArraySerType : IBinarySerType
    {
        private string[] value;
        private int[] valueLengths;

        public StringArraySerType(string[] value)
        {
            this.value = value;

            //Compute lengths now
            valueLengths = new int[value.Length];
            for(int i = 0; i<value.Length; i++)
            {
                valueLengths[i] = Encoding.UTF8.GetByteCount(value[i]);
            }
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_STRING_ARRAY;

        public override int GetLength()
        {
            //Calculate
            int length = 4;
            foreach (var l in valueLengths)
                length += 4 + l;
            return length;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            //Write string count
            BitConverter.GetBytes((int)value.Length).CopyTo(buffer, bufferPos);
            int offset = 0;

            //Write each
            for(int i = 0; i<value.Length; i+=1)
            {
                //Write string length
                BitConverter.GetBytes(valueLengths[i]).CopyTo(buffer, bufferPos + offset);
                offset += 4;

                //Write string
                Encoding.UTF8.GetBytes(value[i]).CopyTo(buffer, bufferPos + offset);
                offset += valueLengths[i];
            }
        }
    }
}
