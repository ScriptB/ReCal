# Release Process

Public releases are distributed via GitHub Releases. Do not commit build outputs or release ZIPs to the repo.

Repository: https://github.com/ScriptB/ReCal
Releases: https://github.com/ScriptB/ReCal/releases

## 1. Update Versions

- `src/ReCal.csproj`: update `AssemblyVersion`, `FileVersion`, `AssemblyInformationalVersion`.
- `src/Models/AutoUpdater.cs`: update `CURRENT_VERSION`.

## 2. Build Release

Option A: Visual Studio
- Open `ReCal.sln` and build `Release | Any CPU`.

Option B: CLI
- `dotnet build -c Release`

Output path:
- `src/bin/Release/net48/`

## 3. Create Release ZIP

1. Create an `artifacts/` folder in the repo root (it is ignored by git).
2. Zip the contents of `src/bin/Release/net48/` into `artifacts/ReCal-X.Y.Z.zip`.
3. Exclude `*.pdb` files from the ZIP.

## 4. Tag and Publish

1. Tag the release: `git tag vX.Y.Z`
2. Push tag: `git push origin vX.Y.Z`
3. Create a GitHub Release for the tag and upload `ReCal-X.Y.Z.zip`.

## 5. Verify

- Download the release ZIP from GitHub.
- Run `ReCal.exe` and confirm the update checker resolves the latest tag.
