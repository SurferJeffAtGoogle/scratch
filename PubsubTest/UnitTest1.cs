using System;
using System.Collections.Generic;
using Xunit;

namespace PubsubTest
{
    public class UnitTest1 : IDisposable
    {
        List<Dictionary> dictionariesToDelete = new List<Dictionary>();
        public void Dispose() {
            var spellChecker = SpellCheckerService.CreateClient();
            foreach (var dictionary in dictionariesToDelete) {
                try {
                    spellChecker.DeleteDictionary(dictionary);
                } catch (Exception e) {
                    Logging.LogException(e);
                }
            }
        }

        [Fact]
        public void Test1()
        {
            var spellChecker = SpellCheckerService.CreateClient();
            var adictionary = spellChecker.CreateDictionary(
                "test-" + Guid.NewGuid().ToString());
            try
            {
                var bdictionary = spellChecker.CreateDictionary(
                    "test-" + Guid.NewGuid().ToString());
                try
                {
                    Assert.Empty(adictionary);
                    adictionary.AddWord("Jeff");
                    Assert.Contains("Jeff", adictionary);
                }
                finally
                {
                    spellChecker.DeleteDictionary(bdictionary);
                }
            }
            finally
            {
                spellChecker.DeleteDictionary(adictionary);
            }
        }

    }
}
