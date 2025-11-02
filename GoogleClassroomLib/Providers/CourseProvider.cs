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

        public async Task<List<Course>> GetAllCoursesAsync() {
            var result = new List<Course>();

            try {
                _logger.LogInformation("Requesting all courses from Google Classroom API...");
                var request = _service.Courses.List();
                request.PageSize = 100;

                var response = await request.ExecuteAsync();

                if(response.Courses != null) {
                    foreach(var c in response.Courses) {
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
                }
                else {
                    _logger.LogWarning("No courses returned by the API.");
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving courses from Google Classroom API.");
            }

            return result;
        }

    }
}
