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
                channel.Writer.Complete(ex);
                return;
            }

            //Respond
            channel.Writer.WriteAsync(data).GetAwaiter().GetResult();
            channel.Writer.Complete();
        }

        public async Task<T> GetTask()
        {
            var result = await this.channel.Reader.ReadAsync();
            return result;
        }
    }
}
