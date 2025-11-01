using GoogleClassroomLib.Services;

namespace GoogleClassroomLib.Models {
    /// <summary>
    /// Represents a student enrolled in a Google Classroom course.
    /// </summary>
    public class Student {
        /// <summary>
        /// The Google Classroom-assigned user ID.
        /// Example: "117876543210987654321".
        /// </summary>
        public string Id {
            get; set;
        }

        /// <summary>
        /// The student's full display name (e.g., "Alice Johnson").
        /// </summary>
        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
                StudentService.SetFamilyAndGiven(this);
            }
        }

        public string FamilyName {
            get; set;
        }

        public string GivenName {
            get; set;
        }

        /// <summary>
        /// The student's Google account email address.
        /// May be null or unavailable due to privacy restrictions.
        /// </summary>
        public string Email {
            get; set;
        }

        /// <summary>
        /// The enrollment status of the student (ACTIVE, INVITED, or DECLINED).
        /// </summary>
        public string EnrollmentStatus {
            get; set;
        }

        /// <summary>
        /// The course ID this student belongs to (optional for context).
        /// </summary>
        public string CourseId {
            get; set;
        }

        /// <summary>
        /// A read-only formatted label for display.
        /// </summary>
        public string DisplayLabel =>
            string.IsNullOrWhiteSpace(Email)
                ? $"{Name} (email unavailable)"
                : $"{Name} <{Email}>";

        public override string ToString() => DisplayLabel;
    }
}
