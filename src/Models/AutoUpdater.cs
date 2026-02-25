using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ReCal.Models
{
    public class UpdateInfo
    {
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleaseNotes { get; set; }
        public bool IsMandatory { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class AutoUpdater
    {
        private static readonly string GITHUB_REPO = "ScriptB/ReCal";
        private static readonly string GITHUB_API_URL = "https://api.github.com/repos/" + GITHUB_REPO + "/releases/latest";
        private static readonly string CURRENT_VERSION = "1.4.1b";

        public static async Task CheckForUpdatesAsync()
        {
            try
            {
                using (var client = new WebClient())
                {
                client.Headers.Add("User-Agent", "ReCal");
                    
                    string json = await client.DownloadStringTaskAsync(GITHUB_API_URL);
                    var latestRelease = JsonConvert.DeserializeObject<GitHubRelease>(json);
                    
                    if (IsNewerVersion(latestRelease.TagName))
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            ShowUpdateDialog(latestRelease);
                        });
                    }
                    else
                    {
                        // Optional: Show notification that app is up to date (can be disabled)
                        // await Application.Current.Dispatcher.InvokeAsync(() =>
                        // {
                        //     Debug.WriteLine("Application is up to date. Current version: " + CURRENT_VERSION + ", Latest: " + latestRelease.TagName);
                        // });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't bother user on startup
                Debug.WriteLine("Update check failed: " + ex.Message);
                // Silently continue - app should work even if update check fails
            }
        }

        private static bool IsNewerVersion(string latestVersion)
        {
            try
            {
                // Remove common tag prefixes and compare versions
                var cleanLatest = TrimTagPrefix(latestVersion);
                var current = new Version(TrimTagPrefix(CURRENT_VERSION));
                var latest = new Version(cleanLatest);
                
                return latest > current;
            }
            catch
            {
                return false;
            }
        }

        private static string TrimTagPrefix(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var trimmed = value.Trim();
            if (trimmed.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(1);
            }

            if (trimmed.StartsWith("ReCal-", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("Recal-", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = trimmed.Substring(6);
            }

            return trimmed;
        }

        private static void ShowUpdateDialog(GitHubRelease release)
        {
            var result = MessageBox.Show(
                "A new version (" + release.TagName + ") is available!\n\n" +
                "Source: " + GITHUB_REPO + "\n" +
                "Release Notes:\n" + release.Body + "\n\n" +
                "Would you like to download and install the update?",
                "Update Available",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                DownloadAndInstallUpdate(release);
            }
        }

        private static async void DownloadAndInstallUpdate(GitHubRelease release)
        {
            try
            {
                // Find the executable asset (usually .exe)
                var asset = release.Assets?.Find(a => a.Name.EndsWith(".exe") || a.Name.EndsWith(".zip"));
                if (asset == null)
                {
                    MessageBox.Show("No downloadable update found.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Show progress dialog
                var progressDialog = new UpdateProgressDialog();
                progressDialog.Show();

                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            progressDialog.UpdateProgress(e.ProgressPercentage);
                        });
                    };

                    client.DownloadFileCompleted += (s, e) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            progressDialog.Close();
                            
                            if (e.Error == null)
                            {
                                string downloadedFile = Path.Combine(Path.GetTempPath(), asset.Name);
                                File.WriteAllBytes(downloadedFile, File.ReadAllBytes(downloadedFile));
                                
                                // Start the installer/updater
                                Process.Start(downloadedFile);
                                Application.Current.Shutdown();
                            }
                            else
                            {
                                MessageBox.Show("Download failed: " + e.Error.Message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                    };

                    await client.DownloadFileTaskAsync(new Uri(asset.BrowserDownloadUrl), Path.Combine(Path.GetTempPath(), asset.Name));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class GitHubRelease
    {
        public string TagName { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public DateTime PublishedAt { get; set; }
        public List<GitHubAsset> Assets { get; set; }
    }

    public class GitHubAsset
    {
        public string Name { get; set; }
        public string BrowserDownloadUrl { get; set; }
        public long Size { get; set; }
    }
}
