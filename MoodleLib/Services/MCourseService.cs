using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MoodleLib.Models;
using MoodleLib.Providers;

namespace MoodleLib.Services {
    /// <summary>
    /// Provides higher-level business logic for Moodle courses.
    /// Wraps <see cref="MCourseProvider"/> and applies filtering or sorting.
    /// </summary>
    public class MCourseService {
        private readonly MCourseProvider _provider;
        private readonly ILogger<MCourseService> _logger;

        public MCourseService(MCourseProvider provider, ILogger<MCourseService> logger) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<MCourseService>.Instance;
        }

        /// <summary>
        /// Retrieves all courses sorted alphabetically by full name.
        /// </summary>
        public async Task<List<MCourse>> GetCoursesSortedAsync() {
            var courses = await _provider.GetAllCoursesAsync();
            var sorted = courses.OrderBy(c => c.FullName).ToList();

            _logger.LogInformation("Returning {Count} sorted courses.", sorted.Count);
            return sorted;
        }

        /// <summary>
        /// Retrieves only visible (active) courses.
        /// </summary>
        public async Task<List<MCourse>> GetVisibleCoursesAsync() {
            var courses = await _provider.GetAllCoursesAsync();
            var visible = courses.Where(c => c.Visible).ToList();

            _logger.LogInformation("Found {Count} visible courses.", visible.Count);
            return visible;
        }

        /// <summary>
        /// Retrieves courses whose name contains the given substring (case-insensitive).
        /// </summary>
        public async Task<List<MCourse>> SearchCoursesByNameAsync(string search) {
            var courses = await _provider.GetAllCoursesAsync();
            var filtered = courses
                .Where(c => !string.IsNullOrWhiteSpace(c.FullName) &&
                            c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.FullName)
                .ToList();

            _logger.LogInformation("Found {Count} courses matching '{Search}'.", filtered.Count, search);
            return filtered;
        }
    }
}
