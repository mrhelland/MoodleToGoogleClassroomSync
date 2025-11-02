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
    public class GCourseProvider {
        private readonly ClassroomService _service;
        private readonly ILogger<GCourseProvider> _logger;

        public GCourseProvider(ClassroomService service, ILogger<GCourseProvider> logger) {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<GCourse>> GetAllCoursesAsync() {
            var result = new List<GCourse>();

            try {
                _logger.LogInformation("Requesting all courses from Google Classroom API...");
                var request = _service.Courses.List();
                request.PageSize = 100;

                var response = await request.ExecuteAsync();

                if(response.Courses != null) {
                    foreach(var c in response.Courses) {
                        result.Add(new GCourse {
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

        public async Task<GCourse?> GetCourseByIdAsync(string courseId) {
            try {
                _logger.LogInformation("Requesting details for Google Classroom course ID {CourseId}", courseId);

                var request = _service.Courses.Get(courseId);
                var course = await request.ExecuteAsync();

                if(course == null) {
                    _logger.LogWarning("No course found for ID {CourseId}", courseId);
                    return null;
                }

                return new GCourse {
                    Id = course.Id,
                    Name = course.Name,
                    Section = course.Section,
                    Description = course.Description,
                    OwnerEmail = course.OwnerId,
                    State = course.CourseState,
                    CreationTime = course.CreationTimeDateTimeOffset?.UtcDateTime,
                    UpdateTime = course.UpdateTimeDateTimeOffset?.UtcDateTime
                };
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving course by ID {CourseId}", courseId);
                return null;
            }
        }


    }
}
