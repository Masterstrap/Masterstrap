using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Masterstrap.Services
{
    public class ProtocolRegistration
    {
        private const string ProtocolName = "roblox-player";
        private static readonly string LogPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Masterstrap",
            "protocol_registration.log");

        public static bool RegisterProtocolHandler()
        {
            try
            {
                string appPath = Process.GetCurrentProcess().MainModule?.FileName ?? "";
                if (string.IsNullOrEmpty(appPath))
                {
                    Log("Error: Could not get application path");
                    return false;
                }

                Log($"Registering protocol handler: {ProtocolName}://");
                Log($"Application path: {appPath}");

                using (var key = Registry.ClassesRoot.CreateSubKey(ProtocolName))
                {
                    if (key == null)
                    {
                        Log("Error: Could not create registry key");
                        return false;
                    }

                    key.SetValue("", $"URL:{ProtocolName} Protocol");
                    key.SetValue("URL Protocol", "");

                    Log($"Set registry key default value");
                }

                using (var cmdKey = Registry.ClassesRoot.CreateSubKey($@"{ProtocolName}\Shell\Open\Command"))
                {
                    if (cmdKey == null)
                    {
                        Log("Error: Could not create command registry key");
                        return false;
                    }

                    string command = $"\"{appPath}\" \"%1\"";
                    cmdKey.SetValue("", command);

                    Log($"Set command: {command}");
                }

                Log("✓ Protocol registration successful");
                return true;
            }
            catch (Exception ex)
            {
                Log($"Error during registration: {ex.Message}");
                Log($"Stack: {ex.StackTrace}");
                return false;
            }
        }

        public static bool IsProtocolRegistered()
        {
            try
            {
                using (var key = Registry.ClassesRoot.OpenSubKey(ProtocolName))
                {
                    bool registered = key != null;
                    Log($"Protocol registered check: {registered}");
                    return registered;
                }
            }
            catch (Exception ex)
            {
                Log($"Error checking registration: {ex.Message}");
                return false;
            }
        }

        public static bool UnregisterProtocolHandler()
        {
            try
            {
                Log($"Unregistering protocol handler: {ProtocolName}://");

                Registry.ClassesRoot.DeleteSubKeyTree(ProtocolName, false);

                Log("✓ Protocol unregistration successful");
                return true;
            }
            catch (Exception ex)
            {
                Log($"Error during unregistration: {ex.Message}");
                return false;
            }
        }

        private static void Log(string message)
        {
            try
            {
                string dir = System.IO.Path.GetDirectoryName(LogPath);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                System.IO.File.AppendAllText(LogPath, logMessage + Environment.NewLine);
                Console.WriteLine($"[ProtocolRegistration] {message}");
            }
            catch { }
        }
    }
}
