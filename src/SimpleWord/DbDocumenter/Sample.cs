using System.Diagnostics;
using SimpleWord;

namespace DbDocumenter
{
    public class Sample
    {
        public void Show()
        {
            var x = new DataDictionaryData();
            x.GetDataSchema();

            var request = new DocumentGeneratorRequest
            {
                FileName = @"C:\dev\rick\testdoc.docx",
                TemplateFileName = @"C:\dev\rick\testdoc_Template.docx"
            };

            //            request.ColorScheme.DefaultTableHeaderBackgroundColor = System.Drawing.Color.FromName("Blue");
            //            request.ColorScheme.DefaultTableHeaderForegroundColor = System.Drawing.Color.FromName("White");

            var docGenerator = new DataDictionaryDocumentGenerator();
            docGenerator.Generate(request);
            Process.Start(request.FileName);
        }
    }
}