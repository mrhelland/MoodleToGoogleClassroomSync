using GoogleClassroomLib.Services;

namespace GoogleClassroomLib.Models {
    /// <summary>
    /// Represents a student enrolled in a Google Classroom course.
    /// </summary>
    public class GStudent {
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
                if(String.IsNullOrEmpty(value)) {
                    this.GivenName = "";
                    this.FamilyName = "";
                }
                else {
                    // Split by whitespace
                    var parts = value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if(parts.Length == 1) {
                        // Single name provided (e.g. "Plato")
                        this.GivenName = parts[0];
                        this.FamilyName = string.Empty;
                    }
                    else {
                        // All parts except the last are the given name(s)
                        this.GivenName = string.Join(' ', parts[..^1]);
                        this.FamilyName = parts[^1];
                    }
                }
                    
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
