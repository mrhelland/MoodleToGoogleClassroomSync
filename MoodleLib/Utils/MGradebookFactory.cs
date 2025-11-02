using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradebookCore.Models;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;
using MoodleLib.Providers;
using MoodleLib.Services;

namespace MoodleLib.Utils {
    /// <summary>
    /// Builds a complete GradebookCore.Gradebook for a given Moodle course.
    /// </summary>
    public class MGradebookFactory {
        private readonly MAuthProvider _mAuth;
        private readonly MApiProvider _mApi;
        private readonly ILogger<MGradebookFactory> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private readonly MCourseProvider _mCourseProvider;
        private readonly MStudentProvider _mStudentProvider;
        private readonly MSubmissionStatusProvider _mSubmissionProvider;
        private readonly MSubmissionStatusService _mSubmissionService;
        private readonly MGradeItemProvider _mGradeItemProvider;

        public MGradebookFactory(
            MAuthProvider mAuth,
            MApiProvider mApi,
            ILoggerFactory loggerFactory
        ) {
            _mAuth = mAuth ?? throw new ArgumentNullException(nameof(mAuth));
            _mApi = mApi ?? throw new ArgumentNullException(nameof(mApi));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));

            _logger = _loggerFactory.CreateLogger<MGradebookFactory>();

            // Initialize providers with typed loggers
            _mCourseProvider = new MCourseProvider(mApi, _loggerFactory.CreateLogger<MCourseProvider>());
            _mStudentProvider = new MStudentProvider(mApi, _loggerFactory.CreateLogger<MStudentProvider>());
            _mSubmissionProvider = new MSubmissionStatusProvider(mApi, _loggerFactory.CreateLogger<MSubmissionStatusProvider>());
            _mSubmissionService = new MSubmissionStatusService(_mSubmissionProvider, _loggerFactory.CreateLogger<MSubmissionStatusService>());
            _mGradeItemProvider = new MGradeItemProvider(mApi, _loggerFactory.CreateLogger<MGradeItemProvider>());
        }

        /// <summary>
        /// Builds a GradebookCore.Gradebook using Moodle data.
        /// </summary>
        public async Task<Gradebook> GetGradebookAsync(string moodleCourseId) {
            _logger.LogInformation("Building Gradebook for Moodle course {CourseId}", moodleCourseId);

            var moodleCourse = await _mCourseProvider.GetCourseByIdAsync(moodleCourseId);
            if(moodleCourse == null)
                throw new InvalidOperationException($"Course with ID {moodleCourseId} not found.");

            var gradebook = new Gradebook {
                CourseId = $"moodle_course_{moodleCourse.Id}",
                CourseName = moodleCourse.FullName,
                SourceSystem = "Moodle"
            };

            // === 1️⃣ Retrieve enrolled students ===
            _logger.LogInformation("Retrieving enrolled students for course {CourseId}", moodleCourseId);
            var moodleStudents = await _mStudentProvider.GetEnrolledStudentsAsync(moodleCourseId);

            var students = moodleStudents.Select(s => new Student {
                Id = 0, // Temporary; Gradebook.EnsureUniqueIds() will replace with unique int
                SourceSystem = "Moodle",
                SourceSystemId = s.Id.ToString(),
                Username = s.Username,
                Email = s.Email,
                GivenName = s.GivenName,
                FamilyName = s.FamilyName
            }).ToList();

            gradebook.Students = students;

            // === 2️⃣ Retrieve grade items (assignments + categories) ===
            _logger.LogInformation("Retrieving grade items for course {CourseId}", moodleCourseId);
            var gradeItems = await _mGradeItemProvider.GetGradeItemsForCourseAsync(moodleCourseId);

            // Build categories
            var categories = gradeItems
                .Where(g => g.ItemType == "category")
                .Select(gi => new Category {
                    Id = 0, // assigned later by EnsureUniqueIds()
                    Name = gi.ItemName,
                    SourceSystem = "Moodle",
                    SourceSystemId = gi.Id.ToString(),
                    WeightPercent = ParseWeight(gi.WeightFormatted)
                })
                .ToList();

            // === 3️⃣ Retrieve assignments ===
            _logger.LogInformation("Retrieving assignments for course {CourseId}", moodleCourseId);
            var assignmentIds = await _mSubmissionProvider.GetAssignmentIdsForCourseAsync(moodleCourseId);
            var assignments = new List<Assignment>();

            foreach(var aId in assignmentIds) {
                var gi = gradeItems.FirstOrDefault(
                    g => g.ItemModule == "assign" && g.ItemInstance?.ToString() == aId
                );


                // Try to find the corresponding category by comparing Moodle's numeric CategoryId
                Category? cat = null;
                if(gi?.CategoryId != null) {
                    var catIdString = gi.CategoryId.ToString();
                    cat = categories.FirstOrDefault(c => c.SourceSystemId == catIdString);
                }

                assignments.Add(new Assignment {
                    Id = 0, // assigned later by EnsureUniqueIds()
                    Name = gi?.ItemName ?? $"Assignment {aId}",
                    SourceSystem = "Moodle",
                    SourceSystemId = aId.ToString(),
                    MaxPoints = gi?.GradeMax ?? 100,
                    CategoryId = cat != null ? cat.Id.ToString() : null // ✅ fixed
                });
            }

            gradebook.Assignments = assignments;
            gradebook.Categories = categories;


            // === 3️⃣ Retrieve submission and grading statuses ===
            var gradeCells = new List<GradeCell>();
            int nextCellId = 1;

            foreach(var student in students) {
                var statuses = await _mSubmissionService.GetCourseStatusesForUserAsync(moodleCourseId, student.SourceSystemId);
                foreach(var s in statuses) {
                    var assignment = assignments.FirstOrDefault(a => a.SourceSystemId == s.AssignmentId.ToString());
                    if(assignment == null)
                        continue;

                    gradeCells.Add(new GradeCell {
                        Id = nextCellId++,
                        Student = student,
                        Assignment = assignment,
                        Grade = s.Grade,
                        Submitted = s.IsSubmitted,
                        Graded = s.IsGraded,
                        Late = s.IsLate ?? false,
                        Feedback = s.FeedbackText,
                        TimeSubmitted = s.TimeSubmitted,
                        TimeGraded = s.TimeGraded
                    });
                }
            }

            gradebook.Entries = gradeCells;

            _logger.LogInformation(
                "Completed Gradebook for course {CourseId}. Students={StudentCount}, Assignments={AssignmentCount}, Entries={EntryCount}",
                moodleCourseId, gradebook.Students.Count, gradebook.Assignments.Count, gradeCells.Count
            );

            return gradebook;
        }

        private static double? ParseWeight(string? weightFormatted) {
            if(string.IsNullOrWhiteSpace(weightFormatted))
                return null;
            if(weightFormatted.EndsWith("%"))
                weightFormatted = weightFormatted.TrimEnd('%');
            return double.TryParse(weightFormatted, out var w) ? w : null;
        }
    }
}
