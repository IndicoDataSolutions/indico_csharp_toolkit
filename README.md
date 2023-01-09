# Indico-Toolkit
A C# class library to assist Indico IPA development

### Available Functionality
The indico toolkit provides classes and functions to help achieve the following:
* Easy batch workflow submission and retrieval.
* Classes that simplify dataset/doc-extraction functionality.
* Tools to assist with positioning, e.g. row association, distance between preds, relative position validation.
* Common manipulation of prediction/workflow results.
* Objects to simplify parsing OCR responses.
* Class to spoof a human reviewer.

### Installation
```
<TODO>
```

### Example Useage
<TODO>

### Tests
To run the test suite, run `dotnet test` within the `IndicoToolkit.Tests` folder.

You will need the following environment variables to run the entire test suite:
- `INDICO_HOST`: Indico host URL
- `INDICO_KEY`: Corresponding API key value to the host.
- `DATASET_ID`: Dataset Id linked to workflow with fully trained model
- `WORKFLOW_ID`: Corresponding workflow Id to the dataset
- `MODEL_ID`: Corresponding model Id to the dataset

### Example 
How to get prediction results and write to CSV
```
using System;
using System.IO;
using System.Linq;

using IndicoToolkit;
using IndicoToolkit.IndicoWrapper;
using IndicoToolkit.Types;

public class Program
{
    private static string GetHost() -> Environment.GetEnvironmentVariable("INDICO_HOST");
    private static string GetToken() => Environment.GetEnvironmentVariable("INDICO_KEY");
    private static string GetWorkflowId() => Environment.GetEnvironmentVariable("WORKFLOW_ID");
    private static string GetDirectory() => Path.Join(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "base_directory");

    public static async Task Main()
    {
        /// Instantiate the Workflows class
        IndicoClient client = new Client(host: GetHost(), apiTokenString: GetToken()).Create();
        Workflows Wflow = new Workflows(client);

        /// Collect files to submit
        FileProcessing Fp = new FileProcessing();
        Fp.GetFilePathsFromDir(GetDirectory()); 

        /// Submit documents, await the results and write the results to console in batches of 10.
        foreach (List<string> paths in Fp.BatchFiles(batchSize: 10))
        {
            IEnumerable<int> submissionIds = await Wflow.SubmitDocumentsToWorkflow(GetWorkflowId(), paths);
            List<WorkflowResult> results = Wflow.GetSubmissionResultsFromIds(submissionIds.ToList());
            Console.WriteLine(results);
        }
    }
}
```

### Contributing

If you are adding new features to Indico Toolkit, make sure to:

* Add robust integration and unit tests.
* Add a sample usage script to the 'examples/' directory.
* Add a bullet point for what the feature does to the list at the top of this README.md.
* Ensure the full test suite is passing locally before creating a pull request.
* Add [document comments](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments) for methods where usage is non-obvious.