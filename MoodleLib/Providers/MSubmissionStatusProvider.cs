using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;

namespace MoodleLib.Providers {
    /// <summary>
    /// Provides access to Moodle assignment submission status information via REST API.
    /// </summary>
    public class MSubmissionStatusProvider {
        private readonly MApiProvider _api;
        private readonly ILogger<MSubmissionStatusProvider> _logger;

        public MSubmissionStatusProvider(MApiProvider api, ILogger<MSubmissionStatusProvider>? logger = null) {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MSubmissionStatusProvider>.Instance;
        }

        /// <summary>
        /// Retrieves the submission status for a specific assignment and (optionally) user.
        /// </summary>
        /// <param name="assignmentId">The assignment ID as a string.</param>
        /// <param name="userId">Optional user ID as a string.</param>
        /// <returns>A populated <see cref="MSubmissionStatus"/> instance or null if unavailable.</returns>
        public async Task<MSubmissionStatus?> GetSubmissionStatusAsync(string assignmentId, string? userId = null) {
            try {
                var parameters = new Dictionary<string, string> {
                    { "assignid", assignmentId }
                };
                if(!string.IsNullOrWhiteSpace(userId))
                    parameters["userid"] = userId;

                _logger.LogInformation("Fetching submission status for assignment {AssignmentId}, user {UserId}", assignmentId, userId ?? "(none)");

                var response = await _api.GetAsync<dynamic>("mod_assign_get_submission_status", parameters);
                if(response == null) {
                    _logger.LogWarning("Empty response for assignment {AssignmentId}, user {UserId}", assignmentId, userId);
                    return null;
                }

                var lastAttempt = response.lastattempt;
                var submission = lastAttempt?.submission;
                var feedback = response.feedback;
                var assign = response.assign;

                var model = new MSubmissionStatus {
                    AssignmentId = assignmentId,
                    UserId = userId,
                    SubmissionState = submission?.status ?? "unknown",
                    GradingState = lastAttempt?.gradingstatus ?? "notgraded",
                    TimeSubmitted = UnixToDateTime(submission?.timemodified),
                    TimeGraded = UnixToDateTime(feedback?.gradeddate),
                    Grade = feedback?.grade,
                    FeedbackText = feedback?.plugins?[0]?.editorfields?[0]?.text,
                    DueDate = UnixToDateTime(assign?.duedate),
                    CutoffDate = UnixToDateTime(assign?.cutoffdate)
                };

                if(model.DueDate.HasValue && model.TimeSubmitted.HasValue)
                    model.IsLate = model.TimeSubmitted > model.DueDate;

                return model;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving submission status for assignment {AssignmentId}, user {UserId}", assignmentId, userId);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all assignment IDs for a specific course.
        /// </summary>
        /// <param name="courseId">The course ID as a string.</param>
        /// <returns>List of assignment IDs (as strings) belonging to the specified course.</returns>
        public async Task<List<string>> GetAssignmentIdsForCourseAsync(string courseId) {
            try {
                var parameters = new Dictionary<string, string> {
                    { "courseids[0]", courseId }
                };

                _logger.LogInformation("Fetching assignments for course {CourseId}", courseId);
                var response = await _api.GetAsync<dynamic>("mod_assign_get_assignments", parameters);
                var result = new List<string>();

                if(response == null || response.courses == null)
                    return result;

                foreach(var course in response.courses) {
                    foreach(var assign in course.assignments) {
                        if(assign.id != null)
                            result.Add(assign.id.ToString());
                    }
                }

                _logger.LogInformation("Found {Count} assignments in course {CourseId}", result.Count, courseId);
                return result;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error retrieving assignments for course {CourseId}", courseId);
                return new List<string>();
            }
        }

        private static DateTime? UnixToDateTime(object? unix) {
            if(unix == null)
                return null;
            if(long.TryParse(unix.ToString(), out var ts) && ts > 0)
                return DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime;
            return null;
        }
    }
}
