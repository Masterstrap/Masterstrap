using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Masterstrap.Services
{
    public class LaunchManager
    {
        public event EventHandler<string> StatusChanged;
        public event EventHandler LaunchCompleted;
        public event EventHandler<Exception> LaunchFailed;

        public async Task LaunchRobloxAsync(string protocolUrl, bool isProtocolLaunch = false)
        {
            try
            {
                if (!ProtocolHandler.IsValidRobloxProtocol(protocolUrl))
                {
                    throw new ArgumentException("Invalid Roblox protocol URL", nameof(protocolUrl));
                }

                Console.WriteLine($"[LaunchManager] Starting launch sequence for: {protocolUrl}");
                Process.Start(new ProcessStartInfo
                {
                    FileName = protocolUrl,
                    UseShellExecute = true
                });
                OnLaunchCompleted();
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LaunchManager] Error: {ex.Message}");
                OnLaunchFailed(ex);
                throw;
            }
        }

        public void UpdateProgress(string status, int percentage)
        {
            StatusChanged?.Invoke(this, status);
        }

        public void CloseProgressWindow()
        {
        }

        protected virtual void OnLaunchCompleted()
        {
            LaunchCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLaunchFailed(Exception ex)
        {
            LaunchFailed?.Invoke(this, ex);
        }
    }
}
