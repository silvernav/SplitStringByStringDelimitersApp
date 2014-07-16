using System;
using System.Collections.Generic;
using System.Linq;


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
        }
    }

    public class StringSplitter
    {
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
                List<Entry> allEntries = delimiterEntries.OrderBy(e => e.Index).ToList();

                return allEntries;
            }
        }

        public void Split()
        {
            if (Source == null) throw new InvalidOperationException(@"String to split (Property Source) cannot be null");
            if (Delimiters == null) throw new InvalidOperationException(@"Delimiters (Property Delimiters) cannot be null");

            List<Entry> delimiterEntries = GetDelimiterEntries();

            var contentEntries = new List<Entry>();

            if (delimiterEntries.Count == 0)
            {
                //  When no delimiters are found in source, the very source is one single content entry.
                var singleContentEntry = new Entry(Source, 0, Source.Length);
                contentEntries.Add(singleContentEntry);
            }
            else
            {
                GetContentEntries(delimiterEntries, contentEntries);
            }

            //  Set properties.
            DelimiterEntries = delimiterEntries;
            ContentEntries = contentEntries;
        }

        private void GetContentEntries(List<Entry> delimiterEntries, List<Entry> contentEntries)
        {
            string value;

            //  Content entry before first delimiter.
            Entry first = delimiterEntries.First();
            if (first.Index > 0)
            {
                value = Source.Substring(0, first.Index);
                var startEntry = new Entry(value, 0, value.Length);
                contentEntries.Add(startEntry);
            }

            //  Content entries between first and last delimiter.
            GentContentEntriesBetweenFirstAndLastDelimiterEntries(delimiterEntries, contentEntries);

            //  Content entry after last delimiter.
            Entry last = delimiterEntries.Last();
            if (last.Index + last.Length < Source.Length)
            {
                //  Get what comes after last delimter.
                value = Source.Substring(last.Index + last.Length);
                var endEntry = new Entry(value, last.Index + last.Length, value.Length);
                contentEntries.Add(endEntry);
            }
        }

        private void GentContentEntriesBetweenFirstAndLastDelimiterEntries(List<Entry> delimiterEntries, List<Entry> contentEntries)
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
                    string value = Source.Substring(nextNoGapIndex, gapSize);
                    var intermediateEntry = new Entry(value, nextNoGapIndex, gapSize);
                    contentEntries.Add(intermediateEntry);
                }
            }
        }

        private List<Entry> GetDelimiterEntries()
        {
            //
            //  Creates a list where each list item is a entry representing a delimiter:
            //  -   Value: e.g. ";" or "Name:" (ignore quotation marks).
            //  -   Index: Position in source.
            //  -   Length: Length of delimiter.

            var delimiterEntries = new List<Entry>();

            foreach (string delimiter in Delimiters)
            {
                int searchIndex = 0;
                int index;
                do
                {
                    index = Source.IndexOf(delimiter, searchIndex, StringComparison.Ordinal);
                    if (index != -1)
                    {
                        var entry = new Entry(delimiter, index, delimiter.Length);
                        delimiterEntries.Add(entry);
                    }
                    searchIndex = index + delimiter.Length;
                } while (searchIndex <= Source.Length && index != -1);
            }

            delimiterEntries = delimiterEntries.OrderBy(e => e.Index).ToList();

            return delimiterEntries;
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
