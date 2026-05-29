using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Masterstrap.Services
{
    /// <summary>
    /// </summary>
    public sealed class ClientSettingsFlagService
    {
        public const string ClientAppSettingsFileName = "ClientAppSettings.json";
        private const string BackupSuffix = ".masterstrap.bak";

        private readonly Dictionary<string, object> _pendingFlags =
            new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public event Action<string>? OnLog;

        public int PendingFlagCount => _pendingFlags.Count;

        public void ClearPendingFlags() => _pendingFlags.Clear();

        public void UpdateFlags(IReadOnlyDictionary<string, string> flags)
        {
            if (flags == null)
                return;

            foreach (var kvp in flags)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key))
                    continue;
                _pendingFlags[kvp.Key] = ParseFlagValue(kvp.Value);
            }
        }

        public int RemoveFlagsByNames(IEnumerable<string> flagNames)
        {
            int removed = 0;
            if (flagNames == null)
                return 0;

            foreach (string name in flagNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                if (_pendingFlags.Remove(name))
                    removed++;
            }

            return removed;
        }

        /// <summary>
        /// </summary>
        public ClientSettingsApplyResult ApplyToRobloxInstall(string? robloxPlayerExePath, bool writeGlobalFallback = true)
        {
            var result = new ClientSettingsApplyResult();
            if (_pendingFlags.Count == 0)
            {
                result.Message = "No flags queued for ClientAppSettings.";
                return result;
            }

            var targets = ResolveTargetPaths(robloxPlayerExePath, writeGlobalFallback);
            if (targets.Count == 0)
            {
                result.Message = "Could not resolve ClientSettings folder for Roblox install.";
                return result;
            }

            foreach (string jsonPath in targets)
            {
                try
                {
                    string? dir = Path.GetDirectoryName(jsonPath);
                    if (string.IsNullOrEmpty(dir))
                        continue;

                    BackupIfNeeded(jsonPath);

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    var merged = ReadExistingFlags(jsonPath);
                    int written = 0;
                    foreach (var kvp in _pendingFlags)
                    {
                        merged[kvp.Key] = kvp.Value;
                        written++;
                    }

                    string json = JsonSerializer.Serialize(merged, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(jsonPath, json);
                    result.AppliedCount += written;
                    result.WrittenPaths.Add(jsonPath);
                    Log($"[ClientSettings] Wrote {written} flag(s) to {jsonPath}");
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"{jsonPath}: {ex.Message}");
                    Log($"[ClientSettings] Failed {jsonPath}: {ex.Message}");
                }
            }

            result.Success = result.WrittenPaths.Count > 0;
            result.Message = result.Success
                ? $"Applied {_pendingFlags.Count} flag(s) to ClientAppSettings.json ({result.WrittenPaths.Count} path(s))."
                : "Failed to write ClientAppSettings.json.";
            return result;
        }
        public int RestoreClientSettingsBackups(string? robloxPlayerExePath)
        {
            int restored = 0;
            foreach (string jsonPath in ResolveTargetPaths(robloxPlayerExePath, writeGlobalFallback: true))
            {
                string backup = jsonPath + BackupSuffix;
                try
                {
                    if (!File.Exists(backup))
                        continue;

                    File.Copy(backup, jsonPath, overwrite: true);
                    File.Delete(backup);
                    restored++;
                    Log($"[ClientSettings] Restored backup: {jsonPath}");
                }
                catch (Exception ex)
                {
                    Log($"[ClientSettings] Restore failed {jsonPath}: {ex.Message}");
                }
            }

            return restored;
        }

        public static string? ResolveVersionClientAppSettingsPath(string? robloxPlayerExePath)
        {
            if (string.IsNullOrWhiteSpace(robloxPlayerExePath) || !File.Exists(robloxPlayerExePath))
                return null;

            string? versionDir = Path.GetDirectoryName(robloxPlayerExePath);
            if (string.IsNullOrEmpty(versionDir))
                return null;

            return Path.Combine(versionDir, "ClientSettings", ClientAppSettingsFileName);
        }

        public static string GetGlobalClientAppSettingsPath() =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox",
                "ClientSettings",
                ClientAppSettingsFileName);

        private static List<string> ResolveTargetPaths(string? robloxPlayerExePath, bool writeGlobalFallback)
        {
            var paths = new List<string>();
            string? versionPath = ResolveVersionClientAppSettingsPath(robloxPlayerExePath);
            if (!string.IsNullOrEmpty(versionPath))
                paths.Add(versionPath);

            if (writeGlobalFallback)
            {
                string global = GetGlobalClientAppSettingsPath();
                if (!paths.Any(p => string.Equals(Path.GetFullPath(p), Path.GetFullPath(global), StringComparison.OrdinalIgnoreCase)))
                    paths.Add(global);
            }

            return paths;
        }

        private static Dictionary<string, object> ReadExistingFlags(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            try
            {
                string text = File.ReadAllText(jsonPath);
                var raw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(text);
                if (raw == null)
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in raw)
                    result[kvp.Key] = JsonElementToObject(kvp.Value);
                return result;
            }
            catch
            {
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private static void BackupIfNeeded(string jsonPath)
        {
            try
            {
                if (File.Exists(jsonPath) && !File.Exists(jsonPath + BackupSuffix))
                    File.Copy(jsonPath, jsonPath + BackupSuffix, overwrite: false);
            }
            catch
            {
            }
        }

        private static object JsonElementToObject(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? "",
                JsonValueKind.Number when element.TryGetInt64(out long l) => l,
                JsonValueKind.Number => element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => element.ToString()
            };
        }

        private static object ParseFlagValue(string? value)
        {
            if (value == null)
                return "";

            string trimmed = value.Trim();
            if (trimmed.Length == 0)
                return "";

            if (bool.TryParse(trimmed, out bool b))
                return b;

            if (long.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l))
                return l;

            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                return d;

            return trimmed;
        }

        private void Log(string message) => OnLog?.Invoke(message);
    }

    public sealed class ClientSettingsApplyResult
    {
        public bool Success { get; set; }
        public int AppliedCount { get; set; }
        public string Message { get; set; } = "";
        public List<string> WrittenPaths { get; } = new List<string>();
        public List<string> Errors { get; } = new List<string>();
    }
}
