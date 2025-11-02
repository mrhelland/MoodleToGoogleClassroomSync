using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;

namespace MoodleLib.Providers {
    /// <summary>
    /// Provides low-level access to student enrollment data from Moodle.
    /// </summary>
    public class StudentProvider {
        private readonly MoodleApiProvider _api;
        private readonly ILogger<StudentProvider> _logger;

        public StudentProvider(MoodleApiProvider api, ILogger<StudentProvider> logger) {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StudentProvider>.Instance;
        }

        /// <summary>
        /// Retrieves all students enrolled in a specified Moodle course.
        /// </summary>
        public async Task<List<Student>> GetEnrolledStudentsAsync(string courseId) {
            const string functionName = "core_enrol_get_enrolled_users";
            var parameters = new Dictionary<string, string> { ["courseid"] = courseId };

            var result = new List<Student>();

            try {
                _logger.LogInformation("Fetching enrolled students for course {CourseId}...", courseId);

                var response = await _api.GetAsync<List<dynamic>>(functionName, parameters);

                if(response != null) {
                    foreach(var s in response) {
                        result.Add(new Student {
                            Id = s.id?.ToString() ?? string.Empty,
                            FullName = s.fullname ?? string.Empty,
                            Email = s.email ?? string.Empty,
                            CourseId = courseId,
                            EnrollmentStatus = "ACTIVE"
                        });
                    }
                }

                _logger.LogInformation("Retrieved {Count} enrolled students for course {CourseId}.", result.Count, courseId);
                return result;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving students for course {CourseId}", courseId);
                return new List<Student>();
            }
        }
    }
}
