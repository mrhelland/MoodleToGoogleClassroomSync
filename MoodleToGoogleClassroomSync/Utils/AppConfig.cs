using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;

namespace MoodleToGoogleClassroomSync.Utils {
    /// <summary>
    /// Loads configuration settings for Google Classroom and Moodle integrations.
    /// Reads secure credentials (user + token) from external JSON files.
    /// </summary>
    public static class AppConfig {
        private static readonly IConfigurationRoot _config;

        static AppConfig() {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        // === GOOGLE ===
        public static string GetGoogleCredentialsPath()
            => _config["Google:CredentialsPath"]
               ?? throw new InvalidOperationException("Missing Google:CredentialsPath in appsettings.json.");

        public static string GetTokenStorePath()
            => _config["Google:TokenStorePath"]
               ?? throw new InvalidOperationException("Missing Google:TokenStorePath in appsettings.json.");

        // === MOODLE (Base URL and credentials) ===
        public static string GetMoodleBaseUrl()
            => _config["Moodle:BaseUrl"]
               ?? throw new InvalidOperationException("Missing Moodle:BaseUrl in appsettings.json.");

        private static string GetMoodleCredentialsFilePath()
            => _config["Moodle:CredentialsPath"]
               ?? throw new InvalidOperationException("Missing Moodle:CredentialsPath in appsettings.json.");

        public static string GetLoggingPath()
            => _config["Logging:LogFilePath"]
               ?? throw new InvalidOperationException("Missing Logging:LogFilePath in appsettings.json");

        /// <summary>
        /// Reads the Moodle service credentials from the external JSON credentials file.
        /// </summary>
        public static (string User, string Token) GetMoodleCredentials() {
            var path = GetMoodleCredentialsFilePath();

            if(!File.Exists(path))
                throw new FileNotFoundException($"Moodle credentials file not found: {path}");

            var json = File.ReadAllText(path);
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement.GetProperty("MoodleServiceAccount");

            var user = root.GetProperty("User").GetString() ?? string.Empty;
            var token = root.GetProperty("Token").GetString() ?? string.Empty;

            if(string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException($"Moodle credentials file missing required fields: {path}");

            return (user, token);
        }

        /// <summary>
        /// Convenience shortcut if only the token is needed.
        /// </summary>
        public static string GetMoodleToken() {
            var creds = GetMoodleCredentials();
            return creds.Token;
        }

        /// <summary>
        /// Convenience shortcut if only the username is needed.
        /// </summary>
        public static string GetMoodleUser() {
            var creds = GetMoodleCredentials();
            return creds.User;
        }
    }
}
