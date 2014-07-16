using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitStringByStringDelimitersApp.Test2
{
    [TestFixture]
    public class StringSplitterTest
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = @"String to split (Property Source) cannot be null")]
        public void Split_SourceIsNull_ThrowsInvalidOperationException()
        {
            var stringSplitter = new StringSplitter();
            stringSplitter.Delimiters = new List<string> { "a" };
            stringSplitter.Split();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = @"Delimiters (Property Delimiters) cannot be null")]
        public void Split_DelimitersIsNull_ThrowsInvalidOperationException()
        {
            var stringSplitter = new StringSplitter();
            stringSplitter.Source = "";
            stringSplitter.Split();
        }

        [Test]
        public void Split_EmptySource_SourceRecreated()
        {
            string source = "";
            var delimiters = new List<string> { "a", "g" };
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when source is an empty string");
        }

        [Test]
        public void Split_DelimiterFirstAndLast_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string> { "a", "g" };
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when first and last items in source are delimiters");
        }

        [Test]
        public void Split_NoDelimiters_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string>();
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when there are no delimiters");
        }

        [Test]
        public void Split_EmptySourceNoDelimiters_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string>();
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when source is empty and there are no delimiters");
        }

        [Test]
        public void Split_SingleDelimiterInMiddle_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string>{ "c d e"};
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when single delimiter in middle");
        }

        [Test]
        public void Split_SingleDelimiterFirst_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string> { "a b" };
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when single delimiter appears first in source");
        }

        [Test]
        public void Split_SingleDelimiterLast_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string> { "f g" };
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when single delimiter appears last in source");
        }


        [Test]
        public void Split_EntireSourceOnlyContainsDelimiters_SourceRecreated()
        {
            string source = "a b c d e f g";
            var delimiters = new List<string> { "a b ", "c d ", "e f g" };
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when source only contains delimiters");
        }

        [Test]
        public void Split_QuotationMarkAsDelimiter_SourceRecreated()
        {
            string source = "a b c \" e f g";
            var delimiters = new List<string> { "\""};
            string sourceRecreated = SplitMerge(source, delimiters);
            Assert.AreEqual(source, sourceRecreated, "Failed when quotation mark is used as delimiter");
        }

        private string SplitMerge(string source, List<string> delimiters)
        {
            var stringSplitter = new StringSplitter();
            stringSplitter.Source = source;
            stringSplitter.Delimiters = delimiters;
            stringSplitter.Split();

            List<Entry> allEntries = stringSplitter.AllEntries;
            string sourceRecreated = string.Join("", allEntries.Select(e => e.Value));
            return sourceRecreated;
        }

    }
}
