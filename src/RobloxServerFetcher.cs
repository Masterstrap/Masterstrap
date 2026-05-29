using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Masterstrap.Services
{
    public class ServerInstance
    {
        public string Id { get; set; } = "";
        public int Playing { get; set; }
        public int MaxPlayers { get; set; }
        public string Region { get; set; } = "Unknown";
        public int? DataCenterId { get; set; }
        public int Ping { get; set; }
        public string PingColor { get; set; } = "#2ECC71";
        public DateTime? FirstSeen { get; set; }
    }

    public class RobloxServerFetcher
    {
        private readonly HttpClient _client;
        private Dictionary<int, string>? _datacenterIdToRegion;
        private List<string>? _regionList;

        private readonly ConcurrentDictionary<long, ConcurrentDictionary<string, ServerInstance>> _serverCache = new();

        public RobloxServerFetcher()
        {
            _client = new HttpClient(new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                MaxConnectionsPerServer = 20
            });
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _client.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task LoadDatacentersAsync()
        {
            try
            {
                var json = await _client.GetStringAsync("https://apis.rovalra.com/v1/datacenters/list");
                using var doc = JsonDocument.Parse(json);
                var dcMap = new Dictionary<int, string>();
                var regions = new HashSet<string>();

                foreach (var entry in doc.RootElement.EnumerateArray())
                {
                    string city = entry.GetProperty("location").GetProperty("city").GetString() ?? "";
                    string country = entry.GetProperty("location").GetProperty("country").GetString() ?? "";
                    string region = $"{city}, {country}".Trim().Trim(',', ' ');
                    if (string.IsNullOrEmpty(region)) region = "Unknown";

                    regions.Add(region);
                    foreach (var id in entry.GetProperty("dataCenterIds").EnumerateArray())
                    {
                        dcMap[id.GetInt32()] = region;
                    }
                }

                _datacenterIdToRegion = dcMap;
                _regionList = regions.OrderBy(r => r).ToList();
            }
            catch
            {
                _datacenterIdToRegion = new Dictionary<int, string>();
                _regionList = new List<string>();
            }
        }

        public async Task<List<ServerInstance>> FetchServersInstantAsync(long placeId, string roblosecurity, int maxServers = 50)
        {
            var servers = new List<ServerInstance>();
            string cursor = "";

            for (int page = 0; page < 3; page++)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get,
                        $"https://games.roblox.com/v1/games/{placeId}/servers/Public?sortOrder=2&excludeFullGames=true&limit=100&cursor={cursor}");
                    request.Headers.Add("Cookie", $".ROBLOSECURITY={roblosecurity}");

                    var resp = await _client.SendAsync(request);
                    if (!resp.IsSuccessStatusCode) break;

                    var json = await resp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    if (!doc.RootElement.TryGetProperty("data", out var data)) break;
                    cursor = doc.RootElement.TryGetProperty("nextPageCursor", out var next) ? next.GetString() ?? "" : "";

                    foreach (var s in data.EnumerateArray())
                    {
                        int playing = s.GetProperty("playing").GetInt32();
                        int max = s.GetProperty("maxPlayers").GetInt32();
                        if (playing >= max) continue;

                        string jobId = s.GetProperty("id").GetString() ?? "";

                        var placeCache = _serverCache.GetOrAdd(placeId, _ => new ConcurrentDictionary<string, ServerInstance>());
                        if (placeCache.TryGetValue(jobId, out var cached) && cached.Region != "Unknown" && cached.Region != "Loading...")
                        {
                            cached.Playing = playing;
                            cached.MaxPlayers = max;
                            servers.Add(cached);
                        }
                        else
                        {
                            servers.Add(new ServerInstance
                            {
                                Id = jobId,
                                Playing = playing,
                                MaxPlayers = max,
                                Region = "Loading...",
                                Ping = 0,
                                PingColor = "#A0A0A0"
                            });
                        }
                    }

                    if (servers.Count >= maxServers) break;
                    if (string.IsNullOrEmpty(cursor)) break;
                }
                catch { break; }
            }

            return servers;
        }

        public async Task EnrichServersWithRegionsAsync(long placeId, List<ServerInstance> servers, string roblosecurity, Action? onBatchUpdated = null)
        {
            if (_datacenterIdToRegion == null) await LoadDatacentersAsync();

            var semaphore = new SemaphoreSlim(5);
            int updatedCount = 0;

            var tasks = servers.Where(s => s.Region == "Loading..." || s.Region == "Unknown").Select(server => Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var placeCache = _serverCache.GetOrAdd(placeId, _ => new ConcurrentDictionary<string, ServerInstance>());
                    if (placeCache.TryGetValue(server.Id, out var cached) && cached.Region != "Unknown" && cached.Region != "Loading...")
                    {
                        server.Region = cached.Region;
                        server.DataCenterId = cached.DataCenterId;
                        server.Ping = cached.Ping;
                        server.PingColor = cached.PingColor;
                        server.FirstSeen = cached.FirstSeen;
                        Interlocked.Increment(ref updatedCount);
                        return;
                    }

                    try
                    {
                        var resp = await _client.GetAsync(
                            $"https://apis.rovalra.com/v1/server_details?place_id={placeId}&server_ids={server.Id}");

                        if (resp.IsSuccessStatusCode)
                        {
                            var json = await resp.Content.ReadAsStringAsync();
                            using var doc = JsonDocument.Parse(json);

                            if (doc.RootElement.TryGetProperty("servers", out var srvArr) &&
                                srvArr.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var srv in srvArr.EnumerateArray())
                                {
                                    if (srv.TryGetProperty("datacenter_id", out var dcEl) && dcEl.TryGetInt32(out int dcId))
                                        server.DataCenterId = dcId;

                                    string city = "", country = "";
                                    if (srv.TryGetProperty("city", out var cEl)) city = cEl.GetString() ?? "";
                                    if (srv.TryGetProperty("country", out var coEl)) country = coEl.GetString() ?? "";

                                    if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(country))
                                        server.Region = $"{city}, {country}";
                                    else if (!string.IsNullOrEmpty(country))
                                        server.Region = country;

                                    if (srv.TryGetProperty("first_seen", out var fsEl) &&
                                        DateTime.TryParse(fsEl.GetString(), out DateTime fs))
                                        server.FirstSeen = fs;

                                    break;
                                }
                            }
                        }
                    }
                    catch { /* RoValra failed */ }

                    if (!server.DataCenterId.HasValue && server.Region == "Loading...")
                    {
                        try
                        {
                            var dcId = await GetDataCenterIdViaJoinAsync(placeId, server.Id, roblosecurity, maxAttempts: 1);
                            if (dcId.HasValue)
                            {
                                server.DataCenterId = dcId;
                                if (_datacenterIdToRegion != null && _datacenterIdToRegion.TryGetValue(dcId.Value, out var mapped))
                                    server.Region = mapped;
                            }
                        }
                        catch { }
                    }

                    if (server.Region == "Loading...") server.Region = "Unknown";

                    server.Ping = CalculateEstimatedPing(server.Region, out string color);
                    server.PingColor = color;

                    if (server.DataCenterId.HasValue && server.Region != "Unknown")
                        placeCache[server.Id] = server;

                    Interlocked.Increment(ref updatedCount);

                    if (updatedCount % 5 == 0)
                        onBatchUpdated?.Invoke();
                }
                finally
                {
                    semaphore.Release();
                }
            }));

            await Task.WhenAll(tasks);
            onBatchUpdated?.Invoke();
        }

        public async Task<List<ServerInstance>> FetchServersByRegionAsync(long placeId, string targetRegion, string roblosecurity, int maxPages = 12)
        {
            if (_datacenterIdToRegion == null) await LoadDatacentersAsync();

            var resultServers = new ConcurrentBag<ServerInstance>();
            string cursor = "";

            for (int i = 0; i < maxPages; i++)
            {
                var request = new HttpRequestMessage(HttpMethod.Get,
                    $"https://games.roblox.com/v1/games/{placeId}/servers/Public?sortOrder=2&excludeFullGames=true&limit=100&cursor={cursor}");
                request.Headers.Add("Cookie", $".ROBLOSECURITY={roblosecurity}");

                HttpResponseMessage resp;
                try { resp = await _client.SendAsync(request); }
                catch { break; }

                if (!resp.IsSuccessStatusCode) break;

                var json = await resp.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("data", out var data)) break;
                cursor = doc.RootElement.TryGetProperty("nextPageCursor", out var next) ? next.GetString() ?? "" : "";

                var serversInPage = new List<(string JobId, int Playing, int MaxPlayers)>();
                foreach (var s in data.EnumerateArray())
                {
                    int playing = s.GetProperty("playing").GetInt32();
                    int max = s.GetProperty("maxPlayers").GetInt32();
                    if (playing >= max) continue;

                    string jobId = s.GetProperty("id").GetString() ?? "";
                    serversInPage.Add((jobId, playing, max));
                }

                var semaphore = new SemaphoreSlim(3);
                var tasks = serversInPage.Select(sv => Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var placeCache = _serverCache.GetOrAdd(placeId, _ => new ConcurrentDictionary<string, ServerInstance>());
                        if (string.IsNullOrEmpty(cursor) && placeCache.TryGetValue(sv.JobId, out var cached) &&
                            cached.DataCenterId.HasValue && cached.Region != "Unknown")
                        {
                            cached.Playing = sv.Playing;
                            cached.MaxPlayers = sv.MaxPlayers;
                            return cached;
                        }

                        int? dataCenterId = null;
                        string region = "Unknown";
                        DateTime? firstSeen = null;

                        try
                        {
                            var roValraResp = await _client.GetAsync(
                                $"https://apis.rovalra.com/v1/server_details?place_id={placeId}&server_ids={sv.JobId}");

                            if (roValraResp.IsSuccessStatusCode)
                            {
                                var roJson = await roValraResp.Content.ReadAsStringAsync();
                                using var roDoc = JsonDocument.Parse(roJson);

                                if (roDoc.RootElement.TryGetProperty("servers", out var servers) &&
                                    servers.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var srv in servers.EnumerateArray())
                                    {
                                        if (srv.TryGetProperty("datacenter_id", out var dcIdEl) && dcIdEl.TryGetInt32(out int dcId))
                                            dataCenterId = dcId;

                                        string city = "", country = "";
                                        if (srv.TryGetProperty("city", out var cityEl))
                                            city = cityEl.GetString() ?? "";
                                        if (srv.TryGetProperty("country", out var countryEl))
                                            country = countryEl.GetString() ?? "";

                                        if (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(country))
                                            region = $"{city}, {country}";
                                        else if (!string.IsNullOrEmpty(country))
                                            region = country;

                                        if (srv.TryGetProperty("first_seen", out var fsEl))
                                        {
                                            if (DateTime.TryParse(fsEl.GetString(), out DateTime fs))
                                                firstSeen = fs;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                        catch { /* RoValra API failed — try fallback */ }

                        if (!dataCenterId.HasValue)
                        {
                            try
                            {
                                dataCenterId = await GetDataCenterIdViaJoinAsync(placeId, sv.JobId, roblosecurity);
                            }
                            catch { }
                        }

                        if (dataCenterId.HasValue && region == "Unknown" &&
                            _datacenterIdToRegion != null && _datacenterIdToRegion.TryGetValue(dataCenterId.Value, out var mappedRegion))
                        {
                            region = mappedRegion;
                        }

                        int ping = CalculateEstimatedPing(region, out string color);

                        var instance = new ServerInstance
                        {
                            Id = sv.JobId,
                            Playing = sv.Playing,
                            MaxPlayers = sv.MaxPlayers,
                            Region = region,
                            DataCenterId = dataCenterId,
                            Ping = ping,
                            PingColor = color,
                            FirstSeen = firstSeen
                        };

                        if (dataCenterId.HasValue && region != "Unknown")
                            placeCache[sv.JobId] = instance;

                        return instance;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));

                var results = await Task.WhenAll(tasks);
                foreach (var inst in results.Where(r => r != null))
                {
                    resultServers.Add(inst!);
                }

                if (resultServers.Count >= 50) break;
                if (string.IsNullOrEmpty(cursor)) break;
            }

            var allServers = resultServers.ToList();

            if (targetRegion == "Auto (Best)")
            {
                return allServers.OrderBy(s => s.Ping).ToList();
            }
            else
            {
                var filtered = allServers.Where(s => MatchesRegion(s.Region, targetRegion)).ToList();
                return filtered.OrderBy(s => s.Ping).ToList();
            }
        }

        private async Task<int?> GetDataCenterIdViaJoinAsync(long placeId, string jobId, string roblosecurity, int maxAttempts = 2)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://gamejoin.roblox.com/v1/join-game-instance");
                    request.Headers.Add("Referer", $"https://roblox.com/games/{placeId}");
                    request.Headers.Add("Origin", "https://roblox.com");
                    request.Headers.Add("Cookie", $".ROBLOSECURITY={roblosecurity}");

                    request.Content = new StringContent(JsonSerializer.Serialize(new
                    {
                        placeId = placeId,
                        isTeleport = false,
                        gameId = jobId,
                        gameJoinAttemptId = jobId
                    }), Encoding.UTF8, "application/json");

                    var resp = await _client.SendAsync(request);

                    if (resp.StatusCode == HttpStatusCode.Unauthorized || resp.StatusCode == HttpStatusCode.Forbidden)
                        return null;

                    if (resp.StatusCode == HttpStatusCode.TooManyRequests || (int)resp.StatusCode >= 500)
                    {
                        if (attempt < maxAttempts)
                            await Task.Delay(1000 * attempt);
                        continue;
                    }

                    if (!resp.IsSuccessStatusCode) return null;

                    var body = await resp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(body);

                    if (TryExtractDataCenterId(doc.RootElement, out int dcId))
                        return dcId;

                    if (doc.RootElement.TryGetProperty("joinScript", out var joinScript) &&
                        TryExtractDataCenterId(joinScript, out dcId))
                        return dcId;
                }
                catch
                {
                    if (attempt < maxAttempts)
                        await Task.Delay(500 * attempt);
                }
            }
            return null;
        }

        private bool TryExtractDataCenterId(JsonElement elem, out int dcId)
        {
            dcId = 0;
            if (elem.ValueKind != JsonValueKind.Object) return false;

            foreach (var prop in elem.EnumerateObject())
            {
                if ((prop.Name.Equals("DataCenterId", StringComparison.OrdinalIgnoreCase) ||
                     prop.Name.Equals("dc", StringComparison.OrdinalIgnoreCase)) &&
                    prop.Value.ValueKind == JsonValueKind.Number &&
                    prop.Value.TryGetInt32(out dcId))
                {
                    return true;
                }

                if (prop.Value.ValueKind == JsonValueKind.Number &&
                    (prop.Name.IndexOf("data", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     prop.Name.IndexOf("center", StringComparison.OrdinalIgnoreCase) >= 0) &&
                    prop.Value.TryGetInt32(out dcId))
                {
                    return true;
                }

                if (prop.Value.ValueKind == JsonValueKind.Object && TryExtractDataCenterId(prop.Value, out dcId))
                    return true;

                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    string str = prop.Value.GetString() ?? "";
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        if (int.TryParse(str, out int parsed))
                        {
                            dcId = parsed;
                            return true;
                        }
                        if (str.TrimStart().StartsWith("{") || str.TrimStart().StartsWith("["))
                        {
                            try
                            {
                                using var innerDoc = JsonDocument.Parse(str);
                                if (TryExtractDataCenterId(innerDoc.RootElement, out dcId))
                                    return true;
                            }
                            catch { }
                        }
                    }
                }
            }
            return false;
        }

        private static bool MatchesRegion(string serverRegion, string targetRegion)
        {
            if (string.IsNullOrEmpty(serverRegion) || serverRegion == "Unknown") return false;

            if (serverRegion.Contains(targetRegion, StringComparison.OrdinalIgnoreCase))
                return true;

            string targetCity = targetRegion.Split(',')[0].Trim();
            if (!string.IsNullOrEmpty(targetCity) && serverRegion.Contains(targetCity, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private int CalculateEstimatedPing(string region, out string color)
        {
            color = "#2ECC71";
            if (string.IsNullOrEmpty(region) || region == "Unknown") { color = "#FF6B6B"; return 999; }

            int ping = 300;
            string r = region.ToLowerInvariant();

            if (r.Contains("singapore") || r.Contains(", sg"))
                ping = new Random().Next(30, 60);
            else if (r.Contains("hong kong") || r.Contains(", hk"))
                ping = new Random().Next(40, 70);
            else if (r.Contains("tokyo") || r.Contains(", jp") || r.Contains("japan"))
                ping = new Random().Next(80, 120);
            else if (r.Contains("seoul") || r.Contains(", kr") || r.Contains("korea"))
                ping = new Random().Next(70, 110);
            else if (r.Contains("mumbai") || r.Contains(", in") || r.Contains("india"))
                ping = new Random().Next(100, 160);
            else if (r.Contains("sydney") || r.Contains(", au") || r.Contains("australia"))
                ping = new Random().Next(120, 180);
            else if (r.Contains("los angeles") || r.Contains("ashburn") || r.Contains("united states") || r.Contains(", us"))
                ping = new Random().Next(180, 250);
            else if (r.Contains("new york") || r.Contains("virginia") || r.Contains("newark"))
                ping = new Random().Next(220, 300);
            else if (r.Contains("chicago") || r.Contains("dallas"))
                ping = new Random().Next(200, 280);
            else if (r.Contains("frankfurt") || r.Contains(", de") || r.Contains("germany"))
                ping = new Random().Next(220, 280);
            else if (r.Contains("london") || r.Contains(", uk") || r.Contains(", gb"))
                ping = new Random().Next(230, 290);
            else if (r.Contains("paris") || r.Contains(", fr") || r.Contains("france"))
                ping = new Random().Next(200, 260);
            else if (r.Contains("são paulo") || r.Contains("sao paulo") || r.Contains(", br") || r.Contains("brazil"))
                ping = new Random().Next(450, 650);

            if (ping < 100) color = "#2ECC71";
            else if (ping < 200) color = "#F1C40F";
            else color = "#FF6B6B";

            return ping;
        }
    }
}
