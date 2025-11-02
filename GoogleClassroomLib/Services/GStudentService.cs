using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1.Data;
using GoogleClassroomLib.Providers;
using Microsoft.Extensions.Logging;

namespace GoogleClassroomLib.Services {
    /// <summary>
    /// Provides higher-level business logic for student data.
    /// Wraps <see cref="GStudentProvider"/> and performs data transformation and filtering.
    /// </summary>
    public class GStudentService {
        private readonly GStudentProvider _provider;
        private readonly ILogger<GStudentService> _logger;

        public GStudentService(GStudentProvider provider, ILogger<GStudentService> logger) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all students for a course, transforming API data into Student models.
        /// Optionally filters out students without email addresses.
        /// </summary>
        public async Task<List<Models.GStudent>> GetStudentsAsync(string courseId, bool excludeNoEmail = false) {
            _logger.LogInformation("Retrieving student roster for course {CourseId}...", courseId);

            var apiStudents = await _provider.GetGoogleStudentsAsync(courseId);
            var result = new List<Models.GStudent>();

            foreach(var s in apiStudents) {
                var email = s.Profile?.EmailAddress;
                if(excludeNoEmail && string.IsNullOrWhiteSpace(email)) {
                    _logger.LogWarning("Excluding student {Name} ({Id}) due to missing email.", s.Profile?.Name?.FullName, s.Profile?.Id);
                    continue;
                }

                if(string.IsNullOrWhiteSpace(email)) {
                    _logger.LogWarning("Student {Name} ({Id}) has no visible email address.", s.Profile?.Name?.FullName, s.Profile?.Id);
                }

                result.Add(new Models.GStudent {
                    Id = s.Profile?.Id,
                    Name = s.Profile?.Name?.FullName ?? "(unknown)",
                    Email = email,
                    EnrollmentStatus = string.IsNullOrEmpty(s.UserId) ? "UNKNOWN" : "ACTIVE",
                    CourseId = courseId
                });
            }

            _logger.LogInformation("Returning {Count} transformed students for course {CourseId}.", result.Count, courseId);
            return result;
        }

        /// <summary>
        /// Retrieves students sorted by last name.
        /// </summary>
        public async Task<List<Models.GStudent>> GetStudentsSortedAsync(string courseId, bool excludeNoEmail = false) {
            var students = await GetStudentsAsync(courseId, excludeNoEmail);
            return students.OrderBy(s => GetLastName(s.Name)).ToList();
        }

        private static string GetLastName(string fullName) {
            if(string.IsNullOrWhiteSpace(fullName))
                return string.Empty;
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[^1] : parts[0];
        }
    }
}
