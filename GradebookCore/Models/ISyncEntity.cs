namespace GradebookCore.Models {
    /// <summary>
    /// Defines a common identity and cross-system mapping contract for gradebook entities.
    /// Implemented by any class that can exist in multiple systems (e.g., Moodle, Google Classroom).
    /// </summary>
    public interface ISyncEntity {
        /// <summary>
        /// Local unique identifier (internal to this system).
        /// </summary>
        int Id {
            get; set;
        }

        /// <summary>
        /// Identifier from the source system (originating platform).
        /// </summary>
        string Name {
            get; set;
        }

        /// <summary>
        /// Identifier from the source system (originating platform).
        /// </summary>
        string? SourceSystemId {
            get; set;
        }

        /// <summary>
        /// Name of the source system (e.g., "Moodle").
        /// </summary>
        string? SourceSystem {
            get; set;
        }

        /// <summary>
        /// Identifier in the destination system (target platform).
        /// </summary>
        string? DestinationSystemId {
            get; set;
        }

        /// <summary>
        /// Name of the destination system (e.g., "Google Classroom").
        /// </summary>
        string? DestinationSystem {
            get; set;
        }
    }
}
