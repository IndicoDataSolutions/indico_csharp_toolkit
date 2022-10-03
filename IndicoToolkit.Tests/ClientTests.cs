using System.Net.Http;
using Xunit;
using IndicoV2;

namespace IndicoToolkit.Tests
{
    public class ClientTests
    {

        [Fact]
        public void TestClientCreation()
        {
            IndicoClient client = new Client(host: "http://app.indico.io", apiTokenString: Utils.api_key).Create();
            Assert.IsType<IndicoClient>(client);
        }

        [Fact]
        public void TestClientFail()
        {
            IndicoClient client = new Client(host: "http://app.indico.io", apiTokenString: "not_a_real_token").Create();
            Assert.ThrowsAsync<HttpRequestException>(() => client.DataSets().ListFullAsync());
        }
    }
}