using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SimpleWord
{
    public abstract class AbstractDocumentGenerator
    {
        protected SimpleWordDocument Document;
        protected DocumentGeneratorRequest Request;

        public void Generate(DocumentGeneratorRequest request)
        {
            Request = request;

            using (CreateDocument())
            {
                PerformGeneration();
            }

            Request = null;
        }

        public abstract void PerformGeneration();

        protected SimpleWordDocument CreateDocument()
        {
            if (File.Exists(Request.FileName))
            {
                File.Delete(Request.FileName);
            }

            if (!string.IsNullOrEmpty(Request.TemplateFileName))
            {
                CreateFileFromTemplate();
            }
            else
            {
                CreateFile();
            }

            return Document;
        }

        protected void CreateFileFromTemplate()
        {
            File.Copy(Request.TemplateFileName, Request.FileName);

            var document = WordprocessingDocument.Open(Request.FileName, true);
            var body = new SimpleWordBody(document.MainDocumentPart.Document.Body);

            Document = new SimpleWordDocument(document, body);
        }

        protected void CreateFile()
        {
            var document = WordprocessingDocument.Create(Request.FileName, WordprocessingDocumentType.Document);
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = new Body();

            mainPart.Document.AppendChild(body);

            Document = new SimpleWordDocument(document, new SimpleWordBody(body));
        }

        public void AddBlankLine()
        {
            Document.AddBlankLine();
        }

        public void AddPageBreak()
        {
            Document.AddPageBreak();
        }

        public TableBuilder<TDataClass> CreateTableBuilder<TDataClass>(TableDefinition<TDataClass> definition, ColorScheme colorScheme)
        {
            return new TableBuilder<TDataClass>(Document, definition, colorScheme);
        }

        public void AddTable(SimpleWordTable table)
        {
            Document.AddTable(table);
        }

        public void AddText(string text)
        {
            Document.AddText(text);
        }
        public void AddText(string text, string style)
        {
            Document.AddText(text, style);
        }
    }
}