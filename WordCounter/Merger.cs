using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCounter
{
    public static class Merger
    {
        public static Dictionary<string, int> Merge(Dictionary<string, int> finalResult,
            Dictionary<string, int> results)
        {
            foreach (var keyValuePair in results)
            {
                if (!finalResult.ContainsKey(keyValuePair.Key))
                {
                    finalResult.Add(keyValuePair.Key, 0);
                }

                finalResult[keyValuePair.Key] += keyValuePair.Value;
            }


            return finalResult;
        }
    }
}