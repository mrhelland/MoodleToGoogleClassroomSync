using System.Net.Http;
using System.Net;

namespace MoodleLib.Utils;

public static class HttpClientFactoryUtil {
    public static HttpClient Create() {
        var handler = new HttpClientHandler {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        var client = new HttpClient(handler);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MoodleLib/1.0");
        return client;
    }
}
