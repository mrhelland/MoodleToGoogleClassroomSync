using System;

namespace GradebookCore.Models {
    /// <summary>
    /// Represents a single cell in the gradebook grid: one student's grade for one assignment.
    /// Uses string-based unique identifiers consistent across systems.
    /// </summary>
    [Serializable]
    public class GradeCell {
        /// <summary>
        /// Globally unique string identifier for this grade cell.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique ID of the student associated with this grade cell (string-based).
        /// </summary>
        public Student? Student { get; set; }

        /// <summary>
        /// The unique ID of the assignment associated with this grade cell (string-based).
        /// </summary>
        public Assignment? Assignment { get; set; }

        /// <summary>
        /// The grade value (formatted, e.g., "95.00" or "A-").
        /// </summary>
        public string? Grade {
            get; set;
        }

        /// <summary>
        /// True if the student has submitted work for this assignment.
        /// </summary>
        public bool Submitted {
            get; set;
        }

        /// <summary>
        /// True if the submission has been graded.
        /// </summary>
        public bool Graded {
            get; set;
        }

        /// <summary>
        /// True if the submission was late.
        /// </summary>
        public bool Late {
            get; set;
        }

        /// <summary>
        /// Optional instructor feedback.
        /// </summary>
        public string? Feedback {
            get; set;
        }

        /// <summary>
        /// Time the student submitted work, if applicable.
        /// </summary>
        public DateTime? TimeSubmitted {
            get; set;
        }

        /// <summary>
        /// Time the submission was graded, if applicable.
        /// </summary>
        public DateTime? TimeGraded {
            get; set;
        }

        /// <summary>
        /// Convenience property for grade status summary.
        /// </summary>
        public string Status {
            get {
                if(Graded)
                    return "Graded";
                if(Submitted)
                    return "Submitted";
                return "Pending";
            }
        }

        public override string ToString() =>
            $"Id={Id}, Student={Student.Name}, Assignment={Assignment.Name}, Grade={Grade}, " +
            $"Submitted={Submitted}, Graded={Graded}, Late={Late}";
    }
}

