using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands
{
    class OpenDatabaseCommand : BaseMultithreadCommand<ContentDatabaseSession>
    {
        public string pathname;
        public MultithreadedContentDatabaseSession session;
        public int pageSize;

        public OpenDatabaseCommand(MultithreadedContentDatabaseSession session, string pathname, int pageSize)
        {
            this.session = session;
            this.pathname = pathname;
            this.pageSize = pageSize;
        }
        
        public override ContentDatabaseSession Work()
        {
            ContentDatabaseSession r;
            if (File.Exists(pathname))
                r = ContentDatabaseSession.OpenDatabase(pathname);
            else
                r = ContentDatabaseSession.CreateDatabase(pathname, pageSize);
            session.session = r;
            return r;
        }
    }
}
