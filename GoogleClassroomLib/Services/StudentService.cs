using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleClassroomLib.Models;
using GoogleClassroomLib.Providers;
using Microsoft.Extensions.Logging;

namespace GoogleClassroomLib.Services {
    /// <summary>
    /// Provides higher-level operations and transformations for student data.
    /// </summary>
    public class StudentService {
        private readonly StudentProvider _provider;
        private readonly ILogger<StudentService> _logger;

        public StudentService(StudentProvider provider, ILogger<StudentService> logger) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves and sorts students alphabetically by name.
        /// </summary>
        public async Task<List<Student>> GetStudentsSortedAsync(string courseId) {
            _logger.LogInformation("Retrieving and sorting students for CourseId: {CourseId}", courseId);

            var students = await _provider.GetStudentsAsync(courseId);
            if(students == null || !students.Any()) {
                _logger.LogWarning("No students found for course {CourseId}.", courseId);
                return new List<Student>();
            }

            var sorted = students.OrderBy(s => s.Name).ToList();

            _logger.LogInformation("Returning {Count} sorted students for course {CourseId}.", sorted.Count, courseId);
            return sorted;
        }

        /// <summary>
        /// Retrieves only students with visible email addresses.
        /// </summary>
        public async Task<List<Student>> GetStudentsWithEmailsAsync(string courseId) {
            var students = await _provider.GetStudentsAsync(courseId);
            var filtered = students.Where(s => !string.IsNullOrWhiteSpace(s.Email)).ToList();

            _logger.LogInformation("Filtered {Count} students with visible emails for course {CourseId}.", filtered.Count, courseId);
            return filtered;
        }

        public static void SetFamilyAndGiven(Student student) {
            if(student == null || string.IsNullOrWhiteSpace(student.Name))
                return;
            var parts = student.Name.Trim().Split(' ');
            if(parts.Length > 1) {
                student.GivenName = parts[0];
                student.FamilyName = parts[1];
            }


        }
    }


}

