using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;

namespace MoodleLib.Providers {
    /// <summary>
    /// Provides low-level access to Moodle course data via the REST API.
    /// Responsible only for communicating with the API and returning raw models.
    /// </summary>
    public class MCourseProvider {
        private readonly MApiProvider _api;
        private readonly ILogger<MCourseProvider> _logger;

        public MCourseProvider(MApiProvider api, ILogger<MCourseProvider> logger) {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MCourseProvider>.Instance;
        }

        /// <summary>
        /// Retrieves all Moodle courses visible to the authenticated user.
        /// </summary>
        public async Task<List<MCourse>> GetAllCoursesAsync() {
            const string functionName = "core_course_get_courses";

            try {
                _logger.LogInformation("Fetching all Moodle courses via REST API...");
                var response = await _api.GetAsync<List<dynamic>>(functionName);

                var result = new List<MCourse>();

                if(response != null) {
                    foreach(var c in response) {
                        result.Add(new MCourse {
                            Id = c.id?.ToString() ?? string.Empty,
                            ShortName = c.shortname ?? string.Empty,
                            FullName = c.fullname ?? string.Empty,
                            Summary = c.summary ?? string.Empty,
                            Category = c.categoryid?.ToString() ?? string.Empty,
                            IdNumber = c.idnumber ?? string.Empty,
                            Format = c.format ?? string.Empty,
                            Visible = c.visible == 1
                        });
                    }
                }

                _logger.LogInformation("Retrieved {Count} courses from Moodle.", result.Count);
                return result;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving courses from Moodle.");
                return new List<MCourse>();
            }
        }

        /// <summary>
        /// Retrieves a single Moodle course by its numeric ID.
        /// </summary>
        /// <param name="courseId">The Moodle course ID.</param>
        /// <returns>A single <see cref="MCourse"/> object or null if not found.</returns>
        public async Task<MCourse?> GetCourseByIdAsync(string courseId) {
            const string functionName = "core_course_get_courses";
            try {
                _logger.LogInformation("Fetching Moodle course with ID {CourseId} via REST API...", courseId);

                // The Moodle API allows filtering by IDs array
                var parameters = new Dictionary<string, string> {
                    { "options[ids][0]", courseId }
                };

                var response = await _api.GetAsync<List<dynamic>>(functionName, parameters);

                if(response == null || response.Count == 0) {
                    _logger.LogWarning("No Moodle course found for ID {CourseId}", courseId);
                    return null;
                }

                var c = response.First();

                var course = new MCourse {
                    Id = c.id?.ToString() ?? string.Empty,
                    ShortName = c.shortname ?? string.Empty,
                    FullName = c.fullname ?? string.Empty,
                    Summary = c.summary ?? string.Empty,
                    Category = c.categoryid?.ToString() ?? string.Empty,
                    IdNumber = c.idnumber ?? string.Empty,
                    Format = c.format ?? string.Empty,
                    Visible = c.visible == 1
                };

                _logger.LogInformation("Successfully retrieved course '{FullName}' (ID {CourseId})", course.FullName, courseId);
                return course;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving Moodle course with ID {CourseId}", courseId);
                return null;
            }
        }

    }
}
