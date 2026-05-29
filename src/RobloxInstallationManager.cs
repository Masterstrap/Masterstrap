using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Masterstrap.Helpers;

namespace Masterstrap.Services
{
    public static class RobloxInstallationManager
    {
        public static async Task<(bool success, string robloxExePath)> EnsureRobloxInstalledAsync(
            Action<string> logCallback,
            Action<string> updateStatusCallback,
            bool silent = false)
        {
            Action<string> safeLog = (msg) =>
            {
                try { logCallback?.Invoke(msg); } catch { /* swallow */ }
            };

            Action<string> safeUpdateStatus = (status) =>
            {
                try { updateStatusCallback?.Invoke(status); } catch { /* swallow */ }
            };

            try
            {
                safeLog("[Install] === ENSURE ROBLOX INSTALLED ===");
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string versionsDir = Path.Combine(appDirectory, "versions");

                safeUpdateStatus("Checking Roblox...");

                RobloxVersionData versionData = null;

                safeLog("[Install] Fetching latest Roblox version from API...");
                versionData = await FetchRobloxVersionDataAsync(safeLog, shortBudget: false).ConfigureAwait(false);

                if (versionData == null || string.IsNullOrWhiteSpace(versionData.ClientVersionUpload))
                {
                    string localHash = TryResolveLocalRobloxBuildHash(versionsDir, safeLog);
                    if (!string.IsNullOrEmpty(localHash))
                    {
                        safeLog($"[Install] API unreachable — using existing install in versions/: {localHash}");
                        safeUpdateStatus("Using installed Roblox build...");
                        versionData = new RobloxVersionData
                        {
                            ClientVersionUpload = localHash,
                            Version = ""
                        };
                    }
                    else
                    {
                        safeLog("[Install] ✗ Failed to fetch version data from API and no local version-* folder with RobloxPlayerBeta.exe");
                        safeUpdateStatus("Failed to fetch Roblox version");
                        return (false, null);
                    }
                }

                string apiLatestVersionHash = versionData.ClientVersionUpload;
                safeLog($"[Install] ✓ Target Roblox build: {apiLatestVersionHash}");

                string desiredVersionHash = apiLatestVersionHash;

                string versionPath = Path.Combine(versionsDir, desiredVersionHash);
                string robloxExePath = Path.Combine(versionPath, "RobloxPlayerBeta.exe");

                if (File.Exists(robloxExePath))
                {
                    string downloadsDir = Path.Combine(versionPath, "downloads");
                    bool hasZipInDownloads =
                        Directory.Exists(downloadsDir) &&
                        Directory.EnumerateFiles(downloadsDir, "*.zip", SearchOption.TopDirectoryOnly).Any();

                    bool hasZipInVersionRoot =
                        Directory.EnumerateFiles(versionPath, "*.zip", SearchOption.TopDirectoryOnly).Any();

                    if (!hasZipInDownloads && !hasZipInVersionRoot)
                    {
                        safeLog($"[Install] ✓ CASE 2: Roblox {desiredVersionHash} already installed");
                        safeLog($"[Install] Skipping install, using existing version (version={desiredVersionHash})");
                        CleanupOldVersionFoldersExcept(versionsDir, desiredVersionHash, safeLog);
                        return (true, robloxExePath);
                    }

                    safeLog($"[Install] ⚠️ CASE 2: Roblox exe exists but leftover zip(s) found. Finishing install...");
                    safeLog($"[Install]      hasZipInDownloads={hasZipInDownloads}, hasZipInVersionRoot={hasZipInVersionRoot}");

                    try
                    {
                        Directory.CreateDirectory(downloadsDir);
                        foreach (var zipPath in Directory.EnumerateFiles(versionPath, "*.zip", SearchOption.TopDirectoryOnly))
                        {
                            string fileName = Path.GetFileName(zipPath);
                            string destPath = Path.Combine(downloadsDir, fileName);
                            if (string.IsNullOrWhiteSpace(fileName))
                                continue;

                            if (File.Exists(destPath))
                            {
                                File.Delete(zipPath);
                            }
                            else
                            {
                                File.Move(zipPath, destPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        safeLog($"[Install] ⚠️ Could not normalize leftover zip locations: {ex.Message}");
                    }
                }

                safeLog($"[Install] ⚠️ CASE 1: Roblox {desiredVersionHash} not found - auto-installing...");

                Directory.CreateDirectory(versionPath);
                safeLog($"[Install] Install directory: {versionPath}");
safeLog("[Install] Starting manifest-based download...");

                var downloader = new RobloxManifestDownloader(
                    desiredVersionHash,
                    versionPath,
                    log: msg => safeLog($"[Install] {msg}"),
                    addActivity: (msg) => { },
                    updateInstallStatus: (status, progress) => { }
));
                    }
                );

                downloader.DownloadProgressChanged += (s, args) =>
                {
                    System.Windows.Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                    {
                        progressWindow?.UpdateDownloadProgress(
                            args.PercentComplete,
                            args.SpeedMBps,
                            args.EstimatedTimeRemaining,
                            args.CurrentPackage
                        );
                    }));
                };
await downloader.DownloadAndAssembleRobloxAsync();

                safeLog("[Install] ✓ Roblox installation complete!");
if (!File.Exists(robloxExePath))
                {
                    safeLog("[Install] ✗ RobloxPlayerBeta.exe not found after installation");
                    safeUpdateStatus("Installation verification failed");
                    return (false, null);
                }

                safeLog($"[Install] ✓ Installation verified: {robloxExePath}");

                CleanupOldVersionFoldersExcept(versionsDir, desiredVersionHash, safeLog);

                safeUpdateStatus("Roblox installed");
                return (true, robloxExePath);
            }
            catch (Exception ex)
            {
                safeLog($"[Install] ✗ Error: {ex.Message}");
                safeLog($"[Install] ✗ Stack: {ex.StackTrace}");
safeUpdateStatus("Installation failed");
                return (false, null);
            }
        }

        private static string TryResolveLocalRobloxBuildHash(string versionsDir, Action<string> safeLog)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(versionsDir) || !Directory.Exists(versionsDir))
                    return null;

                string[] dirs = Directory.GetDirectories(versionsDir, "version-*", SearchOption.TopDirectoryOnly);
                if (dirs.Length == 0)
                    return null;

                string bestName = null;
                DateTime bestUtc = DateTime.MinValue;

                foreach (string dir in dirs)
                {
                    string exe = Path.Combine(dir, "RobloxPlayerBeta.exe");
                    if (!File.Exists(exe))
                        continue;

                    DateTime t = File.GetLastWriteTimeUtc(exe);
                    string name = Path.GetFileName(dir);
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    if (t >= bestUtc)
                    {
                        bestUtc = t;
                        bestName = name;
                    }
                }

                if (bestName != null)
                    safeLog?.Invoke($"[Install] Local build candidate: {bestName} (exe UTC {bestUtc:yyyy-MM-dd HH:mm})");

                return bestName;
            }
            catch (Exception ex)
            {
                safeLog?.Invoke($"[Install] Local version scan failed: {ex.Message}");
                return null;
            }
        }

        private static async Task<RobloxVersionData> FetchRobloxVersionDataAsync(Action<string> logDiag, bool shortBudget = false)
        {
            string[] versionUrls =
            {
                "https://clientsettings.roblox.com/v2/client-version/WindowsPlayer",
                "https://clientsettings.roblox.com/v2/client-version/WindowsPlayer/channel/LIVE"
            };

            Exception lastErr = null;
            int maxAttempts = shortBudget ? 2 : 5;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                string versionUrl = versionUrls[(attempt - 1) % versionUrls.Length];
                try
                {
                    int timeoutSec = shortBudget ? 18 : (attempt >= maxAttempts ? 55 : 40);
                    using var client = new HttpClient(
                        OutboundHttp.CreateHandler(DecompressionMethods.All, tls12Only: false),
                        disposeHandler: true)
                    {
                        Timeout = TimeSpan.FromSeconds(timeoutSec)
                    };
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "User-Agent",
                        "Roblox/WinInet RobloxPlayer/0 (Masterstrap)");

                    string jsonResponse = await client.GetStringAsync(versionUrl).ConfigureAwait(false);

                    using var doc = System.Text.Json.JsonDocument.Parse(jsonResponse);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("clientVersionUpload", out System.Text.Json.JsonElement clientVersionUpload))
                    {
                        string hash = clientVersionUpload.GetString();

                        if (string.IsNullOrWhiteSpace(hash))
                            throw new InvalidOperationException("clientVersionUpload value is empty");

                        string version = "";
                        if (root.TryGetProperty("version", out System.Text.Json.JsonElement versionProp))
                            version = versionProp.GetString() ?? "";

                        logDiag?.Invoke($"[Install] API version fetch OK ({versionUrl})");
                        return new RobloxVersionData
                        {
                            ClientVersionUpload = hash,
                            Version = version
                        };
                    }

                    throw new InvalidOperationException("clientVersionUpload field not found in API response");
                }
                catch (Exception ex)
                {
                    lastErr = ex;
                    string line = $"[Install] Fetch version attempt {attempt}/{maxAttempts} ({versionUrl}) failed: {ex.GetType().Name}: {ex.Message}";
                    logDiag?.Invoke(line);
                    Console.WriteLine(line);
                    if (attempt < maxAttempts)
                    {
                        int delayMs = shortBudget ? 350 : (800 + attempt * 200);
                        await Task.Delay(delayMs).ConfigureAwait(false);
                    }
                }
            }

            if (lastErr != null)
                logDiag?.Invoke($"[Install] Giving up Roblox API version fetch: {lastErr.GetType().Name}: {lastErr.Message}");
            return null;
        }

        private class RobloxVersionData
        {
            public string ClientVersionUpload { get; set; }
            public string Version { get; set; }
        }

        public static void CleanupOldVersionFoldersExcept(string versionsDir, string keepVersionName, Action<string> logCallback)
        {
            Action<string> safeLog = (msg) =>
            {
                try { logCallback?.Invoke(msg); } catch { /* swallow */ }
            };
            try
            {
                if (!Directory.Exists(versionsDir) || string.IsNullOrWhiteSpace(keepVersionName))
                    return;

                foreach (var dir in Directory.GetDirectories(versionsDir, "version-*", SearchOption.TopDirectoryOnly))
                {
                    var folderName = Path.GetFileName(dir);
                    if (string.Equals(folderName, keepVersionName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    try
                    {
                        Directory.Delete(dir, true);
                        safeLog($"[Install] Cleanup old Roblox version folder: {folderName}");
                    }
                    catch (Exception exDel)
                    {
                        safeLog($"[Install] Could not delete old version folder {folderName}: {exDel.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                safeLog($"[Install] Old version cleanup error: {ex.Message}");
            }
        }
catch { } });
            await d.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            await Task.Delay(300);
        }
    }
}
