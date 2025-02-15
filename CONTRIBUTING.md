# Contribution Guidelines


## Contributing to Infer#

To contribute, fork InferSharp and file a [pull request](https://github.com/microsoft/infersharp/pulls). When modifying OCaml source code, please follow Infer's [contribution guidelines](https://github.com/facebook/infer/blob/master/CONTRIBUTING.md).

### Prerequisites

* [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

* Packages listed in our dockerfile [here](https://github.com/microsoft/infersharp/blob/main/Dockerfile)

### Installation and Build

Both the C# and the OCaml components must be separately built. Each set of build commands is assumed to be executed from the repository root.

**OCaml**

```bash
cd infer
./build-infer.sh java
./autogen.sh
sudo make install 
```

Note -- you might need to complete these instructions with sudo if there are permission issues.

**C#**

For the core translation pipeline:
```bash
dotnet build Infersharp.sln
```

For C# library models (optional but highly recommended for reducing false positive warnings):

```bash
./build_csharp_models.sh
``` 

### Debugging Infer#

To obtain an analysis on a directory tree of .NET binaries (comprised of both DLLs and PDBs), execute the following commands from the repository root:
```bash
# Extract CFGs from binary files.
dotnet Cilsil/bin/Debug/net6.0/Cilsil.dll translate {directory_to_binary_files} \
                                                --outcfg {output_directory}/cfg.json \
                                                --outtenv {output_directory}/tenv.json \
                                                --cfgtxt {output_directory}/cfg.txt

# Run Infer on extracted CFGs.
infer capture
mkdir infer-out/captured
infer analyzejson --debug \
                  --cfg-json {output_directory}/cfg.json \
                  --tenv-json {output_directory}/tenv.json
```

For debugging Infer# in your test, please note:
* The CFG is expressed in a text format in {output_directory}/cfg.txt.
* Reported bugs are located at {output_directory}/infer-out/bugs.txt.
* Infer output is located at {output_directory}/infer-out/; detailed analysis information is located at {output_directory}/infer-out/captured/.

## Coding Style

### All Languages

* Line width limit is 100 characters.
* Conform to indentation conventions and other stylistic aspects of the surrounding code.
 
### C#

Please adhere to Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

### OCaml

Please adhere to Infer's [OCaml Style Guide](https://github.com/facebook/infer/blob/master/CONTRIBUTING.md#ocaml).

## Testing Your Changes

The testing framework first programmatically generating pieces of C# code, the underlying bytecode of which isolates the instructions to be validated. It then builds the source code and runs the translation core on the resulting binaries in order to produce the CFG, represented as a JSON file. Finally, Infer analyzes the CFG and produces warnings, which in turn are validated against those which are expected. 

Utilities for generating test code are located [here](https://github.com/microsoft/infersharp/blob/main/Cilsil.Test/Assets/Utils.cs). The test execution is orchestrated [here](https://github.com/microsoft/infersharp/blob/main/Cilsil.Test/TestRunManager.cs). After making a technical contribution to the codebase, please also consider the following:

  * [Build](https://github.com/microsoft/infersharp/blob/main/CONTRIBUTING.md#installation-and-build) the modified codebase. 

  * Add test cases to Cilsil.Test/E2E/NPETest.cs. 
  
  * Try to reuse existing [test assets](https://github.com/microsoft/infersharp/tree/main/Cilsil.Test/Assets), but make modifications as necessary.
  
  * Run the tests via: `dotnet test`.


## Reporting Issues

If you encounter any issues, please open an [issue](https://github.com/microsoft/infersharp/issues).


## Contributor License Agreement

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

