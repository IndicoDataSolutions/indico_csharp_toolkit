using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace IndicoToolkit.Results;


public class Document : PrettyPrint
{
    public ulong? Id { get; init; }  // v1 result files don't include Document IDs.
    public string? Name { get; init; }  // v1 result files don't include Document Names.
    public string EtlOutputUrl { get; init; }
    public string FullTextUrl { get; init; }

    // Auto review changes must reproduce all model sections that were present in the
    // original result file. This may not be possible from the predictions alone--if a
    // model had an empty section because it didn't produce predictions or if all of
    // the predictions were removed to reject them. As such, the models seen when
    // parsing result files are tracked per-document so that the empty sections can be
    // reproduced later.
    [NoPrint]
    public HashSet<string> ModelSections { get; init; }

    // Create a Document from the root structure of a v1 result file.
    public static Document FromV1Json(JToken json)
    {
        var etlOutputUrl = Utils.Get<string>(json, "etl_output");
        var fullTextUrl = etlOutputUrl.Replace("etl_output.json", "full_text.txt");

        return new Document
        {
            Id = null,
            Name = null,
            EtlOutputUrl = etlOutputUrl,
            FullTextUrl = fullTextUrl,
            ModelSections = new HashSet<string>(),
        };
    }

    // Create a Document from a v3 `submission_results` list item.
    public static Document FromV3Json(JToken json)
    {
        var etlOutputUrl = Utils.Get<string>(json, "etl_output");
        var fullTextUrl = etlOutputUrl.Replace("etl_output.json", "full_text.txt");

        return new Document
        {
            Id = Utils.Get<ulong>(json, "submissionfile_id"),
            Name = Utils.Get<string>(json, "input_filename"),
            EtlOutputUrl = etlOutputUrl,
            FullTextUrl = fullTextUrl,
            ModelSections = new HashSet<string>(),
        };
    }
}
