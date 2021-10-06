using System.Linq;

namespace WordCounter
{
    public static class Extensions
    {
        public static string Sanitize(this string word) => string.Join("", word.Where(char.IsLetter));
    }
}