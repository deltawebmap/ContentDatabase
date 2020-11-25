using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using DeltaWebMap.ContentDatabase.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase
{
    /// <summary>
    /// Deserialized object from the database
    /// </summary>
    public class DatabaseObject : Dictionary<string, object>
    {
        public readonly ulong object_id;
        public readonly ulong commit_id;
        public readonly int group_id;
        public readonly byte commit_type;

        internal DatabaseObject()
        {
            
        }

        internal DatabaseObject(ulong object_id, ulong commit_id, int group_id, byte commit_type)
        {
            this.object_id = object_id;
            this.commit_id = commit_id;
            this.group_id = group_id;
            this.commit_type = commit_type;
        }

        /// <summary>
        /// Deserializes this object and returns the number of bytes read
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferIndex"></param>
        /// <returns></returns>
        internal int DeserializeObject(INameTableProvider nameTable, byte[] buffer, int bufferIndex)
        {
            //Make counter for offset from buffer index. This'll be the length we eventually return
            int offset = 0;

            //Read the number of objects
            int objectCount = BitConverter.ToInt32(buffer, bufferIndex + offset);
            offset += 4;

            //Read each object
            for(int i = 0; i<objectCount; i++)
            {
                //Read object data
                short keyIndex = BitConverter.ToInt16(buffer, bufferIndex + offset);
                offset += 2;
                short typeId = BitConverter.ToInt16(buffer, bufferIndex + offset);
                offset += 2;
                int length = BitConverter.ToInt32(buffer, bufferIndex + offset);
                offset += 4;

                //Deserialize
                object value = DeserializeSingleObject(nameTable, buffer, bufferIndex + offset, typeId, length);
                offset += length;

                //Add
                Add(nameTable.GetNameTableValue(keyIndex), value);
            }

            return offset;
        }

        private object DeserializeSingleObject(INameTableProvider nameTable, byte[] buffer, int bufferIndex, short typeId, int length)
        {
            switch(typeId)
            {
                case WriteCommitObject.SER_TYPE_INT8: return buffer[bufferIndex];
                case WriteCommitObject.SER_TYPE_INT16: return BitConverter.ToInt16(buffer, bufferIndex);
                case WriteCommitObject.SER_TYPE_INT32: return BitConverter.ToInt32(buffer, bufferIndex);
                case WriteCommitObject.SER_TYPE_INT64: return BitConverter.ToInt64(buffer, bufferIndex);
                case WriteCommitObject.SER_TYPE_FLOAT: return BitConverter.ToSingle(buffer, bufferIndex);

                case WriteCommitObject.SER_TYPE_INT8_ARRAY: return DeserializeFixedArray<byte>(length, 1, (int offset) => buffer[bufferIndex + offset]);
                case WriteCommitObject.SER_TYPE_INT16_ARRAY: return DeserializeFixedArray<short>(length, 2, (int offset) => BitConverter.ToInt16(buffer, bufferIndex + offset));
                case WriteCommitObject.SER_TYPE_INT32_ARRAY: return DeserializeFixedArray<int>(length, 4, (int offset) => BitConverter.ToInt32(buffer, bufferIndex + offset));
                case WriteCommitObject.SER_TYPE_INT64_ARRAY: return DeserializeFixedArray<long>(length, 8, (int offset) => BitConverter.ToInt64(buffer, bufferIndex + offset));
                case WriteCommitObject.SER_TYPE_FLOAT_ARRAY: return DeserializeFixedArray<float>(length, 4, (int offset) => BitConverter.ToSingle(buffer, bufferIndex + offset));

                case WriteCommitObject.SER_TYPE_STRING: return Encoding.UTF8.GetString(buffer, bufferIndex, length);
                case WriteCommitObject.SER_TYPE_STRING_ARRAY: return DeserializeDynamicArray<string>(buffer, bufferIndex, (int offset, out int len) =>
                {
                    len = BitConverter.ToInt32(buffer, bufferIndex + offset) + 4;
                    return Encoding.UTF8.GetString(buffer, bufferIndex + offset + 4, len - 4);
                });

                case WriteCommitObject.SER_TYPE_OBJECT:
                    DatabaseObject child = new DatabaseObject();
                    child.DeserializeObject(nameTable, buffer, bufferIndex);
                    return child;
                case WriteCommitObject.SER_TYPE_OBJECT_ARRAY: return DeserializeDynamicArray<DatabaseObject>(buffer, bufferIndex, (int offset, out int len) =>
                {
                    DatabaseObject child = new DatabaseObject();
                    len = child.DeserializeObject(nameTable, buffer, bufferIndex + offset);
                    return child;
                });

                case WriteCommitObject.SER_TYPE_BOOL: return buffer[bufferIndex] == 0x01;
                case WriteCommitObject.SER_TYPE_BOOL_ARRAY: return DeserializeFixedArray<bool>(length, 1, (int offset) => buffer[bufferIndex + offset] == 0x01);

                case WriteCommitObject.SER_TYPE_DOUBLE: return BitConverter.ToDouble(buffer, bufferIndex);
                case WriteCommitObject.SER_TYPE_DOUBLE_ARRAY: return DeserializeFixedArray<double>(length, 8, (int offset) => BitConverter.ToDouble(buffer, bufferIndex + offset));

                case WriteCommitObject.SER_TYPE_STRING_ENUM: return nameTable.GetNameTableValue(BitConverter.ToInt16(buffer, bufferIndex));
                default: throw new Exception("Unknown Type ID!");
            }
        }

        private delegate T FixedArrayReadDelegate<T>(int offset);
        private T[] DeserializeFixedArray<T>(int length, int elementLength, FixedArrayReadDelegate<T> read)
        {
            //Get the length of the array
            int count = length / elementLength;

            //Open array
            T[] elements = new T[count];

            //Read
            for(int i = 0; i<count; i++)
            {
                elements[i] = read(i * elementLength);
            }

            return elements;
        }

        private delegate T DynamicArrayReadDelegate<T>(int offset, out int elementLength);
        private T[] DeserializeDynamicArray<T>(byte[] buffer, int bufferIndex, DynamicArrayReadDelegate<T> read)
        {
            //Read count
            int count = BitConverter.ToInt32(buffer, bufferIndex);
            int offset = 4;

            //Open array
            T[] elements = new T[count];

            //Read each
            for (int i = 0; i<count; i++)
            {
                elements[i] = read(offset, out int len);
                offset += len;
            }

            return elements;
        }
    }
}
