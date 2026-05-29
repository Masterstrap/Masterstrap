using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Masterstrap.Services
{
    internal static class RobloxFontModService
    {
        private const string BackupSuffix = ".masterstrap.bak";

        private static readonly HashSet<string> SupportedFontExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".ttf", ".otf", ".ttc" };

        private static bool IsCustomizableFontFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            if (fileName.EndsWith(BackupSuffix, StringComparison.OrdinalIgnoreCase))
                return false;
            if (string.Equals(fileName, "TwemojiMozilla.ttf", StringComparison.OrdinalIgnoreCase))
                return false;
            return SupportedFontExtensions.Contains(Path.GetExtension(filePath));
        }

        public static async Task<bool> ApplyCustomFontAsync(string robloxVersionRoot, string customFontSourcePath, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(robloxVersionRoot) || string.IsNullOrWhiteSpace(customFontSourcePath))
                return false;
            if (!File.Exists(customFontSourcePath))
                return false;

            string fontsDir = Path.Combine(robloxVersionRoot, "content", "fonts");
            if (!Directory.Exists(fontsDir))
                return false;

            var targets = Directory.GetFiles(fontsDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(IsCustomizableFontFile)
                .ToList();

            if (targets.Count == 0)
                return false;

            await Task.Run(() =>
            {
                foreach (string target in targets)
                {
                    string backupPath = target + BackupSuffix;
                    try
                    {
                        if (File.Exists(target) && !File.Exists(backupPath))
                            File.Copy(target, backupPath, true);
                    }
                    catch (Exception ex)
                    {
                        log?.Invoke($"[Mods] Backup font failed ({Path.GetFileName(target)}): {ex.Message}");
                    }

                    try
                    {
                        File.Copy(customFontSourcePath, target, true);
                    }
                    catch (Exception ex)
                    {
                        log?.Invoke($"[Mods] Apply custom font failed ({Path.GetFileName(target)}): {ex.Message}");
                    }
                }
            });

            log?.Invoke($"[Mods] Custom font applied to {targets.Count} Roblox font files.");
            return true;
        }

        public static async Task<bool> RestoreDefaultFontsAsync(string robloxVersionRoot, Action<string>? log = null)
        {
            if (string.IsNullOrWhiteSpace(robloxVersionRoot))
                return false;

            string fontsDir = Path.Combine(robloxVersionRoot, "content", "fonts");
            if (!Directory.Exists(fontsDir))
                return false;

            var backups = Directory.GetFiles(fontsDir, "*" + BackupSuffix, SearchOption.TopDirectoryOnly);
            if (backups.Length == 0)
                return false;

            await Task.Run(() =>
            {
                foreach (string backupPath in backups)
                {
                    string originalPath = backupPath.Substring(0, backupPath.Length - BackupSuffix.Length);
                    try
                    {
                        File.Copy(backupPath, originalPath, true);
                        File.Delete(backupPath);
                    }
                    catch (Exception ex)
                    {
                        log?.Invoke($"[Mods] Restore font failed ({Path.GetFileName(originalPath)}): {ex.Message}");
                    }
                }
            });

            log?.Invoke($"[Mods] Restored {backups.Length} font files from backup.");
            return true;
        }
    }
}
