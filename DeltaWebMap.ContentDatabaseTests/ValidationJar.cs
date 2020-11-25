using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabaseTests
{
    public class ValidationJar
    {
        public int checked_ok;
        public int checked_failed;
        
        public void ValidateBasic(string name, object expected, object recieved)
        {
            Validate(name, expected.Equals(recieved), expected.ToString(), recieved.ToString());
        }

        public void ValidateArray<T>(string name, T[] expected, T[] recieved)
        {
            bool valid = true;
            if(expected.Length == recieved.Length)
            {
                for (int i = 0; i < expected.Length; i++)
                    valid = valid && ((object)expected[i]).Equals((object)recieved[i]);
            } else
            {
                valid = false;
            }
            Validate(name, valid, "[ " + expected.Length.ToString() + " ]", "[ " + recieved.Length.ToString() + " ]");
        }

        public void Validate(string name, bool match, string expected, string recieved)
        {
            if (match)
            {
                checked_ok++;
            } else
            {
                checked_failed++;
                Console.WriteLine($"BAD MATCH \"{name}\"; Expected={expected}; Recieved={recieved}");
            }
        }
    }
}
