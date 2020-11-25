using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController
{
    public interface IMultithreadCommand
    {
        /// <summary>
        /// The command to be executed on the worker thread
        /// </summary>
        void ExecuteAll();
    }
}
