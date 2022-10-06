namespace IndicoToolkit.Types
{
    public class DocOffset
    {
        public int Start { get; set; }
        public int End { get; set; }

        public DocOffset(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}