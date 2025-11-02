using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace MoodleLib.Models {
    /// <summary>
    /// Represents a Google Classroom course with minimal metadata needed
    /// for synchronization and display in external applications.
    /// </summary>

    public class MCourse {

        public string Id { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public bool Visible {
            get; set;
        }

        /// <summary>
        /// A computed display property combining name and section.
        /// </summary>
        public string DisplayName =>
            string.IsNullOrWhiteSpace(FullName) ? ShortName : FullName;

        public override string ToString() => DisplayName;
    }
}
