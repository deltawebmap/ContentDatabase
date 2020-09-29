using DeltaWebMap.ContentDatabase;
using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;

namespace DeltaWebMap.ContentDatabaseTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //Write
            ContentDatabaseSession db = ContentDatabaseSession.CreateDatabase("E:\\test.db", 512);
            var commit = db.CreateWriteCommit(747669614976172074, 0xAA);
            Random rand = new Random();
            for(int i = 0; i<9000; i++)
            {
                //Make root object
                var obj = new WriteCommitObject((ulong)i, 123456789);

                //Make location
                var location = new WriteObject()
                    .WriteFloat("x", -377973.28f)
                    .WriteFloat("y", -451888.78f)
                    .WriteFloat("z", -14395.185f)
                    .WriteFloat("yaw", -27.822876f)
                    .WriteFloat("pitch", 0)
                    .WriteFloat("roll", 0);

                //Begin writing
                obj.WriteInt64("dino_id", 0);
                obj.WriteBool("is_tamed", true);
                obj.WriteBool("is_female", true);
                obj.WriteInt32Array("color_indexes", new int[] { 1, 2, 3, 4, 5, 6 });
                obj.WriteString("tamed_name", "Testing Tamed Name");
                obj.WriteString("tamer_name", "Testing Tamer Name");
                obj.WriteString("classname", "Testing Classname");
                obj.WriteFloatArray("current_stats", new float[] { 1, 2, 3, 4, 5, 6 });
                obj.WriteFloatArray("max_stats", new float[] { 1, 2, 3, 4, 5, 6 });
                obj.WriteInt32Array("base_levelups_applied", new int[] { 1, 2, 3, 4, 5, 6 });
                obj.WriteInt32Array("tamed_levelups_applied", new int[] { 1, 2, 3, 4, 5, 6 });
                obj.WriteInt32("base_level", 100);
                obj.WriteInt32("level", 100);
                obj.WriteFloat("experience", 100);
                obj.WriteBool("is_baby", false);
                obj.WriteFloat("baby_age", 1);
                obj.WriteDouble("next_imprint_time", 1);
                obj.WriteFloat("imprint_quality", 1);
                obj.WriteChildObject("location", location);
                obj.WriteStringEnum("status", "TEST", new string[] { "UNKNOWN", "PASSIVE", "NEUTRAL", "AGGRESSIVE", "PASSIVE_FLEE" });
                obj.WriteFloat("taming_effectiveness", 1);
                obj.WriteBool("is_cryo", true);
                obj.WriteString("cryo_inventory_id", "test");
                obj.WriteInt32("cryo_inventory_type", 1);
                obj.WriteInt64("cryo_inventory_itemid", 1);
                obj.WriteFloat("experience_points", 1);
                obj.WriteInt64("last_sync_time", 1);
                obj.WriteBool("is_alive", true);
                commit.CommitObject(obj);
            }
            db.ApplyWriteCommit(commit);
            db.Dispose();

            //Load
            db = ContentDatabaseSession.OpenDatabase("E:\\test.db");
            var results = db.FindByGroup(123456789, 0, 4);
            var r = results.results[0].Deserialize();
            db.Dispose();
        }
    }
}
