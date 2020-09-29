using System;
using System.Collections.Generic;
using System.Text;

namespace DeltaWebMap.ContentDatabase.CommitBuilders.Find
{
    /// <summary>
    /// Database results without being deserialized
    /// </summary>
    public class FindCommitResults
    {
        public List<FindCommitResultsObject> results = new List<FindCommitResultsObject>();
    }
}
