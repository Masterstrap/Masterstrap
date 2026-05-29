using System;
using System.Collections.Generic;
using System.Linq;
using Masterstrap.Models;
using Masterstrap.Services;

namespace Masterstrap
{
    public partial class MainWindow
    {
        private void EnsureFFlagServiceInitialized()
        {
            if (this._fflagService != null)
                return;

            this._fflagService = new FFlagService();
            this._fflagService.OnLogGenerated += msg => this.Log(msg);
        }

        /// <summary>
        /// Applies queued FFlags by overwriting Roblox ClientAppSettings.json (Bloxstrap method).
        /// Build 013.
        /// </summary>
        private bool TryApplyFastFlagsViaClientSettings(IReadOnlyList<FlagItem>? editorFlags, string logPrefix = "[FFlags]")
        {
            try
            {
                this.EnsureFFlagServiceInitialized();
                if (this._fflagService == null)
                    return false;

                bool allowManage = this.AllowManageFastFlagsToggle?.IsChecked ?? true;
                if (!allowManage)
                {
                    this.Log($"{logPrefix} Skipped — Allow Manage Fast Flags is off.");
                    return true;
                }

                this.ApplyFastFlagSettingsPresetsToService(persistPayloadToDisk: false);

                var result = this._fflagService.ApplyEditorFlags(this._robloxExecutablePath, editorFlags);
                if (result.Success)
                {
                    this.Log($"{logPrefix} {result.Message}");
                    foreach (string path in result.WrittenPaths)
                        this.Log($"{logPrefix} → {path}");
                    return true;
                }

                if (this._fflagService.PendingFlagCount == 0)
                {
                    this.Log($"{logPrefix} No flags to write to ClientAppSettings.");
                    return true;
                }

                this.Log($"{logPrefix} Failed to write ClientAppSettings: {result.Message}");
                return false;
            }
            catch (Exception ex)
            {
                this.Log($"{logPrefix} ClientSettings apply error: {ex.Message}");
                return false;
            }
        }
    }
}
