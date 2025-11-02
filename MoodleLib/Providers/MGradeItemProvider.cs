using MoodleLib.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoodleLib.Providers {
    /// <summary>
    /// Provides low-level access to Moodle grade items via REST API.
    /// </summary>
    public class MGradeItemProvider {
        private readonly MApiProvider _api;
        private readonly ILogger<MGradeItemProvider> _logger;

        public MGradeItemProvider(MApiProvider api, ILogger<MGradeItemProvider>? logger = null) {
            _api = api;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MGradeItemProvider>.Instance;
        }

        /// <summary>
        /// Retrieves all grade items (including categories and course totals) for a given user in a course.
        /// </summary>
        /// <param name="courseId">The Moodle course ID (string).</param>
        /// <param name="userId">The Moodle user ID (string).</param>
        /// <returns>List of <see cref="MGradeItem"/> for the given course and user.</returns>
        public async Task<List<MGradeItem>> GetGradeItemsAsync(string courseId, string userId) {
            _logger.LogInformation("Fetching grade items for CourseId={CourseId}, UserId={UserId}", courseId, userId);

            var parameters = new Dictionary<string, string> {
                { "courseid", courseId },
                { "userid", userId }
            };

            var result = await _api.GetAsync<MoodleGradeItemResponse>("gradereport_user_get_grade_items", parameters);

            if(result?.GradeItems == null) {
                _logger.LogWarning("No grade items returned for CourseId={CourseId}, UserId={UserId}", courseId, userId);
                return new List<MGradeItem>();
            }

            _logger.LogInformation("Retrieved {Count} grade items for CourseId={CourseId}, UserId={UserId}", result.GradeItems.Count, courseId, userId);
            return result.GradeItems;
        }

        /// <summary>
        /// Retrieves all grade items for a given Moodle course, independent of any specific user.
        /// </summary>
        /// <param name="courseId">The Moodle course ID (string).</param>
        /// <returns>List of <see cref="MGradeItem"/> representing all gradeable items in the course.</returns>
        public async Task<List<MGradeItem>> GetGradeItemsForCourseAsync(string courseId) {
            _logger.LogInformation("Fetching all grade items for CourseId={CourseId}", courseId);

            var parameters = new Dictionary<string, string> {
                { "courseid", courseId },
                { "userid", "0" } // Moodle treats 0 as “default user” or “course context”
            };

            var response = await _api.GetAsync<MoodleUserGradesEnvelope>("gradereport_user_get_grade_items", parameters);

            if(response?.UserGrades == null || response.UserGrades.Count == 0) {
                _logger.LogWarning("No grade items found for CourseId={CourseId}", courseId);
                return new List<MGradeItem>();
            }

            var gradeItems = response.UserGrades.FirstOrDefault()?.GradeItems ?? new List<MGradeItem>();

            _logger.LogInformation("Retrieved {Count} grade items for CourseId={CourseId}", gradeItems.Count, courseId);
            return gradeItems;
        }

        // Existing private response model for GetGradeItemsAsync
        private class MoodleGradeItemResponse {
            [Newtonsoft.Json.JsonProperty("gradeitems")]
            public List<MGradeItem> GradeItems { get; set; } = new();
        }

        // Internal model for the course-level version (matches Moodle’s JSON)
        private class MoodleUserGradesEnvelope {
            [Newtonsoft.Json.JsonProperty("usergrades")]
            public List<MoodleUserGrade> UserGrades { get; set; } = new();
        }

        private class MoodleUserGrade {
            [Newtonsoft.Json.JsonProperty("courseid")]
            public int CourseId {
                get; set;
            }

            [Newtonsoft.Json.JsonProperty("userid")]
            public int UserId {
                get; set;
            }

            [Newtonsoft.Json.JsonProperty("gradeitems")]
            public List<MGradeItem> GradeItems { get; set; } = new();
        }
    }
}
