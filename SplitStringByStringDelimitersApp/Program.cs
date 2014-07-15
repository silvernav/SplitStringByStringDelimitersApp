using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitStringByStringDelimitersApp
{
    class Program
    {
        static void Main()
        {
            var program = new Program();
            program.Run();

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private void Run()
        {
            var splitter = new StringSplitter();

            splitter.Source = "The quick brown fox jumps over the lazy dog The quick brown fox jumps over the lazy dog";
            splitter.Delimiters = new List<string> { "quick", "jumps", "lazy" };
            splitter.Split();

            List<Entry> del = splitter.DelimiterEntries;
            List<int> delIndexes = del.Select(e => e.Index).ToList();
            List<string> delValues = del.Select(e => e.Value).ToList();


            List<Entry> x = splitter.ContentEntries;
            List<int> xIndexes = x.Select(e => e.Index).ToList();
            List<string> xValues = x.Select(e => e.Value).ToList();


            List<Entry> z = splitter.AllEntries;
            List<int> zIndexes = z.Select(e => e.Index).ToList();
            List<string> zValues = z.Select(e => e.Value).ToList();

        }
    }

    public class StringSplitter
    {
        private string source = "The quick brown fox jumps over the lazy dog The quick brown fox jumps over the lazy dog";
        private List<string> delimiters = new List<string> { "quick", "jumps", "lazy" };

        public string Source { get; set; }

        public List<string> Delimiters { get; set; }

        public List<Entry> DelimiterEntries { get; private set; }

        public List<Entry> ContentEntries { get; private set; }

        public List<Entry> AllEntries
        {
            get
            {
                //  Make shallow copies of lists containing delimiters and content.
                List<Entry> delimiterEntries = DelimiterEntries.ToList();
                List<Entry> contentEntries = ContentEntries.ToList();

                //  Make into one single list.
                delimiterEntries.AddRange(contentEntries);

                //  Sort by indexes.
                delimiterEntries = delimiterEntries.OrderBy(e => e.Index).ToList();

                return delimiterEntries;
            }
        }

        public void Split()
        {
            var delimiterEntries = new List<Entry>();
            var contentEntries = new List<Entry>();

            GetDelimiterEntries(delimiterEntries);

            delimiterEntries = delimiterEntries.OrderBy(e => e.Index).ToList();


            string value;
            if (delimiterEntries.Count == 0)
            {
                var singleEntry = new Entry(source, 0, source.Length);
                contentEntries.Add(singleEntry);
            }
            else if (delimiterEntries.Count == 1)
            {
                //  Content entry before delimiter.
                if (delimiterEntries.Single().Index > 0)
                {
                    value = source.Substring(0, delimiterEntries[0].Index);
                    var startEntry = new Entry(value, delimiterEntries[0].Index, delimiterEntries[0].Index);
                    contentEntries.Add(startEntry);
                }

                //  Content entry after last delimiter.
                Entry last = delimiterEntries.Last();
                if (last.Index + last.Length < source.Length)
                {
                    //  Get what comes after last delimter.
                    value = source.Substring(last.Index + last.Length);
                    var endEntry = new Entry(value, last.Index + last.Length, value.Length);
                    contentEntries.Add(endEntry);
                }
            }
            else
            {
                GetContentEntriesWhenMoreThanTwoDelimiters(delimiterEntries, contentEntries);
            }

            string check = string.Join("", contentEntries.Select(s => s.Value));

            //  Set properties.
            DelimiterEntries = delimiterEntries;
            ContentEntries = contentEntries;
        }

        private void GetContentEntriesWhenMoreThanTwoDelimiters(List<Entry> delimiterEntries, List<Entry> contentEntries)
        {
            string value;

            //  Content entry before first delimiter.
            Entry first = delimiterEntries.First();
            if (first.Index > 0)
            {
                value = source.Substring(0, first.Index);
                var startEntry = new Entry(value, 0, value.Length);
                contentEntries.Add(startEntry);
            }

            //  Content entries between first and last delimiter.
            ManageBetweenFirstAndLast(delimiterEntries, contentEntries);

            //  Content entry after last delimiter.
            Entry last = delimiterEntries.Last();
            if (last.Index + last.Length < source.Length)
            {
                //  Get what comes after last delimter.
                value = source.Substring(last.Index + last.Length);
                var endEntry = new Entry(value, last.Index + last.Length, value.Length);
                contentEntries.Add(endEntry);
            }
        }

        private void ManageBetweenFirstAndLast(List<Entry> delimiterEntries, List<Entry> contentEntries)
        {
            //  Manage content entries between first and last delimiter.

            //  If there is a gap between two delimiters, index + length 
            //  of the first delimiter is less than index of next delimiter.
            //
            //  E.g.
            //  If "bc" and "gh" are delimters.
            //  |0||1||2||3||4||5||6||7||8||9|
            //  |a||b||c||d||e||f||g||h||i||j|
            //
            //  "def" is 
            //
            //  Index of first delimiter: 1
            //  Length of first delimiter: 2
            //  Index of next delimiter: 6
            //
            //  1 + 2 = 3 is less than 6.

            for (int entryIndex = 0; entryIndex < delimiterEntries.Count - 1; entryIndex++)
            {
                Entry currentEntry = delimiterEntries[entryIndex];
                Entry nextEntry = delimiterEntries[entryIndex + 1];

                //  What would next delimiter index be, if there was not a gap.
                int nextNoGapIndex = currentEntry.Index + currentEntry.Length;
                int gapSize = nextEntry.Index - nextNoGapIndex;
                if (gapSize > 0)
                {
                    //  We have a gap i.e. content between two delimiters.
                    //  We create a content entry and add it to otherEntries.
                    string value = source.Substring(nextNoGapIndex, gapSize);
                    var intermediateEntry = new Entry(value, nextNoGapIndex, gapSize);
                    contentEntries.Add(intermediateEntry);
                }
            }
        }

        private void GetDelimiterEntries(List<Entry> delimiterEntries)
        {
            //
            //  Creates a list where each list item is a entry representing a delimiter:
            //  -   Value: e.g. ";" or "Name:" (ignore quotation marks).
            //  -   Index: Position in source.
            //  -   Length: Length of delimiter.
            foreach (string delimiter in delimiters)
            {
                int searchIndex = 0;
                int index;
                do
                {
                    index = source.IndexOf(delimiter, searchIndex);
                    if (index != -1)
                    {
                        var entry = new Entry(delimiter, index, delimiter.Length);
                        delimiterEntries.Add(entry);
                    }
                    searchIndex = index + delimiter.Length;
                } while (searchIndex <= source.Length && index != -1);
            }
        }
    }

    public class Entry
    {
        public string Value { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public Entry(string value, int index, int length)
        {
            Value = value;
            Index = index;
            Length = length;
        }
    }
}
