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
    public class CourseProvider {
        private readonly MoodleApiProvider _api;
        private readonly ILogger<CourseProvider> _logger;

        public CourseProvider(MoodleApiProvider api, ILogger<CourseProvider> logger) {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<CourseProvider>.Instance;
        }

        /// <summary>
        /// Retrieves all Moodle courses visible to the authenticated user.
        /// </summary>
        public async Task<List<Course>> GetAllCoursesAsync() {
            const string functionName = "core_course_get_courses";

            try {
                _logger.LogInformation("Fetching all Moodle courses via REST API...");
                var response = await _api.GetAsync<List<dynamic>>(functionName);

                var result = new List<Course>();

                if(response != null) {
                    foreach(var c in response) {
                        result.Add(new Course {
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
                return new List<Course>();
            }
        }
    }
}
