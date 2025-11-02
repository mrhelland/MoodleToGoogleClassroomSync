using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;
using MoodleLib.Providers;

namespace MoodleLib.Services {
    /// <summary>
    /// Provides higher-level access to Moodle assignment submission status data.
    /// Wraps the <see cref="MSubmissionStatusProvider"/> with aggregation and summarization logic.
    /// </summary>
    public class MSubmissionStatusService {
        private readonly MSubmissionStatusProvider _provider;
        private readonly ILogger<MSubmissionStatusService> _logger;

        public MSubmissionStatusService(MSubmissionStatusProvider provider, ILogger<MSubmissionStatusService>? logger = null) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MSubmissionStatusService>.Instance;
        }

        /// <summary>
        /// Retrieves all submission statuses for a specific user across multiple assignments.
        /// </summary>
        /// <param name="assignmentIds">Collection of assignment IDs (as strings).</param>
        /// <param name="userId">User ID (as string).</param>
        public async Task<List<MSubmissionStatus>> GetStatusesForUserAsync(IEnumerable<string> assignmentIds, string userId) {
            var result = new List<MSubmissionStatus>();

            foreach(var id in assignmentIds) {
                var status = await _provider.GetSubmissionStatusAsync(id, userId);
                if(status != null)
                    result.Add(status);
            }

            _logger.LogInformation("Retrieved {Count} submission statuses for user {UserId}", result.Count, userId);
            return result;
        }

        /// <summary>
        /// Retrieves a human-readable summary string for a user's submission status for a specific assignment.
        /// </summary>
        /// <param name="assignmentId">Assignment ID (as string).</param>
        /// <param name="userId">User ID (as string).</param>
        public async Task<string> GetStatusSummaryAsync(string assignmentId, string userId) {
            var s = await _provider.GetSubmissionStatusAsync(assignmentId, userId);
            if(s == null)
                return "Unknown";

            if(!s.IsSubmitted)
                return "Not submitted";
            if(s.IsGraded)
                return $"Graded ({s.Grade})";
            if(s.IsLate == true)
                return "Submitted late";
            return "Submitted, awaiting grading";
        }

        /// <summary>
        /// Retrieves all submission statuses for all assignments in a course for a single user.
        /// </summary>
        /// <param name="courseId">Course ID (as string).</param>
        /// <param name="userId">User ID (as string).</param>
        public async Task<List<MSubmissionStatus>> GetCourseStatusesForUserAsync(string courseId, string userId) {
            var assignmentIds = await _provider.GetAssignmentIdsForCourseAsync(courseId);
            if(assignmentIds.Count == 0) {
                _logger.LogWarning("No assignments found in course {CourseId}", courseId);
                return new List<MSubmissionStatus>();
            }

            var statuses = await GetStatusesForUserAsync(assignmentIds, userId);
            _logger.LogInformation("Retrieved {Count} submission statuses for course {CourseId}, user {UserId}", statuses.Count, courseId, userId);

            return statuses;
        }

        /// <summary>
        /// Retrieves a dictionary of userId → list of submission statuses for all users in a course.
        /// </summary>
        /// <param name="courseId">Course ID (as string).</param>
        /// <param name="userIds">Collection of user IDs (as strings).</param>
        public async Task<Dictionary<string, List<MSubmissionStatus>>> GetCourseStatusesForUsersAsync(string courseId, IEnumerable<string> userIds) {
            var assignmentIds = await _provider.GetAssignmentIdsForCourseAsync(courseId);
            var result = new Dictionary<string, List<MSubmissionStatus>>();

            foreach(var userId in userIds) {
                var statuses = await GetStatusesForUserAsync(assignmentIds, userId);
                result[userId] = statuses;
                _logger.LogInformation("Retrieved {Count} submission statuses for user {UserId} in course {CourseId}", statuses.Count, userId, courseId);
            }

            return result;
        }
    }
}
