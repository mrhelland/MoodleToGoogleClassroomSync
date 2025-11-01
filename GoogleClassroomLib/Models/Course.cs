namespace GoogleClassroomLib.Models {
    /// <summary>
    /// Represents a Google Classroom course with minimal metadata needed
    /// for synchronization and display in external applications.
    /// </summary>
    public class Course {
        /// <summary>
        /// The unique identifier assigned by Google Classroom.
        /// Example: "123456789012".
        /// </summary>
        public string Id {
            get; set;
        }

        /// <summary>
        /// The full name of the course (e.g., "Algebra 1").
        /// </summary>
        public string Name {
            get; set;
        }

        /// <summary>
        /// The section or subdivision of the course (optional).
        /// Example: "Period 3" or "Section A".
        /// </summary>
        public string Section {
            get; set;
        }

        /// <summary>
        /// The textual description of the course, if available.
        /// </summary>
        public string Description {
            get; set;
        }

        /// <summary>
        /// The owner (teacher) email address associated with the course.
        /// </summary>
        public string OwnerEmail {
            get; set;
        }

        /// <summary>
        /// The course state (ACTIVE, ARCHIVED, PROVISIONED, DECLINED, SUSPENDED).
        /// </summary>
        public string State {
            get; set;
        }

        /// <summary>
        /// The creation time in UTC, if provided by the API.
        /// </summary>
        public DateTime? CreationTime {
            get; set;
        }

        /// <summary>
        /// The last update time in UTC, if provided by the API.
        /// </summary>
        public DateTime? UpdateTime {
            get; set;
        }

        /// <summary>
        /// A computed display property combining name and section.
        /// </summary>
        public string DisplayName =>
            string.IsNullOrWhiteSpace(Section) ? Name : $"{Name} ({Section})";

        public override string ToString() => DisplayName;
    }
}
