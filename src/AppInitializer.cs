using System;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using Masterstrap.Services;

namespace Masterstrap
{
    public static class AppInitializer
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Masterstrap"
        );

        private static readonly string InitialSetupFlagFile = Path.Combine(SettingsPath, ".protocolregistered");

        public static void PerformInitialSetup()
        {
            try
            {
                Directory.CreateDirectory(SettingsPath);

                if (!File.Exists(InitialSetupFlagFile))
                {
                    ReregisterProtocolHandler();
                    CreateSetupFlagFile();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AppInitializer] ✗ Error during initial setup: {ex.Message}");
            }
        }

        private static void RegisterProtocolHandler()
        {
            try
            {
                Console.WriteLine("[AppInitializer] Registering roblox:// protocol handler...");
                ProtocolHandler.RegisterProtocolHandler();
                Console.WriteLine("[AppInitializer] ✓ Protocol handler registered successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AppInitializer] ✗ Failed to register protocol handler: {ex.Message}");
            }
        }

        private static void CreateSetupFlagFile()
        {
            try
            {
                File.WriteAllText(InitialSetupFlagFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AppInitializer] Warning: Could not create setup flag file: {ex.Message}");
            }
        }

        public static void ReregisterProtocolHandler()
        {
            try
            {
                var settings = new AppSettingsManager();
                bool enabled = settings.IsProtocolInterceptionEnabled();

                if (enabled)
                {
                    Console.WriteLine("[AppInitializer] Re-registering protocol handlers to Masterstrap...");
                    ProtocolHandler.RegisterProtocolHandler();
                    Console.WriteLine("[AppInitializer] ✓ Protocol handlers re-registered to Masterstrap");
                }
                else
                {
                    Console.WriteLine("[AppInitializer] Protocol interception disabled. Registering Roblox directly...");
                    Task.Run(async () =>
                    {
                        try
                        {
                            var (success, robloxExePath) = await RobloxInstallationManager.EnsureRobloxInstalledAsync(
                                logCallback: _ => { },
                                updateStatusCallback: _ => { },
                                silent: true
                            );

                            if (success && !string.IsNullOrEmpty(robloxExePath))
                            {
                                ProtocolHandler.RegisterProtocolHandler(robloxExePath);
                                Console.WriteLine($"[AppInitializer] ✓ Protocol handlers redirected to Roblox: {robloxExePath}");
                            }
                            else
                            {
                                ProtocolHandler.UnregisterProtocolHandler();
                                Console.WriteLine("[AppInitializer] ✓ Protocol handlers unregistered (Roblox not found)");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[AppInitializer] ✗ Error redirecting protocol to Roblox: {ex.Message}");
                            ProtocolHandler.UnregisterProtocolHandler();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update protocol handler: {ex.Message}",
                               "Setup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void UnregisterProtocolHandlers()
        {
            try
            {
                Console.WriteLine("[AppInitializer] Unregistering protocol handlers...");
                ProtocolHandler.UnregisterProtocolHandler();

                try
                {
                    File.Delete(InitialSetupFlagFile);
                }
                catch { }

                Console.WriteLine("[AppInitializer] ✓ Protocol handlers unregistered");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to unregister protocol handlers: {ex.Message}",
                               "Setup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
