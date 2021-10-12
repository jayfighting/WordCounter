using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCounter
{
    public class FileSplitter
    {
        private readonly int _bufferSize;
        private readonly string _outputDirectory;
        private readonly byte[] _buffer;
        private readonly char[] _charBuffer = new char[1];
        public FileSplitter(int bufferSize, string outputDirectory)
        {
            _bufferSize = bufferSize;
            _outputDirectory = outputDirectory;
            _buffer = new byte[bufferSize];
        }

        public async Task<string[]> SplitFiles(string filePath, int wordCount)
        {
            var totalLineCount = File.ReadLines(filePath).Count();
            var currentLineCount = 0;
            var originalFileName = Path.GetFileName(filePath);
            int _noOfFiles = 0; 
            string lastLine = string.Empty;
            string carryOverChars = string.Empty;

            var paths = new List<string>();
            await using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            await using (var bs = new BufferedStream(fs))
            {
                await using (var memoryStream = new MemoryStream(_buffer))
                // var memoryStream = new MemoryStream(_buffer);
                using (var stream = new StreamReader(memoryStream))

                    while (await bs.ReadAsync(_buffer, 0, _bufferSize) != 0)
                    {
                        _noOfFiles++;
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var path = $"{_outputDirectory}/{originalFileName}-Chunks{_noOfFiles}.txt";
                        await File.WriteAllTextAsync(path, lastLine + carryOverChars);

                        // await File.WriteAllBytesAsync(path, _buffer);

                        while (!stream.EndOfStream)
                        {
                            var currentLine = new StringBuilder();
                            var line = ReadLineWithAccumulation(stream, currentLine);

                            if (!line.isLastLine)
                            {
                                currentLineCount++;

                                if (currentLineCount <= totalLineCount)
                                {
                                    await using var w = File.AppendText(path);
                                    await w.WriteLineAsync(line.text);

                                    if (!string.IsNullOrWhiteSpace(line.text))
                                    {
                                        lastLine = line.text;
                                    }
                                }
                            }
                            else
                            {
                                carryOverChars = line.text;
                            }
                        }

                        // Split the file, but put the also append the last n-1 words to next line so we can use to generate key
                        lastLine = GetTheLastCountedWords(lastLine, wordCount);

                        paths.Add(path);
                    }
            }

            return paths.ToArray();
        }

        private static string GetTheLastCountedWords(string s, int count)
        {
            var split = s.Split();
            var stack = new Stack<string>();
            
            for (int i = split.Length - 1; i >= 0; i--)
            {
                if (!string.IsNullOrWhiteSpace(split[i]))
                {
                    stack.Push(split[i]);
                }
                
                if (stack.Count == count - 1)
                {
                    break;
                }
            }

            var sb = new StringBuilder();
            while (stack.Any())
            {
                sb.Append(stack.Pop());
                sb.Append(' ');
            }

            return sb.ToString();
        }

        private (string text, bool isLastLine) ReadLineWithAccumulation(StreamReader stream, StringBuilder currentLine)
        {
            while (stream.Read(_charBuffer, 0, 1) > 0)
            {
                if (_charBuffer[0].Equals('\n'))
                {
                    var result = currentLine.ToString();
                    currentLine.Clear();

                    if (result.Last() == '\r') //remove if newlines are single character
                    {
                        result = result.Substring(0, result.Length - 1);
                    }

                    return ( text: $"{result}", isLastLine: false);
                }

                currentLine.Append(_charBuffer[0]);
            }

            return ( text: $"{currentLine}", isLastLine: true);
        }
    }
}