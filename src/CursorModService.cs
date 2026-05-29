using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Masterstrap.Services
{
    internal static class CursorModService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string BackupSuffix = ".masterstrap.bak";

        private static readonly string CursorPackFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Masterstrap", "CursorPack");

        public static List<string> GetAvailableCursorPresets()
        {
            var result = new List<string> { "Default", "Custom", "From 2006", "From 2013" };
            try
            {
                string cursorsPath = Path.Combine(CursorPackFolder, "Cursors");
                if (Directory.Exists(cursorsPath))
                {
                    foreach (var dir in Directory.GetDirectories(cursorsPath))
                    {
                        string name = Path.GetFileName(dir);
                        if (!result.Contains(name, StringComparer.OrdinalIgnoreCase))
                            result.Add(name);
                    }
                }
            }
            catch { }

            if (result.Count <= 4)
            {
                if (!result.Contains("Standard")) result.Add("Standard");
                if (!result.Contains("Modern")) result.Add("Modern");
                if (!result.Contains("Legacy")) result.Add("Legacy");
            }

            return result;
        }

        public static List<string> GetAvailableShiftlockPresets()
        {
            EnsureBundledCrosshairPresets();

            var result = new List<string> { "Default", "Masterstrap" };
            if (HasCustomShiftlockPreset())
                result.Add("Custom");

            try
            {
                foreach (string preset in GetShiftlockFilePresetNames())
                {
                    if (!result.Contains(preset, StringComparer.OrdinalIgnoreCase))
                        result.Add(preset);
                }
            }
            catch
            {
            }

            return result;
        }

        public static bool HasCustomShiftlockPreset()
        {
            string customFile = Path.Combine(CursorPackFolder, "CustomShiftlock.png");
            return File.Exists(customFile);
        }

        public static void EnsureBundledCrosshairPresets(Action<string>? log = null)
        {
            try
            {
                string destFolder = Path.Combine(CursorPackFolder, "crosshair");
                bool copiedAny = false;

                foreach (string sourceFolder in GetBundledCrosshairSourceFolders())
                {
                    if (!Directory.Exists(sourceFolder))
                        continue;

                    if (!Directory.Exists(CursorPackFolder))
                        Directory.CreateDirectory(CursorPackFolder);
                    if (!Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);

                    foreach (string sourceFile in Directory.EnumerateFiles(sourceFolder, "*.png", SearchOption.TopDirectoryOnly))
                    {
                        string destFile = Path.Combine(destFolder, Path.GetFileName(sourceFile));
                        if (!File.Exists(destFile) ||
                            File.GetLastWriteTimeUtc(sourceFile) > File.GetLastWriteTimeUtc(destFile))
                        {
                            File.Copy(sourceFile, destFile, overwrite: true);
                            copiedAny = true;
                        }
                    }
                }

                if (copiedAny)
                    log?.Invoke("[Mods] Bundled crosshair presets synced to CursorPack.");
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Failed to sync bundled crosshair presets: {ex.Message}");
            }
        }

        private static IEnumerable<string> GetBundledCrosshairSourceFolders()
        {
            var folders = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, "crosshair"),
                Path.Combine(AppContext.BaseDirectory, "Crosshair"),
            };

            try
            {
                string? dir = AppContext.BaseDirectory;
                for (int i = 0; i < 6 && !string.IsNullOrEmpty(dir); i++)
                {
                    string candidate = Path.Combine(dir, "crosshair");
                    if (Directory.Exists(candidate) &&
                        !folders.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                    {
                        folders.Add(candidate);
                    }

                    dir = Directory.GetParent(dir)?.FullName;
                }
            }
            catch
            {
            }

            return folders.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        public static string GetMasterstrapShiftlockCachePath()
        {
            return Path.Combine(CursorPackFolder, "MasterstrapShiftlock.png");
        }

        public static string? ResolveMasterstrapIconPath()
        {
            string[] candidates =
            {
                Path.Combine(AppContext.BaseDirectory, "Masterstrap.ico"),
                Path.Combine(AppContext.BaseDirectory, "masterstrap.ico"),
            };

            foreach (string candidate in candidates)
            {
                if (File.Exists(candidate))
                    return candidate;
            }

            try
            {
                var uri = new Uri("pack://application:,,,/Masterstrap.ico", UriKind.Absolute);
                var streamInfo = Application.GetResourceStream(uri);
                if (streamInfo?.Stream == null)
                    return null;

                if (!Directory.Exists(CursorPackFolder))
                    Directory.CreateDirectory(CursorPackFolder);

                string extracted = Path.Combine(CursorPackFolder, "Masterstrap.ico");
                using (streamInfo.Stream)
                using (var output = File.Create(extracted))
                    streamInfo.Stream.CopyTo(output);

                return extracted;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<string?> EnsureMasterstrapShiftlockPngAsync(Action<string>? log = null)
        {
            string? iconPath = ResolveMasterstrapIconPath();
            if (string.IsNullOrWhiteSpace(iconPath) || !File.Exists(iconPath))
            {
                log?.Invoke("[Mods] Masterstrap.ico not found.");
                return null;
            }

            if (!Directory.Exists(CursorPackFolder))
                Directory.CreateDirectory(CursorPackFolder);

            string cachePath = GetMasterstrapShiftlockCachePath();
            if (!File.Exists(cachePath) ||
                File.GetLastWriteTimeUtc(iconPath) > File.GetLastWriteTimeUtc(cachePath))
            {
                await ShiftlockImageNormalizer.NormalizeAsync(
                    iconPath,
                    cachePath,
                    ShiftlockImageNormalizer.DefaultTargetSize,
                    log);
            }

            return cachePath;
        }

        public static async Task EnsureCursorPackDownloadedAsync(Action<string>? log = null)
        {
            try
            {
                if (!Directory.Exists(CursorPackFolder))
                    Directory.CreateDirectory(CursorPackFolder);

                string packTag = Path.Combine(CursorPackFolder, ".pack_done");
                if (!File.Exists(packTag))
                {
                    log?.Invoke("[Mods] Downloading Cursor & Shiftlock Pack...");

                    var parts = new Dictionary<string, string>
                    {
                        { "Cursors", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Cursors.zip" },
                        { "Shiftlock", "https://raw.githubusercontent.com/RealMeddsam/config/main/Download/Shiftlock.zip" }
                    };

                    foreach (var part in parts)
                    {
                        try
                        {
                            string tempZip = Path.Combine(Path.GetTempPath(), $"MS_{part.Key}.zip");
                            var data = await _httpClient.GetByteArrayAsync(part.Value);
                            await File.WriteAllBytesAsync(tempZip, data);

                            using (var zip = ZipFile.OpenRead(tempZip))
                            {
                                foreach (var entry in zip.Entries)
                                {
                                    if (string.IsNullOrEmpty(entry.Name)) continue;

                                    string destPath = Path.Combine(CursorPackFolder, part.Key, entry.FullName);
                                    Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                                    entry.ExtractToFile(destPath, true);
                                }
                            }
                            if (File.Exists(tempZip)) File.Delete(tempZip);
                        }
                        catch (Exception ex)
                        {
                            log?.Invoke($"[Mods] Failed to download {part.Key}: {ex.Message}");
                        }
                    }

                    File.WriteAllText(packTag, DateTime.Now.ToString());
                    log?.Invoke("[Mods] Cursor Pack initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Cursor Pack error: {ex.Message}");
            }
        }

        public static async Task ApplyCursorPresetAsync(string presetName, string robloxRoot, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(robloxRoot)) return;

            string cursorDir = Path.Combine(robloxRoot, "content", "textures", "Cursors", "KeyboardMouse");
            string arrow = Path.Combine(cursorDir, "ArrowCursor.png");
            string far = Path.Combine(cursorDir, "ArrowFarCursor.png");
            string ibeam = Path.Combine(cursorDir, "IBeamCursor.png");

            if (presetName == "Default")
            {
                RestoreDefault(arrow);
                RestoreDefault(far);
                RestoreDefault(ibeam);
                log?.Invoke("[Mods] Cursors restored to default.");
                return;
            }

            if (presetName == "Custom")
            {
                string customPath = Path.Combine(CursorPackFolder, "Cursors", "Custom");
                if (Directory.Exists(customPath))
                {
                    await ApplyLocalFileAsync(Path.Combine(customPath, "ArrowCursor.png"), arrow);
                    await ApplyLocalFileAsync(Path.Combine(customPath, "ArrowFarCursor.png"), far);
                    await ApplyLocalFileAsync(Path.Combine(customPath, "IBeamCursor.png"), ibeam);
                    log?.Invoke("[Mods] Applied custom cursor style.");
                }
                return;
            }

            string presetPath = Path.Combine(CursorPackFolder, "Cursors", presetName);
            if (Directory.Exists(presetPath))
            {
                var files = Directory.GetFiles(presetPath, "*.png", SearchOption.AllDirectories);

                string? sArrow = files.FirstOrDefault(f => f.Contains("ArrowCursor", StringComparison.OrdinalIgnoreCase));
                string? sFar = files.FirstOrDefault(f => f.Contains("ArrowFarCursor", StringComparison.OrdinalIgnoreCase));
                string? sIBeam = files.FirstOrDefault(f => f.Contains("IBeamCursor", StringComparison.OrdinalIgnoreCase));

                if (sArrow != null) await ApplyLocalFileAsync(sArrow, arrow);
                if (sFar != null) await ApplyLocalFileAsync(sFar, far);
                if (sIBeam != null) await ApplyLocalFileAsync(sIBeam, ibeam);

                log?.Invoke($"[Mods] Applied cursor preset: {presetName}");
                return;
            }

            if (presetName == "From 2006")
            {
                await ApplyRemoteAsync("https://raw.githubusercontent.com/bloxstraplabs/bloxstrap/main/Bloxstrap/Resources/Mods/Cursor.From2006.ArrowCursor.png", arrow);
                await ApplyRemoteAsync("https://raw.githubusercontent.com/bloxstraplabs/bloxstrap/main/Bloxstrap/Resources/Mods/Cursor.From2006.ArrowFarCursor.png", far);
            }
            else if (presetName == "From 2013")
            {
                await ApplyRemoteAsync("https://raw.githubusercontent.com/bloxstraplabs/bloxstrap/main/Bloxstrap/Resources/Mods/Cursor.From2013.ArrowCursor.png", arrow);
                await ApplyRemoteAsync("https://raw.githubusercontent.com/bloxstraplabs/bloxstrap/main/Bloxstrap/Resources/Mods/Cursor.From2013.ArrowFarCursor.png", far);
            }
        }

        public static async Task ApplyShiftlockPresetAsync(string presetName, string robloxRoot, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(robloxRoot))
            {
                log?.Invoke("[Mods] Shiftlock apply skipped: Roblox install path not found.");
                return;
            }

            string keyboardMouseTarget = Path.Combine(robloxRoot, "content", "textures", "Cursors", "KeyboardMouse", "MouseLockedCursor.png");
            string legacyTarget = Path.Combine(robloxRoot, "content", "textures", "MouseLockedCursor.png");
            string platformPcTarget = Path.Combine(robloxRoot, "PlatformContent", "pc", "textures", "MouseLockedCursor.png");
            string keyboardMouseDir = Path.GetDirectoryName(keyboardMouseTarget) ?? "";
            string legacyDir = Path.GetDirectoryName(legacyTarget) ?? "";
            string platformPcDir = Path.GetDirectoryName(platformPcTarget) ?? "";

            if (presetName == "Default")
            {
                RestoreDefault(keyboardMouseTarget);
                RestoreDefault(legacyTarget);
                RestoreDefault(platformPcTarget);
                RestoreDefaultShiftlockVariants(keyboardMouseDir, log);
                RestoreDefaultShiftlockVariants(legacyDir, log);
                RestoreDefaultShiftlockVariants(platformPcDir, log);
                log?.Invoke("[Mods] Shiftlock restored to default.");
                return;
            }

            if (presetName == "Custom")
            {
                string customFile = Path.Combine(CursorPackFolder, "CustomShiftlock.png");
                if (!File.Exists(customFile))
                {
                    log?.Invoke($"[Mods] Custom shiftlock image missing: {customFile}");
                    return;
                }

                string normalizedCustom = await PrepareShiftlockSourceAsync(customFile, log);
                await ApplyShiftlockToAllTargetsAsync(normalizedCustom, keyboardMouseTarget, legacyTarget, platformPcTarget, keyboardMouseDir, legacyDir, platformPcDir, log);
                log?.Invoke("[Mods] Applied custom shiftlock icon.");
                return;
            }

            if (string.Equals(presetName, "Masterstrap", StringComparison.OrdinalIgnoreCase))
            {
                string? masterstrapPng = await EnsureMasterstrapShiftlockPngAsync(log);
                if (string.IsNullOrWhiteSpace(masterstrapPng) || !File.Exists(masterstrapPng))
                {
                    log?.Invoke("[Mods] Masterstrap shiftlock icon unavailable.");
                    return;
                }

                await ApplyShiftlockToAllTargetsAsync(
                    masterstrapPng,
                    keyboardMouseTarget,
                    legacyTarget,
                    platformPcTarget,
                    keyboardMouseDir,
                    legacyDir,
                    platformPcDir,
                    log);
                log?.Invoke("[Mods] Applied Masterstrap shiftlock icon.");
                return;
            }

            string? filePreset = ResolveShiftlockFilePresetPath(presetName);
            if (!string.IsNullOrWhiteSpace(filePreset) && File.Exists(filePreset))
            {
                string normalizedPreset = await PrepareShiftlockSourceAsync(filePreset, log);
                await ApplyShiftlockToAllTargetsAsync(normalizedPreset, keyboardMouseTarget, legacyTarget, platformPcTarget, keyboardMouseDir, legacyDir, platformPcDir, log);
                log?.Invoke($"[Mods] Applied shiftlock file preset: {presetName}");
                return;
            }

            string presetDir = Path.Combine(CursorPackFolder, "Shiftlock", presetName);
            if (Directory.Exists(presetDir))
            {
                string presetFile = Path.Combine(presetDir, "MouseLockedCursor.png");

                if (!File.Exists(presetFile))
                {
                    presetFile = Directory.GetFiles(presetDir, "*.png", SearchOption.AllDirectories).FirstOrDefault() ?? "";
                }

                if (File.Exists(presetFile))
                {
                    string normalizedPreset = await PrepareShiftlockSourceAsync(presetFile, log);
                    await ApplyShiftlockToAllTargetsAsync(normalizedPreset, keyboardMouseTarget, legacyTarget, platformPcTarget, keyboardMouseDir, legacyDir, platformPcDir, log);
                    log?.Invoke($"[Mods] Applied shiftlock preset: {presetName}");
                }
                else
                {
                    log?.Invoke($"[Mods] Shiftlock preset '{presetName}' contains no PNG files — restoring default instead.");
                    RestoreDefault(keyboardMouseTarget);
                    RestoreDefault(legacyTarget);
                    RestoreDefault(platformPcTarget);
                    RestoreDefaultShiftlockVariants(keyboardMouseDir, log);
                    RestoreDefaultShiftlockVariants(legacyDir, log);
                    RestoreDefaultShiftlockVariants(platformPcDir, log);
                }
            }
            else
            {
                string directFile = Path.Combine(CursorPackFolder, "Shiftlock", presetName + ".png");
                if (File.Exists(directFile))
                {
                    string normalizedDirect = await PrepareShiftlockSourceAsync(directFile, log);
                    await ApplyShiftlockToAllTargetsAsync(normalizedDirect, keyboardMouseTarget, legacyTarget, platformPcTarget, keyboardMouseDir, legacyDir, platformPcDir, log);
                    log?.Invoke($"[Mods] Applied shiftlock file: {presetName}.png");
                }
                else
                {
                    log?.Invoke($"[Mods] Shiftlock preset '{presetName}' not found in pack (checked {presetDir} and {directFile}) — restoring default instead.");
                    RestoreDefault(keyboardMouseTarget);
                    RestoreDefault(legacyTarget);
                    RestoreDefault(platformPcTarget);
                    RestoreDefaultShiftlockVariants(keyboardMouseDir, log);
                    RestoreDefaultShiftlockVariants(legacyDir, log);
                    RestoreDefaultShiftlockVariants(platformPcDir, log);
                }
            }
        }

        private static async Task ApplyShiftlockToAllTargetsAsync(
            string sourcePng,
            string keyboardMouseTarget,
            string legacyTarget,
            string platformPcTarget,
            string keyboardMouseDir,
            string legacyDir,
            string platformPcDir,
            Action<string>? log)
        {
            await ApplyLocalFileAsync(sourcePng, keyboardMouseTarget, log);
            await ApplyLocalFileAsync(sourcePng, legacyTarget, log);
            await ApplyLocalFileAsync(sourcePng, platformPcTarget, log);
            await ApplyShiftlockVariantsAsync(sourcePng, keyboardMouseDir, log);
            await ApplyShiftlockVariantsAsync(sourcePng, legacyDir, log);
            await ApplyShiftlockVariantsAsync(sourcePng, platformPcDir, log);
        }

        private static async Task ApplyShiftlockVariantsAsync(string sourcePng, string destDir, Action<string>? log)
        {
            if (string.IsNullOrWhiteSpace(destDir) || !Directory.Exists(destDir))
                return;

            try
            {
                foreach (var file in Directory.EnumerateFiles(destDir, "MouseLockedCursor*.png", SearchOption.AllDirectories))
                {
                    await ApplyLocalFileAsync(sourcePng, file, log);
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Shiftlock variants apply failed in '{destDir}': {ex.Message}");
            }
        }

        private static void RestoreDefaultShiftlockVariants(string destDir, Action<string>? log)
        {
            if (string.IsNullOrWhiteSpace(destDir) || !Directory.Exists(destDir))
                return;

            try
            {
                foreach (var file in Directory.EnumerateFiles(destDir, "MouseLockedCursor*.png", SearchOption.AllDirectories))
                {
                    RestoreDefault(file);
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Mods] Shiftlock variants restore failed in '{destDir}': {ex.Message}");
            }
        }

        public static async Task ImportCustomShiftlockAsync(string sourcePath, Action<string>? log = null)
        {
            if (!File.Exists(sourcePath)) return;
            if (!Directory.Exists(CursorPackFolder)) Directory.CreateDirectory(CursorPackFolder);

            string dest = Path.Combine(CursorPackFolder, "CustomShiftlock.png");
            await ShiftlockImageNormalizer.NormalizeAsync(sourcePath, dest, ShiftlockImageNormalizer.DefaultTargetSize, log);
        }

        private static async Task<string> PrepareShiftlockSourceAsync(string sourcePath, Action<string>? log = null)
        {
            string normalizedPath = Path.Combine(CursorPackFolder, ".shiftlock_apply.png");
            await ShiftlockImageNormalizer.NormalizeAsync(
                sourcePath,
                normalizedPath,
                ShiftlockImageNormalizer.DefaultTargetSize,
                log);
            return normalizedPath;
        }

        public static string GetShiftlockImagePath(string presetName)
        {
            if (presetName == "Custom")
                return Path.Combine(CursorPackFolder, "CustomShiftlock.png");

            if (string.Equals(presetName, "Masterstrap", StringComparison.OrdinalIgnoreCase))
            {
                string cached = GetMasterstrapShiftlockCachePath();
                if (File.Exists(cached))
                    return cached;

                return ResolveMasterstrapIconPath() ?? cached;
            }

            string? resolvedFilePresetPath = ResolveShiftlockFilePresetPath(presetName);
            if (!string.IsNullOrWhiteSpace(resolvedFilePresetPath))
                return resolvedFilePresetPath;

            return Path.Combine(CursorPackFolder, "Shiftlock", presetName, "MouseLockedCursor.png");
        }

        private static IEnumerable<string> GetShiftlockFilePresetNames()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string folder in GetShiftlockPresetSearchFolders())
            {
                if (!Directory.Exists(folder))
                    continue;

                foreach (string file in Directory.EnumerateFiles(folder, "*.png", SearchOption.AllDirectories))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrWhiteSpace(fileName) ||
                        fileName.Equals("MouseLockedCursor", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    names.Add(fileName);
                }
            }

            return names.OrderBy(name => name, StringComparer.OrdinalIgnoreCase);
        }

        private static string? ResolveShiftlockFilePresetPath(string presetName)
        {
            if (string.IsNullOrWhiteSpace(presetName))
                return null;

            foreach (string folder in GetShiftlockPresetSearchFolders())
            {
                if (!Directory.Exists(folder))
                    continue;

                string direct = Path.Combine(folder, presetName + ".png");
                if (File.Exists(direct))
                    return direct;

                string? match = Directory.EnumerateFiles(folder, "*.png", SearchOption.AllDirectories)
                    .FirstOrDefault(file =>
                        string.Equals(Path.GetFileNameWithoutExtension(file), presetName, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            return null;
        }

        private static IEnumerable<string> GetShiftlockPresetSearchFolders()
        {
            var folders = new[]
            {
                Path.Combine(CursorPackFolder, "Shiftlock"),
                Path.Combine(CursorPackFolder, "crosshair"),
                Path.Combine(CursorPackFolder, "Crosshair"),
                Path.Combine(AppContext.BaseDirectory, "Shiftlock"),
                Path.Combine(AppContext.BaseDirectory, "crosshair"),
                Path.Combine(AppContext.BaseDirectory, "Crosshair"),
                Path.Combine(Directory.GetCurrentDirectory(), "Shiftlock"),
                Path.Combine(Directory.GetCurrentDirectory(), "crosshair"),
                Path.Combine(Directory.GetCurrentDirectory(), "Crosshair")
            };

            return folders
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static async Task ApplyLocalFileAsync(string source, string dest, Action<string>? log = null)
        {
            if (!File.Exists(source))
            {
                log?.Invoke($"[Error] Source file missing: {source}");
                return;
            }

            try
            {
                string? dir = Path.GetDirectoryName(dest);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    log?.Invoke($"[System] Created directory: {dir}");
                }

                BackupIfNeeded(dest, log);

                int retries = 3;
                while (retries > 0)
                {
                    try
                    {
                        File.Copy(source, dest, true);
                        log?.Invoke($"[Success] Applied file to: {dest}");
                        return;
                    }
                    catch (IOException ex) when (retries > 1)
                    {
                        log?.Invoke($"[Retry] File locked, retrying... ({ex.Message})");
                        await Task.Delay(500);
                        retries--;
                    }
                    catch (Exception ex)
                    {
                        log?.Invoke($"[Error] Failed to copy file: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Critical] ApplyLocalFileAsync failed: {ex.Message}");
            }
        }

        private static async Task ApplyRemoteAsync(string url, string dest, Action<string>? log = null)
        {
            try
            {
                var data = await _httpClient.GetByteArrayAsync(url);

                string? dir = Path.GetDirectoryName(dest);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                BackupIfNeeded(dest, log);

                int retries = 3;
                while (retries > 0)
                {
                    try
                    {
                        await File.WriteAllBytesAsync(dest, data);
                        log?.Invoke($"[Success] Downloaded and applied file to: {dest}");
                        return;
                    }
                    catch (IOException ex) when (retries > 1)
                    {
                        log?.Invoke($"[Retry] Remote apply file locked, retrying... ({ex.Message})");
                        await Task.Delay(500);
                        retries--;
                    }
                    catch (Exception ex)
                    {
                        log?.Invoke($"[Error] Failed to write remote file: {ex.Message}");
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                log?.Invoke($"[Critical] ApplyRemoteAsync failed: {ex.Message}");
            }
        }

        private static void BackupIfNeeded(string file, Action<string>? log = null)
        {
            if (!File.Exists(file))
            {
                log?.Invoke($"[Info] No existing file to backup at: {file}");
                return;
            }

            string backup = file + BackupSuffix;
            if (!File.Exists(backup))
            {
                try
                {
                    File.Copy(file, backup, true);
                    log?.Invoke($"[Backup] Created backup: {Path.GetFileName(backup)}");
                }
                catch (Exception ex)
                {
                    log?.Invoke($"[Backup Error] Failed to create backup: {ex.Message}");
                }
            }
        }

        private static void RestoreDefault(string file)
        {
            string backup = file + BackupSuffix;
            if (File.Exists(backup))
            {
                File.Copy(backup, file, true);
            }
        }
    }
}
