using System;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SimpleWord
{
    public class SimpleWordBody: IDisposable
    {
        private Body _body;

        public SimpleWordBody(Body body)
        {
            _body = body;
        }

        public void Dispose()
        {
            _body = null;
        }

        public void AddBlankLine()
        {
            Paragraph para = _body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text(""));
        }

        public void AddPageBreak()
        {
            _body.AppendChild(new Paragraph(new Run(new Break { Type = BreakValues.Page })));
        }

        public void AddText(string text)
        {
            _body.Append(new Paragraph(new Run( new Text(text))));
        }

        public void AddText(string text, string style)
        {
            var para = _body.AppendChild(new Paragraph());
            para.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = style });
            para.AppendChild(new Run(new Text(text)));
        }

        public void AddTable(SimpleWordTable table)
        {
            _body.Append(table.Table);
        }
    }
}