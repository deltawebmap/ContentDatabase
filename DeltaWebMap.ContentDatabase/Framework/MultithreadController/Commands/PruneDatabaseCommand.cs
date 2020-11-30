using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands
{
    public class PruneDatabaseCommand : BaseMultithreadCommand<int>
    {
        private ulong commitId;
        private byte commitType;
        private MultithreadedContentDatabaseSession session;

        public PruneDatabaseCommand(MultithreadedContentDatabaseSession session, ulong commitId, byte commitType)
        {
            this.session = session;
            this.commitId = commitId;
            this.commitType = commitType;
        }

        public override int Work()
        {
            return session.session.PruneOldCommits(commitId, commitType);
        }
    }
}
