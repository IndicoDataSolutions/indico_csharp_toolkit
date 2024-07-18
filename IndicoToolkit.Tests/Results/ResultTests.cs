using IndicoToolkit.Results;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Xunit;

namespace IndicoToolkit.Tests;


public class ResultTests
{
    // The base directory will be IndicoToolkit.Tests/bin/Debug/net8.0/
    private string SamplesFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "..", "..", "..", "Results", "Samples"
    );

    [Theory]
    [InlineData("2910_v1_unreviewed.json")]
    [InlineData("2911_v1_accepted.json")]
    [InlineData("2912_v1_rejected.json")]
    [InlineData("2913_v3_unreviewed.json")]
    [InlineData("2914_v3_accepted.json")]
    [InlineData("2915_v3_rejected.json")]
    public void TestSampleFiles(string filename)
    {
        var json = File.ReadAllText(Path.Combine(SamplesFolder, filename));
        var result = Result.FromJson(JObject.Parse(json));
        var changes = result.PreReview.ToChanges(result);
        Assert.NotNull(result);
        Assert.NotNull(changes);
    }
}
