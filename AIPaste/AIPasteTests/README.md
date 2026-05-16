# AIPasteTests

`AIPasteTests` is the MSTest project for AIPaste.

## Prerequisites

Run the commands below from the repository root in **Developer PowerShell for Visual Studio** or another shell where `msbuild` and `vstest.console.exe` are available.

```powershell
Set-Location <repository-root>
```

## Build the test assembly

```powershell
msbuild `
  '.\AIPaste\AIPasteTests\AIPasteTests.csproj' `
  /t:Build `
  /p:Configuration=Debug `
  /p:Platform=x64 `
  /nologo
```

## Run all tests

```powershell
vstest.console.exe `
  '.\AIPaste\AIPasteTests\bin\x64\Debug\net8.0-windows10.0.26100.0\AIPasteTests.dll' `
  /Platform:x64
```

## Run a filtered set of tests

```powershell
vstest.console.exe `
  '.\AIPaste\AIPasteTests\bin\x64\Debug\net8.0-windows10.0.26100.0\AIPasteTests.dll' `
  /Platform:x64 `
  /TestCaseFilter:"FullyQualifiedName~SettingsPageViewModelTests|FullyQualifiedName~SettingsServiceTests"
```

## Notes

- Some tests are integration-style tests.
- `Models\LLMModels\LlmTextCorrectorTests.GenerateTextByGeminiLlm` requires a `.env` file under `AIPasteTests` with `GEMINI_API_KEY=...`.
- `Models\LLMModels\LlmTextCorrectorTests.GenerateTextByLocalLlm` downloads and loads a local model, so it is slower than the unit tests.
- In this repository, `dotnet test` may fail in some environments because of Windows App SDK build task resolution. If that happens, use the `msbuild` + `vstest.console.exe` flow above.
