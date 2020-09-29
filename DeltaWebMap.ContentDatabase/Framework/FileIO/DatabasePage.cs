using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.FileIO
{
    class DatabasePage
    {
        public long index;
        public int type;
        public int modified_utc;
        public int total_blob_length;
        public int flags;
        public long next_page_index;

        public const int FLAG_CONTINUATION = 0;

        public bool CheckFlag(int index)
        {
            return ((flags >> index) & 1) == 1;
        }
    }
}
