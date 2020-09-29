using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.BinarySerializer
{
    public class StringEnumSerType : IBinarySerType
    {
        private string value;
        private string[] possibleOptions;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="possibleOptions">The possible options for the value. If the value is invalid, the first option in this list will be used as a default</param>
        public StringEnumSerType(string value, string[] possibleOptions)
        {
            this.value = value;
            this.possibleOptions = possibleOptions;
            if (possibleOptions.Length == 0)
                throw new Exception("No possible options specified!");
        }

        public override short SerTypeId => CommitBuilders.Write.WriteCommitObject.SER_TYPE_STRING_ENUM;

        public override int GetLength()
        {
            return 2;
        }

        public override void Serialize(byte[] buffer, int bufferPos, INameTableProvider nameTable)
        {
            //Ensure this is inside the possible options.
            //We check this because otherwise an attacker could flood our name table and cause slowdowns
            string realValue = value;
            if (!possibleOptions.Contains(value))
                realValue = possibleOptions[0];
            
            //Look up name table entry
            short entry = nameTable.GetNameTableIndex(realValue);

            //Write
            BitConverter.GetBytes(entry).CopyTo(buffer, bufferPos);
        }
    }
}
