using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleWord;

namespace DbDocumenter.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var request = new DocumentGeneratorRequest
            {
                FileName = @".\testdoc.docx",
                TemplateFileName = @".\DataDocumentationTemplate.docx",
                ColorScheme =
                {
                    DefaultTableHeaderBackgroundColor = System.Drawing.Color.FromName("Blue"),
                    DefaultTableHeaderForegroundColor = System.Drawing.Color.FromName("White")
                }
            };


            var docGenerator = new DataDictionaryDocumentGenerator();
            docGenerator.Generate(request);
            Process.Start(request.FileName);
        }
    }
}
