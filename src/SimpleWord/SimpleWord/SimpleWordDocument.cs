using System;
using DocumentFormat.OpenXml.Packaging;

namespace SimpleWord
{
    public class SimpleWordDocument : IDisposable
    {

        public SimpleWordBody Body { get; private set; }
        public WordprocessingDocument Document { get; private set; }
    
        public SimpleWordDocument(WordprocessingDocument document, SimpleWordBody body)
        {
            Document = document;
            Body = body;
        }

        public void Dispose()
        {
            Body?.Dispose();
            Document?.Dispose();
            Body = null;
            Document = null;
        }

        public void AddBlankLine()
        {
            Body.AddBlankLine();
        }

        public void AddPageBreak()
        {
            Body.AddPageBreak();
        }

        public void AddText(string text)
        {
            Body.AddText(text);
        }

        public void AddText(string text, string style)
        {
            Body.AddText(text,style);
        }

        public void AddTable(SimpleWordTable table)
        {
            Body.AddTable(table);
        }
    }
}