using System;

namespace GradebookCore.Models {
    /// <summary>
    /// Represents a single graded activity (assignment, quiz, etc.).
    /// </summary>
    [Serializable]
    public class Assignment : ISyncEntity {

        /// <summary>
        /// Local unique integer identifier for this assignment (internal system ID).
        /// </summary>
        public int Id {
            get; set;
        }

        /// <summary>
        /// Human-readable name of the assignment.
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
        /// Optional description or instructions for the assignment.
        /// </summary>
        public string? Description {
            get; set;
        }

        /// <summary>
        /// Identifier of the category this assignment belongs to (if any).
        /// </summary>
        public string? CategoryId {
            get; set;
        }

        /// <summary>
        /// Maximum number of points possible for this assignment.
        /// </summary>
        public double MaxPoints { get; set; } = 100;

        /// <summary>
        /// Optional due date for the assignment.
        /// </summary>
        public DateTime? DueDate {
            get; set;
        }

        /// <summary>
        /// Optional cutoff date (final submission deadline).
        /// </summary>
        public DateTime? CutoffDate {
            get; set;
        }

        public override string ToString() => $"{Name} ({MaxPoints} pts)";
    }
}
