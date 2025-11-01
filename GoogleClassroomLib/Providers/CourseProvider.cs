using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1;
using GoogleClassroomLib.Models;
using Microsoft.Extensions.Logging;

namespace GoogleClassroomLib.Providers {
    /// <summary>
    /// Provides low-level access to Google Classroom course data.
    /// Responsible only for communicating with the Classroom API.
    /// </summary>
    public class CourseProvider {
        private readonly ClassroomService _service;
        private readonly ILogger<CourseProvider> _logger;

        public CourseProvider(ClassroomService service, ILogger<CourseProvider> logger) {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all Google Classroom courses visible to the authenticated teacher.
        /// You can filter for active courses and/or partial name matches.
        /// </summary>
        /// <param name="activeCourses">If true, returns only ACTIVE courses.</param>
        /// <param name="nameContains">If not null or empty, returns courses whose name contains this text (case-insensitive).</param>
        public async Task<List<Course>> GetCoursesAsync(bool activeCourses = true, string nameContains = null) {
            var result = new List<Course>();

            try {
                _logger.LogInformation("Requesting course list from Google Classroom API...");

                var request = _service.Courses.List();
                request.PageSize = 100;

                var response = await request.ExecuteAsync();

                if(response.Courses != null) {
                    IEnumerable<Google.Apis.Classroom.v1.Data.Course> filtered = response.Courses;

                    // Filter by active state
                    if(activeCourses) {
                        filtered = filtered.Where(c => string.Equals(c.CourseState, "ACTIVE", StringComparison.OrdinalIgnoreCase));
                    }

                    // Filter by name substring
                    if(!string.IsNullOrWhiteSpace(nameContains)) {
                        filtered = filtered.Where(c =>
                            !string.IsNullOrEmpty(c.Name) &&
                            c.Name.IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0);
                    }

                    foreach(var c in filtered) {
                        result.Add(new Course {
                            Id = c.Id,
                            Name = c.Name,
                            Section = c.Section,
                            Description = c.Description,
                            OwnerEmail = c.OwnerId,
                            State = c.CourseState,
                            CreationTime = c.CreationTimeDateTimeOffset?.UtcDateTime,
                            UpdateTime = c.UpdateTimeDateTimeOffset?.UtcDateTime
                        });
                    }

                    _logger.LogInformation("Retrieved {Count} courses after filtering (active={Active}, nameContains='{Filter}').",
                        result.Count, activeCourses, nameContains);
                }
                else {
                    _logger.LogWarning("No courses returned by the API (empty or null response).");
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving courses from Google Classroom API.");
            }

            return result;
        }
    }
}
