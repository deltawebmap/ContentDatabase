using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands
{
    class WriteCommitCommand : BaseMultithreadCommand<int>
    {
        private WriteCommit commit;
        private MultithreadedContentDatabaseSession session;

        public WriteCommitCommand(MultithreadedContentDatabaseSession session, WriteCommit commit)
        {
            this.session = session;
            this.commit = commit;
        }

        public override int Work()
        {
            return session.session.ApplyWriteCommit(commit);
        }
    }
}
