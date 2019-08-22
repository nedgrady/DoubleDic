using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoubleDic.Test
{
    [TestClass]
    public class DoubleDicTests
    {
        [TestMethod]
        public void NewDictionary_BothShouldBeEmpty()
        {
            var dic = new DoubleDic<string, string>("XXX");

            Assert.AreEqual(0, dic.Exposed.Count);
            Assert.AreEqual(0, dic.Redacted.Count);
        }

        [TestMethod]
        public void AddingItems_ShouldBeReflectedInBoth()
        {
            var dic = new DoubleDic<string, string>("XXX");
            const string test = "test";
            dic[test] = test;

            Assert.AreEqual(test, dic.Exposed[test]);
            Assert.AreEqual(test, dic.Redacted[test]);
        }

        [TestMethod]
        public void RemovingItems_ShouldBeReflectedInBoth()
        {
            var dic = new DoubleDic<string, string>("XXX");
            const string test = "test";
            dic[test] = test;
            dic.Remove(test);

            Assert.ThrowsException<KeyNotFoundException>(() => dic.Exposed[test]);
            Assert.ThrowsException<KeyNotFoundException>(() => dic.Redacted[test]);
        }

        [TestMethod]
        public void NullRedactedValue_ShouldBeNull()
        {
            const string sensitiveKey = "sensitive";

            var dic = new DoubleDic<string, string>(null as string, sensitiveKey);
            dic[sensitiveKey] = "test";
            Assert.IsNull(dic.Redacted[sensitiveKey]);
        }

        [TestMethod]
        public void NullRedactedAction_ShouldThrow()
        {
            // ReSharper disable once AssignNullToNotNullAttribute - testing nullability
            Assert.ThrowsException<ArgumentNullException>(() => new DoubleDic<string, string>(null as Func<string, string>));
        }

        [TestMethod]
        public void DefaultReplacements_ShouldBeEmpty_Params()
        {
            var dic = new DoubleDic<string, string>("XXX", default);
            Assert.AreEqual(0, dic.SensitiveKeys.Count);
        }

        [TestMethod]
        public void DefaultReplacements_ShouldBeEmpty_Enumerable()
        {
            var dic = new DoubleDic<string, string>("XXX", (IEnumerable<string>)default);
            Assert.AreEqual(0, dic.SensitiveKeys.Count);
        }

        [TestMethod]
        public void DefaultReplacementFunc_ShouldBeEmpty_Params()
        {
            var dic = new DoubleDic<string, string>(d => "", default);
            Assert.AreEqual(0, dic.SensitiveKeys.Count);
        }

        [TestMethod]
        public void DefaultReplacementFunc_ShouldBeEmpty_Enumerable()
        {
            var dic = new DoubleDic<string, string>(d => "", (IEnumerable<string>)default);
            Assert.AreEqual(0, dic.SensitiveKeys.Count);
        }

        [TestMethod]
        public void RedactedItem_WhenDefaultSpecified_ShouldBeRedacted()
        {
            
        }
    }
}
