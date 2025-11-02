using Microsoft.Extensions.Logging;

namespace MoodleLib.Providers;

public class MAuthProvider {
    private readonly string _baseUrl;
    private readonly string _token;
    private readonly ILogger<MAuthProvider> _logger;

    public MAuthProvider(string baseUrl, string token, ILogger<MAuthProvider>? logger = null) {
        _baseUrl = baseUrl.TrimEnd('/');
        _token = token;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MAuthProvider>.Instance;
    }

    public string BuildServiceUrl(string functionName) {
        var url = $"{_baseUrl}/webservice/rest/server.php?wstoken={_token}&moodlewsrestformat=json&wsfunction={functionName}";
        _logger.LogDebug("Constructed Moodle service URL: {Url}", url);
        return url;
    }
}
