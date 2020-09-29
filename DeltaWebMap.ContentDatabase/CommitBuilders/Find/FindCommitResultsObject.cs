using DeltaWebMap.ContentDatabase.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.CommitBuilders.Find
{
    public class FindCommitResultsObject
    {
        public ulong object_id;
        public ulong commit_id;
        public int group_id;
        public byte commit_type;
        public byte[] blob;
        internal INameTableProvider nameTable;

        /// <summary>
        /// Deserializes this into a DatabaseObject
        /// </summary>
        /// <returns></returns>
        public DatabaseObject Deserialize()
        {
            DatabaseObject obj = new DatabaseObject(object_id, commit_id, group_id, commit_type);
            obj.DeserializeObject(nameTable, blob, 0);
            return obj;
        }
    }
}
