using System;

namespace MoodleLib.Models {
    /// <summary>
    /// Represents a student's submission and grading status for an assignment.
    /// Compatible with Moodle 4.5 web service responses (mod_assign_get_submission_status).
    /// </summary>
    public class MSubmissionStatus {
        public string AssignmentId { get; set; } = string.Empty;
        public string? UserId {
            get; set;
        }

        // Moodle: "status" (e.g., "new", "submitted", "draft")
        public string SubmissionState { get; set; } = "unknown";
        public bool IsSubmitted => string.Equals(SubmissionState, "submitted", StringComparison.OrdinalIgnoreCase);

        // Moodle: "gradingstatus" (e.g., "graded", "notgraded")
        public string GradingState { get; set; } = "notgraded";
        public bool IsGraded => string.Equals(GradingState, "graded", StringComparison.OrdinalIgnoreCase);

        // Moodle: "timecreated", "timemodified"
        public DateTime? TimeSubmitted {
            get; set;
        }

        // Moodle: "gradeddate"
        public DateTime? TimeGraded {
            get; set;
        }

        // Moodle: grade or feedback.grade
        public string? Grade {
            get; set;
        }

        // Moodle: feedback.text
        public string? FeedbackText {
            get; set;
        }

        // Moodle: duedate / cutoffdate
        public DateTime? DueDate {
            get; set;
        }
        public DateTime? CutoffDate {
            get; set;
        }

        // Derived: not returned directly by Moodle
        public bool? IsLate {
            get; set;
        }

        /// <summary>
        /// Derived property to check if submission occurred after DueDate.
        /// </summary>
        public bool IsAfterDueDate => DueDate.HasValue && TimeSubmitted.HasValue && TimeSubmitted > DueDate;

        public override string ToString() =>
            $"AssignID={AssignmentId}, User={UserId}, Status={SubmissionState}, Graded={IsGraded}, Grade={Grade}, Due={DueDate}, Feedback={FeedbackText}";
    }
}
