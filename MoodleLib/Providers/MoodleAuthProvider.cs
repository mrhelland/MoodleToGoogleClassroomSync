using Microsoft.Extensions.Logging;

namespace MoodleLib.Providers;

public class MoodleAuthProvider {
    private readonly string _baseUrl;
    private readonly string _token;
    private readonly ILogger<MoodleAuthProvider> _logger;

    public MoodleAuthProvider(string baseUrl, string token, ILogger<MoodleAuthProvider>? logger = null) {
        _baseUrl = baseUrl.TrimEnd('/');
        _token = token;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MoodleAuthProvider>.Instance;
    }

    public string BuildServiceUrl(string functionName) {
        var url = $"{_baseUrl}/webservice/rest/server.php?wstoken={_token}&moodlewsrestformat=json&wsfunction={functionName}";
        _logger.LogDebug("Constructed Moodle service URL: {Url}", url);
        return url;
    }
}
