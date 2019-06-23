namespace SimpleWord
{
    public class DocumentGeneratorRequest
    {
        public string FileName { get; set; }
        public string TemplateFileName { get; set; }
        public ColorScheme ColorScheme { get; set; } = new DefaultColorScheme();
    }
}
