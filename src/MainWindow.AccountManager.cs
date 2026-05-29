using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Masterstrap.Models;
using Masterstrap.Services;

namespace Masterstrap
{
    public partial class MainWindow
    {
        private ObservableCollection<RobloxGame> _continuePlayingGames = new ObservableCollection<RobloxGame>();
        private ObservableCollection<RobloxGame> _searchResultsGames = new ObservableCollection<RobloxGame>();
        private ObservableCollection<AccountProfile> _accounts = new ObservableCollection<AccountProfile>();

        private void InitializeAccountManager()
        {
            try
            {
                if (this.ContinuePlayingItemsControl != null)
                {
                    this.ContinuePlayingItemsControl.ItemsSource = this._continuePlayingGames;
                }

                if (this.SearchResultsItemsControl != null)
                {
                    this.SearchResultsItemsControl.ItemsSource = this._searchResultsGames;
                }

                _ = this.LoadAccountsAsync();

                this.LoadPlayedHistory();

                var settings = new AppSettingsManager();
                long lastPlaceId = settings.GetLastAccountManagerPlaceId();
                if (lastPlaceId > 0)
                {
                    this.SelectedPlaceIdInput.Text = lastPlaceId.ToString();
                    this.SelectedGameTitle.Text = settings.GetLastAccountManagerPlaceTitle();
                    _ = this.LoadGameMetadataAsync(lastPlaceId);
                    this.StartAutoServerFinding(lastPlaceId);
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Init error: {ex.Message}");
            }
        }

        private void LoadPlayedHistory()
        {
            try
            {
                var history = new AppSettingsManager().GetPlayedGames();
                this.Dispatcher.Invoke(() => {
                    if (this.ContinuePlayingTitle != null) this.ContinuePlayingTitle.Text = LocalizationService.Translate("Continue Playing");
                    this._continuePlayingGames.Clear();
                    foreach (var game in history)
                    {
                        this._continuePlayingGames.Add(game);
                    }

                    if (this._continuePlayingGames.Count == 0)
                    {
                    }
                });
            }
            catch { }
        }

        private void RefreshAccountManager_Click(object sender, RoutedEventArgs e)
        {
            this.Log("[AccountManager] Refreshing games...");
            _ = Task.Run(() => this.LoadAccountManagerDataAsync());
            _ = this.LoadAccountsAsync();
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var profiles = AccountSwitcherManager.LoadProfiles();

                this.Dispatcher.Invoke(() => {
                    this._accounts.Clear();
                    foreach (var p in profiles) this._accounts.Add(p);

                    if (string.IsNullOrEmpty(this._selectedAccount) && profiles.Count > 0)
                    {
                        this._selectedAccount = profiles[0].AccountName;
                        try { this._settingsManager?.SetSelectedAccount(this._selectedAccount); } catch { }
                    }
                    this.UpdateActiveAccountCard();
                });

                _ = Task.Run(async () => {
                    bool anyUpdated = false;
                    foreach (var profile in profiles)
                    {
                        if (await AccountSwitcherManager.RefreshProfileAsync(profile))
                        {
                            anyUpdated = true;
                            if (profile.AccountName.Equals(this._selectedAccount, StringComparison.OrdinalIgnoreCase))
                            {
                                this.Dispatcher.Invoke(() => this.UpdateActiveAccountCard());
                            }
                        }
                    }

                    if (anyUpdated)
                    {
                        AccountSwitcherManager.SaveProfiles(profiles);
                    }
                });
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Load accounts error: {ex.Message}");
            }
        }

        private void UpdateActiveAccountCard()
        {
            try
            {
                var active = this._accounts.FirstOrDefault(a =>
                    a.AccountName.Equals(this._selectedAccount, StringComparison.OrdinalIgnoreCase));

                if (active == null && this._accounts.Count > 0)
                {
                    active = this._accounts[0];
                    this._selectedAccount = active.AccountName;
                }

                if (active != null)
                {
                    this.ActiveAccountDisplayName.Text = active.DisplayName ?? active.AccountName;
                    this.ActiveAccountUsername.Text = $"@{active.Username ?? active.AccountName}";
                    string statsTemplate = LocalizationService.Translate("{0} Friends   {1} Followers   {2} Following");
                    this.ActiveAccountStats.Text = string.Format(statsTemplate, active.FriendCount, active.FollowerCount, active.FollowingCount);

                    if (!string.IsNullOrEmpty(active.AvatarUrl))
                    {
                        try
                        {
                            this.ActiveAccountAvatar.Source = new BitmapImage(new Uri(active.AvatarUrl));
                        }
                        catch { }
                    }
                }
                else
                {
                    this.ActiveAccountDisplayName.Text = LocalizationService.Translate("No Account");
                    this.ActiveAccountUsername.Text = LocalizationService.Translate("Click ··· to add an account");
                    this.ActiveAccountStats.Text = "";
                    this.ActiveAccountAvatar.Source = null;
                }
            }
            catch { }
        }

        private void AccountMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            var menu = new System.Windows.Controls.ContextMenu();
            menu.Background = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            menu.BorderBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            menu.BorderThickness = new Thickness(1);
            menu.Padding = new Thickness(0);

            var menuTemplate = new ControlTemplate(typeof(System.Windows.Controls.ContextMenu));
            var menuBorder = new FrameworkElementFactory(typeof(Border));
            menuBorder.SetValue(Border.BackgroundProperty, menu.Background);
            menuBorder.SetValue(Border.BorderBrushProperty, menu.BorderBrush);
            menuBorder.SetValue(Border.BorderThicknessProperty, menu.BorderThickness);
            menuBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(6));

            var itemsPresenter = new FrameworkElementFactory(typeof(ItemsPresenter));
            itemsPresenter.SetValue(ItemsPresenter.MarginProperty, new Thickness(2));
            menuBorder.AppendChild(itemsPresenter);
            menuTemplate.VisualTree = menuBorder;
            menu.Template = menuTemplate;

            foreach (var acct in this._accounts)
            {
                bool isCurrent = acct.AccountName.Equals(this._selectedAccount, StringComparison.OrdinalIgnoreCase);
                this.AddStyledMenuItem(menu,
                    $"{acct.DisplayName ?? acct.AccountName}",
                    isCurrent ? "✓" : "👤",
                    () => {
                        this._selectedAccount = acct.AccountName;
                        try { this._settingsManager?.SetSelectedAccount(this._selectedAccount); } catch { }
                        try { PopulateAccountDropdown(); } catch { }
                        this.UpdateActiveAccountCard();
                        this.Log($"[AccountManager] Switched to: {acct.DisplayName} (@{acct.Username})");
                    },
                    isCurrent ? Brushes.Cyan : Brushes.White);
            }

            if (this._accounts.Count > 0)
                menu.Items.Add(new Separator { Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Margin = new Thickness(5, 5, 5, 5) });

            this.AddStyledMenuItem(menu, LocalizationService.Translate("Add Account"), "➕", () => {
                var login = new Masterstrap.Views.LoginWindow();
                if (login.ShowDialog() == true)
                {
                    if (!string.IsNullOrEmpty(login.DetectedUsername) && !string.IsNullOrEmpty(login.ExtractedCookie))
                    {
                        AccountSwitcherManager.SaveSessionCookie(login.DetectedUsername, login.ExtractedCookie);
                        this._selectedAccount = login.DetectedUsername;
                        try { this._settingsManager?.SetSelectedAccount(this._selectedAccount); } catch { }
                        try { PopulateAccountDropdown(); } catch { }
                        this.Log($"[AccountManager] Added account: {login.DetectedUsername}");
                        _ = this.LoadAccountsAsync();
                    }
                }
            });

            if (!string.IsNullOrEmpty(this._selectedAccount))
            {
                this.AddStyledMenuItem(menu, LocalizationService.Translate("Remove Current"), "🗑", () => {
                    string toRemove = this._selectedAccount;
                    string prompt = string.Format(LocalizationService.Translate("Remove account '{0}'?"), toRemove);
                    var res = MessageBox.Show(prompt, LocalizationService.Translate("Remove Account"), MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        AccountSwitcherManager.DeleteAccount(toRemove);
                        this._selectedAccount = null;
                        try { this._settingsManager?.SetSelectedAccount(this._selectedAccount ?? ""); } catch { }
                        try { PopulateAccountDropdown(); } catch { }
                        this.Log($"[AccountManager] Removed account: {toRemove}");
                        _ = this.LoadAccountsAsync();
                    }
                }, new SolidColorBrush(Color.FromRgb(239, 68, 68)));
            }

            btn.ContextMenu = menu;
            menu.PlacementTarget = btn;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            menu.IsOpen = true;
        }

        private void AddStyledMenuItem(System.Windows.Controls.ContextMenu menu, string header, string icon, Action onClick, Brush foreground = null)
        {
            var item = new MenuItem
            {
                Foreground = foreground ?? Brushes.White,
                Cursor = Cursors.Hand
            };

            var template = new ControlTemplate(typeof(MenuItem));

            var border = new FrameworkElementFactory(typeof(Border));
            border.Name = "Bd";
            border.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            border.SetValue(Border.PaddingProperty, new Thickness(12, 8, 24, 8));

            var stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            var iconTxt = new FrameworkElementFactory(typeof(TextBlock));
            iconTxt.SetValue(TextBlock.TextProperty, icon);
            iconTxt.SetValue(TextBlock.WidthProperty, 24.0);
            iconTxt.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
            iconTxt.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 10, 0));
            iconTxt.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            iconTxt.SetValue(TextBlock.FontSizeProperty, 14.0);

            var headerTxt = new FrameworkElementFactory(typeof(TextBlock));
            headerTxt.SetValue(TextBlock.TextProperty, header);
            headerTxt.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            headerTxt.SetValue(TextBlock.FontSizeProperty, 13.0);
            headerTxt.SetValue(TextBlock.FontWeightProperty, FontWeights.Medium);

            stack.AppendChild(iconTxt);
            stack.AppendChild(headerTxt);
            border.AppendChild(stack);
            template.VisualTree = border;

            var trigger = new Trigger { Property = MenuItem.IsHighlightedProperty, Value = true };
            trigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(45, 45, 45)), "Bd"));
            template.Triggers.Add(trigger);

            item.Template = template;
            item.Click += (s, e) => onClick();
            menu.Items.Add(item);
        }

        private async void JoinGameServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.Tag is RobloxGame game)
                {
                    this.SelectedPlaceIdInput.Text = game.PlaceId.ToString();
                    await this.LoadGameMetadataAsync(game.PlaceId);

                    string targetRegion = (this.AccountManagerRegionCombo2.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Auto (Best)";
                    string url = $"https://www.roblox.com/games/{game.PlaceId}/";

                    this.Log($"[AccountManager] Joining: {game.Title} (Target Region: {targetRegion})");

                    var fetcher = new RobloxServerFetcher();
                    string cookie = Masterstrap.Services.AccountSwitcherManager.GetSessionCookie(this._selectedAccount ?? "") ?? "";
                    string serverSize = (this.ServerSizeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Large Servers";

                    if (string.IsNullOrWhiteSpace(cookie))
                    {
                        this.Log("[AccountManager] Missing account cookie. Joining default server.");
                    }
                    else
                    {
                        this.Log($"[AccountManager] Searching best server for {game.Title} | Region={targetRegion} | Size={serverSize}...");
                        var servers = await fetcher.FetchServersByRegionAsync(game.PlaceId, targetRegion, cookie, maxPages: 12);
                        var selectedServer = SelectBestServerForPreference(servers, serverSize);
                        if (selectedServer != null)
                        {
                            url += $"?jobId={selectedServer.Id}";
                            this.Log($"[AccountManager] Selected {selectedServer.Region} server: {selectedServer.Id} ({selectedServer.Playing}/{selectedServer.MaxPlayers}) - Est. {selectedServer.Ping}ms");
                        }
                        else
                        {
                            this.Log($"[AccountManager] No matching servers found for Region={targetRegion}. Joining default.");
                        }
                    }

                    new AppSettingsManager().SetLastAccountManagerGame(game.PlaceId, game.Title);

                    await this.PlayPrivateServerLink(url, game.Title);
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Join error: {ex.Message}");
            }
        }

        private async void SelectedPlaceIdInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string query = this.SelectedPlaceIdInput.Text.Trim();
                if (string.IsNullOrEmpty(query))
                {
                    this.Dispatcher.Invoke(() => {
                        this.SearchResultsSection.Visibility = Visibility.Collapsed;
                        this._searchResultsGames.Clear();
                    });
                    return;
                }

                if (long.TryParse(query, out long placeId))
                {
                    await this.LoadGameMetadataAsync(placeId);
                }
                else
                {
                    await this.SearchGamesAsync(query);
                }
            }
        }

        private async Task SearchGamesAsync(string query)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                this.Log($"[AccountManager] Searching: {query}...");

                this.Dispatcher.Invoke(() => {
                    this.SelectedGameTitle.Text = "Searching...";
                    this.SelectedGameCreator.Text = "Please wait...";
                });

                var response = await client.GetAsync($"https://apis.roblox.com/search-api/omni-search?pageType=Game&searchQuery={Uri.EscapeDataString(query)}&sessionId=0");
                if (!response.IsSuccessStatusCode) return;

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("searchResults", out var searchResults) || searchResults.ValueKind != JsonValueKind.Array)
                    return;

                var list = new List<RobloxGame>();
                foreach (var group in searchResults.EnumerateArray())
                {
                    if (!group.TryGetProperty("contents", out var contents)) continue;
                    foreach (var item in contents.EnumerateArray())
                    {
                        if (item.TryGetProperty("universeId", out var uid))
                        {
                            int playerCount = 0;
                            if (item.TryGetProperty("playerCount", out var pcEl) && pcEl.ValueKind == JsonValueKind.Number)
                                playerCount = pcEl.GetInt32();

                            long upVotes = 0;
                            if (item.TryGetProperty("totalUpVotes", out var uvEl) && uvEl.ValueKind == JsonValueKind.Number)
                                upVotes = uvEl.GetInt64();

                            list.Add(new RobloxGame {
                                Title = item.GetProperty("name").GetString() ?? "Unknown",
                                Subtitle = $"{FormatNumber(playerCount)} Playing • 👍 {FormatNumber(upVotes)}",
                                PlaceId = item.GetProperty("rootPlaceId").GetInt64(),
                                UniverseId = uid.GetInt64(),
                                ThumbnailUrl = $"https://www.roblox.com/asset-thumbnail/image?assetId={item.GetProperty("rootPlaceId").GetInt64()}&width=420&height=420&format=png"
                            });
                        }
                    }
                }

                if (list.Any())
                {
                    var universeIds = list.Where(g => g.UniverseId > 0).Select(g => g.UniverseId).Distinct().ToList();
                    if (universeIds.Any())
                    {
                        try
                        {
                            string idsParam = string.Join(",", universeIds);
                            var thumbResp = await client.GetAsync($"https://thumbnails.roblox.com/v1/games/icons?universeIds={idsParam}&returnPolicy=PlaceHolder&size=150x150&format=Png&isCircular=false");
                            if (thumbResp.IsSuccessStatusCode)
                            {
                                var thumbJson = await thumbResp.Content.ReadAsStringAsync();
                                using var thumbDoc = JsonDocument.Parse(thumbJson);
                                if (thumbDoc.RootElement.TryGetProperty("data", out var thumbData))
                                {
                                    var thumbMap = new Dictionary<long, string>();
                                    foreach (var t in thumbData.EnumerateArray())
                                    {
                                        long targetId = t.GetProperty("targetId").GetInt64();
                                        string imageUrl = t.GetProperty("imageUrl").GetString() ?? "";
                                        thumbMap[targetId] = imageUrl;
                                    }
                                    foreach (var g in list)
                                    {
                                        if (g.UniverseId > 0 && thumbMap.TryGetValue(g.UniverseId, out var url))
                                            g.ThumbnailUrl = url;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                if (list.Count > 0)
                {
                    this.Dispatcher.Invoke(() => {
                        this.SearchResultsSection.Visibility = Visibility.Visible;
                        this._searchResultsGames.Clear();
                        foreach (var item in list) this._searchResultsGames.Add(item);

                        var first = list[0];
                        this.SelectedGameTitle.Text = first.Title;
                        this.SelectedPlaceIdInput.Text = first.PlaceId.ToString();

                        try {
                            var parts = first.Subtitle.Split('•');
                            this.SelectedGamePlaying.Text = parts[0].Trim();
                            this.SelectedGameVisits.Text = parts.Length > 1 ? parts[1].Trim().Replace("👍 ", "") : "0";
                        } catch { }

                        try { this.SelectedGameThumbnail.Source = new BitmapImage(new Uri(first.ThumbnailUrl)); } catch { }
                        this.Log($"[AccountManager] Found {list.Count} games for '{query}'");
                    });

                    _ = this.LoadGameMetadataFromSearchAsync(list[0], client);
                }
                else
                {
                    this.Dispatcher.Invoke(() => {
                        this.SearchResultsSection.Visibility = Visibility.Collapsed;
                        this._searchResultsGames.Clear();
                    });
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Search error: {ex.Message}");
            }
        }

        private static string FormatNumber(long num)
        {
            if (num >= 1_000_000_000) return $"{num / 1_000_000_000.0:0.#}B";
            if (num >= 1_000_000) return $"{num / 1_000_000.0:0.#}M";
            if (num >= 1_000) return $"{num / 1_000.0:0.#}K";
            return num.ToString("N0");
        }

        private static string FormatNumber(int num) => FormatNumber((long)num);

        private static bool IsSmallServerPreference(string serverSizeText)
        {
            return !string.IsNullOrWhiteSpace(serverSizeText) &&
                   serverSizeText.Contains("small", StringComparison.OrdinalIgnoreCase);
        }

        private static ServerInstance? SelectBestServerForPreference(IEnumerable<ServerInstance> servers, string serverSizeText)
        {
            var candidates = servers?.Where(s => s != null).ToList() ?? new List<ServerInstance>();
            if (candidates.Count == 0)
                return null;

            if (IsSmallServerPreference(serverSizeText))
            {
                return candidates
                    .OrderBy(s => s.Ping)
                    .ThenBy(s => s.Playing)
                    .ThenBy(s => s.MaxPlayers)
                    .First();
            }

            return candidates
                .OrderBy(s => s.Ping)
                .ThenByDescending(s => s.Playing)
                .ThenByDescending(s => s.MaxPlayers)
                .First();
        }

        private async void LargeJoinGame_Click(object sender, RoutedEventArgs e)
        {
            string placeIdStr = this.SelectedPlaceIdInput.Text.Trim();
            string jobId = this.SelectedJobIdInput.Text.Trim();
            string region = (this.AccountManagerRegionCombo2.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Auto (Best)";

            if (long.TryParse(placeIdStr, out long placeId))
            {
                string url = $"https://www.roblox.com/games/{placeId}/";

                if (!string.IsNullOrEmpty(jobId))
                {
                    url += $"?jobId={jobId}";
                }
                else
                {
                    var fetcher = new RobloxServerFetcher();
                    string cookie = Masterstrap.Services.AccountSwitcherManager.GetSessionCookie(this._selectedAccount ?? "") ?? "";
                    try
                    {
                        string serverSize = (this.ServerSizeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Large Servers";
                        if (string.IsNullOrWhiteSpace(cookie))
                        {
                            this.Log("[AccountManager] Missing account cookie. Joining default server.");
                        }
                        else if (region == "Auto (Best)")
                        {
                            this.Log($"[AccountManager] Instant server search for Place {placeId} | Size={serverSize}...");
                            var servers = await fetcher.FetchServersInstantAsync(placeId, cookie, maxServers: 100);
                            var selectedServer = SelectBestServerForPreference(servers, serverSize);
                            if (selectedServer != null)
                            {
                                url += $"?jobId={selectedServer.Id}";
                                this.Log($"[AccountManager] Instant selected server: {selectedServer.Id} ({selectedServer.Playing}/{selectedServer.MaxPlayers})");
                            }
                            else
                            {
                                this.Log($"[AccountManager] No servers found. Joining default.");
                            }
                        }
                        else
                        {
                            this.Log($"[AccountManager] Searching best server for Place {placeId} | Region={region} | Size={serverSize}...");
                            var servers = await fetcher.FetchServersByRegionAsync(placeId, region, cookie, maxPages: 8);
                            var selectedServer = SelectBestServerForPreference(servers, serverSize);
                            if (selectedServer != null)
                            {
                                url += $"?jobId={selectedServer.Id}";
                                this.Log($"[AccountManager] Selected {selectedServer.Region} server: {selectedServer.Id} ({selectedServer.Playing}/{selectedServer.MaxPlayers}) - Est. {selectedServer.Ping}ms");
                            }
                            else
                            {
                                this.Log($"[AccountManager] No matching servers found for Region={region}. Joining default.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Log($"[AccountManager] Server fetch failed: {ex.Message}");
                    }
                }

                string title = this.SelectedGameTitle.Text;
                if (string.IsNullOrWhiteSpace(title) || title == "Select a game") title = "Roblox Game";

                new AppSettingsManager().AddToPlayedGames(new RobloxGame {
                    Title = title,
                    PlaceId = placeId,
                    ThumbnailUrl = (this.SelectedGameThumbnail.Source as BitmapImage)?.UriSource?.AbsoluteUri ?? ""
                });
                this.LoadPlayedHistory();

                new AppSettingsManager().SetLastAccountManagerGame(placeId, title);

                await this.PlayPrivateServerLink(url, title);
            }
            else
            {
                this.Log("[AccountManager] Invalid Place ID. Please search for a game first.");
                MessageBox.Show("Please select or search for a game before playing.", "Masterstrap", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenServerBrowser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string placeIdStr = this.SelectedPlaceIdInput.Text.Trim();
                if (long.TryParse(placeIdStr, out long placeId))
                {
                    string cookie = Masterstrap.Services.AccountSwitcherManager.GetSessionCookie(this._selectedAccount ?? "") ?? "";
                    if (string.IsNullOrEmpty(cookie))
                    {
                        MessageBox.Show("Please select an account first to use the Server Browser.", "Masterstrap", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    this.Log($"[AccountManager] Opening Server Browser for Place {placeId}...");
                    var browser = new Views.ServerBrowserWindow(placeId, this.SelectedGameTitle.Text, cookie);
                    browser.Owner = this;
                    if (browser.ShowDialog() == true && !string.IsNullOrEmpty(browser.SelectedJobId))
                    {
                        this.SelectedJobIdInput.Text = browser.SelectedJobId;
                        this.Log($"[AccountManager] Selected server from browser: {browser.SelectedJobId}");
                        LargeJoinGame_Click(null, null);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a game first to view available servers.", "Server Browser", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Error opening Server Browser: {ex.Message}");
                MessageBox.Show("Could not open Server Browser. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadGameMetadataAsync(long placeId)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                var uniResp = await client.GetAsync($"https://apis.roblox.com/universes/v1/places/{placeId}/universe");
                if (!uniResp.IsSuccessStatusCode)
                {
                    this.Log($"[AccountManager] Failed to resolve universe for Place {placeId}");
                    return;
                }

                var uniJson = await uniResp.Content.ReadAsStringAsync();
                using var uniDoc = JsonDocument.Parse(uniJson);
                long universeId = uniDoc.RootElement.GetProperty("universeId").GetInt64();

                var metaResp = await client.GetAsync($"https://games.roblox.com/v1/games?universeIds={universeId}");
                if (!metaResp.IsSuccessStatusCode) return;

                var metaJson = await metaResp.Content.ReadAsStringAsync();
                using var metaDoc = JsonDocument.Parse(metaJson);
                var gameMeta = metaDoc.RootElement.GetProperty("data")[0];

                string name = gameMeta.GetProperty("name").GetString() ?? "Unknown";
                string creatorName = "";
                if (gameMeta.TryGetProperty("creator", out var creator) && creator.TryGetProperty("name", out var cn))
                    creatorName = cn.GetString() ?? "";
                int playing = gameMeta.GetProperty("playing").GetInt32();

                long upVotes = 0;
                try
                {
                    var voteResp = await client.GetAsync($"https://games.roblox.com/v1/games/votes?universeIds={universeId}");
                    if (voteResp.IsSuccessStatusCode)
                    {
                        var voteJson = await voteResp.Content.ReadAsStringAsync();
                        using var voteDoc = JsonDocument.Parse(voteJson);
                        upVotes = voteDoc.RootElement.GetProperty("data")[0].GetProperty("upVotes").GetInt64();
                    }
                }
                catch { }

                string thumbnailUrl = "";
                try
                {
                    var thumbResp = await client.GetAsync($"https://thumbnails.roblox.com/v1/games/icons?universeIds={universeId}&returnPolicy=PlaceHolder&size=512x512&format=Png&isCircular=false");
                    if (thumbResp.IsSuccessStatusCode)
                    {
                        var thumbJson = await thumbResp.Content.ReadAsStringAsync();
                        using var thumbDoc = JsonDocument.Parse(thumbJson);
                        thumbnailUrl = thumbDoc.RootElement.GetProperty("data")[0].GetProperty("imageUrl").GetString() ?? "";
                    }
                }
                catch { }

                this.Dispatcher.Invoke(() => {
                    this.SelectedGameTitle.Text = name;
                    this.SelectedGameCreator.Text = $"By {creatorName}";
                    this.SelectedGamePlaying.Text = FormatNumber(playing);
                    this.SelectedGameVisits.Text = FormatNumber(upVotes);
                    if (!string.IsNullOrEmpty(thumbnailUrl))
                    {
                        try { this.SelectedGameThumbnail.Source = new BitmapImage(new Uri(thumbnailUrl)); } catch { }
                    }
                    this.Log($"[AccountManager] Loaded: {name} | {FormatNumber(playing)} playing | 👍 {FormatNumber(upVotes)}");

                    this.StartAutoServerFinding(placeId);
                });

                try
                {
                    this.Log($"[AccountManager] Fetching sub-places for Universe {universeId}...");
                    var subResp = await client.GetAsync($"https://games.roblox.com/v1/games/{universeId}/sub-places?limit=10");
                    var subList = new List<RobloxGame>();

                    if (subResp.IsSuccessStatusCode)
                    {
                        var subJson = await subResp.Content.ReadAsStringAsync();
                        using var subDoc = JsonDocument.Parse(subJson);
                        foreach (var p in subDoc.RootElement.GetProperty("places").EnumerateArray())
                        {
                            subList.Add(new RobloxGame {
                                Title = p.GetProperty("name").GetString() ?? "Unknown",
                                PlaceId = p.GetProperty("id").GetInt64(),
                                ThumbnailUrl = $"https://www.roblox.com/asset-thumbnail/image?assetId={p.GetProperty("id").GetInt64()}&width=420&height=420&format=png"
                            });
                        }
                    }

                    if (subList.Count == 0)
                    {
                        subList.Add(new RobloxGame {
                            Title = name,
                            PlaceId = placeId,
                            ThumbnailUrl = thumbnailUrl
                        });
                    }

                    this.Dispatcher.Invoke(() => {
                        this.SubPlacesItemsControl.ItemsSource = null;
                        this.SubPlacesItemsControl.ItemsSource = subList;
                    });
                }
                catch (Exception ex)
                {
                    this.Log($"[AccountManager] Sub-places error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Metadata error: {ex.Message}");
            }
        }

        private async Task LoadGameMetadataFromSearchAsync(RobloxGame game, HttpClient client)
        {
            try
            {
                string thumbnailUrl = game.ThumbnailUrl;
                long upVotes = 0;
                string creatorName = "";

                if (game.UniverseId > 0)
                {
                    try
                    {
                        var thumbResp = await client.GetAsync($"https://thumbnails.roblox.com/v1/games/icons?universeIds={game.UniverseId}&returnPolicy=PlaceHolder&size=512x512&format=Png&isCircular=false");
                        if (thumbResp.IsSuccessStatusCode)
                        {
                            var thumbJson = await thumbResp.Content.ReadAsStringAsync();
                            using var thumbDoc = JsonDocument.Parse(thumbJson);
                            thumbnailUrl = thumbDoc.RootElement.GetProperty("data")[0].GetProperty("imageUrl").GetString() ?? thumbnailUrl;
                        }
                    }
                    catch { }

                    try
                    {
                        var metaResp = await client.GetAsync($"https://games.roblox.com/v1/games?universeIds={game.UniverseId}");
                        if (metaResp.IsSuccessStatusCode)
                        {
                            var metaJson = await metaResp.Content.ReadAsStringAsync();
                            using var metaDoc = JsonDocument.Parse(metaJson);
                            var gameMeta = metaDoc.RootElement.GetProperty("data")[0];
                            if (gameMeta.TryGetProperty("creator", out var creator) && creator.TryGetProperty("name", out var cn))
                                creatorName = cn.GetString() ?? "";
                        }
                    }
                    catch { }

                    try
                    {
                        var voteResp = await client.GetAsync($"https://games.roblox.com/v1/games/votes?universeIds={game.UniverseId}");
                        if (voteResp.IsSuccessStatusCode)
                        {
                            var voteJson = await voteResp.Content.ReadAsStringAsync();
                            using var voteDoc = JsonDocument.Parse(voteJson);
                            upVotes = voteDoc.RootElement.GetProperty("data")[0].GetProperty("upVotes").GetInt64();
                        }
                    }
                    catch { }
                }

                string finalThumbUrl = thumbnailUrl;
                this.Dispatcher.Invoke(() => {
                    this.SelectedGameTitle.Text = game.Title;
                    this.SelectedGameCreator.Text = string.IsNullOrEmpty(creatorName) ? "" : $"By {creatorName}";
                    if (upVotes > 0) this.SelectedGameVisits.Text = FormatNumber(upVotes);
                    if (!string.IsNullOrEmpty(finalThumbUrl))
                    {
                        try { this.SelectedGameThumbnail.Source = new BitmapImage(new Uri(finalThumbUrl)); } catch { }
                    }
                });
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Search metadata error: {ex.Message}");
            }
        }

        private async Task LoadAccountManagerDataAsync()
        {
            try
            {
                string accountName = "";
                this.Dispatcher.Invoke(() => accountName = _selectedAccount ?? "");

                if (string.IsNullOrEmpty(accountName))
                {
                    this.Log("[AccountManager] No account selected.");
                    return;
                }

                string cookie = Masterstrap.Services.AccountSwitcherManager.GetSessionCookie(accountName) ?? "";
                if (string.IsNullOrEmpty(cookie))
                {
                    this.Log($"[AccountManager] No cookie found for {accountName}.");
                    return;
                }

                await this.FetchContinuePlayingAsync(cookie);

                this.Log("[AccountManager] Data refreshed successfully.");
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Refresh error: {ex.Message}");
            }
        }

        private async Task FetchContinuePlayingAsync(string cookie)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Cookie", $".ROBLOSECURITY={cookie}");

                var response = await client.GetAsync("https://games.roblox.com/v1/games/list?model.sortToken=v1_ContinuePlaying&model.gameSortsContext=HomeSorts&model.maxRows=12");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    var list = new List<RobloxGame>();
                    if (doc.RootElement.TryGetProperty("games", out var gamesArray))
                    {
                        var uids = new List<long>();
                        foreach (var game in gamesArray.EnumerateArray())
                        {
                            long placeId = game.GetProperty("placeId").GetInt64();
                            long universeId = game.GetProperty("universeId").GetInt64();
                            list.Add(new RobloxGame
                            {
                                Title = game.GetProperty("name").GetString() ?? "Unknown Game",
                                Subtitle = game.TryGetProperty("creatorName", out var cn) ? $"By {cn.GetString()}" : "Recently Played",
                                ThumbnailUrl = "",
                                PlaceId = placeId,
                                UniverseId = universeId
                            });
                            uids.Add(universeId);
                        }

                        if (uids.Any())
                        {
                            try
                            {
                                string ids = string.Join(",", uids);
                                var thumbResp = await client.GetAsync($"https://thumbnails.roblox.com/v1/games/icons?universeIds={ids}&returnPolicy=PlaceHolder&size=150x150&format=Png&isCircular=false");
                                if (thumbResp.IsSuccessStatusCode)
                                {
                                    var thumbJson = await thumbResp.Content.ReadAsStringAsync();
                                    using var thumbDoc = JsonDocument.Parse(thumbJson);
                                    var thumbMap = thumbDoc.RootElement.GetProperty("data").EnumerateArray()
                                        .ToDictionary(t => t.GetProperty("targetId").GetInt64(), t => t.GetProperty("imageUrl").GetString());

                                    foreach (var g in list)
                                    {
                                        if (thumbMap.TryGetValue(g.UniverseId, out var url)) g.ThumbnailUrl = url;
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    this.Dispatcher.Invoke(() => {
                        this._continuePlayingGames.Clear();
                        foreach (var g in list) this._continuePlayingGames.Add(g);

                        if (list.Count > 0)
                        {
                            new AppSettingsManager().SavePlayedGames(list.Take(10).ToList());
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Continue Playing error: {ex.Message}");
            }
        }

        private async void RefreshContinuePlaying_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var active = this._accounts.FirstOrDefault(a => a.AccountName.Equals(this._selectedAccount, StringComparison.OrdinalIgnoreCase));
                if (active != null)
                {
                    string cookie = Masterstrap.Services.AccountSwitcherManager.GetSessionCookie(active.AccountName);
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        await this.FetchContinuePlayingAsync(cookie);
                        this.Log($"[AccountManager] Refreshed continue playing for: {active.AccountName}");
                    }
                    else
                    {
                        this.Log($"[AccountManager] Cannot refresh: No cookie found for {active.AccountName}");
                    }
                }
                else
                {
                    this.Log("[AccountManager] Cannot refresh: No active account selected");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[AccountManager] Refresh continue playing error: {ex.Message}");
            }
        }

        private void HorizontalScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;

                var parent = this.AccountManagerMainScroll as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }

        private System.Threading.CancellationTokenSource _autoServerCts;
        private async void StartAutoServerFinding(long placeId)
        {
            try
            {
                _autoServerCts?.Cancel();
                _autoServerCts = new System.Threading.CancellationTokenSource();
                var token = _autoServerCts.Token;

                this.Dispatcher.Invoke(() => {
                    this.SelectedJobIdInput.Text = "";
                    this.AutoServerStatusText.Text = LocalizationService.Translate("Finding best server...");
                    this.AutoServerStatusText.Visibility = Visibility.Visible;
                });

                string region = "";
                string serverSize = "";
                string cookie = "";

                this.Dispatcher.Invoke(() => {
                    region = (this.AccountManagerRegionCombo2.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Auto (Best)";
                    serverSize = (this.ServerSizeCombo.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Large Servers";
                    cookie = Masterstrap.Services.AccountSwitcherManager.GetSessionCookie(this._selectedAccount ?? "") ?? "";
                });

                var fetcher = new RobloxServerFetcher();
                var servers = await fetcher.FetchServersByRegionAsync(placeId, region, cookie, maxPages: 4);

                if (token.IsCancellationRequested) return;

                this.Dispatcher.Invoke(() => {
                    if (servers != null && servers.Any())
                    {
                        ServerInstance selected;
                        if (IsSmallServerPreference(serverSize))
                            selected = servers.OrderBy(s => s.Ping).ThenBy(s => s.Playing).First();
                        else
                            selected = servers.OrderBy(s => s.Ping).ThenByDescending(s => s.Playing).First();

                        this.SelectedJobIdInput.Text = selected.Id;
                        this.AutoServerStatusText.Text = string.Format(
                            LocalizationService.Translate("✔ Found best server: {0} - {1}ms"),
                            selected.Region,
                            selected.Ping);
                    }
                    else
                    {
                        this.AutoServerStatusText.Text = LocalizationService.Translate("⚠ No regional servers found. Using default.");
                    }
                });
            }
            catch { }
        }

    }
}
