using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WordCounter
{
    public class WordCounter
    {
        private readonly string _fileName;

        public WordCounter(string fileName)
        {
            _fileName = fileName;
        }

        public async Task<Dictionary<string, int>> CountTopWords(int wordCount, int returnCount)
        {
            using var sr = new StreamReader(_fileName);
            string line;
            var wordBuilder = new Queue<string>();
            Dictionary<string, int> dictionary = new(StringComparer.OrdinalIgnoreCase);

            while ((line = await sr.ReadLineAsync()) != null)
            {
                var split = line.Split();

                foreach (var word in split)
                {
                    var currentWord = word.Sanitize();
                    if (!string.IsNullOrEmpty(currentWord))
                    {
                        wordBuilder.Enqueue(currentWord);
                    }

                    // we accumulate enough words
                    if (wordBuilder.Count == wordCount)
                    {
                        var key = string.Join(" ", wordBuilder);

                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, 0);
                        }

                        dictionary[key]++;
                        wordBuilder.Dequeue();
                    }
                }
            }

            return dictionary;
        }
    }
}