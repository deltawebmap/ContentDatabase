using DeltaWebMap.ContentDatabase;
using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabaseTests
{
    class TestDataset : TestDatasetChild
    {
        public int id;

        public TestDataset(int id)
        {
            this.id = id;
        }
        
        public TestDatasetChild test_object;
        public TestDatasetChild[] test_object_array;

        public override void Randomize(Random rand)
        {
            base.Randomize(rand);

            test_object = GenerateRandObject(rand);

            int arrayLen = rand.Next(1, 5);
            test_object_array = new TestDatasetChild[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_object_array[i] = GenerateRandObject(rand);
        }

        public override void WriteToObject(WriteObject obj)
        {
            base.WriteToObject(obj);

            WriteObject o = new WriteObject();
            test_object.WriteToObject(o);
            obj.WriteChildObject("test_object", o);

            WriteObject[] os = new WriteObject[test_object_array.Length];
            for (int i = 0; i < os.Length; i++)
            {
                os[i] = new WriteObject();
                test_object_array[i].WriteToObject(os[i]);
            }
            obj.WriteChildObjectArray("test_object_array", os);
        }

        private TestDatasetChild GenerateRandObject(Random rand)
        {
            var d = new TestDatasetChild();
            d.Randomize(rand);
            return d;
        }
    }
}
