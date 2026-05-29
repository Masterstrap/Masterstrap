using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Masterstrap.Services
{
    public static class ProtocolHandler
    {
        private const string ProtocolName = "roblox";
        private const string ProtocolName2 = "roblox-player";
        private const string ProtocolDescription = "Masterstrap Roblox Protocol Handler";

        public static void RegisterProtocolHandler(string exePath = null)
        {
            try
            {
                if (string.IsNullOrEmpty(exePath))
                {
                    exePath = Process.GetCurrentProcess().MainModule?.FileName;
                }

                if (string.IsNullOrEmpty(exePath))
                {
                    Console.WriteLine("[ProtocolHandler] ✗ Could not get executable path");
                    return;
                }

                RegisterProtocol(ProtocolName, exePath);
                RegisterProtocol(ProtocolName2, exePath);

                Console.WriteLine($"[ProtocolHandler] ✓ Protocol handlers registered to: {exePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProtocolHandler] ✗ Error registering protocol: {ex.Message}");
            }
        }

        private static void RegisterProtocol(string protocolName, string exePath)
        {
            try
            {
                using (RegistryKey classesKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{protocolName}"))
                {
                    classesKey.SetValue("", $"URL:{ProtocolDescription}");
                    classesKey.SetValue("URL Protocol", "");
                }

                using (RegistryKey commandKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{protocolName}\shell\open\command"))
                {
                    commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                using (RegistryKey iconKey = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{protocolName}\DefaultIcon"))
                {
                    iconKey.SetValue("", $"{exePath},0");
                }

                Console.WriteLine($"[ProtocolHandler] ✓ Registered {protocolName}:// protocol");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProtocolHandler] ✗ Error registering {protocolName} protocol: {ex.Message}");
            }
        }

        public static void UnregisterProtocolHandler()
        {
            try
            {
                UnregisterProtocol(ProtocolName);
                UnregisterProtocol(ProtocolName2);
                Console.WriteLine("[ProtocolHandler] ✓ Protocol handlers unregistered");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProtocolHandler] ✗ Error unregistering protocol: {ex.Message}");
            }
        }

        private static void UnregisterProtocol(string protocolName)
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree($@"Software\Classes\{protocolName}", false);
                Console.WriteLine($"[ProtocolHandler] ✓ Unregistered {protocolName}:// protocol");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProtocolHandler] ✗ Error unregistering {protocolName}: {ex.Message}");
            }
        }

        public static string ExtractLaunchUrl(string protocolUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(protocolUrl))
                    return string.Empty;

                if (protocolUrl.StartsWith("roblox://", StringComparison.OrdinalIgnoreCase) ||
                    protocolUrl.StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase))
                {
                    return protocolUrl;
                }

                if (protocolUrl.StartsWith("roblox-player:", StringComparison.OrdinalIgnoreCase))
                {
                    string placeId = ExtractPlaceId(protocolUrl);
                    if (!string.IsNullOrEmpty(placeId))
                    {
                        string standardUrl = $"roblox-player://experiences/start?placeId={placeId}";
                        Console.WriteLine($"[ProtocolHandler] ✓ Converted roblox.com format to: {standardUrl}");
                        return standardUrl;
                    }
                }

                Console.WriteLine($"[ProtocolHandler] ⚠ Unknown protocol format, returning as-is");
                return protocolUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProtocolHandler] ✗ Error extracting launch URL: {ex.Message}");
                return string.Empty;
            }
        }

        public static bool IsValidRobloxProtocol(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            url = url.Trim(' ', '"', '\'', '\t', '\n', '\r');

            if (url.StartsWith("roblox://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (url.StartsWith("roblox-player:", StringComparison.OrdinalIgnoreCase) &&
                (url.Contains("+launchmode:") || url.Contains("+gameinfo:")))
            {
                return true;
            }

            return false;
        }

        public static string ExtractPlaceId(string protocolUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(protocolUrl))
                    return string.Empty;

                Console.WriteLine($"[ProtocolHandler] Parsing: {protocolUrl.Substring(0, Math.Min(100, protocolUrl.Length))}...");

                if (protocolUrl.Contains("placeId="))
                {
                    var parts = protocolUrl.Split(new[] { "placeId=" }, StringSplitOptions.None);
                    if (parts.Length > 1)
                    {
                        string placeId = parts[1].Split(new[] { '&', ' ' }, StringSplitOptions.None)[0];
                        Console.WriteLine($"[ProtocolHandler] ✓ Extracted placeId: {placeId}");
                        return placeId;
                    }
                }

                if (protocolUrl.Contains("placeId%3D"))
                {
                    Match match = Regex.Match(protocolUrl, @"placeId%3D(\d+)");
                    if (match.Success)
                    {
                        string placeId = match.Groups[1].Value;
                        Console.WriteLine($"[ProtocolHandler] ✓ Extracted placeId from roblox.com format: {placeId}");
                        return placeId;
                    }
                }

                Console.WriteLine($"[ProtocolHandler] ✗ Could not extract placeId");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProtocolHandler] ✗ Error extracting placeId: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
