using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Classroom.v1;
using Microsoft.Extensions.Logging;
using Google.Apis.Classroom.v1.Data;

namespace GoogleClassroomLib.Providers {
    /// <summary>
    /// Provides low-level access to student roster data from Google Classroom.
    /// Responsible only for communicating with the Classroom API.
    /// </summary>
    public class GStudentProvider {
        private readonly ClassroomService _service;
        private readonly ILogger<GStudentProvider> _logger;

        public GStudentProvider(ClassroomService service, ILogger<GStudentProvider> logger) {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves raw Google Classroom student objects for the specified course.
        /// </summary>
        public async Task<IList<Student>> GetGoogleStudentsAsync(string courseId) {
            var result = new List<Student>();

            try {
                _logger.LogInformation("Requesting raw student roster from Google Classroom API for CourseId: {CourseId}", courseId);

                var request = _service.Courses.Students.List(courseId);
                request.PageSize = 100;

                var response = await request.ExecuteAsync();

                if(response.Students != null) {
                    result.AddRange(response.Students);
                    _logger.LogInformation("Retrieved {Count} students from API for course {CourseId}.", result.Count, courseId);
                }
                else {
                    _logger.LogWarning("No students returned by API for CourseId {CourseId}.", courseId);
                }
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving students from Google Classroom API for CourseId {CourseId}", courseId);
            }

            return result;
        }
    }
}
