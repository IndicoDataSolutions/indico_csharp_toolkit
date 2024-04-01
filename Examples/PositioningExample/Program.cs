using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using IndicoToolkit;
using IndicoToolkit.IndicoWrapper;
using IndicoToolkit.AutoReview;
using IndicoToolkit.Types;

namespace Examples
{
    /// <summary>
    /// Extracts token data from document and stores all the tokens that are under tokens with
    /// 'header' in its text.
    /// </summary>
    public class PositioningExample
    {   
        private static string GetHost() => Environment.GetEnvironmentVariable("INDICO_HOST");
        private static string GetToken() => Environment.GetEnvironmentVariable("INDICO_KEY");
        private static string GetPath() => Environment.GetEnvironmentVariable("FILE_PATH");

        public static async Task Main()
        {
            /// Instantiate the Positioning class
            Positioning positioning = new Positioning();

            /// Instantiate the DocExtraction class
            IndicoClient client = new Client(host: GetHost(), apiTokenString: GetToken()).Create();
            DocExtraction docExtraction = new DocExtraction(client, docExtractionPreset: DocumentExtractionPreset.OnDocument);

            /// Submit document and save token position data to variable
            OnDoc docText = docExtraction.RunOCR(GetPath());

            // Split tokens between header tokens and doc tokens
            List<Token> headerTokens = new List<Token>();
            List<Token> docTokens = new List<Token>();
            foreach (Token docToken in docText.ondoc.tokens)
            {
                if (docToken.text.Contains("header"))
                {
                    headerTokens.Add(docToken);
                }
                else
                {
                    docTokens.Add(docToken);
                }
            }

            // for each docToken, check if it's under a header and
            // .7 of its position horizontally overlaps with the header
            List<Token> underHeaderTokens = new List<Token>();
            foreach (docToken in docTokens)
            {
                foreach (headerToken in headerTokens)
                {
                    if (positioning.positionedAboveOverlap(headerToken.position, docToken.position, 0.7f))
                    {
                        underHeaderTokens.Add(docToken);
                    }
                }
            }

            // Print output to console
            Console.WriteLine(underHeaderTokens);
    }
}
