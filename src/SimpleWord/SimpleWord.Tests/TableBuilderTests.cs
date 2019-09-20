using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleWord.Tests
{
    [TestClass]
    public class TableBuilderTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sut=new TableBuilder<TestData>(new SimpleWordDocument(new WordprocessingDocument(), new SimpleWordBody()))
        }
    }

    public class TestData
    {

    }
}
