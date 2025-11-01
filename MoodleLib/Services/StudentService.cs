using MoodleLib.Models;
using MoodleLib.Providers;
using Microsoft.Extensions.Logging;

namespace MoodleLib.Services;

public class StudentService {
    private readonly MoodleApiProvider _api;
    private readonly ILogger<StudentService> _logger;

    public StudentService(MoodleApiProvider api, ILogger<StudentService>? logger = null) {
        _api = api;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<StudentService>.Instance;
    }

    public async Task<List<Student>> GetEnrolledStudentsAsync(int courseId) {
        var parameters = new Dictionary<string, string> { ["courseid"] = courseId.ToString() };
        var result = await _api.GetAsync<List<Student>>("core_enrol_get_enrolled_users", parameters);
        _logger.LogInformation("Retrieved {Count} students for course {CourseId}", result?.Count ?? 0, courseId);
        return result ?? new List<Student>();
    }
}
