using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using DeltaWebMap.ContentDatabase.Framework;
using DeltaWebMap.ContentDatabase.Framework.BinarySerializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase
{
    public class WriteObject
    {
        internal Dictionary<string, IBinarySerType> values;

        public WriteObject()
        {
            values = new Dictionary<string, IBinarySerType>();
        }

        public const int ENTRY_HEADER_BYTES = 8;
        public const int HEADER_BYTES = 4;

        /* Private API */

        internal void SerializeTo(INameTableProvider nameTable, byte[] buffer, int bufferPos)
        {
            //Write count
            BitConverter.GetBytes((int)values.Count).CopyTo(buffer, bufferPos);
            bufferPos += 4;

            //Loop through
            foreach (var v in values)
            {
                //Lookup key name and write it's index
                short keyIndex = nameTable.GetNameTableIndex(v.Key);
                BitConverter.GetBytes(keyIndex).CopyTo(buffer, bufferPos + 0);

                //Get the type ID and write it
                short typeId = v.Value.SerTypeId;
                BitConverter.GetBytes(typeId).CopyTo(buffer, bufferPos + 2);

                //Get the length and write it
                int length = v.Value.GetLength();
                BitConverter.GetBytes(length).CopyTo(buffer, bufferPos + 4);

                //Now serialize to the buffer
                v.Value.Serialize(buffer, bufferPos + ENTRY_HEADER_BYTES, nameTable);

                //Update
                bufferPos += length + ENTRY_HEADER_BYTES;
            }
        }

        internal int GetLength()
        {
            //Get the total length of all values
            int length = (ENTRY_HEADER_BYTES * values.Count) + HEADER_BYTES;
            foreach (var v in values)
                length += v.Value.GetLength();
            return length;
        }

        private void AddValue(string key, IBinarySerType value)
        {
            values.Add(key, value);
        }

        /* Public API */

        public WriteObject WriteInt8(string key, byte value)
        {
            AddValue(key, new Int8SerType(value));
            return this;
        }

        public WriteObject WriteInt16(string key, short value)
        {
            AddValue(key, new Int16SerType(value));
            return this;
        }

        public WriteObject WriteInt32(string key, int value)
        {
            AddValue(key, new Int32SerType(value));
            return this;
        }

        public WriteObject WriteInt64(string key, long value)
        {
            AddValue(key, new Int64SerType(value));
            return this;
        }

        public WriteObject WriteFloat(string key, float value)
        {
            AddValue(key, new FloatSerType(value));
            return this;
        }

        public WriteObject WriteInt8Array(string key, byte[] value)
        {
            AddValue(key, new Int8ArraySerType(value));
            return this;
        }

        public WriteObject WriteInt16Array(string key, short[] value)
        {
            AddValue(key, new Int16ArraySerType(value));
            return this;
        }

        public WriteObject WriteInt32Array(string key, int[] value)
        {
            AddValue(key, new Int32ArraySerType(value));
            return this;
        }

        public WriteObject WriteInt64Array(string key, long[] value)
        {
            AddValue(key, new Int64ArraySerType(value));
            return this;
        }

        public WriteObject WriteFloatArray(string key, float[] value)
        {
            AddValue(key, new FloatArraySerType(value));
            return this;
        }

        public WriteObject WriteString(string key, string value)
        {
            AddValue(key, new StringSerType(value));
            return this;
        }

        public WriteObject WriteStringArray(string key, string[] value)
        {
            AddValue(key, new StringArraySerType(value));
            return this;
        }

        public WriteObject WriteChildObject(string key, WriteObject value)
        {
            AddValue(key, new ObjectSerType(value));
            return this;
        }

        public WriteObject WriteChildObjectArray(string key, WriteObject[] value)
        {
            AddValue(key, new ObjectArraySerType(value));
            return this;
        }

        public WriteObject WriteBool(string key, bool value)
        {
            AddValue(key, new BoolSerType(value));
            return this;
        }

        public WriteObject WriteDouble(string key, double value)
        {
            AddValue(key, new DoubleSerType(value));
            return this;
        }

        public WriteObject WriteDoubleArray(string key, double[] value)
        {
            AddValue(key, new DoubleArraySerType(value));
            return this;
        }

        public WriteObject WriteStringEnum(string key, string value, string[] possibleValues)
        {
            AddValue(key, new StringEnumSerType(value, possibleValues));
            return this;
        }
    }
}
