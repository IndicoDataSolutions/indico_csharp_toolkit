using Indico;

namespace IndicoToolkit
{
    /// <summary>
    /// Client for the Indico Toolkit.
    /// </summary>
    public class Client {
        public string Host { get; }
        public string ApiTokenString { get; }
        public string ApiTokenPath { get; }
        public bool Verify { get; }

        public Client(
            string host,
            string apiTokenString = "",
            string apiTokenPath = "",
            bool verify = true
        ) {
            Host = host;
            ApiTokenString = apiTokenString;
            ApiTokenPath = apiTokenPath;
            Verify = verify;
        }

        /// <summary>
        /// Instantiates Indico API client.
        /// </summary>
        public IndicoClient Create() {
            IndicoConfig config = new IndicoConfig(
                apiToken: ApiTokenString, 
                tokenPath: ApiTokenPath,
                host: Host,
                verify: Verify
            );
            return new IndicoClient(config);
        } 
        
    }
}