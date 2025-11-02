using MoodleLib.Models;
using MoodleLib.Providers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoodleLib.Services {
    /// <summary>
    /// Provides higher-level logic for working with Moodle grade items.
    /// </summary>
    public class MGradeItemService {
        private readonly MGradeItemProvider _provider;
        private readonly ILogger<MGradeItemService> _logger;

        public MGradeItemService(MGradeItemProvider provider, ILogger<MGradeItemService>? logger = null) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MGradeItemService>.Instance;
        }

        /// <summary>
        /// Gets all visible graded activities (assignments, quizzes, etc.) for a course and user.
        /// </summary>
        /// <param name="courseId">The Moodle course ID (string).</param>
        /// <param name="userId">The Moodle user ID (string).</param>
        /// <returns>List of visible <see cref="MGradeItem"/> representing module activities.</returns>
        public async Task<List<MGradeItem>> GetVisibleActivityGradeItemsAsync(string courseId, string userId) {
            var items = await _provider.GetGradeItemsAsync(courseId, userId);

            var filtered = items
                .Where(i => i.IsModule && !i.Hidden)
                .OrderBy(i => i.ItemName)
                .ToList();

            _logger.LogInformation("Filtered {Count} visible grade items (activities only) for CourseId={CourseId}.", filtered.Count, courseId);
            return filtered;
        }

        /// <summary>
        /// Builds a map of category ID → category name using 'category' type grade items.
        /// </summary>
        /// <param name="courseId">The Moodle course ID (string).</param>
        /// <param name="userId">The Moodle user ID (string).</param>
        /// <returns>Dictionary mapping category IDs (string) to category names.</returns>
        public async Task<Dictionary<string, string>> GetCategoryMapAsync(string courseId, string userId) {
            var items = await _provider.GetGradeItemsAsync(courseId, userId);

            // Convert numeric IDs to strings consistently
            var categories = items
                .Where(i => i.IsCategory)
                .ToDictionary(
                    i => i.Id.ToString(),
                    i => i.ItemName ?? "(Unnamed Category)"
                );

            _logger.LogInformation("Found {Count} grade categories in CourseId={CourseId}.", categories.Count, courseId);
            return categories;
        }

        /// <summary>
        /// Gets a list of visible activities grouped by their grade category name.
        /// </summary>
        /// <param name="courseId">The Moodle course ID (string).</param>
        /// <param name="userId">The Moodle user ID (string).</param>
        /// <returns>Dictionary of category name → list of <see cref="MGradeItem"/>.</returns>
        public async Task<Dictionary<string, List<MGradeItem>>> GetActivitiesByCategoryAsync(string courseId, string userId) {
            var items = await _provider.GetGradeItemsAsync(courseId, userId);

            // Build category map (categoryId → categoryName)
            var categoryMap = items
                .Where(i => i.IsCategory)
                .ToDictionary(
                    i => i.Id.ToString(),
                    i => i.ItemName ?? "(Unnamed Category)"
                );

            // Only visible module-type items (assignments, quizzes, etc.)
            var activities = items.Where(i => i.IsModule && !i.Hidden);

            // Group by category name, falling back to "(Uncategorized)"
            var grouped = activities
                .GroupBy(i => {
                    var categoryKey = i.CategoryId?.ToString() ?? string.Empty;
                    return categoryMap.TryGetValue(categoryKey, out var name)
                        ? name
                        : "(Uncategorized)";
                })
                .ToDictionary(g => g.Key, g => g.ToList());

            _logger.LogInformation(
                "Grouped {ActivityCount} visible activities into {CategoryCount} categories for CourseId={CourseId}.",
                activities.Count(), grouped.Count, courseId
            );

            return grouped;
        }
    }
}
