using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace ReCal.Security
{
    public static class IntegrityChecker
    {
        private const string ManifestFileName = "Integrity.manifest.json";

        public static void VerifyOrWarn(string baseDirectory)
        {
            try
            {
                var manifestPath = Path.Combine(baseDirectory, ManifestFileName);
                if (!File.Exists(manifestPath))
                {
                    return;
                }

                var manifestJson = File.ReadAllText(manifestPath, Encoding.UTF8);
                var manifest = JsonConvert.DeserializeObject<IntegrityManifest>(manifestJson);
                if (manifest?.Files == null || manifest.Files.Count == 0)
                {
                    return;
                }

                var failures = new List<string>();
                foreach (var entry in manifest.Files)
                {
                    if (string.IsNullOrWhiteSpace(entry.Path) || string.IsNullOrWhiteSpace(entry.Sha256))
                    {
                        continue;
                    }

                    var filePath = Path.Combine(baseDirectory, entry.Path);
                    if (!File.Exists(filePath))
                    {
                        failures.Add($"{entry.Path} (missing)");
                        continue;
                    }

                    var hash = ComputeSha256(filePath);
                    if (!hash.Equals(entry.Sha256, StringComparison.OrdinalIgnoreCase))
                    {
                        failures.Add($"{entry.Path} (modified)");
                    }
                }

                if (failures.Count > 0)
                {
                    MessageBox.Show(
                        "Integrity check warning:\n\n" + string.Join("\n", failures) +
                        "\n\nThis does not always indicate malware, but the app files appear altered. " +
                        "If you did not modify the app, re-download from the official source.",
                        "Integrity Warning",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch
            {
                // Never block app startup on integrity checks.
            }
        }

        private static string ComputeSha256(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }

    public sealed class IntegrityManifest
    {
        [JsonProperty("files")]
        public List<IntegrityManifestEntry> Files { get; set; }
    }

    public sealed class IntegrityManifestEntry
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("sha256")]
        public string Sha256 { get; set; }
    }
}
