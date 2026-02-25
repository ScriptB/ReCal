# ReCal

ReCal is a WPF desktop application for calculating the cost and material requirements for crafting Wall Lockers in Unturned. It includes automatic update support via GitHub Releases.

## Download (Easy)

1. Go to the [GitHub Releases page](https://github.com/ScriptB/ReCal/releases).
2. Download the latest `ReCal-<version>.zip` asset.
3. Extract the ZIP and run `ReCal.exe`.

Tip: The EXE is inside the ZIP so users always get the required DLLs.

## Installation (Quick Guide)

1. Open the [Releases page](https://github.com/ScriptB/ReCal/releases).
2. Under the latest release, download `ReCal-<version>.zip`.
3. Right-click the ZIP and choose **Extract All**.
4. Open the extracted folder and double-click `ReCal.exe`.
5. If Windows shows a SmartScreen prompt, click **More info** then **Run anyway**.

## Build (Developer)

1. Install Visual Studio 2022 (or later) with .NET Framework 4.8.
2. Open `ReCal.sln`.
3. Build the `Release` configuration.

The output will be in `src/bin/Release/net48/`.

## Repository Layout

- `src/` WPF application source (net48)
- `assets/` icons and images used by the app
- `docs/` additional documentation
- `tools/` build and release scripts
- `ReCal.sln` solution file

## Release Process

See `docs/RELEASE.md` for the public release workflow.

## License

Proprietary. See `LICENSE`.
