namespace MoodleLib.Models;

public class Course {
    public int Id {
        get; set;
    }
    public string ShortName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
}
