using System;
using System.Collections.Generic;

namespace GradebookCore.Models {
    /// <summary>
    /// Represents a gradebook category (e.g., Homework, Quizzes, Exams).
    /// </summary>
    [Serializable]
    public class Category : ISyncEntity {

        /// <summary>
        /// Local unique integer identifier for this assignment (internal system ID).
        /// </summary>
        public int Id {
            get; set;
        }

        /// <summary>
        /// Human-readable category name.
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
        /// Percentage weight of this category toward the course total (0–100).
        /// </summary>
        public double? WeightPercent {
            get; set;
        }

        /// <summary>
        /// Optional parent category ID (supports hierarchical gradebook structures).
        /// </summary>
        public string? ParentCategoryId {
            get; set;
        }

        /// <summary>
        /// Assignments belonging to this category.
        /// </summary>
        public List<Assignment> Assignments { get; set; } = new();

        public override string ToString() =>
            $"{Name} (Weight={WeightPercent ?? 0:F1}%)";
    }
}
