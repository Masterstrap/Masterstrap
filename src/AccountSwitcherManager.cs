using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Masterstrap.Services
{
    public class AccountProfile
    {
        public string AccountName { get; set; }
        public string Cookie { get; set; }
        public string CookieBackupFileName { get; set; }

        public bool CookiesDatBackupTrusted { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string AvatarUrl { get; set; }
        public int FriendCount { get; set; }
        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsOnline { get; set; }
    }

    public static class AccountSwitcherManager
    {
        private static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Masterstrap");
        private static string AccountsFilePath => Path.Combine(AppDataPath, "Accounts.json");
        private static string CookieBackupsPath => Path.Combine(AppDataPath, "CookieBackups");
        private static string RobloxCookieDatPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "LocalStorage", "RobloxCookies.dat");

        public static void EnsureDirectories()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);
            if (!Directory.Exists(CookieBackupsPath))
                Directory.CreateDirectory(CookieBackupsPath);
        }

        public static List<AccountProfile> LoadProfiles()
        {
            EnsureDirectories();
            if (!File.Exists(AccountsFilePath)) return new List<AccountProfile>();
            try
            {
                var json = File.ReadAllText(AccountsFilePath);
                return JsonSerializer.Deserialize<List<AccountProfile>>(json) ?? new List<AccountProfile>();
            }
            catch { return new List<AccountProfile>(); }
        }

        public static void SaveProfiles(List<AccountProfile> profiles)
        {
            EnsureDirectories();
            var json = JsonSerializer.Serialize(profiles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AccountsFilePath, json);
        }

        public static List<string> GetSavedAccounts()
        {
            return LoadProfiles().Select(p => p.AccountName).ToList();
        }

        public static void SaveSessionCookie(string accountName, string cookie)
        {
            var profiles = LoadProfiles();
            var existing = profiles.FirstOrDefault(p => p.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Cookie = cookie;
                if (string.IsNullOrWhiteSpace(existing.CookieBackupFileName))
                    existing.CookieBackupFileName = BuildBackupFileName(accountName);
            }
            else
            {
                existing = new AccountProfile
                {
                    AccountName = accountName,
                    Cookie = cookie,
                    CookieBackupFileName = BuildBackupFileName(accountName)
                };
                profiles.Add(existing);
            }

            existing.CookiesDatBackupTrusted = false;
            TryDeleteFile(GetBackupPath(existing));

            SaveProfiles(profiles);
        }

        public static bool TryCaptureTrustedRobloxCookiesDat(string accountName, out string error)
        {
            error = null;
            try
            {
                if (string.IsNullOrWhiteSpace(accountName))
                {
                    error = "Invalid account.";
                    return false;
                }

                var profiles = LoadProfiles();
                var existing = profiles.FirstOrDefault(p => p.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
                if (existing == null)
                {
                    error = "Account not found.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(existing.CookieBackupFileName))
                    existing.CookieBackupFileName = BuildBackupFileName(accountName);

                if (!File.Exists(RobloxCookieDatPath))
                {
                    error = "RobloxCookies.dat not found. Log into the Roblox app on this PC first.";
                    return false;
                }

                EnsureRobloxClosed();
                CopyWithRetry(RobloxCookieDatPath, GetBackupPath(existing), overwrite: true);
                existing.CookiesDatBackupTrusted = true;
                SaveProfiles(profiles);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static string GetSessionCookie(string accountName)
        {
            var profiles = LoadProfiles();
            return profiles.FirstOrDefault(p => p.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase))?.Cookie;
        }

        public static async Task<bool> RefreshProfileAsync(AccountProfile profile)
        {
            if (string.IsNullOrEmpty(profile.Cookie)) return false;

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Cookie", $".ROBLOSECURITY={profile.Cookie}");

                var response = await client.GetAsync("https://users.roblox.com/v1/users/authenticated");
                if (!response.IsSuccessStatusCode) return false;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var userId = doc.RootElement.GetProperty("id").GetInt64().ToString();
                profile.UserId = userId;
                profile.Username = doc.RootElement.GetProperty("name").GetString();
                profile.DisplayName = doc.RootElement.GetProperty("displayName").GetString();

                var thumbResponse = await client.GetAsync($"https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds={userId}&size=150x150&format=Png&isCircular=true");
                if (thumbResponse.IsSuccessStatusCode)
                {
                    var thumbJson = await thumbResponse.Content.ReadAsStringAsync();
                    using var thumbDoc = JsonDocument.Parse(thumbJson);
                    profile.AvatarUrl = thumbDoc.RootElement.GetProperty("data")[0].GetProperty("imageUrl").GetString();
                }

                var friendsResponse = await client.GetAsync($"https://friends.roblox.com/v1/users/{userId}/friends/count");
                if (friendsResponse.IsSuccessStatusCode)
                {
                    var friendsJson = await friendsResponse.Content.ReadAsStringAsync();
                    using var friendsDoc = JsonDocument.Parse(friendsJson);
                    profile.FriendCount = friendsDoc.RootElement.GetProperty("count").GetInt32();
                }

                var followersResponse = await client.GetAsync($"https://friends.roblox.com/v1/users/{userId}/followers/count");
                if (followersResponse.IsSuccessStatusCode)
                {
                    var followersJson = await followersResponse.Content.ReadAsStringAsync();
                    using var followersDoc = JsonDocument.Parse(followersJson);
                    profile.FollowerCount = followersDoc.RootElement.GetProperty("count").GetInt32();
                }

                var followingResponse = await client.GetAsync($"https://friends.roblox.com/v1/users/{userId}/followings/count");
                if (followingResponse.IsSuccessStatusCode)
                {
                    var followingJson = await followingResponse.Content.ReadAsStringAsync();
                    using var followingDoc = JsonDocument.Parse(followingJson);
                    profile.FollowingCount = followingDoc.RootElement.GetProperty("count").GetInt32();
                }

                profile.IsOnline = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void RenameAccount(string oldName, string newName)
        {
            var profiles = LoadProfiles();
            var existing = profiles.FirstOrDefault(p => p.AccountName.Equals(oldName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                string oldBackup = GetBackupPath(existing);
                existing.AccountName = newName;
                if (string.IsNullOrWhiteSpace(existing.CookieBackupFileName))
                    existing.CookieBackupFileName = BuildBackupFileName(newName);
                string newBackup = GetBackupPath(existing);
                TryMoveFile(oldBackup, newBackup);
                SaveProfiles(profiles);
            }
        }

        public static void DeleteAccount(string accountName)
        {
            var profiles = LoadProfiles();
            var target = profiles.FirstOrDefault(p => p.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
            if (target != null)
            {
                var backupPath = GetBackupPath(target);
                TryDeleteFile(backupPath);
                profiles.Remove(target);
            }
            SaveProfiles(profiles);
        }

        public static bool TryRestoreCookieFileForAccount(string accountName, out string reason)
        {
            reason = string.Empty;
            try
            {
                var profiles = LoadProfiles();
                var profile = profiles.FirstOrDefault(p => p.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
                if (profile == null)
                {
                    reason = "Account not found.";
                    return false;
                }

                if (!profile.CookiesDatBackupTrusted)
                {
                    reason = "This account has no trusted RobloxCookies.dat session yet. Capture session from Roblox app before launch.";
                    return false;
                }

                var backupPath = GetBackupPath(profile);
                if (!File.Exists(backupPath))
                {
                    reason = "No local cookie backup for this account.";
                    return false;
                }

                EnsureRobloxClosed();
                BackupCurrentCookieForSafety();
                CopyWithRetry(backupPath, RobloxCookieDatPath, overwrite: true);
                reason = "Restored RobloxCookies.dat from local account backup.";
                return true;
            }
            catch (Exception ex)
            {
                reason = ex.Message;
                return false;
            }
        }

        public static async Task<string> GenerateAuthTicketAsync(string cookie)
        {
            const string url = "https://auth.roblox.com/v1/authentication-ticket";
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var handler = new SocketsHttpHandler
                {
                    UseProxy = false,
                    AutomaticDecompression = DecompressionMethods.All,
                    ConnectTimeout = TimeSpan.FromSeconds(30),
                };
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
                client.DefaultRequestHeaders.Add("Cookie", $".ROBLOSECURITY={cookie}");
                client.DefaultRequestHeaders.Add("Referer", "https://www.roblox.com/");

                try
                {
                    using var content1 = new StringContent("", Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content1).ConfigureAwait(false);

                    string csrfToken = "";
                    if (response.Headers.TryGetValues("x-csrf-token", out var csrfValues))
                        csrfToken = csrfValues.FirstOrDefault() ?? "";

                    if (string.IsNullOrEmpty(csrfToken))
                        return null;

                    using var client2 = new HttpClient(new SocketsHttpHandler
                    {
                        UseProxy = false,
                        AutomaticDecompression = DecompressionMethods.All,
                        ConnectTimeout = TimeSpan.FromSeconds(30),
                    })
                    {
                        Timeout = TimeSpan.FromSeconds(60),
                    };
                    client2.DefaultRequestHeaders.Add("Cookie", $".ROBLOSECURITY={cookie}");
                    client2.DefaultRequestHeaders.Add("Referer", "https://www.roblox.com/");
                    client2.DefaultRequestHeaders.Add("X-CSRF-TOKEN", csrfToken);

                    using var content2 = new StringContent("", Encoding.UTF8, "application/json");
                    var response2 = await client2.PostAsync(url, content2).ConfigureAwait(false);
                    if (response2.IsSuccessStatusCode &&
                        response2.Headers.TryGetValues("rbx-authentication-ticket", out var ticketValues))
                    {
                        return ticketValues.FirstOrDefault();
                    }

                    return null;
                }
                catch (Exception) when (attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt)).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

        public static async Task<(bool usedCookiesDatRestore, string authenticationTicket, string detailMessage)> PrepareLaunchForAccountAsync(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
                return (false, null, "");

            try
            {
                EnsureRobloxClosed();
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }

            bool restored = TryRestoreCookieFileForAccount(accountName, out string restoreReason);
            if (restored)
                return (true, null, restoreReason);

            string cookie = GetSessionCookie(accountName);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                string ticket = await GenerateAuthTicketAsync(cookie).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(ticket))
                {
                    string ticketSessionDetail = PrepareLocalSessionForAuthTicketLaunch();
                    string detail = string.IsNullOrWhiteSpace(restoreReason)
                        ? "Auth ticket ready from saved cookie."
                        : restoreReason.Trim() + " Using auth ticket from saved cookie.";
                    if (!string.IsNullOrWhiteSpace(ticketSessionDetail))
                        detail = detail + " " + ticketSessionDetail;
                    return (false, ticket, detail);
                }

                string failDetail = string.IsNullOrWhiteSpace(restoreReason)
                    ? "Could not obtain auth ticket from saved cookie."
                    : restoreReason.Trim() + " Auth ticket request failed.";
                return (false, null, failDetail);
            }

            string tail = string.IsNullOrWhiteSpace(restoreReason)
                ? "No saved cookie for this account."
                : restoreReason;
            return (false, null, tail);
        }

        private static string PrepareLocalSessionForAuthTicketLaunch()
        {
            try
            {
                if (!File.Exists(RobloxCookieDatPath))
                    return "No local RobloxCookies.dat found before ticket launch.";

                BackupCurrentCookieForSafety();
                TryDeleteFile(RobloxCookieDatPath);
                return "Cleared local RobloxCookies.dat before ticket launch.";
            }
            catch
            {
                return "Could not clear local RobloxCookies.dat before ticket launch.";
            }
        }

        private static string BuildBackupFileName(string accountName)
        {
            string safe = new string((accountName ?? "account")
                .Select(ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch)
                .ToArray())
                .Trim();
            if (string.IsNullOrWhiteSpace(safe))
                safe = "account";
            return $"{safe}.dat";
        }

        private static string GetBackupPath(AccountProfile profile)
        {
            EnsureDirectories();
            var name = profile?.CookieBackupFileName;
            if (string.IsNullOrWhiteSpace(name))
                name = BuildBackupFileName(profile?.AccountName ?? "account");
            return Path.Combine(CookieBackupsPath, name);
        }

        private static void BackupCurrentCookieForSafety()
        {
            try
            {
                if (!File.Exists(RobloxCookieDatPath))
                    return;
                EnsureDirectories();
                string file = Path.Combine(CookieBackupsPath, $"_auto_before_switch_{DateTime.Now:yyyyMMdd_HHmmss}.dat");
                CopyWithRetry(RobloxCookieDatPath, file, overwrite: false);
            }
            catch
            {
            }
        }

        private static void EnsureRobloxClosed()
        {
            if (System.Diagnostics.Process.GetProcessesByName("RobloxPlayerBeta").Length > 0 ||
                System.Diagnostics.Process.GetProcessesByName("RobloxPlayerLauncher").Length > 0)
            {
                throw new InvalidOperationException("Please close Roblox before switching account.");
            }
        }

        private static void CopyWithRetry(string src, string dest, bool overwrite, int retries = 5)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? AppDataPath);
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    using var s = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var d = new FileStream(dest, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    s.CopyTo(d);
                    return;
                }
                catch when (i < retries - 1)
                {
                    Thread.Sleep(100 + (i * 120));
                }
            }
        }

        private static void TryMoveFile(string oldPath, string newPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(oldPath) || string.IsNullOrWhiteSpace(newPath))
                    return;
                if (!File.Exists(oldPath))
                    return;
                Directory.CreateDirectory(Path.GetDirectoryName(newPath) ?? AppDataPath);
                if (string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase))
                    return;
                if (File.Exists(newPath))
                    File.Delete(newPath);
                File.Move(oldPath, newPath);
            }
            catch
            {
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }
        }
    }
}
