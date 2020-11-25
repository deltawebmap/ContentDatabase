using DeltaWebMap.ContentDatabase.CommitBuilders.Find;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands
{
    class FindCommand : BaseMultithreadCommand<FindCommitResults>
    {
        private int? groupId;
        private int skip;
        private int limit;
        private MultithreadedContentDatabaseSession session;

        public FindCommand(MultithreadedContentDatabaseSession session, int? groupId, int skip, int limit)
        {
            this.session = session;
            this.groupId = groupId;
            this.skip = skip;
            this.limit = limit;
        }
        
        public override FindCommitResults Work()
        {
            if (groupId.HasValue)
                return session.session.FindByGroup(groupId.Value, skip, limit);
            else
                return session.session.FindAll(skip, limit);
        }
    }
}
