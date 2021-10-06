using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCounter
{
    public static class Merger
    {
        public static List<KeyValuePair<string, int>> Merge(List<KeyValuePair<string, int>> finalResult,
            List<KeyValuePair<string, int>> results, int returnCount)
        {
            var finalDict = finalResult.ToDictionary(x => x.Key, x => x.Value,
                StringComparer.OrdinalIgnoreCase);

            foreach (var keyValuePair in results)
            {
                if (!finalDict.ContainsKey(keyValuePair.Key))
                {
                    finalDict.Add(keyValuePair.Key, 0);
                }

                finalDict[keyValuePair.Key] += keyValuePair.Value;
            }

            var result = finalDict.OrderByDescending(kvp => kvp.Value).Take(returnCount).ToList();

            return result;
        }
    }
}