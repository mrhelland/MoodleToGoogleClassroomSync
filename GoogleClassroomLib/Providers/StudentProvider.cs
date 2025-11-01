using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1;
using Microsoft.Extensions.Logging;

namespace GoogleClassroomLib.Providers {
    /// <summary>
    /// Provides low-level access to student roster data from Google Classroom.
    /// </summary>
    public class StudentProvider {
        private readonly ClassroomService _service;
        private readonly ILogger<StudentProvider> _logger;

        public StudentProvider(ClassroomService service, ILogger<StudentProvider> logger) {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all students enrolled in a specified Google Classroom course.
        /// </summary>
        public async Task<List<Models.Student>> GetStudentsAsync(string courseId) {
            var result = new List<Models.Student>();

            try {
                _logger.LogInformation("Requesting student roster for CourseId: {CourseId}", courseId);

                var request = _service.Courses.Students.List(courseId);
                request.PageSize = 100;

                var response = await request.ExecuteAsync();

                if(response.Students != null) {
                    foreach(var s in response.Students) {
                        var email = s.Profile?.EmailAddress;
                        if(string.IsNullOrWhiteSpace(email)) {
                            _logger.LogWarning("Student {Name} ({Id}) has no visible email.", s.Profile?.Name?.FullName, s.Profile?.Id);
                        }

                        result.Add(new Models.Student {
                            Id = s.Profile?.Id,
                            Name = s.Profile?.Name?.FullName ?? "(unknown)",
                            Email = email,
                            EnrollmentStatus = s.UserId == null ? "UNKNOWN" : "ACTIVE",
                            CourseId = courseId
                        });
                    }

                    _logger.LogInformation("Retrieved {Count} students for course {CourseId}.", result.Count, courseId);
                }
                else {
                    _logger.LogWarning("No students returned for CourseId {CourseId}.", courseId);
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving students for course {CourseId}", courseId);
            }

            return result;
        }
    }
}
