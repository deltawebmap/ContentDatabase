using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework
{
    public interface INameTableProvider
    {
        short GetNameTableIndex(string value);
        string GetNameTableValue(short index);
    }
}
