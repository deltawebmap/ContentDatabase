using DeltaWebMap.ContentDatabase.Framework.MultithreadController;
using DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DeltaWebMap.ContentDatabase
{
    /// <summary>
    /// Since ContentDatabsaeSessions are inherently single-threaded, this class can be used to interact with them from a single, multithread-safe, thread.
    /// </summary>
    public class MultithreadedContentDatabaseController
    {
        private ConcurrentQueue<IMultithreadCommand> commandQueue;
        private Thread workingThread;
        private ConcurrentDictionary<string, MultithreadedContentDatabaseSession> databaseSessions; //Key is the path to the file

        public MultithreadedContentDatabaseController()
        {
            this.commandQueue = new ConcurrentQueue<IMultithreadCommand>();
            this.databaseSessions = new ConcurrentDictionary<string, MultithreadedContentDatabaseSession>(); 
            this.workingThread = new Thread(RunWorkingThread);
            this.workingThread.IsBackground = true;
            this.workingThread.Start();
        }

        private void RunWorkingThread()
        {
            while(true)
            {
                //Get next command
                IMultithreadCommand cmd;
                while (!commandQueue.TryDequeue(out cmd))
                    Thread.Sleep(5);

                //Run
                cmd.ExecuteAll();
            }
        }

        internal void QueueCommand(IMultithreadCommand cmd)
        {
            commandQueue.Enqueue(cmd);
        }

        /// <summary>
        /// Gets or creates a database session and immediately returns a session.
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns></returns>
        public MultithreadedContentDatabaseSession GetDatabaseSession(string pathname, int pageSize)
        {
            //Try to load from the session list
            if (databaseSessions.TryGetValue(pathname, out MultithreadedContentDatabaseSession session))
                return session;

            //Create
            session = new MultithreadedContentDatabaseSession(this);
            databaseSessions.TryAdd(pathname, session);

            //Begin loading
            OpenDatabaseCommand cmd = new OpenDatabaseCommand(session, pathname, pageSize);
            commandQueue.Enqueue(cmd);

            return session;
        }
    }
}
