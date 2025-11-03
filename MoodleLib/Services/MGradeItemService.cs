//using MoodleLib.Models;
//using MoodleLib.Providers;
//using Microsoft.Extensions.Logging;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MoodleLib.Services {
//    /// <summary>
//    /// Provides higher-level logic for working with Moodle grade items.
//    /// </summary>
//    public class MGradeItemService {
//        private readonly MGradeItemProvider _gradeItemProvider;
//        private readonly MSubmissionStatusService _submissionStatusService;
//        private readonly ILogger<MGradeItemService> _logger;

//        public MGradeItemService(MGradeItemProvider provider,
//            MSubmissionStatusService submissionStatusService,
//            ILogger<MGradeItemService>? logger = null) 
//        {
//            _gradeItemProvider = provider ?? throw new ArgumentNullException(nameof(provider));
//            _submissionStatusService = submissionStatusService;
//            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MGradeItemService>.Instance;
//        }

//        /// <summary>
//        /// Gets all visible graded activities (assignments, quizzes, etc.) for a course and user.
//        /// </summary>
//        /// <param name="courseId">The Moodle course ID (string).</param>
//        /// <param name="userId">The Moodle user ID (string).</param>
//        /// <returns>List of visible <see cref="MGradeItem"/> representing module activities.</returns>
//        public async Task<List<MGradeItem>> GetVisibleActivityGradeItemsAsync(string courseId, string userId) {
//            var items = await _gradeItemProvider.GetGradeItemsAsync(courseId, userId);

//            var filtered = items
//                .Where(i => i.IsModule && !i.Hidden)
//                .OrderBy(i => i.ItemName)
//                .ToList();

//            _logger.LogInformation("Filtered {Count} visible grade items (activities only) for CourseId={CourseId}.", filtered.Count, courseId);
//            return filtered;
//        }

//        /// <summary>
//        /// Builds a map of category ID → category name using 'category' type grade items.
//        /// </summary>
//        /// <param name="courseId">The Moodle course ID (string).</param>
//        /// <param name="userId">The Moodle user ID (string).</param>
//        /// <returns>Dictionary mapping category IDs (string) to category names.</returns>
//        public async Task<Dictionary<string, string>> GetCategoryMapAsync(string courseId, string userId) {
//            var items = await _gradeItemProvider.GetGradeItemsAsync(courseId, userId);

//            // Convert numeric IDs to strings consistently
//            var categories = items
//                .Where(i => i.IsCategory)
//                .ToDictionary(
//                    i => i.Id.ToString(),
//                    i => i.ItemName ?? "(Unnamed Category)"
//                );

//            _logger.LogInformation("Found {Count} grade categories in CourseId={CourseId}.", categories.Count, courseId);
//            return categories;
//        }

//        /// <summary>
//        /// Gets a list of visible activities grouped by their grade category name.
//        /// </summary>
//        /// <param name="courseId">The Moodle course ID (string).</param>
//        /// <param name="userId">The Moodle user ID (string).</param>
//        /// <returns>Dictionary of category name → list of <see cref="MGradeItem"/>.</returns>
//        public async Task<Dictionary<string, List<MGradeItem>>> GetActivitiesByCategoryAsync(string courseId, string userId) {
//            var items = await _gradeItemProvider.GetGradeItemsAsync(courseId, userId);

//            // Build category map (categoryId → categoryName)
//            var categoryMap = items
//                .Where(i => i.IsCategory)
//                .ToDictionary(
//                    i => i.Id.ToString(),
//                    i => i.ItemName ?? "(Unnamed Category)"
//                );

//            // Only visible module-type items (assignments, quizzes, etc.)
//            var activities = items.Where(i => i.IsModule && !i.Hidden);

//            // Group by category name, falling back to "(Uncategorized)"
//            var grouped = activities
//                .GroupBy(i => {
//                    var categoryKey = i.CategoryId?.ToString() ?? string.Empty;
//                    return categoryMap.TryGetValue(categoryKey, out var name)
//                        ? name
//                        : "(Uncategorized)";
//                })
//                .ToDictionary(g => g.Key, g => g.ToList());

//            var courseTotal = items.FirstOrDefault(i => i.IsCourseTotal);
//            if(courseTotal != null)
//                grouped["(Course Total)"] = new List<MGradeItem> { courseTotal };

//            _logger.LogInformation(
//                "Grouped {ActivityCount} visible activities into {CategoryCount} categories for CourseId={CourseId}.",
//                activities.Count(), grouped.Count, courseId
//            );

//            return grouped;
//        }

//        /// <summary>
//        /// Retrieves all grade items for a user in a course and enriches them with
//        /// submission state information (submitted, draft, graded, etc.).
//        /// </summary>
//        public async Task<List<MGradeItem>> GetGradeItemsWithSubmissionStatusAsync(string courseId, string userId) {
//            _logger.LogInformation("Fetching grade items with submission states for CourseId={CourseId}, UserId={UserId}", courseId, userId);

//            var items = await _gradeItemProvider.GetGradeItemsAsync(courseId, userId);
//            if(items == null || items.Count == 0) {
//                _logger.LogWarning("No grade items found for CourseId={CourseId}, UserId={UserId}", courseId, userId);
//                return new List<MGradeItem>();
//            }

//            var assignItems = items.Where(i => i.ItemModule == "assign" && i.ItemInstance != null).ToList();
//            if(assignItems.Count == 0) {
//                foreach(var gi in items)
//                    gi.SubmissionState = "not_submitted";
//                return items;
//            }

//            // ✅ Use service, not provider
//            var submissionStatuses = await _submissionStatusService.GetCourseStatusesForUserAsync(courseId, userId);
//            var flatStatuses = submissionStatuses ?? new List<MSubmissionStatus>();

//            foreach(var gi in assignItems) {
//                var match = flatStatuses.FirstOrDefault(s => s.AssignmentId == gi.ItemInstanceString);
//                gi.SubmissionState = NormalizeSubmissionState(match?.SubmissionState);
//            }

//            foreach(var gi in items)
//                gi.SubmissionState ??= "not_submitted";

//            _logger.LogInformation("Enriched {Count} grade items with submission states for CourseId={CourseId}.", assignItems.Count, courseId);
//            return items;
//        }


//        /// <summary>
//        /// Normalizes Moodle submission states to consistent, human-readable values.
//        /// </summary>
//        private static string NormalizeSubmissionState(string? rawState) {
//            if(string.IsNullOrWhiteSpace(rawState))
//                return "not_submitted";

//            return rawState.ToLowerInvariant() switch {
//                "new" => "not_submitted",
//                "draft" => "draft",
//                "submitted" => "submitted",
//                "reopened" => "reopened",
//                "graded" => "graded",
//                _ => "not_submitted"
//            };
//        }
//    }
//}
using MoodleLib.Models;
using MoodleLib.Providers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MoodleLib.Services {
    /// <summary>
    /// Provides higher-level logic for working with Moodle grade items.
    /// Automatically enriches results with submission state information
    /// and filters out hidden items.
    /// </summary>
    public class MGradeItemService {
        private readonly MGradeItemProvider _gradeItemProvider;
        private readonly MSubmissionStatusService _submissionStatusService;
        private readonly ILogger<MGradeItemService> _logger;

        public MGradeItemService(
            MGradeItemProvider gradeItemProvider,
            MSubmissionStatusService submissionStatusService,
            ILogger<MGradeItemService>? logger = null) {

            _gradeItemProvider = gradeItemProvider ?? throw new ArgumentNullException(nameof(gradeItemProvider));
            _submissionStatusService = submissionStatusService ?? throw new ArgumentNullException(nameof(submissionStatusService));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MGradeItemService>.Instance;
        }

        // ===============================================================
        // === Public Methods ============================================
        // ===============================================================

        /// <summary>
        /// Gets all visible graded activities (assignments, quizzes, etc.) for a course and user,
        /// enriched with submission states.
        /// </summary>
        public async Task<List<MGradeItem>> GetVisibleActivityGradeItemsAsync(string courseId, string userId) {
            var items = await GetEnrichedGradeItemsAsync(courseId, userId);

            var visibleActivities = items
                .Where(i => i.IsModule && !i.Hidden)
                .OrderBy(i => i.ItemName)
                .ToList();

            _logger.LogInformation(
                "Retrieved {Count} visible activity grade items (enriched) for CourseId={CourseId}, UserId={UserId}.",
                visibleActivities.Count, courseId, userId
            );

            return visibleActivities;
        }

        /// <summary>
        /// Builds a map of category ID → category name using visible 'category' type grade items.
        /// </summary>
        public async Task<Dictionary<string, string>> GetCategoryMapAsync(string courseId, string userId) {
            var items = await GetEnrichedGradeItemsAsync(courseId, userId);

            var categories = items
                .Where(i => i.IsCategory && !i.Hidden)
                .ToDictionary(
                    i => i.IdString,
                    i => i.ItemName ?? "(Unnamed Category)"
                );

            _logger.LogInformation("Built category map with {Count} visible categories for CourseId={CourseId}.", categories.Count, courseId);
            return categories;
        }

        /// <summary>
        /// Gets visible activities grouped by their grade category name,
        /// enriched with submission states.
        /// </summary>
        public async Task<Dictionary<string, List<MGradeItem>>> GetActivitiesByCategoryAsync(string courseId, string userId) {
            var items = await GetEnrichedGradeItemsAsync(courseId, userId);

            var categoryMap = items
                .Where(i => i.IsCategory && !i.Hidden)
                .ToDictionary(
                    i => i.IdString,
                    i => i.ItemName ?? "(Unnamed Category)"
                );

            var visibleActivities = items.Where(i => i.IsModule && !i.Hidden);

            var grouped = visibleActivities
                .GroupBy(i => {
                    var key = i.CategoryIdString ?? string.Empty;
                    return categoryMap.TryGetValue(key, out var name) ? name : "(Uncategorized)";
                })
                .ToDictionary(g => g.Key, g => g.ToList());

            // Optionally include Course Total
            var courseTotal = items.FirstOrDefault(i => i.IsCourseTotal && !i.Hidden);
            if(courseTotal != null)
                grouped["(Course Total)"] = new List<MGradeItem> { courseTotal };

            _logger.LogInformation(
                "Grouped {ActivityCount} visible activities into {CategoryCount} categories for CourseId={CourseId}, UserId={UserId}.",
                visibleActivities.Count(), grouped.Count, courseId, userId
            );

            return grouped;
        }

        /// <summary>
        /// Retrieves all visible grade items for a user in a course,
        /// enriched with submission state information (submitted, draft, graded, etc.).
        /// </summary>
        public async Task<List<MGradeItem>> GetGradeItemsWithSubmissionStatusAsync(string courseId, string userId) {
            var items = await GetEnrichedGradeItemsAsync(courseId, userId);

            var visible = items.Where(i => !i.Hidden).ToList();
            _logger.LogInformation("Retrieved {Count} visible enriched grade items for CourseId={CourseId}, UserId={UserId}.",
                visible.Count, courseId, userId);

            return visible;
        }

        // ===============================================================
        // === Private Helpers ===========================================
        // ===============================================================

        /// <summary>
        /// Fetches all grade items and populates submission states for assign-type modules.
        /// Always ensures every item has a valid SubmissionState.
        /// </summary>
        private async Task<List<MGradeItem>> GetEnrichedGradeItemsAsync(string courseId, string userId) {
            _logger.LogInformation("Fetching and enriching grade items for CourseId={CourseId}, UserId={UserId}", courseId, userId);

            var items = await _gradeItemProvider.GetGradeItemsAsync(courseId, userId);
            if(items == null || items.Count == 0) {
                _logger.LogWarning("No grade items found for CourseId={CourseId}, UserId={UserId}", courseId, userId);
                return new List<MGradeItem>();
            }

            // Retrieve submission statuses for all assign-type modules
            var assignItems = items.Where(i => i.ItemModule == "assign" && i.ItemInstance != null).ToList();
            if(assignItems.Count > 0) {
                var submissionStatuses = await _submissionStatusService.GetCourseStatusesForUserAsync(courseId, userId);
                var flatStatuses = submissionStatuses ?? new List<MSubmissionStatus>();
                
                Debug.WriteLine("Assign items: {List}", string.Join(",", assignItems.Select(a => a.ItemInstanceString)));
                Debug.WriteLine("Submission statuses: {List}", string.Join(",", flatStatuses.Select(s => s.AssignmentId)));

                foreach(var gi in assignItems) {
                    var match = flatStatuses.FirstOrDefault(s => s.AssignmentId == gi.ItemInstanceString);
                    gi.SubmissionState = NormalizeSubmissionState(match?.SubmissionState);
                }
            }

            // Apply default state to all items without one
            foreach(var gi in items)
                gi.SubmissionState ??= "not_submitted";

            return items;
        }

        /// <summary>
        /// Normalizes Moodle submission states to consistent, human-readable values.
        /// </summary>
        private static string NormalizeSubmissionState(string? rawState) {
            if(string.IsNullOrWhiteSpace(rawState))
                return "not_submitted";

            return rawState.ToLowerInvariant() switch {
                "new" => "not_submitted",
                "draft" => "draft",
                "submitted" => "submitted",
                "reopened" => "reopened",
                "graded" => "graded",
                _ => "not_submitted"
            };
        }
    }
}
