using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using MoodleLib.Utils;

namespace MoodleLib.Providers;

public class MoodleApiProvider {
    private readonly HttpClient _client;
    private readonly MoodleAuthProvider _auth;
    private readonly ILogger<MoodleApiProvider> _logger;

    public MoodleApiProvider(MoodleAuthProvider auth, ILogger<MoodleApiProvider>? logger = null) {
        _auth = auth;
        _client = HttpClientFactoryUtil.Create();
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MoodleApiProvider>.Instance;
    }

    public async Task<T?> GetAsync<T>(string functionName, Dictionary<string, string>? parameters = null) {
        var url = _auth.BuildServiceUrl(functionName);
        if(parameters != null && parameters.Count > 0) {
            var query = string.Join("&", parameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
            url += "&" + query;
        }

        _logger.LogInformation("GET {Url}", url);
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        _logger.LogDebug("Raw API response: {Content}", content);

        try {
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch(JsonException ex) {
            _logger.LogError(ex, "Failed to deserialize JSON: {Content}", content);
            throw;
        }
    }
}
