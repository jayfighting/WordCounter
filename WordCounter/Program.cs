using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WordCounter
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var wordCount = 3;
            var resultCount = 100;
            var splitFileSize = 100_000;
            var noSplitFileSize = -1;
            var originalFiles = new List<string>();

            foreach (var arg in args)
            {
                if (File.Exists(arg))
                {
                    originalFiles.Add(arg);
                }
            }

            originalFiles.Add("pg2009.txt");
            
           var result = await CountWithSplit(noSplitFileSize, wordCount, resultCount, originalFiles);

            foreach (var keyValuePair in result)
            {
                Console.WriteLine($"{keyValuePair.Value}-{keyValuePair.Key}");
            }

            Console.ReadLine();
        }

        /// <summary>
        ///     Splits the file into smaller size
        ///     This needs to be benchmarked to find the optimized size to split
        ///     Not worth the overhead to split and merge if file size is small or the processing on the file is light
        /// </summary>
        /// <param name="splitFileSize">file size for each chunk</param>
        /// <param name="wordCount">number of word to user for key</param>
        /// <param name="resultCount">top n result</param>
        /// <param name="originalFiles">original files</param>
        /// <returns></returns>
        private static async Task<List<KeyValuePair<string, int>>> CountWithSplit(int splitFileSize, int wordCount,
            int resultCount, List<string> originalFiles)
        {
            var files = new List<string>();
            
            if (splitFileSize > 0)
            {
                var fileSplitter = new FileSplitter(splitFileSize);

                foreach (var originalFile in originalFiles)
                {
                    files.AddRange(await fileSplitter.SplitFiles(originalFile, wordCount));
                }
            }
            else
            {
                files = originalFiles;
            }

            // Create a processor/counter for each file, so we can process in parallel
            var counters = new List<Task<List<KeyValuePair<string, int>>>>();

            foreach (var file in files)
            {
                var newCounter = new WordCounter(file);
                counters.Add(newCounter.CountTopWords(wordCount, resultCount));
            }

            var finalResult = new List<KeyValuePair<string, int>>();

            while (counters.Any())
            {
                var finishedCounting = await Task.WhenAny(counters);
                counters.Remove(finishedCounting);
                // Merge the result as they are finishing
                finalResult = Merger.Merge(finalResult, await finishedCounting, resultCount);
            }

            return finalResult;
        }
    }
}