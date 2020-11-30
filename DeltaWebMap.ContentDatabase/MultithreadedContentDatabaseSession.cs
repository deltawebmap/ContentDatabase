using DeltaWebMap.ContentDatabase.CommitBuilders.Find;
using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using DeltaWebMap.ContentDatabase.Framework.MultithreadController.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeltaWebMap.ContentDatabase
{
    public class MultithreadedContentDatabaseSession
    {
        /// <summary>
        /// Because of the nature of how queueing works, this will always be ready by the time an action to access it is executed
        /// </summary>
        internal ContentDatabaseSession session;

        /// <summary>
        /// This is a controller
        /// </summary>
        private MultithreadedContentDatabaseController controller;

        public MultithreadedContentDatabaseSession(MultithreadedContentDatabaseController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Writes a commit to the disk in a thread-safe manner
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        public Task<int> WriteCommitAsync(WriteCommit commit)
        {
            var cmd = new WriteCommitCommand(this, commit);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }

        /// <summary>
        /// Removes all commits that match this type but do NOT match this ID
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        public Task<int> PruneOldCommitsAsync(ulong commitId, byte commitType)
        {
            var cmd = new PruneDatabaseCommand(this, commitId, commitType);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }

        /// <summary>
        /// Counts all items belonging to a team
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public Task<long> CountTeamItemsAsync(int teamId)
        {
            var cmd = new CountCommand(this, teamId);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }

        /// <summary>
        /// Counts all items in the database
        /// </summary>
        /// <returns></returns>
        public Task<long> CountAllItemsAsync()
        {
            var cmd = new CountCommand(this, null);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }

        /// <summary>
        /// Searches for all items in a team
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Task<FindCommitResults> FindTeamItemsAsync(int teamId, int skip, int limit = int.MaxValue)
        {
            var cmd = new FindCommand(this, teamId, skip, limit);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }

        /// <summary>
        /// Searches all items for us
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public Task<FindCommitResults> FindAllItemsAsync(int skip = 0, int limit = int.MaxValue)
        {
            var cmd = new FindCommand(this, null, skip, limit);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }

        /// <summary>
        /// Closes and disposes the database correctly.
        /// </summary>
        /// <returns></returns>
        public Task CloseDatabaseAsync()
        {
            var cmd = new CloseDatabaseCommand(this);
            controller.QueueCommand(cmd);
            return cmd.GetTask();
        }
    }
}
