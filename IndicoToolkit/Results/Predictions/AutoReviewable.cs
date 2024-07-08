using Newtonsoft.Json.Linq;

namespace IndicoToolkit.Results;


public class AutoReviewable : Prediction
{
    [NoPrint]
    public bool Accepted { get; protected set; }
    [NoPrint]
    public bool Rejected { get; protected set; }

    public void Accept()
    {
        Unreject();
        Accepted = true;
    }

    public void Unaccept()
    {
        Accepted = false;
    }

    public void Reject()
    {
        Unaccept();
        Rejected = true;
    }

    public void Unreject()
    {
        Rejected = false;
    }
}
