using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework
{
    class DatabaseObjectHeader
    {
        public ulong object_id;
        public ulong page_id;
        public ulong commit_id;
        public int group_id;
        public byte commit_type;
        public byte flags;
        public ushort reserved;
    }
}
