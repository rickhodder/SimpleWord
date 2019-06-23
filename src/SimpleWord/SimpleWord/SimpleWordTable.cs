using System;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SimpleWord
{
    public class SimpleWordTable : IDisposable
    {
        public Table Table { get; private set; }

        public SimpleWordTable(Table table)
        {
            Table = table;
        }

        public void Dispose()
        {
            Table = null;
        }
    }
}