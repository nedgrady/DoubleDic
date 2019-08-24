using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoubleDic.Test
{
    [TestClass]
    public class SensitiveDictionaryViewTests
    {
        [TestMethod]
        public void CreatingView_WithDictionary_ReflectsTheDictionary()
        {
            const string test = "test";
            var source = new Dictionary<string, string>
            {
                [test] = test
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                Enumerable.Empty<string>(),
                s => "REDACTED");

            Assert.AreEqual(source[test], sut[test]);
        }

        [TestMethod]
        public void CreatingView_WithNullDictionary_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SensitiveDictionaryView<string, string>(
                null,
                Enumerable.Empty<string>(),
                s => "REDACTED"));
        }

        [TestMethod]
        public void CreatingView_WithNullSensitiveKeys_HasNoSensitiveKeys()
        {
            var sut = new SensitiveDictionaryView<string, string>(
                new Dictionary<string, string>(),
                null,
                s => "REDACTED");

            Assert.AreEqual(0, sut.SensitiveKeys.Count());
        }

        [TestMethod]
        public void CreatingView_WithNullReplacementFunc_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SensitiveDictionaryView<string, string>(
                new Dictionary<string, string>(),
                Enumerable.Empty<string>(),
                null));
        }

        [TestMethod]
        public void CreatingView_WithSensitiveKeys_RedactsValueForTheKeys()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                new[]{ sensitive },
                s => redacted);

            Assert.AreEqual(redacted, sut[sensitive]);
        }

        [TestMethod]
        public void CreatingView_WithSensitiveKeys_DoesNotRedactNonSensitiveKeys()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>()
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                new[] { sensitive },
                s => redacted);

            Assert.AreEqual(test, sut[test]);
        }

        [TestMethod]
        public void AddingSensitiveKey_RedactsValueForTheKeys()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                Enumerable.Empty<string>(),
                s => redacted);

            sut.AddSensitive(sensitive);

            Assert.AreEqual(redacted, sut[sensitive]);
        }

        [TestMethod]
        public void RemovingSensitiveKey_DoesNotRedactTheValue()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = test
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                new[] { sensitive },
                s => redacted);

            sut.RemoveSensitive(sensitive);

            Assert.AreEqual(test, sut[sensitive]);
        }

        [TestMethod]
        public void TryGetting_MissingKey_ReturnsFalse()
        {
            var source = new Dictionary<string, string>();

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                Enumerable.Empty<string>(),
                s => "Redacted");

            var didGetValue = sut.TryGetValue("missing", out _);

            Assert.IsFalse(didGetValue);
        }

        [TestMethod]
        public void TryGetting_ExistingKey_ReturnsTrue()
        {
            const string existing = "existing";

            var source = new Dictionary<string, string>
            {
                [existing] = existing
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                Enumerable.Empty<string>(),
                s => "Redacted");

            var didGetValue = sut.TryGetValue(existing, out _);

            Assert.IsTrue(didGetValue);
        }

        [TestMethod]
        public void TryGetting_SensitiveKey_OutputsRedacted()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                new[] {sensitive },
                s => redacted);

            sut.TryGetValue(sensitive, out var value);

            Assert.AreEqual(redacted, value);
        }

        [TestMethod]
        public void TryGetting_NonSensitiveKey_OutputsValue()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                new[] { sensitive },
                s => redacted);

            sut.TryGetValue(test, out var value);

            Assert.AreEqual(test, value);
        }

        [TestMethod]
        public void EnumeratingDictionary_WithNoSensitiveKeys_OutputsSameValues()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                Enumerable.Empty<string>(),
                s => redacted);

            using (var sourceIt = source.GetEnumerator())
            using (var sutIt = sut.GetEnumerator())
            {
                while (sourceIt.MoveNext() && sutIt.MoveNext())
                {
                    Assert.AreEqual(sutIt.Current, sourceIt.Current);
                }
            }
        }

        [TestMethod]
        public void EnumeratingDictionary_WithSensitiveKeys_OutputsSensitiveValues()
        {
            const string test = "test";
            const string sensitive = "sensitive";
            const string redacted = "redacted";

            var source = new Dictionary<string, string>
            {
                [test] = test,
                [sensitive] = sensitive
            };

            var sut = new SensitiveDictionaryView<string, string>(
                source,
                new []{ sensitive },
                s => redacted);

            sut.TryGetValue(test, out var value);

            using (var sourceIt = source.GetEnumerator())
            using (var sutIt = sut.GetEnumerator())
            {
                while (sourceIt.MoveNext() && sutIt.MoveNext())
                {
                    var expected = sourceIt.Current.Key == sensitive ? redacted : test;
                    var actual = sutIt.Current.Value;

                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }

}
