using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Masterstrap.Services
{
    public class ActivityHistory
    {
        private ObservableCollection<ActivityRecord> _history = new ObservableCollection<ActivityRecord>();

        public ObservableCollection<ActivityRecord> History => _history;

        public int TotalJsonLoaded { get; private set; } = 0;
        public int TotalFlagsEdited { get; private set; } = 0;
        public int TotalFlagApplyAttempts { get; private set; } = 0;
        public int TotalFlagApplySuccess { get; private set; } = 0;
        public int TotalFlagsApplied { get; private set; } = 0;

        public string RobloxVersion { get; set; } = "unknown";
        public string SoftwareVersion { get; set; } = "unknown";

        public void LogVersionInfo(string robloxVer, string softwareVer)
        {
            RobloxVersion = robloxVer;
            SoftwareVersion = softwareVer;
            AddRecord(ActivityType.Version, $"Roblox: {robloxVer} | Software: {softwareVer}");
        }

        public void LogJsonLoaded(string fileName)
        {
            TotalJsonLoaded++;
            AddRecord(ActivityType.JsonLoaded, $"Loaded: {fileName}");
        }

        public void LogFlagEdited(string flagName, string value)
        {
            TotalFlagsEdited++;
            AddRecord(ActivityType.FlagEdited, $"Edited: {flagName} = {value}");
        }

        public void LogFlagApplyStart(int flagCount)
        {
            TotalFlagApplyAttempts++;
            AddRecord(ActivityType.FlagApplyStart, $"Starting application of {flagCount} FFlags...");
        }

        public void LogFlagApplySuccess(int flagCount)
        {
            TotalFlagApplySuccess++;
            TotalFlagsApplied += flagCount;
            AddRecord(ActivityType.FlagApplySuccess, $"✅ Successfully applied {flagCount} FFlags");
        }

        public void LogFlagApplyFailed(string reason)
        {
            AddRecord(ActivityType.FlagApplyFailed, $"❌ Application failed: {reason}");
        }

        public void AddRecord(ActivityType type, string message)
        {
            var record = new ActivityRecord
            {
                Timestamp = DateTime.Now,
                Type = type,
                Message = message
            };

            _history.Insert(0, record);

            while (_history.Count > 50)
                _history.RemoveAt(_history.Count - 1);
        }

        public string GetSummary()
        {
            return $@"=== ACTIVITY SUMMARY ===
Roblox Version: {RobloxVersion}
Software Version: {SoftwareVersion}

JSON Files Loaded: {TotalJsonLoaded}
FFlags Edited: {TotalFlagsEdited}
Apply Attempts: {TotalFlagApplyAttempts}
Apply Success: {TotalFlagApplySuccess}/{TotalFlagApplyAttempts}
Total FFlags Applied: {TotalFlagsApplied}

Success Rate: {(TotalFlagApplyAttempts > 0 ? (TotalFlagApplySuccess * 100 / TotalFlagApplyAttempts) : 0)}%";
        }

        public void Clear()
        {
            _history.Clear();
            TotalJsonLoaded = 0;
            TotalFlagsEdited = 0;
            TotalFlagApplyAttempts = 0;
            TotalFlagApplySuccess = 0;
            TotalFlagsApplied = 0;
        }
    }

    public class ActivityRecord
    {
        public DateTime Timestamp { get; set; }
        public ActivityType Type { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] [{Type}] {Message}";
        }
    }

    public enum ActivityType
    {
        Version,
        JsonLoaded,
        FlagEdited,
        FlagApplyStart,
        FlagApplySuccess,
        FlagApplyFailed,
        System
    }
}
