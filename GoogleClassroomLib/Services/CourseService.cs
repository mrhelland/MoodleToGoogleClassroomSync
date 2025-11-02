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

        public async Task<List<Models.Course>> GetCoursesAsync(bool onlyActive = true, string nameContains = null, bool sortByName = true) {
            _logger.LogInformation("Retrieving courses via CourseProvider...");
            var courses = await _provider.GetAllCoursesAsync();

            if(courses == null || courses.Count == 0) {
                _logger.LogWarning("No courses retrieved.");
                return new List<Models.Course>();
            }

            IEnumerable<Models.Course> filtered = courses;

            if(onlyActive)
                filtered = filtered.Where(c => string.Equals(c.State, "ACTIVE", StringComparison.OrdinalIgnoreCase));

            if(!string.IsNullOrWhiteSpace(nameContains))
                filtered = filtered.Where(c => c.Name?.IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0);

            if(sortByName)
                filtered = filtered.OrderBy(c => c.Name);

            var finalList = filtered.ToList();
            _logger.LogInformation("Returning {Count} courses after filtering.", finalList.Count);
            return finalList;
        }
    }
}
