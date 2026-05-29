using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Masterstrap.Services
{
    /// <summary>
    /// Applies FastFlag Settings tab presets from persisted <c>fastFlagSettingsState</c>
    /// without requiring MainWindow UI controls (desktop shortcut / QuickLaunch apply path).
    /// </summary>
    internal static class FastFlagSettingsSavedApplicator
    {
        private static readonly string[] ForceLogicalProcessorsTags =
        {
            "Automatic", "1", "2", "3", "4", "6", "8", "10", "12", "16", "32", "64"
        };

        private static readonly string[] ForceAsyncThreadsTags =
        {
            "Automatic", "1", "2", "3", "4", "6", "8", "12", "16"
        };

        private static readonly string[] DynamicResTags =
        {
            "Default", "240p", "360p", "480p", "720p", "1080p", "2K", "4K"
        };

        private static readonly string[] LightingTags =
        {
            "Default", "Voxel", "ShadowMap", "Future", "Unified"
        };

        private static readonly string[] RenderApiTags =
        {
            "Auto", "D3D11", "Vulkan", "OpenGL"
        };

        public static bool TryApplyToService(FFlagService fflagService, out int appliedCount, out int removedCount)
        {
            appliedCount = 0;
            removedCount = 0;
            if (fflagService == null)
                return false;

            try
            {
                var settings = new AppSettingsManager();
                string stateJson = settings.GetFastFlagSettingsState();
                Dictionary<string, string>? state = null;
                if (!string.IsNullOrWhiteSpace(stateJson))
                {
                    state = JsonSerializer.Deserialize<Dictionary<string, string>>(stateJson);
                }

                if (state == null || state.Count == 0)
                {
                    return TryApplyPayloadFallback(fflagService, settings, out appliedCount);
                }

                var presets = BuildPresetsFromState(state);
                var flagsToRemove = BuildRemovalsFromState(state);

                if (flagsToRemove.Count > 0)
                    removedCount = fflagService.RemoveFlagsByNames(flagsToRemove);

                if (presets.Count > 0)
                {
                    var stringPresets = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var kvp in presets)
                    {
                        if (kvp.Value != null)
                            stringPresets[kvp.Key] = kvp.Value.ToString() ?? "";
                    }

                    if (stringPresets.Count > 0)
                    {
                        fflagService.UpdateFlags(stringPresets);
                        appliedCount = stringPresets.Count;
                    }
                }

                return appliedCount > 0 || removedCount > 0 || state.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FastFlagSettingsSavedApplicator] Apply failed: " + ex.Message);
                return false;
            }
        }

        public static void TryApplySavedNetworkBoost(FFlagService? fflagService, int robloxPid)
        {
            if (fflagService == null || robloxPid <= 0)
                return;

            try
            {
                string stateJson = new AppSettingsManager().GetFastFlagSettingsState();
                if (string.IsNullOrWhiteSpace(stateJson))
                    return;

                var state = JsonSerializer.Deserialize<Dictionary<string, string>>(stateJson);
                if (state == null)
                    return;

                if (IsToggleOn(state, "FFSPingBoostToggle"))
                    fflagService.ApplyGodNetworkPriority(robloxPid);

                if (IsToggleOn(state, "FFSDscpQosToggle"))
                    fflagService.ApplyQosOptimization();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FastFlagSettingsSavedApplicator] Network boost apply failed: " + ex.Message);
            }
        }

        private static bool TryApplyPayloadFallback(FFlagService fflagService, AppSettingsManager settings, out int appliedCount)
        {
            appliedCount = 0;
            try
            {
                string jsonPayload = settings.GetFastFlagSettingsPayload();
                if (string.IsNullOrWhiteSpace(jsonPayload))
                    return false;

                var presets = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonPayload);
                if (presets == null || presets.Count == 0)
                    return false;

                fflagService.UpdateFlags(presets);
                appliedCount = presets.Count;
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static Dictionary<string, object> BuildPresetsFromState(Dictionary<string, string> state)
        {
            var f = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            string cpuTag = GetComboTag(state, "FFSForceLogicalProcessorsCombo", "Automatic", ForceLogicalProcessorsTags);
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

            string asyncTag = GetComboTag(state, "FFSForceAsyncThreadsCombo", "Automatic", ForceAsyncThreadsTags);
            if (asyncTag != "Automatic" && int.TryParse(asyncTag, out int asyncCores))
            {
                int adjustedAsync = Math.Max(asyncCores - 1, 1);
                f["FIntTaskSchedulerAsyncTasksMinimumThreadCount"] = adjustedAsync;
            }

            if (IsToggleOn(state, "FFSHyperThreadingToggle"))
            {
                f["FFlagDebugCheckRenderThreading"] = "True";
                f["FFlagRenderDebugCheckThreading2"] = "True";
            }

            string dr = GetComboTag(state, "FFSDynamicResCombo", "Default", DynamicResTags);
            if (dr != "Default")
            {
                string? kp = dr switch
                {
                    "240p" => "30",
                    "360p" => "77",
                    "480p" => "230",
                    "720p" => "410",
                    "1080p" => "922",
                    "2K" => "2074",
                    "4K" => "3686",
                    _ => null
                };
                if (kp != null)
                    f["DFIntDebugDynamicRenderKiloPixels"] = kp;
            }

            string lt = GetComboTag(state, "FFSLightingCombo", "Default", LightingTags);
            switch (lt)
            {
                case "Voxel": f["DFFlagDebugRenderForceTechnologyVoxel"] = "True"; break;
                case "ShadowMap": f["FFlagDebugForceFutureIsBrightPhase2"] = "True"; break;
                case "Future": f["FFlagDebugForceFutureIsBrightPhase3"] = "True"; break;
                case "Unified": f["FFlagRenderUnifiedLighting14"] = "True"; break;
            }

            string ra = GetComboTag(state, "FFSRenderApiCombo", "Auto", RenderApiTags);
            switch (ra)
            {
                case "D3D11":
                    f["FFlagDebugGraphicsPreferD3D11"] = "True";
                    break;
                case "Vulkan":
                    f["FFlagDebugGraphicsPreferVulkan"] = "True";
                    f["FFlagDebugGraphicsDisableDirect3D11"] = "True";
                    break;
                case "OpenGL":
                    f["FFlagDebugGraphicsPreferOpenGL"] = "True";
                    f["FFlagDebugGraphicsDisableDirect3D11"] = "True";
                    break;
            }

            if (IsToggleOn(state, "FFSDisableShadowsToggle"))
            {
                f["FIntRenderShadowIntensity"] = 0;
                f["DFFlagDebugPauseVoxelizer"] = "True";
                f["FIntRenderShadowmapBias"] = -1;
            }

            if (IsToggleOn(state, "FFSDisablePostFxToggle"))
                f["FFlagDisablePostFx"] = "True";

            if (IsToggleOn(state, "FFSDisableTerrainToggle"))
                f["FIntTerrainArraySliceSize"] = 0;

            if (IsToggleOn(state, "FFSLessLagToggle"))
            {
                f["DFIntBandwidthManagerApplicationDefaultBps"] = 796850000;
                f["DFIntBandwidthManagerDataSenderMaxWorkCatchupMs"] = 5;
            }

            if (IsToggleOn(state, "FFSNoPayloadLimitToggle"))
            {
                string[] payloadFlags =
                {
                    "DFIntRccMaxPayloadSnd", "DFIntCliMaxPayloadRcv",
                    "DFIntCliMaxPayloadSnd", "DFIntRccMaxPayloadRcv",
                    "DFIntCliTcMaxPayloadRcv", "DFIntRccTcMaxPayloadRcv",
                    "DFIntCliTcMaxPayloadSnd", "DFIntRccTcMaxPayloadSnd",
                    "DFIntMaxDataPayloadSize", "DFIntMaxUREPayloadSingleLimit",
                    "DFIntTotalRepPayloadLimit"
                };
                foreach (var pf in payloadFlags)
                    f[pf] = 2147483647;
            }

            if (IsToggleOn(state, "FFSLargeReplicatorToggle"))
            {
                f["FFlagLargeReplicatorEnabled7"] = "True";
                f["FFlagLargeReplicatorWrite5"] = "True";
                f["FFlagLargeReplicatorRead5"] = "True";
                f["FFlagLargeReplicatorSerializeRead3"] = "True";
                f["FFlagLargeReplicatorSerializeWrite3"] = "True";
            }

            if (IsToggleOn(state, "FFSBetterPacketToggle"))
            {
                f["DFIntNetworkStopProducingPacketsToProcessThresholdMs"] = 0;
                f["DFIntMaxWaitTimeBeforeForcePacketProcessMS"] = 0;
                f["DFIntClientPacketMaxDelayMs"] = 1;
            }

            if (IsToggleOn(state, "FFSDisableTelemetryToggle"))
            {
                f["DFStringTelemetryV2Url"] = "0.0.0.0";
                string[] telFlags =
                {
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
                foreach (var tf in telFlags)
                    f[tf] = "False";
                f["FLogRobloxTelemetry"] = 0;
            }

            if (IsToggleOn(state, "FFSDisableAdsToggle"))
            {
                f["FFlagAdServiceEnabled"] = "False";
                f["FFlagEnableSponsoredAdsGameCarouselTooltip3"] = "False";
                f["FFlagLuaAppSponsoredGridTiles"] = "False";
            }

            if (IsToggleOn(state, "FFSBlockTencentToggle"))
            {
                f["FStringTencentAuthPath"] = "/tencent/";
                f["DFFlagPolicyServiceReportIsNotSubjectToChinaPolicies"] = "True";
            }

            if (IsToggleOn(state, "FFSNoGuiBlurToggle"))
                f["FIntRobloxGuiBlurIntensity"] = 0;

            if (IsToggleOn(state, "FFSUnlimitedZoomToggle"))
                f["FIntCameraMaxZoomDistance"] = 2147483647;

            if (IsToggleOn(state, "FFSCustomDisconnectToggle"))
            {
                f["FFlagReconnectDisabled"] = "True";
                string msg = "Disconnected by Masterstrap";
                if (state.TryGetValue("FFSCustomDisconnectText", out string? savedMsg) && !string.IsNullOrWhiteSpace(savedMsg))
                    msg = savedMsg;
                f["FStringReconnectDisabledReason"] = msg;
            }

            return f;
        }

        internal static List<string> BuildRemovalsFromState(Dictionary<string, string> state)
        {
            var flagsToRemove = new List<string>();

            string cpuTag = GetComboTag(state, "FFSForceLogicalProcessorsCombo", "Automatic", ForceLogicalProcessorsTags);
            if (cpuTag == "Automatic")
            {
                flagsToRemove.AddRange(new[]
                {
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

            string asyncTag = GetComboTag(state, "FFSForceAsyncThreadsCombo", "Automatic", ForceAsyncThreadsTags);
            if (asyncTag == "Automatic")
                flagsToRemove.Add("FIntTaskSchedulerAsyncTasksMinimumThreadCount");

            string dr = GetComboTag(state, "FFSDynamicResCombo", "Default", DynamicResTags);
            if (dr == "Default")
                flagsToRemove.Add("DFIntDebugDynamicRenderKiloPixels");

            string lt = GetComboTag(state, "FFSLightingCombo", "Default", LightingTags);
            if (lt == "Default")
            {
                flagsToRemove.AddRange(new[]
                {
                    "DFFlagDebugRenderForceTechnologyVoxel",
                    "FFlagDebugForceFutureIsBrightPhase2",
                    "FFlagDebugForceFutureIsBrightPhase3",
                    "FFlagRenderUnifiedLighting14"
                });
            }

            string ra = GetComboTag(state, "FFSRenderApiCombo", "Auto", RenderApiTags);
            if (ra == "Auto")
            {
                flagsToRemove.AddRange(new[]
                {
                    "FFlagDebugGraphicsPreferD3D11",
                    "FFlagDebugGraphicsPreferVulkan",
                    "FFlagDebugGraphicsPreferOpenGL",
                    "FFlagDebugGraphicsDisableDirect3D11"
                });
            }

            if (!IsToggleOn(state, "FFSHyperThreadingToggle"))
            {
                flagsToRemove.AddRange(new[] { "FFlagDebugCheckRenderThreading", "FFlagRenderDebugCheckThreading2" });
            }

            if (!IsToggleOn(state, "FFSDisableShadowsToggle"))
            {
                flagsToRemove.AddRange(new[] { "FIntRenderShadowIntensity", "DFFlagDebugPauseVoxelizer", "FIntRenderShadowmapBias" });
            }

            if (!IsToggleOn(state, "FFSDisablePostFxToggle"))
                flagsToRemove.Add("FFlagDisablePostFx");

            if (!IsToggleOn(state, "FFSDisableTerrainToggle"))
                flagsToRemove.Add("FIntTerrainArraySliceSize");

            if (!IsToggleOn(state, "FFSLessLagToggle"))
            {
                flagsToRemove.AddRange(new[] { "DFIntBandwidthManagerApplicationDefaultBps", "DFIntBandwidthManagerDataSenderMaxWorkCatchupMs" });
            }

            if (!IsToggleOn(state, "FFSNoPayloadLimitToggle"))
            {
                flagsToRemove.AddRange(new[]
                {
                    "DFIntRccMaxPayloadSnd", "DFIntCliMaxPayloadRcv", "DFIntCliMaxPayloadSnd", "DFIntRccMaxPayloadRcv",
                    "DFIntCliTcMaxPayloadRcv", "DFIntRccTcMaxPayloadRcv", "DFIntCliTcMaxPayloadSnd", "DFIntRccTcMaxPayloadSnd",
                    "DFIntMaxDataPayloadSize", "DFIntMaxUREPayloadSingleLimit", "DFIntTotalRepPayloadLimit"
                });
            }

            if (!IsToggleOn(state, "FFSLargeReplicatorToggle"))
            {
                flagsToRemove.AddRange(new[]
                {
                    "FFlagLargeReplicatorEnabled7", "FFlagLargeReplicatorWrite5", "FFlagLargeReplicatorRead5",
                    "FFlagLargeReplicatorSerializeRead3", "FFlagLargeReplicatorSerializeWrite3"
                });
            }

            if (!IsToggleOn(state, "FFSBetterPacketToggle"))
            {
                flagsToRemove.AddRange(new[]
                {
                    "DFIntNetworkStopProducingPacketsToProcessThresholdMs", "DFIntMaxWaitTimeBeforeForcePacketProcessMS", "DFIntClientPacketMaxDelayMs"
                });
            }

            if (!IsToggleOn(state, "FFSDisableTelemetryToggle"))
            {
                flagsToRemove.Add("DFStringTelemetryV2Url");
                flagsToRemove.AddRange(new[]
                {
                    "FFlagEnableTelemetryProtocol", "DFFlagGraphicsQualityUsageTelemetry", "DFFlagGpuVsCpuBoundTelemetry", "DFFlagSendRenderFidelityTelemetry",
                    "DFFlagReportRenderDistanceTelemetry", "DFFlagCollectAudioPluginTelemetry", "DFFlagEnableFmodErrorsTelemetry", "DFFlagRccLoadSoundLengthTelemetryEnabled",
                    "DFFlagReportAssetRequestV1Telemetry", "DFFlagRobloxTelemetryAddDeviceRAMPointsV2", "DFFlagEnableTelemetryV2FRMStats", "DFFlagEnableSkipUpdatingGlobalTelemetryInfo2",
                    "DFFlagEmitSafetyTelemetryInCallbackEnable", "DFFlagRobloxTelemetryV2PointEncoding", "DFFlagDSTelemetryV2ReplaceSeparator", "FFlagEnableTelemetryService1",
                    "FFlagPropertiesEnableTelemetry", "FFlagOpenTelemetryEnabled", "FLogRobloxTelemetry"
                });
            }

            if (!IsToggleOn(state, "FFSDisableAdsToggle"))
            {
                flagsToRemove.AddRange(new[] { "FFlagAdServiceEnabled", "FFlagEnableSponsoredAdsGameCarouselTooltip3", "FFlagLuaAppSponsoredGridTiles" });
            }

            if (!IsToggleOn(state, "FFSBlockTencentToggle"))
            {
                flagsToRemove.AddRange(new[] { "FStringTencentAuthPath", "DFFlagPolicyServiceReportIsNotSubjectToChinaPolicies" });
            }

            if (!IsToggleOn(state, "FFSNoGuiBlurToggle"))
                flagsToRemove.Add("FIntRobloxGuiBlurIntensity");

            if (!IsToggleOn(state, "FFSUnlimitedZoomToggle"))
                flagsToRemove.Add("FIntCameraMaxZoomDistance");

            if (!IsToggleOn(state, "FFSCustomDisconnectToggle"))
            {
                flagsToRemove.AddRange(new[] { "FFlagReconnectDisabled", "FStringReconnectDisabledReason" });
            }

            return flagsToRemove;
        }

        private static bool IsToggleOn(Dictionary<string, string> state, string name)
        {
            return state.TryGetValue(name, out string? v) &&
                   string.Equals(v, "True", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetComboTag(Dictionary<string, string> state, string comboName, string defaultTag, string[] tags)
        {
            if (!state.TryGetValue(comboName, out string? idxStr) || !int.TryParse(idxStr, out int idx))
                return defaultTag;
            if (idx < 0 || idx >= tags.Length)
                return defaultTag;
            return tags[idx];
        }
    }
}
