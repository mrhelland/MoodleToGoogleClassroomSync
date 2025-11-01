using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace GoogleClassroomLib.Utils {
    /// <summary>
    /// Provides application configuration for GoogleClassroomLib.
    /// Loads all settings exclusively from appsettings.json located in the host application's directory.
    /// </summary>
    public static class AppConfig {
        private static IConfigurationRoot _config;

        static AppConfig() {
            try {
                // Use the host application's base directory (not the library's)
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                _config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
            }
            catch(Exception ex) {
                Console.Error.WriteLine($"[AppConfig] Failed to load appsettings.json: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows a host (e.g., WinForms, Console) to inject its own configuration if it has already built one.
        /// </summary>
        public static void Initialize(IConfigurationRoot externalConfig) {
            _config = externalConfig ?? throw new ArgumentNullException(nameof(externalConfig));
        }

        /// <summary>
        /// Gets the full path to the Google API credentials file (credentials.json).
        /// </summary>
        public static string? GetGoogleCredentialsPath()
            => _config["GoogleApi:CredentialsPath"];

        /// <summary>
        /// Gets the full path to the Google API token store directory.
        /// </summary>
        public static string? GetTokenStorePath()
            => _config["GoogleApi:TokenStorePath"];

        /// <summary>
        /// Gets the full path to the application's log file.
        /// </summary>
        public static string? GetLogFilePath()
            => _config["Logging:LogFilePath"];
    }
}
