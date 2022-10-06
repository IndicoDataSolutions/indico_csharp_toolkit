namespace IndicoToolkit.Types
{

    public class OcrToken
    {
        public DocOffset DocOffset { get; set; }
        public int Index { get; set; }
        public Position Position { get; set; }

        public OcrToken(DocOffset docOffset, int index, Position position)
        {
            DocOffset = docOffset;
            Index = index;
            Position = position;
        }

    }


}