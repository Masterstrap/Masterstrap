using System;
using System.Collections.Generic;
using System.Linq;

namespace Masterstrap.Services
{
    public static class PrivateServerLinkParser
    {
        public static bool TryNormalize(string? input, out string normalizedUrl, out string suggestedName, out long? extractedPlaceId, out string errorMessage)
        {
            normalizedUrl = string.Empty;
            suggestedName = "Private Server";
            extractedPlaceId = null;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Invalid link. Please enter a Roblox private server link.";
                return false;
            }

            string candidate = input.Trim();
            if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
            {
                errorMessage = "Invalid link. Please enter a valid URL.";
                return false;
            }

            if (!string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(uri.Host, "www.roblox.com", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Invalid link. Only Roblox links from https://www.roblox.com are supported.";
                return false;
            }

            string[] segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 &&
                string.Equals(segments[0], "games", StringComparison.OrdinalIgnoreCase) &&
                long.TryParse(segments[1], out var placeId))
            {
                var canonical = new UriBuilder("https", "www.roblox.com")
                {
                    Path = $"/games/{placeId}"
                };

                var query = ParseQuery(uri.Query);
                var queryParts = new List<string>();

                if (query.TryGetValue("privateServerLinkCode", out var privateServerCode) && !string.IsNullOrWhiteSpace(privateServerCode))
                {
                    queryParts.Add($"privateServerLinkCode={Uri.EscapeDataString(privateServerCode)}");
                }

                if (query.TryGetValue("jobId", out var jobId) && !string.IsNullOrWhiteSpace(jobId))
                {
                    queryParts.Add($"jobId={Uri.EscapeDataString(jobId)}");
                }
                else if (query.TryGetValue("instanceId", out var instanceId) && !string.IsNullOrWhiteSpace(instanceId))
                {
                    queryParts.Add($"instanceId={Uri.EscapeDataString(instanceId)}");
                }

                if (queryParts.Count > 0)
                {
                    canonical.Query = string.Join("&", queryParts);
                }

                normalizedUrl = canonical.Uri.AbsoluteUri;
                suggestedName = $"Place {placeId}";
                extractedPlaceId = placeId;
                return true;
            }

            if (string.Equals(uri.AbsolutePath, "/share", StringComparison.OrdinalIgnoreCase))
            {
                var query = ParseQuery(uri.Query);
                bool hasCode = query.TryGetValue("code", out var shareCode) && !string.IsNullOrWhiteSpace(shareCode);
                bool hasType = query.TryGetValue("type", out var type) && !string.IsNullOrWhiteSpace(type);

                if (!hasCode || !hasType)
                {
                    errorMessage = "Invalid link. Supported share link format is /share?code=...&type=...";
                    return false;
                }

                var canonical = new UriBuilder("https", "www.roblox.com")
                {
                    Path = "/share",
                    Query = $"code={Uri.EscapeDataString(shareCode)}&type={Uri.EscapeDataString(type)}"
                };

                normalizedUrl = canonical.Uri.AbsoluteUri;
                suggestedName = $"Share {type}";
                return true;
            }

            errorMessage = "Invalid link. Supported formats are Roblox /games private server links and /share experience invite links.";
            return false;
        }

        public static bool TryBuildLaunchArgument(string? input, out string launchArgument, out string errorMessage)
        {
            launchArgument = string.Empty;
            errorMessage = string.Empty;

            if (!TryNormalize(input, out var normalized, out _, out _, out var validationError))
            {
                errorMessage = validationError;
                return false;
            }

            if (!Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
            {
                errorMessage = "Invalid link. Could not parse launch URL.";
                return false;
            }

            var query = ParseQuery(uri.Query);
            string[] segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2 &&
                string.Equals(segments[0], "games", StringComparison.OrdinalIgnoreCase) &&
                long.TryParse(segments[1], out var placeId))
            {
                launchArgument = $"roblox://experiences/start?placeId={placeId}";

                if (query.TryGetValue("privateServerLinkCode", out var privateServerCode) && !string.IsNullOrWhiteSpace(privateServerCode))
                {
                    launchArgument += $"&linkCode={Uri.EscapeDataString(privateServerCode)}";
                }

                string instanceId = "";
                if (query.TryGetValue("instanceId", out var id1) && !string.IsNullOrWhiteSpace(id1)) instanceId = id1;
                else if (query.TryGetValue("jobId", out var id2) && !string.IsNullOrWhiteSpace(id2)) instanceId = id2;

                if (!string.IsNullOrEmpty(instanceId))
                {
                    launchArgument += $"&gameInstanceId={Uri.EscapeDataString(instanceId)}";
                }

                return true;
            }

            if (string.Equals(uri.AbsolutePath, "/share", StringComparison.OrdinalIgnoreCase) &&
                query.TryGetValue("code", out var shareCode) &&
                !string.IsNullOrWhiteSpace(shareCode) &&
                query.TryGetValue("type", out var type) &&
                !string.IsNullOrWhiteSpace(type))
            {
                launchArgument =
                    $"roblox://navigation/share_links?type={Uri.EscapeDataString(type)}&code={Uri.EscapeDataString(shareCode)}";
                return true;
            }

            errorMessage = "Invalid link. Unsupported Roblox link format for direct in-app launch.";
            return false;
        }

        private static Dictionary<string, string> ParseQuery(string query)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(query))
                return result;

            string q = query.TrimStart('?');
            foreach (var pair in q.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                int idx = pair.IndexOf('=');
                if (idx <= 0)
                    continue;

                string key = Uri.UnescapeDataString(pair.Substring(0, idx));
                string value = Uri.UnescapeDataString(pair[(idx + 1)..]);
                if (!string.IsNullOrWhiteSpace(key))
                    result[key] = value;
            }

            return result;
        }
    }
}
