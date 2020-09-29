using DeltaWebMap.ContentDatabase.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.CommitBuilders.Write
{
    public class WriteCommit
    {
        public readonly ulong commitId;
        public readonly byte commitType;
        private INameTableProvider nameTable;

        internal List<WriteCommitObjectFinalized> commits;

        public WriteCommit(ulong commitId, byte commitType, INameTableProvider nameTable)
        {
            this.commitId = commitId;
            this.commitType = commitType;
            this.nameTable = nameTable;
            this.commits = new List<WriteCommitObjectFinalized>();
        }

        public void CommitObject(WriteCommitObject obj)
        {
            //Serialize. Get length
            int len = obj.GetLength();

            //Open buffer
            byte[] payload = new byte[len];

            //Serialize to this
            obj.SerializeTo(nameTable, payload, 0);

            //Add
            commits.Add(new WriteCommitObjectFinalized(obj, payload));
        }
    }
}
