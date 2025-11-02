using System;
using System.Linq;

namespace GradebookCore.Models {
    /// <summary>
    /// Represents a student enrolled in a course.
    /// </summary>
    [Serializable]
    public class Student : ISyncEntity {
        /// <summary>
        /// Local unique integer identifier for this student (internal system ID).
        /// </summary>
        public int Id {
            get; set;
        }

        /// <summary>
        /// Human-readable display name for the student.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identifier from the source system (originating platform).
        /// </summary>
        public string? SourceSystemId {
            get; set;
        }

        /// <summary>
        /// Name of the source system (e.g., "Moodle").
        /// </summary>
        public string? SourceSystem {
            get; set;
        }

        /// <summary>
        /// Identifier in the destination system (target platform).
        /// </summary>
        public string? DestinationSystemId {
            get; set;
        }

        /// <summary>
        /// Name of the destination system (e.g., "Google Classroom").
        /// </summary>
        public string? DestinationSystem {
            get; set;
        }

        /// <summary>
        /// Username associated with the student (e.g., login name).
        /// </summary>
        public string? Username {
            get; set;
        }

        /// <summary>
        /// Email address of the student.
        /// </summary>
        public string? Email {
            get; set;
        }

        /// <summary>
        /// Given name (first name) of the student.
        /// </summary>
        public string? GivenName {
            get; set;
        }

        /// <summary>
        /// Family name (last name) of the student.
        /// </summary>
        public string? FamilyName {
            get; set;
        }

        /// <summary>
        /// Convenience property combining given and family names.
        /// </summary>
        public string FullName =>
            string.Join(" ", new[] { GivenName, FamilyName }.Where(s => !string.IsNullOrWhiteSpace(s)));

        public override string ToString() => $"{FullName} ({Email})";
    }
}
