using MoodleLib.Models;
using MoodleLib.Providers;
using Microsoft.Extensions.Logging;

namespace MoodleLib.Services;

public class CourseService {
    private readonly MoodleApiProvider _api;
    private readonly ILogger<CourseService> _logger;

    public CourseService(MoodleApiProvider api, ILogger<CourseService>? logger = null) {
        _api = api;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<CourseService>.Instance;
    }

    public async Task<List<Course>> GetAllCoursesAsync() {
        var result = await _api.GetAsync<List<Course>>("core_course_get_courses");
        _logger.LogInformation("Retrieved {Count} courses", result?.Count ?? 0);
        return result ?? new List<Course>();
    }
}
