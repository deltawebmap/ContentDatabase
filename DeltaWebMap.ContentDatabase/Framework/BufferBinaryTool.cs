using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework
{
    class BufferBinaryTool
    {
        private byte[] buffer;

        public BufferBinaryTool(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public short ReadInt16(int index)
        {
            return BitConverter.ToInt16(buffer, index);
        }

        public ushort ReadUInt16(int index)
        {
            return BitConverter.ToUInt16(buffer, index);
        }

        public int ReadInt32(int index)
        {
            return BitConverter.ToInt32(buffer, index);
        }

        public uint ReadUInt32(int index)
        {
            return BitConverter.ToUInt32(buffer, index);
        }

        public long ReadInt64(int index)
        {
            return BitConverter.ToInt64(buffer, index);
        }

        public ulong ReadUInt64(int index)
        {
            return BitConverter.ToUInt64(buffer, index);
        }

        public void WriteInt16(int index, short value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }

        public void WriteUInt16(int index, ushort value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }

        public void WriteInt32(int index, int value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }

        public void WriteUInt32(int index, uint value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }

        public void WriteInt64(int index, long value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }

        public void WriteUInt64(int index, ulong value)
        {
            BitConverter.GetBytes(value).CopyTo(buffer, index);
        }
    }
}
