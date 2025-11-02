using System.Xml.Linq;

namespace MoodleLib.Models {
    /// <summary>
    /// Represents a student enrolled in a Moodle course.
    /// </summary>
    public class MStudent {

        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string GivenName { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = "ACTIVE";

        /// <summary>
        /// A read-only formatted label for display.
        /// </summary>
        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(FullName) ? Username : FullName;

        public override string ToString() => DisplayLabel;
    }
}
