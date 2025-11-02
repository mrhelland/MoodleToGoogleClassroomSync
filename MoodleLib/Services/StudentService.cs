using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;
using MoodleLib.Providers;
using MoodleLib.Utils;

namespace MoodleLib.Services {
    /// <summary>
    /// Provides higher-level business logic for Moodle student data.
    /// Wraps <see cref="StudentProvider"/> and adds transformations or sorting.
    /// </summary>
    public class StudentService {
        private readonly StudentProvider _provider;
        private readonly ILogger<StudentService> _logger;

        public StudentService(StudentProvider provider, ILogger<StudentService> logger) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StudentService>.Instance;
        }

        /// <summary>
        /// Retrieves students for a course and parses their names into given/family names.
        /// </summary>
        public async Task<List<Student>> GetStudentsAsync(string courseId, bool excludeNoEmail = false) {
            _logger.LogInformation("Retrieving students for course {CourseId}...", courseId);

            var students = await _provider.GetEnrolledStudentsAsync(courseId);
            var result = new List<Student>();

            foreach(var s in students) {
                if(excludeNoEmail && string.IsNullOrWhiteSpace(s.Email))
                    continue;

                result.Add(s);
            }

            _logger.LogInformation("Returning {Count} processed students for course {CourseId}.", result.Count, courseId);
            return result;
        }

        /// <summary>
        /// Retrieves students sorted by last name.
        /// </summary>
        public async Task<List<Student>> GetStudentsSortedAsync(string courseId, bool excludeNoEmail = false) {
            var students = await GetStudentsAsync(courseId, excludeNoEmail);
            return students.OrderBy(s => s.FamilyName).ThenBy(s => s.GivenName).ToList();
        }
    }
}
