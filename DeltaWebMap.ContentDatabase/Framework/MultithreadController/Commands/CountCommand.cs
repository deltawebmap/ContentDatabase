using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands
{
    class CountCommand : BaseMultithreadCommand<long>
    {
        private MultithreadedContentDatabaseSession session;
        private int? groupId;

        public CountCommand(MultithreadedContentDatabaseSession session, int? groupId)
        {
            this.session = session;
            this.groupId = groupId;
        }
        
        public override long Work()
        {
            if(groupId.HasValue)
                return session.session.CountTeamItems(groupId.Value);
            else
                return session.session.CountAllItems();
        }
    }
}
