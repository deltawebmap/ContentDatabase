using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands
{
    class CloseDatabaseCommand : BaseMultithreadCommand<bool>
    {
        private MultithreadedContentDatabaseSession session;

        public CloseDatabaseCommand(MultithreadedContentDatabaseSession session)
        {
            this.session = session;
        }
        
        public override bool Work()
        {
            session.session.Dispose();
            return true;
        }
    }
}
