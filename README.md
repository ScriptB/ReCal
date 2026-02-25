# ReCal

ReCal is a WPF desktop application for calculating the cost and material requirements for crafting Wall Lockers in Unturned. It includes automatic update support via GitHub Releases.

## Download (Easy)

1. Go to the GitHub Releases page for this repo.
2. Download the latest `ReCal-<version>.zip` asset.
3. Extract the ZIP and run `ReCal.exe`.

Tip: The EXE is inside the ZIP so users always get the required DLLs.

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
