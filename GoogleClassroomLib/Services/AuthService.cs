using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Logging;

namespace GoogleClassroomLib.Services {
    /// <summary>
    /// Handles Google OAuth2 authentication and returns an authenticated ClassroomService instance.
    /// </summary>
    public class AuthService {
        private readonly string _credentialsPath;
        private readonly string _tokenStorePath;
        private readonly ILogger<AuthService> _logger;

        private readonly string[] _scopes = new[]
        {
            ClassroomService.Scope.ClassroomCoursesReadonly,
            ClassroomService.Scope.ClassroomRostersReadonly,
            ClassroomService.Scope.ClassroomProfileEmails
        };

        public AuthService(string credentialsPath, string tokenStorePath, ILogger<AuthService> logger) {
            _credentialsPath = credentialsPath ?? throw new ArgumentNullException(nameof(credentialsPath));
            _tokenStorePath = tokenStorePath ?? throw new ArgumentNullException(nameof(tokenStorePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Authenticates with Google using OAuth2 and returns an authorized ClassroomService instance.
        /// </summary>
        public async Task<ClassroomService> GetAuthenticatedServiceAsync() {
            try {
                _logger.LogInformation("Starting authentication process with Google Classroom API...");

                using var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read);
                var credPath = Path.Combine(_tokenStorePath, "token.json");

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    _scopes,
                    "teacher",
                    CancellationToken.None,
                    new FileDataStore(_tokenStorePath, true)
                );

                _logger.LogInformation("Authentication successful. Access token expires at {Expiration}.",
                    credential.Token.IssuedUtc.AddSeconds(credential.Token.ExpiresInSeconds ?? 0));

                var service = new ClassroomService(new BaseClientService.Initializer {
                    HttpClientInitializer = credential,
                    ApplicationName = "GoogleClassroomLib"
                });

                return service;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error during Google Classroom authentication.");
                throw;
            }
        }
    }
}
