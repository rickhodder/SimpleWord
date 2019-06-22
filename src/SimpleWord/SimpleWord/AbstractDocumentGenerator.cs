using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Break = DocumentFormat.OpenXml.Drawing.Break;
using Paragraph = DocumentFormat.OpenXml.Drawing.Paragraph;
using Run = DocumentFormat.OpenXml.Drawing.Run;

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
            Paragraph para = _body.AppendChild(new Paragraph());
            para.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = style });
            para.AppendChild(new Run(new Text(text)));
        }

    }

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
    }

    public abstract class AbstractDocumentGenerator
    {

        protected SimpleWordDocument _document;
        protected DocumentGeneratorRequest _request;

        public void Generate(DocumentGeneratorRequest request)
        {
            _request = request;

            using (var doc = CreateDocument())
            {
                PerformGeneration();
            }
        }

        public abstract void PerformGeneration();

        protected SimpleWordDocument CreateDocument()
        {
            if (File.Exists(_request.FileName))
            {
                File.Delete(_request.FileName);
            }

            if (!string.IsNullOrEmpty(_request.TemplateFileName))
            {
                CreateFileFromTemplate();
            }
            else
            {
                CreateFile();
            }

            return _document;
        }

        protected void CreateFileFromTemplate()
        {
            File.Copy(_request.TemplateFileName, _request.FileName);

            var document = WordprocessingDocument.Open(_request.FileName, true);
            var body = new SimpleWordBody(document.MainDocumentPart.Document.Body);

            _document = new SimpleWordDocument(document, body);
        }

        protected void CreateFile()
        {
            var document = WordprocessingDocument.Create(_request.FileName, WordprocessingDocumentType.Document);
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();
            
            mainPart.Document.AppendChild(body);

            _document = new SimpleWordDocument(document, new SimpleWordBody(body));
        }

        public void AddBlankLine()
        {
            _document.AddBlankLine();
        }

        public void AddPageBreak()
        {
            _document.AddPageBreak();
        }

        public TableBuilder<TDataClass> CreateTableBuilder<TDataClass>(SimpleWordDocument document, TableDefinition<TDataClass> definition, ColorScheme colorScheme)
        {
            return new TableBuilder<TDataClass>(SimpleWordDocument document, definition, colorScheme);
        }


    }
}