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
    var adictionary = spellChecker.CreateDictionary(
        "test-" + Guid.NewGuid().ToString());
    var bdictionary = spellChecker.CreateDictionary(
        "test-" + Guid.NewGuid().ToString());
    try {
        Assert.Empty(adictionary);
        adictionary.AddWord("Jeff");
        Assert.Contains("Jeff", adictionary);
    } finally {
        spellChecker.DeleteDictionary(adictionary);
        spellChecker.DeleteDictionary(bdictionary);
    }
}

    }
}
