using DocumentFormat.OpenXml.Drawing;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace SimpleWord
{
    public abstract class AbstractGeneratorDocument
    {
        public abstract void Create();
        public abstract void Open();
        public abstract void CreateParagraph();
        public abstract void CreateRun();
        public abstract void CreateText();
        public abstract void CreateTable();
        public abstract void ApplyStyle(); // styles document
        public abstract void ApplyStyle(Table table); // should be on table builder
        public abstract void ApplyStyle(Paragraph paragraph);
        public abstract void ApplyStyle(Run run);
        public abstract void ApplyStyle(Text text);
    }
}