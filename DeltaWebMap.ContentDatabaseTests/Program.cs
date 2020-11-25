using DeltaWebMap.ContentDatabase;
using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaWebMap.ContentDatabaseTests
{
    class Program
    {
        static List<TestDataset> datasets = new List<TestDataset>();
        
        static void Main(string[] args)
        {
            MultithreadedContentDatabaseController mt = new MultithreadedContentDatabaseController();

            //Delete
            if (File.Exists("E:\\test.db"))
                File.Delete("E:\\test.db");

            //Open
            var session = mt.GetDatabaseSession("E:\\test.db", 512);

            //Make items
            var commit = new WriteCommit(747669614976172074, 0xAA);
            Random rand = new Random();
            for (int i = 0; i<2000; i++)
            {
                TestDataset ds = new TestDataset(i);
                ds.Randomize(rand);
                WriteCommitObject obj = new WriteCommitObject((ulong)i, 123456789);
                ds.WriteToObject(obj);
                commit.CommitObject(obj);
                datasets.Add(ds);
            }
            session.WriteCommitAsync(commit);

            //Count
            Console.WriteLine("Count: " + session.CountAllItemsAsync().GetAwaiter().GetResult());

            //Close
            session.CloseDatabaseAsync().GetAwaiter().GetResult();

            //Open
            mt = new MultithreadedContentDatabaseController();
            session = mt.GetDatabaseSession("E:\\test.db", 512);

            //Load and validate
            var r = session.FindAllItemsAsync().GetAwaiter().GetResult();
            ValidationJar jar = new ValidationJar();
            for(int i = 0; i<datasets.Count; i++)
            {
                var des = r.results[i].Deserialize();
                datasets[i].Validate(des, jar);
            }

            Console.WriteLine($"Done. {jar.checked_ok} OK, {jar.checked_failed} failed");
            Console.ReadLine();
        }
    }
}
