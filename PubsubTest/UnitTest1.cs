using System;
using Xunit;

namespace PubsubTest
{
    public class UnitTest1
    {
        [Fact]
public void Test1()
{
    var spellChecker = SpellCheckerService.CreateClient();
    var dictionary = spellChecker.CreateDictionary(
        Guid.NewGuid().ToString());
    try {
        Assert.Empty(dictionary);
        dictionary.AddWord("Jeff");
        Assert.Contains("Jeff", dictionary);
    } finally {
        spellChecker.DeleteDictionary(dictionary);
    }
}

    }
}
