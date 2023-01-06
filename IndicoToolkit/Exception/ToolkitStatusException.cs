namespace IndicoToolkit.Exception
{
    public class ToolkitStatusException : System.Exception
    {
        public ToolkitStatusException() { }

        public ToolkitStatusException(string message) : base(message) { }
    }
}