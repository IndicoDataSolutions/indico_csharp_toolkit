using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Results;


public class Extraction : Prediction
{
    [NoPrint]
    public bool Accepted { get; protected set; }
    [NoPrint]
    public bool Rejected { get; protected set; }

    public string Text { get; set; }
    public int Page { get; set; }

    public void Accept()
    {
        Accepted = true;
        Rejected = false;
    }

    public void Unaccept()
    {
        Accepted = false;
    }

    public void Reject()
    {
        Accepted = false;
        Rejected = true;
    }

    public void Unreject()
    {
        Rejected = false;
    }
}
