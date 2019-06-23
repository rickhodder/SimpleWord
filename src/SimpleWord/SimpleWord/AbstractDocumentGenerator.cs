using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SimpleWord
{
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

        public TableBuilder<TDataClass> CreateTableBuilder<TDataClass>(TableDefinition<TDataClass> definition, ColorScheme colorScheme)
        {
            return new TableBuilder<TDataClass>(_document, definition, colorScheme);
        }

        public void AddTable(SimpleWordTable table)
        {
            _document.AddTable(table);
        }

        public void AddText(string text)
        {
            _document.AddText(text);
        }
        public void AddText(string text, string style)
        {
            _document.AddText(text, style);
        }

    }
}