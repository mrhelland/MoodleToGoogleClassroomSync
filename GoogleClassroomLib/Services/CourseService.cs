using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleClassroomLib.Providers;
using Microsoft.Extensions.Logging;

namespace GoogleClassroomLib.Services {
    /// <summary>
    /// Provides higher-level business logic for Google Classroom courses.
    /// Wraps <see cref="CourseProvider"/> and adds transformations or filtering.
    /// </summary>
    public class CourseService {
        private readonly CourseProvider _provider;
        private readonly ILogger<CourseService> _logger;

        public CourseService(CourseProvider provider, ILogger<CourseService> logger) {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all courses and sorts them alphabetically.
        /// </summary>
        public async Task<List<Models.Course>> GetCoursesSortedAsync() {
            _logger.LogInformation("Retrieving courses via CourseProvider...");

            var courses = await _provider.GetCoursesAsync();

            if(courses == null || !courses.Any()) {
                _logger.LogWarning("No courses were retrieved.");
                return new List<Models.Course>();
            }

            var sorted = courses.OrderBy(c => c.DisplayName).ToList();

            _logger.LogInformation("Returning {Count} sorted courses.", sorted.Count);
            return sorted;
        }

        /// <summary>
        /// Retrieves only active courses (state = ACTIVE).
        /// </summary>
        public async Task<List<Models.Course>> GetActiveCoursesAsync() {
            var allCourses = await _provider.GetCoursesAsync();
            var active = allCourses.Where(c => c.State == "ACTIVE").ToList();

            _logger.LogInformation("Found {Count} active courses.", active.Count);
            return active;
        }
    }
}
