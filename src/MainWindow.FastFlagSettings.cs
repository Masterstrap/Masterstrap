using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Masterstrap.Services;

namespace Masterstrap
{
    /// <summary>
    /// Partial class: FastFlag Settings tab — event handlers, preset builder, JSON writer.
    /// Mirrors Voidstrap FastFlagsViewModel behaviour.
    /// </summary>
    public partial class MainWindow : Window
    {
        // ── Generic change handlers ────────────────────────────────
        private void FFSettings_Toggle_Changed(object sender, RoutedEventArgs e)
        {
            if (!this._initializationComplete) return;
            this.HasUnsavedChanges = true;
            if (sender is ToggleButton tb)
            {
                this.Log($"[FFSettings] Toggle {tb.Name} = {tb.IsChecked}");
                if (tb.Name == "FFSPingBoostToggle")
                {
                    this.HandlePingBoostToggleChanged(tb.IsChecked == true);
                }
                if (tb.Name == "FFSDscpQosToggle")
                {
                    this.HandleDscpQosToggleChanged(tb.IsChecked == true);
                }
            }
        }

        private void FFSettings_Combo_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!this._initializationComplete) return;
            this.HasUnsavedChanges = true;
        }

        private void FFSettings_Text_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!this._initializationComplete) return;
            this.HasUnsavedChanges = true;
        }

        // ── Build FFlag dictionary from current UI state ───────────
        internal Dictionary<string, object> BuildFastFlagSettingsPresets()
        {
            var f = new Dictionary<string, object>();
            try
            {
                // ─── System (CPU & Threads) ───
                string cpuTag = GetComboTag(this.FindName("FFSForceLogicalProcessorsCombo") as ComboBox, "Automatic");
                if (cpuTag != "Automatic" && int.TryParse(cpuTag, out int cores))
                {
                    int tl = Math.Max(cores - 1, 1);
                    f["DFIntRuntimeConcurrency"] = tl;
                    f["FIntTaskSchedulerAutoThreadLimit"] = tl;
                    f["DFIntInterpolationNumParallelTasks"] = cores;
                    f["DFIntMegaReplicatorNumParallelTasks"] = cores;
                    f["DFIntNetworkClusterPacketCacheNumParallelTasks"] = cores;
                    f["DFIntReplicationDataCacheNumParallelTasks"] = cores;
                    f["FIntLuaGcParallelMinMultiTasks"] = cores;
                    f["FIntSmoothClusterTaskQueueMaxParallelTasks"] = cores;
                    f["DFIntPhysicsReceiveNumParallelTasks"] = cores;
                    f["FIntSimWorldTaskQueueParallelTasks"] = cores;
                }

                string asyncTag = GetComboTag(this.FindName("FFSForceAsyncThreadsCombo") as ComboBox, "Automatic");
                if (asyncTag != "Automatic" && int.TryParse(asyncTag, out int asyncCores))
                {
                    int adjustedAsync = Math.Max(asyncCores - 1, 1);
                    f["FIntTaskSchedulerAsyncTasksMinimumThreadCount"] = adjustedAsync;
                }

                if (IsToggleOn("FFSHyperThreadingToggle"))
                {
                    f["FFlagDebugCheckRenderThreading"] = "True";
                    f["FFlagRenderDebugCheckThreading2"] = "True";
                }

                // ─── Framerate & Display ───
                // Roblox Screen Size is handled during process launch in LaunchProgressWindow

                string dr = GetComboTag(this.FindName("FFSDynamicResCombo") as ComboBox, "Default");
                if (dr != "Default")
                {
                    string kp = dr switch
                    {
                        "240p" => "30", "360p" => "77", "480p" => "230", "720p" => "410",
                        "1080p" => "922", "2K" => "2074", "4K" => "3686", _ => null
                    };
                    if (kp != null) f["DFIntDebugDynamicRenderKiloPixels"] = kp;
                }

                // ─── Rendering ───
                string lt = GetComboTag(this.FindName("FFSLightingCombo") as ComboBox, "Default");
                switch (lt)
                {
                    case "Voxel": f["DFFlagDebugRenderForceTechnologyVoxel"] = "True"; break;
                    case "ShadowMap": f["FFlagDebugForceFutureIsBrightPhase2"] = "True"; break;
                    case "Future": f["FFlagDebugForceFutureIsBrightPhase3"] = "True"; break;
                    case "Unified": f["FFlagRenderUnifiedLighting14"] = "True"; break;
                }

                string ra = GetComboTag(this.FindName("FFSRenderApiCombo") as ComboBox, "Auto");
                switch (ra)
                {
                    case "D3D11":
                        f["FFlagDebugGraphicsPreferD3D11"] = "True"; break;
                    case "Vulkan":
                        f["FFlagDebugGraphicsPreferVulkan"] = "True";
                        f["FFlagDebugGraphicsDisableDirect3D11"] = "True"; break;
                    case "OpenGL":
                        f["FFlagDebugGraphicsPreferOpenGL"] = "True";
                        f["FFlagDebugGraphicsDisableDirect3D11"] = "True"; break;
                }

                if (IsToggleOn("FFSDisableShadowsToggle"))
                {
                    f["FIntRenderShadowIntensity"] = 0;
                    f["DFFlagDebugPauseVoxelizer"] = "True";
                    f["FIntRenderShadowmapBias"] = -1;
                }

                if (IsToggleOn("FFSDisablePostFxToggle"))
                    f["FFlagDisablePostFx"] = "True";

                if (IsToggleOn("FFSDisableTerrainToggle"))
                    f["FIntTerrainArraySliceSize"] = 0;

                // ─── Network ───
                if (IsToggleOn("FFSLessLagToggle"))
                {
                    f["DFIntBandwidthManagerApplicationDefaultBps"] = 796850000;
                    f["DFIntBandwidthManagerDataSenderMaxWorkCatchupMs"] = 5;
                }

                if (IsToggleOn("FFSNoPayloadLimitToggle"))
                {
                    string[] payloadFlags = {
                        "DFIntRccMaxPayloadSnd", "DFIntCliMaxPayloadRcv",
                        "DFIntCliMaxPayloadSnd", "DFIntRccMaxPayloadRcv",
                        "DFIntCliTcMaxPayloadRcv", "DFIntRccTcMaxPayloadRcv",
                        "DFIntCliTcMaxPayloadSnd", "DFIntRccTcMaxPayloadSnd",
                        "DFIntMaxDataPayloadSize", "DFIntMaxUREPayloadSingleLimit",
                        "DFIntTotalRepPayloadLimit"
                    };
                    foreach (var pf in payloadFlags) f[pf] = 2147483647;
                }

                if (IsToggleOn("FFSLargeReplicatorToggle"))
                {
                    f["FFlagLargeReplicatorEnabled7"] = "True";
                    f["FFlagLargeReplicatorWrite5"] = "True";
                    f["FFlagLargeReplicatorRead5"] = "True";
                    f["FFlagLargeReplicatorSerializeRead3"] = "True";
                    f["FFlagLargeReplicatorSerializeWrite3"] = "True";
                }

                if (IsToggleOn("FFSBetterPacketToggle"))
                {
                    f["DFIntNetworkStopProducingPacketsToProcessThresholdMs"] = 0;
                    f["DFIntMaxWaitTimeBeforeForcePacketProcessMS"] = 0;
                    f["DFIntClientPacketMaxDelayMs"] = 1;
                }

                // ─── Telemetry & Privacy ───
                if (IsToggleOn("FFSDisableTelemetryToggle"))
                {
                    f["DFStringTelemetryV2Url"] = "0.0.0.0";
                    string[] telFlags = {
                        "FFlagEnableTelemetryProtocol", "DFFlagGraphicsQualityUsageTelemetry",
                        "DFFlagGpuVsCpuBoundTelemetry", "DFFlagSendRenderFidelityTelemetry",
                        "DFFlagReportRenderDistanceTelemetry", "DFFlagCollectAudioPluginTelemetry",
                        "DFFlagEnableFmodErrorsTelemetry", "DFFlagRccLoadSoundLengthTelemetryEnabled",
                        "DFFlagReportAssetRequestV1Telemetry", "DFFlagRobloxTelemetryAddDeviceRAMPointsV2",
                        "DFFlagEnableTelemetryV2FRMStats", "DFFlagEnableSkipUpdatingGlobalTelemetryInfo2",
                        "DFFlagEmitSafetyTelemetryInCallbackEnable", "DFFlagRobloxTelemetryV2PointEncoding",
                        "DFFlagDSTelemetryV2ReplaceSeparator", "FFlagEnableTelemetryService1",
                        "FFlagPropertiesEnableTelemetry", "FFlagOpenTelemetryEnabled"
                    };
                    foreach (var tf in telFlags) f[tf] = "False";
                    f["FLogRobloxTelemetry"] = 0;
                }

                if (IsToggleOn("FFSDisableAdsToggle"))
                {
                    f["FFlagAdServiceEnabled"] = "False";
                    f["FFlagEnableSponsoredAdsGameCarouselTooltip3"] = "False";
                    f["FFlagLuaAppSponsoredGridTiles"] = "False";
                }

                if (IsToggleOn("FFSBlockTencentToggle"))
                {
                    f["FStringTencentAuthPath"] = "/tencent/";
                    f["DFFlagPolicyServiceReportIsNotSubjectToChinaPolicies"] = "True";
                }

                // ─── UI Tweaks ───
                if (IsToggleOn("FFSNoGuiBlurToggle"))
                    f["FIntRobloxGuiBlurIntensity"] = 0;

                if (IsToggleOn("FFSUnlimitedZoomToggle"))
                    f["FIntCameraMaxZoomDistance"] = 2147483647;

                if (IsToggleOn("FFSCustomDisconnectToggle"))
                {
                    f["FFlagReconnectDisabled"] = "True";
                    var tb = this.FindName("FFSCustomDisconnectText") as TextBox;
                    string msg = tb?.Text ?? "Disconnected by Masterstrap";
                    f["FStringReconnectDisabledReason"] = msg;
                }
            }
            catch (Exception ex)
            {
                this.Log($"[FFSettings] Error building presets: {ex.Message}");
            }
            return f;
        }

        // ── Apply presets to FFlagService (ClientSettings \(Bloxstrap-style\)) ────────────────
        internal void ApplyFastFlagSettingsPresetsToService(bool persistPayloadToDisk = true)
        {
            try
            {
                if (this._fflagService == null)
                {
                    this.Log("[FFSettings] Cannot apply presets: FFlagService is not initialized");
                    return;
                }

                if (!this._initializationComplete)
                {
                    if (FastFlagSettingsSavedApplicator.TryApplyToService(this._fflagService, out int applied, out int removed))
                    {
                        this.Log($"[FFSettings] ✓ Applied saved presets without UI (applied={applied}, removed={removed})");
                    }
                    return;
                }

                var presets = this.BuildFastFlagSettingsPresets();

                // Collect FFlags to remove based on default/automatic combobox selections
                var flagsToRemove = new List<string>();

                // 1. FFSForceLogicalProcessorsCombo
                string cpuTag = GetComboTag(this.FindName("FFSForceLogicalProcessorsCombo") as ComboBox, "Automatic");
                if (cpuTag == "Automatic")
                {
                    flagsToRemove.AddRange(new[] {
                        "DFIntRuntimeConcurrency",
                        "FIntTaskSchedulerAutoThreadLimit",
                        "DFIntInterpolationNumParallelTasks",
                        "DFIntMegaReplicatorNumParallelTasks",
                        "DFIntNetworkClusterPacketCacheNumParallelTasks",
                        "DFIntReplicationDataCacheNumParallelTasks",
                        "FIntLuaGcParallelMinMultiTasks",
                        "FIntSmoothClusterTaskQueueMaxParallelTasks",
                        "DFIntPhysicsReceiveNumParallelTasks",
                        "FIntSimWorldTaskQueueParallelTasks"
                    });
                }

                // 2. FFSForceAsyncThreadsCombo
                string asyncTag = GetComboTag(this.FindName("FFSForceAsyncThreadsCombo") as ComboBox, "Automatic");
                if (asyncTag == "Automatic")
                {
                    flagsToRemove.Add("FIntTaskSchedulerAsyncTasksMinimumThreadCount");
                }

                // 3. FFSRobloxResolutionCombo (No FFlags to remove since resolution customization is a window operation)

                // 4. FFSDynamicResCombo
                string dr = GetComboTag(this.FindName("FFSDynamicResCombo") as ComboBox, "Default");
                if (dr == "Default")
                {
                    flagsToRemove.Add("DFIntDebugDynamicRenderKiloPixels");
                }

                // 5. FFSLightingCombo
                string lt = GetComboTag(this.FindName("FFSLightingCombo") as ComboBox, "Default");
                if (lt == "Default")
                {
                    flagsToRemove.AddRange(new[] {
                        "DFFlagDebugRenderForceTechnologyVoxel",
                        "FFlagDebugForceFutureIsBrightPhase2",
                        "FFlagDebugForceFutureIsBrightPhase3",
                        "FFlagRenderUnifiedLighting14"
                    });
                }

                // 6. FFSRenderApiCombo
                string ra = GetComboTag(this.FindName("FFSRenderApiCombo") as ComboBox, "Auto");
                if (ra == "Auto")
                {
                    flagsToRemove.AddRange(new[] {
                        "FFlagDebugGraphicsPreferD3D11",
                        "FFlagDebugGraphicsPreferVulkan",
                        "FFlagDebugGraphicsPreferOpenGL",
                        "FFlagDebugGraphicsDisableDirect3D11"
                    });
                }

                // Collect FFlags to remove for toggles that are OFF
                if (!IsToggleOn("FFSHyperThreadingToggle"))
                {
                    flagsToRemove.AddRange(new[] { "FFlagDebugCheckRenderThreading", "FFlagRenderDebugCheckThreading2" });
                }

                if (!IsToggleOn("FFSDisableShadowsToggle"))
                {
                    flagsToRemove.AddRange(new[] { "FIntRenderShadowIntensity", "DFFlagDebugPauseVoxelizer", "FIntRenderShadowmapBias" });
                }

                if (!IsToggleOn("FFSDisablePostFxToggle"))
                {
                    flagsToRemove.Add("FFlagDisablePostFx");
                }

                if (!IsToggleOn("FFSDisableTerrainToggle"))
                {
                    flagsToRemove.Add("FIntTerrainArraySliceSize");
                }

                if (!IsToggleOn("FFSLessLagToggle"))
                {
                    flagsToRemove.AddRange(new[] { "DFIntBandwidthManagerApplicationDefaultBps", "DFIntBandwidthManagerDataSenderMaxWorkCatchupMs" });
                }

                if (!IsToggleOn("FFSNoPayloadLimitToggle"))
                {
                    flagsToRemove.AddRange(new[] {
                        "DFIntRccMaxPayloadSnd", "DFIntCliMaxPayloadRcv", "DFIntCliMaxPayloadSnd", "DFIntRccMaxPayloadRcv",
                        "DFIntCliTcMaxPayloadRcv", "DFIntRccTcMaxPayloadRcv", "DFIntCliTcMaxPayloadSnd", "DFIntRccTcMaxPayloadSnd",
                        "DFIntMaxDataPayloadSize", "DFIntMaxUREPayloadSingleLimit", "DFIntTotalRepPayloadLimit"
                    });
                }

                if (!IsToggleOn("FFSLargeReplicatorToggle"))
                {
                    flagsToRemove.AddRange(new[] {
                        "FFlagLargeReplicatorEnabled7", "FFlagLargeReplicatorWrite5", "FFlagLargeReplicatorRead5",
                        "FFlagLargeReplicatorSerializeRead3", "FFlagLargeReplicatorSerializeWrite3"
                    });
                }

                if (!IsToggleOn("FFSBetterPacketToggle"))
                {
                    flagsToRemove.AddRange(new[] {
                        "DFIntNetworkStopProducingPacketsToProcessThresholdMs", "DFIntMaxWaitTimeBeforeForcePacketProcessMS", "DFIntClientPacketMaxDelayMs"
                    });
                }

                if (!IsToggleOn("FFSDisableTelemetryToggle"))
                {
                    flagsToRemove.Add("DFStringTelemetryV2Url");
                    flagsToRemove.AddRange(new[] {
                        "FFlagEnableTelemetryProtocol", "DFFlagGraphicsQualityUsageTelemetry", "DFFlagGpuVsCpuBoundTelemetry", "DFFlagSendRenderFidelityTelemetry",
                        "DFFlagReportRenderDistanceTelemetry", "DFFlagCollectAudioPluginTelemetry", "DFFlagEnableFmodErrorsTelemetry", "DFFlagRccLoadSoundLengthTelemetryEnabled",
                        "DFFlagReportAssetRequestV1Telemetry", "DFFlagRobloxTelemetryAddDeviceRAMPointsV2", "DFFlagEnableTelemetryV2FRMStats", "DFFlagEnableSkipUpdatingGlobalTelemetryInfo2",
                        "DFFlagEmitSafetyTelemetryInCallbackEnable", "DFFlagRobloxTelemetryV2PointEncoding", "DFFlagDSTelemetryV2ReplaceSeparator", "FFlagEnableTelemetryService1",
                        "FFlagPropertiesEnableTelemetry", "FFlagOpenTelemetryEnabled", "FLogRobloxTelemetry"
                    });
                }

                if (!IsToggleOn("FFSDisableAdsToggle"))
                {
                    flagsToRemove.AddRange(new[] { "FFlagAdServiceEnabled", "FFlagEnableSponsoredAdsGameCarouselTooltip3", "FFlagLuaAppSponsoredGridTiles" });
                }

                if (!IsToggleOn("FFSBlockTencentToggle"))
                {
                    flagsToRemove.AddRange(new[] { "FStringTencentAuthPath", "DFFlagPolicyServiceReportIsNotSubjectToChinaPolicies" });
                }

                if (!IsToggleOn("FFSNoGuiBlurToggle"))
                {
                    flagsToRemove.Add("FIntRobloxGuiBlurIntensity");
                }

                if (!IsToggleOn("FFSUnlimitedZoomToggle"))
                {
                    flagsToRemove.Add("FIntCameraMaxZoomDistance");
                }

                if (!IsToggleOn("FFSCustomDisconnectToggle"))
                {
                    flagsToRemove.AddRange(new[] { "FFlagReconnectDisabled", "FStringReconnectDisabledReason" });
                }

                if (flagsToRemove.Count > 0)
                {
                    int removedCount = this._fflagService.RemoveFlagsByNames(flagsToRemove);
                    if (removedCount > 0)
                    {
                        this.Log($"[FFSettings] ✓ Cleared {removedCount} stale/default FFlags from ClientAppSettings.json");
                    }
                }

                var stringPresets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in presets)
                {
                    if (kvp.Value != null)
                    {
                        stringPresets[kvp.Key] = kvp.Value.ToString();
                    }
                }

                if (stringPresets.Count > 0)
                {
                    this._fflagService.UpdateFlags(stringPresets);
                }

                if (persistPayloadToDisk)
                {
                    string jsonPayload = JsonSerializer.Serialize(stringPresets);
                    this._settingsManager?.SetFastFlagSettingsPayload(jsonPayload);
                    this.Log($"[FFSettings] ✓ Applied {stringPresets.Count} active presets to ClientAppSettings writer (and saved payload)");
                }
                else
                {
                    this.Log($"[FFSettings] ✓ Applied {stringPresets.Count} active presets to ClientAppSettings writer (disk unchanged)");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[FFSettings] Error applying presets: {ex.Message}");
            }
        }

        // ── Save/Restore toggle states via SettingsManager ─────────
        internal void SaveFastFlagSettingsState()
        {
            try
            {
                var state = new Dictionary<string, string>();
                string[] toggleNames = {
                    "FFSHyperThreadingToggle",
                    "FFSDisableShadowsToggle", "FFSDisablePostFxToggle", "FFSDisableTerrainToggle",
                    "FFSLessLagToggle", "FFSNoPayloadLimitToggle", "FFSLargeReplicatorToggle", "FFSBetterPacketToggle",
                    "FFSDisableTelemetryToggle", "FFSDisableAdsToggle", "FFSBlockTencentToggle",
                    "FFSNoGuiBlurToggle", "FFSUnlimitedZoomToggle", "FFSCustomDisconnectToggle",
                    "FFSPingBoostToggle",
                    "FFSDscpQosToggle"
                };
                foreach (var name in toggleNames)
                {
                    if (this.FindName(name) is ToggleButton tb)
                        state[name] = (tb.IsChecked ?? false).ToString();
                }

                string[] comboNames = { "FFSForceLogicalProcessorsCombo", "FFSForceAsyncThreadsCombo", "FFSRobloxResolutionCombo", "FFSDynamicResCombo",
                                        "FFSLightingCombo", "FFSRenderApiCombo" };
                foreach (var name in comboNames)
                {
                    if (this.FindName(name) is ComboBox cb)
                        state[name] = cb.SelectedIndex.ToString();
                }

                var tb2 = this.FindName("FFSCustomDisconnectText") as TextBox;
                if (tb2 != null) state["FFSCustomDisconnectText"] = tb2.Text ?? "";

                string json = JsonSerializer.Serialize(state);
                this._settingsManager?.SetFastFlagSettingsState(json);
                this.Log("[FFSettings] ✓ Toggle states saved");
            }
            catch (Exception ex)
            {
                this.Log($"[FFSettings] Save state error: {ex.Message}");
            }
        }

        internal void RestoreFastFlagSettingsState()
        {
            try
            {
                string json = this._settingsManager?.GetFastFlagSettingsState();
                if (string.IsNullOrEmpty(json)) return;

                var state = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (state == null) return;

                foreach (var kvp in state)
                {
                    if (kvp.Key.EndsWith("Toggle") && this.FindName(kvp.Key) is ToggleButton tb)
                    {
                        tb.IsChecked = kvp.Value == "True";
                        if (kvp.Key == "FFSPingBoostToggle")
                        {
                            this._settingsManager?.SetProxyPingBoostEnabled(tb.IsChecked == true);
                            if (tb.IsChecked == true)
                            {
                                this.StartRobloxProcessWatcher();
                            }
                        }
                        if (kvp.Key == "FFSDscpQosToggle")
                        {
                            if (tb.IsChecked == true)
                            {
                                this.StartRobloxProcessWatcher();
                            }
                        }
                    }
                    else if (kvp.Key.EndsWith("Combo") && this.FindName(kvp.Key) is ComboBox cb)
                    {
                        if (int.TryParse(kvp.Value, out int idx) && idx >= 0 && idx < cb.Items.Count)
                            cb.SelectedIndex = idx;
                    }
                    else if (kvp.Key == "FFSCustomDisconnectText" && this.FindName(kvp.Key) is TextBox txb)
                    {
                        txb.Text = kvp.Value;
                    }
                }
                this.Log("[FFSettings] ✓ Toggle states restored");
            }
            catch (Exception ex)
            {
                this.Log($"[FFSettings] Restore state error: {ex.Message}");
            }
        }

        // ── Helpers ────────────────────────────────────────────────
        private bool IsToggleOn(string name)
        {
            return this.FindName(name) is ToggleButton tb && tb.IsChecked == true;
        }

        private static string GetComboTag(ComboBox cb, string defaultVal)
        {
            if (cb?.SelectedItem is ComboBoxItem item)
                return item.Tag?.ToString() ?? defaultVal;
            return defaultVal;
        }

        private void HandlePingBoostToggleChanged(bool enabled)
        {
            this.Log(enabled
                ? "[FFSettings] Ping boost (ClientSettings build — network priority not applied on disk)."
                : "[FFSettings] Ping boost disabled.");
        }

        private void HandleDscpQosToggleChanged(bool enabled)
        {
            if (enabled)
                this.StartRobloxProcessWatcher();
            this.Log(enabled
                ? "[FFSettings] DSCP QoS (ClientSettings build — QoS not applied on disk)."
                : "[FFSettings] DSCP QoS disabled.");
        }

        private void StartRobloxProcessWatcher()
        {
        }
    }
}
