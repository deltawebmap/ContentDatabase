using DeltaWebMap.ContentDatabase;
using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabaseTests
{
    class TestDatasetChild
    {
        public byte test_byte;
        public short test_short;
        public int test_int;
        public long test_long;
        public float test_float;

        public byte[] test_byte_array;
        public short[] test_short_array;
        public int[] test_int_array;
        public long[] test_long_array;
        public float[] test_float_array;

        public string test_string;
        public string[] test_string_array;

        public bool test_bool_1;
        public bool test_bool_2;

        public double test_double;
        public double[] test_double_array;

        public string test_enum;

        public virtual void Validate(DatabaseObject obj, ValidationJar jar)
        {
            jar.ValidateBasic("test_byte", test_byte, obj["test_byte"]);
            jar.ValidateBasic("test_short", test_short, obj["test_short"]);
            jar.ValidateBasic("test_int", test_int, obj["test_int"]);
            jar.ValidateBasic("test_long", test_long, obj["test_long"]);
            jar.ValidateBasic("test_float", test_float, obj["test_float"]);

            jar.ValidateArray("test_byte_array", test_byte_array, (byte[])obj["test_byte_array"]);
            jar.ValidateArray("test_short_array", test_short_array, (short[])obj["test_short_array"]);
            jar.ValidateArray("test_int_array", test_int_array, (int[])obj["test_int_array"]);
            jar.ValidateArray("test_long_array", test_long_array, (long[])obj["test_long_array"]);
            jar.ValidateArray("test_float_array", test_float_array, (float[])obj["test_float_array"]);

            jar.ValidateBasic("test_string", test_string, obj["test_string"]);
            jar.ValidateArray("test_string_array", test_string_array, (string[])obj["test_string_array"]);

            jar.ValidateBasic("test_bool_1", test_bool_1, obj["test_bool_1"]);
            jar.ValidateBasic("test_bool_2", test_bool_2, obj["test_bool_2"]);

            jar.ValidateBasic("test_double", test_string, obj["test_string"]);
            jar.ValidateArray("test_double_array", test_double_array, (double[])obj["test_double_array"]);

            jar.ValidateBasic("test_enum", test_enum, obj["test_enum"]);
        }

        public virtual void WriteToObject(WriteObject obj)
        {
            obj.WriteInt8("test_byte", test_byte);
            obj.WriteInt16("test_short", test_short);
            obj.WriteInt32("test_int", test_int);
            obj.WriteInt64("test_long", test_long);
            obj.WriteFloat("test_float", test_float);

            obj.WriteInt8Array("test_byte_array", test_byte_array);
            obj.WriteInt16Array("test_short_array", test_short_array);
            obj.WriteInt32Array("test_int_array", test_int_array);
            obj.WriteInt64Array("test_long_array", test_long_array);
            obj.WriteFloatArray("test_float_array", test_float_array);

            obj.WriteString("test_string", test_string);
            obj.WriteStringArray("test_string_array", test_string_array);

            obj.WriteBool("test_bool_1", test_bool_1);
            obj.WriteBool("test_bool_2", test_bool_2);

            obj.WriteDouble("test_double", test_double);
            obj.WriteDoubleArray("test_double_array", test_double_array);

            obj.WriteStringEnum("test_enum", test_enum, new string[] { "DEFAULT", "VALUE_1", "VALUE_2", "VALUE_3" });
        }

        public virtual void Randomize(Random rand)
        {
            //Misc
            int arrayLen;
            
            //Single
            test_byte = (byte)rand.Next(0, byte.MaxValue);
            test_short = (short)rand.Next(short.MinValue, short.MaxValue);
            test_int = rand.Next(int.MinValue, int.MaxValue);
            test_long = rand.Next(int.MinValue, int.MaxValue);
            test_float = (float)rand.NextDouble();

            //Byte array
            arrayLen = rand.Next(0, 200);
            test_byte_array = new byte[arrayLen];
            for(int i = 0; i<arrayLen; i++)
                test_byte_array[i] = (byte)rand.Next(0, byte.MaxValue);

            //Short array
            arrayLen = rand.Next(0, 200);
            test_short_array = new short[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_short_array[i] = (short)rand.Next(short.MinValue, short.MaxValue);

            //Int array
            arrayLen = rand.Next(0, 200);
            test_int_array = new int[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_int_array[i] = rand.Next(int.MinValue, int.MaxValue);

            //Long array
            arrayLen = rand.Next(0, 200);
            test_long_array = new long[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_long_array[i] = rand.Next(int.MinValue, int.MaxValue);

            //Float array
            arrayLen = rand.Next(0, 200);
            test_float_array = new float[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_float_array[i] = (float)rand.NextDouble();

            //String
            test_string = GenerateRandomString(rand, rand.Next(0, 200));

            //String array
            arrayLen = rand.Next(0, 50);
            test_string_array = new string[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_string_array[i] = GenerateRandomString(rand, rand.Next(0, 100));

            //Bool
            test_bool_1 = true;
            test_bool_2 = false;

            //Double
            test_double = rand.NextDouble();
            test_double_array = new double[arrayLen];
            for (int i = 0; i < arrayLen; i++)
                test_double_array[i] = rand.NextDouble();

            //Enum
            test_enum = "VALUE_2";
        }

        private string GenerateRandomString(Random rand, int len)
        {
            char[] c = new char[len];
            char[] charSet = "1234567890qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();
            for (int i = 0; i < len; i++)
                c[i] = charSet[rand.Next(0, charSet.Length)];
            return new string(c);
        }
    }
}
