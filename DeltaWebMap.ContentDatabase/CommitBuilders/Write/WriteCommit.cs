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

        internal List<WriteCommitObject> commits;

        public WriteCommit(ulong commitId, byte commitType)
        {
            this.commitId = commitId;
            this.commitType = commitType;
            this.commits = new List<WriteCommitObject>();
        }

        public void CommitObject(WriteCommitObject obj)
        {
            //Add
            commits.Add(obj);
        }
    }
}
