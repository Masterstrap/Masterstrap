using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

namespace Masterstrap.Services
{
    internal static class CommunityModService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BackupSuffix = ".masterstrap.bak";

        private static readonly string SkyboxPackFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Masterstrap", "SkyboxPack");

        private static readonly string SkyboxCommitFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Masterstrap", "SkyboxPack", "skybox.commit");

        public static List<string> GetAvailableSkyboxPresets()
        {
            var result = new List<string> { "Default" };
            try
            {
                if (Directory.Exists(SkyboxPackFolder))
                {
                    foreach (var dir in Directory.GetDirectories(SkyboxPackFolder))
                    {
                        string name = Path.GetFileName(dir);
                        if (!string.IsNullOrWhiteSpace(name) && name != "Default")
                            result.Add(name);
                    }
                }
            }
            catch { }
            return result;
        }

        public static async Task EnsureSkyboxPackDownloadedAsync(bool downloadSupplemental = true, Action<string>? log = null)
        {
            try
            {
                if (!Directory.Exists(SkyboxPackFolder))
                {
                    Directory.CreateDirectory(SkyboxPackFolder);
                    log?.Invoke("[Mods] Initializing Skybox Pack directory...");
                }

                int localPresetCount = GetAvailableSkyboxPresets().Count - 1;
                if (localPresetCount < 10)
                {
                    log?.Invoke("[Mods] Skybox pack looks incomplete. Downloading full SkyboxPackV2...");
                    await DownloadPrimarySkyboxPackAsync(log);
                    localPresetCount = GetAvailableSkyboxPresets().Count - 1;
                    log?.Invoke($"[Mods] Full SkyboxPack sync complete ({localPresetCount} presets).");
                }

                var essentialPresets = new Dictionary<string, string>
                {
                    { "Aurora", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Aurora.zip" },
                    { "Beautiful", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Beautiful.zip" },
                    { "Saturn", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Saturn.zip" },
                    { "Blue", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Blue.zip" },
                    { "Vantablack", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Vantablack.zip" }
                };

                foreach (var preset in essentialPresets)
                {
                    string targetFolder = Path.Combine(SkyboxPackFolder, preset.Key);
                    if (!Directory.Exists(targetFolder))
                    {
                        log?.Invoke($"[Mods] Downloading preset: {preset.Key}...");
                        await DownloadSpecificSupplementalZipAsync(preset.Key, preset.Value, log);
                    }
                }

                log?.Invoke($"[Mods] Skybox Pack ready ({GetAvailableSkyboxPresets().Count - 1} presets available).");
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Failed to initialize Skybox Pack: {ex.Message}");
            }
        }

        private static async Task DownloadPrimarySkyboxPackAsync(Action<string>? log = null)
        {
            string tempZip = Path.Combine(Path.GetTempPath(), "Masterstrap_SkyboxPackV2.zip");
            string extractedRoot = Path.Combine(SkyboxPackFolder, "__temp_extract");

            try
            {
                byte[] data = await _httpClient.GetByteArrayAsync("https://github.com/KloBraticc/SkyboxPackV2/archive/refs/heads/main.zip");
                await File.WriteAllBytesAsync(tempZip, data);

                if (Directory.Exists(extractedRoot))
                    Directory.Delete(extractedRoot, true);
                Directory.CreateDirectory(extractedRoot);

                ZipFile.ExtractToDirectory(tempZip, extractedRoot, true);

                string extractedPackRoot = Directory.GetDirectories(extractedRoot).FirstOrDefault() ?? "";
                if (string.IsNullOrWhiteSpace(extractedPackRoot) || !Directory.Exists(extractedPackRoot))
                {
                    log?.Invoke("[Mods] Full SkyboxPack extract failed: no extracted root folder.");
                    return;
                }

                foreach (string dir in Directory.GetDirectories(extractedPackRoot))
                {
                    string presetName = Path.GetFileName(dir);
                    if (string.IsNullOrWhiteSpace(presetName))
                        continue;

                    string target = Path.Combine(SkyboxPackFolder, presetName);
                    if (Directory.Exists(target))
                        continue;

                    DirectoryCopy(dir, target);
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Full SkyboxPack sync failed: {ex.Message}");
            }
            finally
            {
                try { if (Directory.Exists(extractedRoot)) Directory.Delete(extractedRoot, true); } catch { }
                try { if (File.Exists(tempZip)) File.Delete(tempZip); } catch { }
            }
        }

        private static void DirectoryCopy(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                DirectoryCopy(subDir, destSubDir);
            }
        }

        public static async Task EnsureSpecificSkyboxDownloadedAsync(string skyboxName, Action<string>? log = null)
        {
            if (string.IsNullOrEmpty(skyboxName) || skyboxName == "Default") return;

            if (Directory.Exists(Path.Combine(SkyboxPackFolder, skyboxName))) return;

            var supplemental = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Aurora", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Aurora.zip" },
                { "Beautiful", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Beautiful.zip" },
                { "Saturn", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Saturn.zip" }
            };

            if (supplemental.TryGetValue(skyboxName, out string? url))
            {
                log?.Invoke($"[Mods] Supplemental skybox '{skyboxName}' missing. Downloading...");
                await DownloadSpecificSupplementalZipAsync(skyboxName, url, log);
                return;
            }

            string encodedSkyboxName = Uri.EscapeDataString(skyboxName);
            string guessedUrl = $"https://raw.githubusercontent.com/RealMeddsam/config/main/Download/{encodedSkyboxName}.zip";
            log?.Invoke($"[Mods] Skybox '{skyboxName}' missing. Trying dynamic download URL...");
            await DownloadSpecificSupplementalZipAsync(skyboxName, guessedUrl, log);
        }

        private static async Task DownloadSupplementalSkyboxesAsync(Action<string>? log = null)
        {
            var supplemental = new Dictionary<string, string>
            {
                { "Aurora", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Aurora.zip" },
                { "Beautiful", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Beautiful.zip" },
                { "Saturn", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Saturn.zip" }
            };

            foreach (var item in supplemental)
            {
                if (!Directory.Exists(Path.Combine(SkyboxPackFolder, item.Key)))
                {
                    await DownloadSpecificSupplementalZipAsync(item.Key, item.Value, log);
                }
            }
        }

        private static async Task DownloadSpecificSupplementalZipAsync(string name, string url, Action<string>? log = null)
        {
            try
            {
                string targetFolder = Path.Combine(SkyboxPackFolder, name);
                string tempZip = Path.Combine(Path.GetTempPath(), $"MS_Skybox_{name}.zip");

                var data = await _httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(tempZip, data);

                using (var zip = ZipFile.OpenRead(tempZip))
                {
                    foreach (var entry in zip.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue;

                        string relativePath = entry.FullName;
                        string[] parts = entry.FullName.Split('/', '\\');
                        if (parts.Length > 1 && (parts[0].Equals(name, StringComparison.OrdinalIgnoreCase) || parts[0].Equals("sky", StringComparison.OrdinalIgnoreCase)))
                        {
                            relativePath = Path.Combine(parts.Skip(1).ToArray());
                        }

                        if (string.IsNullOrEmpty(relativePath)) continue;

                        string destPath = Path.Combine(targetFolder, relativePath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                        entry.ExtractToFile(destPath, true);
                    }
                }

                try { File.Delete(tempZip); } catch { }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Failed to sync supplemental {name}: {ex.Message}");
            }
        }

        public static async Task ApplySkyboxPresetAsync(string skybox, string robloxVersionRoot, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(skybox) || skybox == "Default")
            {
                if (skybox == "Default" && !string.IsNullOrWhiteSpace(robloxVersionRoot))
                {
                    await RestoreDefaultSkyboxAsync(robloxVersionRoot, log);
                }
                return;
            }

            try
            {
                await EnsureSpecificSkyboxDownloadedAsync(skybox, log);

                string skyboxSourceFolder = Path.Combine(SkyboxPackFolder, skybox);

                if (!Directory.Exists(skyboxSourceFolder))
                {
                    var allDirs = Directory.GetDirectories(SkyboxPackFolder);
                    skyboxSourceFolder = allDirs.FirstOrDefault(d =>
                        Path.GetFileName(d).Equals(skybox, StringComparison.OrdinalIgnoreCase)) ?? "";

                    if (string.IsNullOrEmpty(skyboxSourceFolder))
                    {
                        log?.Invoke($"[Mods] Error: Skybox preset '{skybox}' not found in pack.");
                        log?.Invoke($"[Mods] Available presets: {string.Join(", ", allDirs.Select(Path.GetFileName))}");
                        return;
                    }
                }

                string skyTexturesDir = Path.Combine(robloxVersionRoot, "PlatformContent", "pc", "textures", "sky");

                if (!Directory.Exists(skyTexturesDir))
                {
                    log?.Invoke($"[Mods] Error: Sky textures directory not found: {skyTexturesDir}");
                    return;
                }

                log?.Invoke($"[Mods] Applying skybox '{skybox}'...");
                log?.Invoke($"[Mods] Source: {skyboxSourceFolder}");
                log?.Invoke($"[Mods] Target: {skyTexturesDir}");

                foreach (string existingFile in Directory.GetFiles(skyTexturesDir, "*.*", SearchOption.AllDirectories))
                {
                    try { File.SetAttributes(existingFile, FileAttributes.Normal); } catch { }
                }

                string backupDir = skyTexturesDir + BackupSuffix;
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                    foreach (string file in Directory.GetFiles(skyTexturesDir))
                    {
                        string backupFile = Path.Combine(backupDir, Path.GetFileName(file));
                        File.Copy(file, backupFile, true);
                    }
                    log?.Invoke("[Mods] Original sky textures backed up.");
                }

                Directory.Delete(skyTexturesDir, true);
                Directory.CreateDirectory(skyTexturesDir);

                var sourceFiles = Directory.GetFiles(skyboxSourceFolder, "*.*", SearchOption.AllDirectories);
                int copied = 0;
                foreach (string sourceFile in sourceFiles)
                {
                    string relativePath = Path.GetRelativePath(skyboxSourceFolder, sourceFile);
                    string destFile = Path.Combine(skyTexturesDir, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                    File.Copy(sourceFile, destFile, true);
                    copied++;
                }

                log?.Invoke($"[Mods] Skybox '{skybox}' applied successfully! ({copied} files copied)");

                await ApplySkyboxPatchAsync(log);
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Skybox error: {ex.Message}");
            }
        }

        private static async Task ApplySkyboxPatchAsync(Action<string>? log = null)
        {
            var patchHashes = new Dictionary<string, string>
            {
                { "a564ec8aeef3614e788d02f0090089d8", "a5" },
                { "7328622d2d509b95dd4dd2c721d1ca8b", "73" },
                { "a50f6563c50ca4d5dcb255ee5cfab097", "a5" },
                { "6c94b9385e52d221f0538aadaceead2d", "6c" },
                { "9244e00ff9fd6cee0bb40a262bb35d31", "92" },
                { "78cb2e93aee0cdbd79b15a866bc93a54", "78" }
            };

            string rbxStorage = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "rbx-storage");

            foreach (var entry in patchHashes)
            {
                string dir = Path.Combine(rbxStorage, entry.Value);
                Directory.CreateDirectory(dir);
                string dest = Path.Combine(dir, entry.Key);

                try
                {
                    byte[] data = await _httpClient.GetByteArrayAsync(
                        $"https://raw.githubusercontent.com/KloBraticc/SkyboxPatch/main/assets/{entry.Key}");

                    if (File.Exists(dest))
                        File.SetAttributes(dest, FileAttributes.Normal);

                    await File.WriteAllBytesAsync(dest, data);
                    File.SetAttributes(dest, FileAttributes.ReadOnly);
                }
                catch { /* Skip individual patch failures */ }
            }
            log?.Invoke("[Mods] SkyboxPatch applied to rbx-storage.");
        }

        private static async Task RestoreDefaultSkyboxAsync(string robloxVersionRoot, Action<string>? log = null)
        {
            try
            {
                string skyTexturesDir = Path.Combine(robloxVersionRoot, "PlatformContent", "pc", "textures", "sky");
                string backupDir = skyTexturesDir + BackupSuffix;

                if (!Directory.Exists(backupDir))
                {
                    log?.Invoke("[Mods] No skybox backup found - already using default.");
                    return;
                }

                if (Directory.Exists(skyTexturesDir))
                {
                    foreach (string file in Directory.GetFiles(skyTexturesDir, "*.*", SearchOption.AllDirectories))
                    {
                        try { File.SetAttributes(file, FileAttributes.Normal); } catch { }
                    }
                    Directory.Delete(skyTexturesDir, true);
                }
                Directory.CreateDirectory(skyTexturesDir);

                foreach (string backupFile in Directory.GetFiles(backupDir))
                {
                    string destFile = Path.Combine(skyTexturesDir, Path.GetFileName(backupFile));
                    File.Copy(backupFile, destFile, true);
                }

                log?.Invoke("[Mods] Default skybox restored from backup.");
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Restore default skybox error: {ex.Message}");
            }
        }

        public static async Task ApplyFullbrightAsync(bool enabled, string robloxVersionRoot, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(robloxVersionRoot))
                return;

            try
            {
                string texturesDir = Path.Combine(robloxVersionRoot, "PlatformContent", "pc", "textures");
                if (Directory.Exists(texturesDir))
                {
                    var files = Directory.GetFiles(texturesDir, "brdfLUT.*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (enabled)
                        {
                            BackupIfNeeded(file);
                            File.Delete(file);
                            log?.Invoke($"[Mods] Removed {Path.GetFileName(file)} for Fullbright.");
                        }
                        else
                        {
                            RestoreFromBackup(file);
                            log?.Invoke($"[Mods] Restored {Path.GetFileName(file)}.");
                        }
                    }
                }

                string clientSettingsDir = Path.Combine(robloxVersionRoot, "ClientSettings");
                string jsonPath = Path.Combine(clientSettingsDir, "ClientAppSettings.json");

                if (enabled)
                {
                    if (!Directory.Exists(clientSettingsDir)) Directory.CreateDirectory(clientSettingsDir);

                    Dictionary<string, object> flags = new Dictionary<string, object>();
                    if (File.Exists(jsonPath))
                    {
                        try {
                            flags = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(jsonPath)) ?? new Dictionary<string, object>();
                        } catch { }
                    }

                    flags["FFlagDebugRenderForceFullbright"] = true;
                    File.WriteAllText(jsonPath, JsonSerializer.Serialize(flags, new JsonSerializerOptions { WriteIndented = true }));
                    log?.Invoke("[Mods] FFlag Fullbright applied.");
                }
                else if (File.Exists(jsonPath))
                {
                    try {
                        var flags = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(jsonPath));
                        if (flags != null && flags.Remove("FFlagDebugRenderForceFullbright"))
                        {
                            File.WriteAllText(jsonPath, JsonSerializer.Serialize(flags, new JsonSerializerOptions { WriteIndented = true }));
                            log?.Invoke("[Mods] FFlag Fullbright removed.");
                        }
                    } catch { }
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Fullbright error: {ex.Message}");
            }
        }

        private static void BackupIfNeeded(string filePath)
        {
            try
            {
                if (File.Exists(filePath) && !File.Exists(filePath + BackupSuffix))
                {
                    File.Copy(filePath, filePath + BackupSuffix, true);
                }
            }
            catch { }
        }

        private static void RestoreFromBackup(string filePath)
        {
            try
            {
                string backup = filePath + BackupSuffix;
                if (File.Exists(backup))
                {
                    File.Copy(backup, filePath, true);
                }
            }
            catch { }
        }

        private static async Task DownloadToFileAsync(string url, string destinationPath)
        {
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
