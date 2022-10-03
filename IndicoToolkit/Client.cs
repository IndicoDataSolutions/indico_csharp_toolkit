using System;
using IndicoV2;

namespace IndicoToolkit
{
    /// <summary>
    /// Client for the Indico Toolkit.
    /// </summary>
    public class Client
    {
        public string Host { get; }
        public string ApiTokenString { get; }
        public bool Verify { get; }

        public Client(
            string host,
            string apiTokenString = "",
            bool verify = true
        )
        {
            Host = host;
            ApiTokenString = apiTokenString;
            Verify = verify;
        }

        /// <summary>
        /// Instantiates IndicoV2 API client.
        /// </summary>
        public IndicoClient Create()
        {
            Uri hostUri = new Uri(Host);
            return new IndicoClient(apiToken: ApiTokenString, baseUri: hostUri);
        }

    }
}