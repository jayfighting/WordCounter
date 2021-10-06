using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace WordCounter.UnitTest
{
    public class WordCounterTests
    {
        [Fact]
        public async void AllKeyShouldBeTheCorrectLength()
        {
            var sut = new WordCounter(@"testFiles\Testfile1.txt");

            var keyLength = 3;
            var actual = await sut.CountTopWords(keyLength, 10);
            
            var notKeyLengthKeys = actual.Where(kvp => kvp.Key.Length != keyLength);

            notKeyLengthKeys.Should().BeEmpty();
        }
        
        [Fact]
        public async void KeyShouldBeCaseInsensitive()
        {
            var sut = new WordCounter(@"testFiles\Testfile1.txt");

            var actual = await sut.CountTopWords(3, 10);
            var actualDictionary = actual.ToDictionary(x => x.Key, x => x.Value,
                StringComparer.OrdinalIgnoreCase);

            var key = "I love SandWiches";
            actualDictionary.Should().ContainKey(key);
            actualDictionary[key].Should().Be(2);
        }
    }
}