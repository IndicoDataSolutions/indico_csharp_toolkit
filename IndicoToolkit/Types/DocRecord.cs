namespace IndicoToolkit.Types
{
    public class DocRecord
    {
        public string filename { get; set; }
        public string docText { get; set; }
        public string docClass { get; set; }
    }

    // public class DocRecordMap: ClassMap<DocRecord>
    // {
    //     public DocRecordMap()
    //     {
    //         Map(m => m.filename).Index(0).Name("filename");
    //         Map(m => m.docClass).Index(1).Name("docClass");
    //         Map(m => m.docText).Index(1).Name("docText");
    //     }
    // }
}