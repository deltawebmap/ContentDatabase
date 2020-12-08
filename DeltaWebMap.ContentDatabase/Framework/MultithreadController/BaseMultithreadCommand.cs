using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DeltaWebMap.ContentDatabase.Framework.MultithreadController
{
    public abstract class BaseMultithreadCommand<T> : IMultithreadCommand
    {
        public BaseMultithreadCommand()
        {
            this.channel = Channel.CreateBounded<T>(1);
        }

        private Channel<T> channel;
        private Exception error;

        public abstract T Work();

        public void ExecuteAll()
        {
            //Work
            T data;
            try
            {
                data = Work();
            }
            catch (Exception ex)
            {
                //Failed
                error = ex;
                channel.Writer.Complete(ex);
                return;
            }

            //Respond
            channel.Writer.WriteAsync(data).GetAwaiter().GetResult();
            channel.Writer.Complete();
        }

        public async Task<T> GetTask()
        {
            //Wait for completion
            await this.channel.Reader.Completion;

            //Check if errored
            if (error != null)
                throw error;

            //Read
            var result = await this.channel.Reader.ReadAsync();
            return result;
        }
    }
}
