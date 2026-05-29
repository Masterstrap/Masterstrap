using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Masterstrap.Models;
using Masterstrap.Services;
using Masterstrap.Utils;
using Masterstrap.Views;

namespace Masterstrap
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<PrivateServerLinkEntry> _privateServerLinks = new ObservableCollection<PrivateServerLinkEntry>();

        private void InitializeLinkPsTab()
        {
            try
            {
                this.Log("[LinkPS] Initializing Link PS tab...");

                var links = this._settingsManager.GetPrivateServerLinks();
                this._privateServerLinks.Clear();
                foreach (var link in links)
                {
                    this._privateServerLinks.Add(link);
                }

                if (this.LinkPsItemsControl != null)
                {
                    this.LinkPsItemsControl.ItemsSource = this._privateServerLinks;
                }

                this.UpdateLinkPsVisibility();
                this.Log($"[LinkPS] Loaded {this._privateServerLinks.Count} private server links");
            }
            catch (Exception ex)
            {
                this.Log($"[LinkPS] Error initializing Link PS: {ex.Message}");
            }
        }

        private void UpdateLinkPsVisibility()
        {
            if (this.LinkPsEmptyText != null)
            {
                this.LinkPsEmptyText.Visibility = this._privateServerLinks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void AddPrivateServerLinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new AddPrivateServerLinkWindow();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true && dialog.CreatedEntry != null)
                {
                    this._privateServerLinks.Add(dialog.CreatedEntry);
                    this._settingsManager.SetPrivateServerLinks(this._privateServerLinks);
                    this.UpdateLinkPsVisibility();
                    this.Log($"[LinkPS] Added link: {dialog.CreatedEntry.Name}");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[LinkPS] Error adding link: {ex.Message}");
            }
        }

        private void EditPrivateServerLinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var entry = button?.DataContext as PrivateServerLinkEntry;
                if (entry == null) return;

                var dialog = new AddPrivateServerLinkWindow(entry);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true && dialog.CreatedEntry != null)
                {
                    int index = this._privateServerLinks.IndexOf(entry);
                    if (index >= 0)
                    {
                        this._privateServerLinks[index] = dialog.CreatedEntry;
                        this._settingsManager.SetPrivateServerLinks(this._privateServerLinks);
                        this.Log($"[LinkPS] Updated link: {dialog.CreatedEntry.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log($"[LinkPS] Error editing link: {ex.Message}");
            }
        }

        private void DeletePrivateServerLinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var entry = button?.DataContext as PrivateServerLinkEntry;
                if (entry == null) return;

                string deletePrompt = string.Format(LocalizationService.Translate("Are you sure you want to delete '{0}'?"), entry.Name);
                if (MessageBox.Show(deletePrompt, LocalizationService.Translate("Delete Link"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this._privateServerLinks.Remove(entry);
                    this._settingsManager.SetPrivateServerLinks(this._privateServerLinks);
                    this.UpdateLinkPsVisibility();
                    this.Log($"[LinkPS] Deleted link: {entry.Name}");
                }
            }
            catch (Exception ex)
            {
                this.Log($"[LinkPS] Error deleting link: {ex.Message}");
            }
        }

        private async void PlayPrivateServerLinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var entry = button?.DataContext as PrivateServerLinkEntry;
                if (entry == null) return;

                await this.PlayPrivateServerLink(entry.Url, entry.Name);
            }
            catch (Exception ex)
            {
                this.Log($"[LinkPS] ✗ Error during play: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Masterstrap", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task PlayPrivateServerLink(string url, string title = "Private Server")
        {
            try
            {
                this.Log($"[Launcher] Attempting to join: {title} ({url})");

                if (PrivateServerLinkParser.TryBuildLaunchArgument(url, out string launchArg, out string error))
                {
                    this.Log($"[LinkPS] Launch argument: {launchArg}");

                    string robloxExePath = this._robloxExecutablePath;
                    if (string.IsNullOrEmpty(robloxExePath) || !System.IO.File.Exists(robloxExePath))
                    {
                        this.Log("[LinkPS] Roblox path not found, using installation manager...");
                        var (success, exePath) = await RobloxInstallationManager.EnsureRobloxInstalledAsync(
                            msg => this.Log(msg),
                            status => this.UpdateStatus(status, Colors.Blue)
                        );

                        if (success)
                        {
                            robloxExePath = exePath;
                            this._robloxExecutablePath = exePath;
                        }
                        else
                        {
                            this.Log("[LinkPS] ✗ Failed to ensure Roblox installation");
                            return;
                        }
                    }

                    this.UpdateStatus(LocalizationService.Translate("Launching Roblox..."), Colors.Blue);
                    bool allowManageFastFlags = this.AllowManageFastFlagsToggle?.IsChecked ?? true;
                    var flagsFromEditor = allowManageFastFlags && this._allFlagsList != null && this._allFlagsList.Count > 0
                        ? this._allFlagsList.ToList()
                        : null;

                    if (Application.Current != null)
                        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                    if (!string.IsNullOrEmpty(_selectedAccount))
                    {
                        this.UpdateStatus(LocalizationService.Translate("Authenticating..."), Colors.Yellow);
                        var (usedCookiesDat, authTicket, accountDetail) =
                            await Masterstrap.Services.AccountSwitcherManager.PrepareLaunchForAccountAsync(_selectedAccount);
                        if (!string.IsNullOrWhiteSpace(accountDetail))
                            this.Log($"[LinkPS] {_selectedAccount}: {accountDetail}");
                        if (usedCookiesDat)
                        {
                        }
                        else if (!string.IsNullOrEmpty(authTicket))
                        {
                            if (launchArg.StartsWith("roblox://", StringComparison.OrdinalIgnoreCase) ||
                                launchArg.StartsWith("roblox-player://", StringComparison.OrdinalIgnoreCase))
                            {
                                launchArg = launchArg + " --gameinfo " + authTicket;
                                this.Log($"[LinkPS] Launching with separate auth ticket for roblox:// scheme: {_selectedAccount}");
                            }
                            else if (!launchArg.Contains("+gameinfo:"))
                            {
                                launchArg = launchArg + $"+gameinfo:{authTicket}";
                                this.Log($"[LinkPS] Launching with legacy +gameinfo ticket: {_selectedAccount}");
                            }
                        }
                        else
                        {
                            this.Log($"[LinkPS] No account ticket for {_selectedAccount}. Continuing without +gameinfo.");
                        }
                    }

                                        try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = launchArg,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception launchEx)
                    {
                        this.Log($"[LinkPS] Launch failed: {launchEx.Message}");
                        return;
                    }
                    this._saveAndLaunchMode = false;
                    this.Log("[LinkPS] Roblox launch started");
                }
                else
                {
                    this.Log($"[Launcher] ✗ Error building launch argument: {error}");
                    MessageBox.Show($"Error: {error}", "Masterstrap", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                this.Log($"[Launcher] ✗ Error during launch: {ex.Message}");
                throw;
            }
        }

        private async void LinkPsGameIcon_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var image = sender as System.Windows.Controls.Image;
                if (image == null) return;

                var entry = image.DataContext as PrivateServerLinkEntry;
                if (entry == null) return;

                if (!string.IsNullOrWhiteSpace(entry.ImageUrl))
                {
                    LoadImageFromUrl(image, entry.ImageUrl);
                    return;
                }

                string iconUrl = string.Empty;
                string? gameName = null;

                bool isValidLink = PrivateServerLinkParser.TryNormalize(entry.Url, out _, out _, out var placeId, out _);

                if (isValidLink && placeId.HasValue)
                {
                    (iconUrl, gameName) = await RobloxThumbnailService.GetGameInfoAsync(placeId.Value);
                }
                else if (!string.IsNullOrWhiteSpace(entry.Url) && entry.Url.Contains("/share"))
                {
                    (iconUrl, gameName) = await RobloxThumbnailService.GetShareLinkInfoAsync(entry.Url);
                }

                if (!string.IsNullOrWhiteSpace(iconUrl))
                {
                    entry.ImageUrl = iconUrl;
                    LoadImageFromUrl(image, iconUrl);
                }

                if (!string.IsNullOrWhiteSpace(gameName))
                {
                    string currentName = entry.Name?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(currentName) ||
                        currentName.StartsWith("Place ", StringComparison.OrdinalIgnoreCase) ||
                        currentName.StartsWith("Share ", StringComparison.OrdinalIgnoreCase) ||
                        currentName == "Private Server" ||
                        currentName == "Join Private Server")
                    {
                        entry.Name = gameName;
                    }
                }

                this._settingsManager?.SetPrivateServerLinks(this._privateServerLinks);
            }
            catch (Exception ex)
            {
                this.Log($"[LinkPS] Error loading game icon: {ex.Message}");
            }
        }

        private static void LoadImageFromUrl(System.Windows.Controls.Image image, string url)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(url, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 104;
                bitmap.EndInit();
                image.Source = bitmap;
                image.Visibility = Visibility.Visible;
            }
            catch
            {
            }
        }
    }
}
