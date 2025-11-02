using System;
using Newtonsoft.Json;

namespace MoodleLib.Models {
    /// <summary>
    /// Represents a Moodle grade item (assignment, quiz, manual, category, or course total).
    /// Provides both numeric and string-based identifiers for cross-system compatibility.
    /// </summary>
    [Serializable]
    public class MGradeItem {
        // === Moodle JSON Fields (as returned by the API) ===

        [JsonProperty("id")]
        public int Id {
            get; set;
        }

        [JsonProperty("itemname")]
        public string ItemName { get; set; } = string.Empty;

        [JsonProperty("itemtype")]
        public string ItemType { get; set; } = string.Empty;

        [JsonProperty("itemmodule")]
        public string? ItemModule {
            get; set;
        }

        [JsonProperty("iteminstance")]
        public int? ItemInstance {
            get; set;
        }

        [JsonProperty("categoryid")]
        public int? CategoryId {
            get; set;
        }

        [JsonProperty("grademax")]
        public double? GradeMax {
            get; set;
        }

        [JsonProperty("grademin")]
        public double? GradeMin {
            get; set;
        }

        [JsonProperty("gradeformatted")]
        public string? GradeFormatted {
            get; set;
        }

        [JsonProperty("gradeformattedraw")]
        public double? GradeFormattedRaw {
            get; set;
        }

        [JsonProperty("feedback")]
        public string? Feedback {
            get; set;
        }

        [JsonProperty("status")]
        public string? Status {
            get; set;
        }

        [JsonProperty("weightformatted")]
        public string? WeightFormatted {
            get; set;
        }

        [JsonProperty("hidden")]
        public bool Hidden {
            get; set;
        }

        [JsonProperty("locked")]
        public bool Locked {
            get; set;
        }

        // === Helper String ID Properties ===

        /// <summary>
        /// Returns the string form of this grade item's local (Moodle) ID.
        /// Useful for consistent handling across systems.
        /// </summary>
        [JsonIgnore]
        public string IdString => Id.ToString();

        /// <summary>
        /// Returns the string form of this grade item's parent category ID.
        /// </summary>
        [JsonIgnore]
        public string? CategoryIdString => CategoryId?.ToString();

        /// <summary>
        /// Returns the string form of the module instance ID (if applicable).
        /// </summary>
        [JsonIgnore]
        public string? ItemInstanceString => ItemInstance?.ToString();

        // === Convenience Flags ===

        /// <summary>
        /// True if this item represents a grade category folder.
        /// </summary>
        [JsonIgnore]
        public bool IsCategory => ItemType?.Equals("category", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// True if this item represents the course total.
        /// </summary>
        [JsonIgnore]
        public bool IsCourseTotal => ItemType?.Equals("course", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// True if this item represents a graded activity (assignment, quiz, etc.).
        /// </summary>
        [JsonIgnore]
        public bool IsModule => ItemType?.Equals("mod", StringComparison.OrdinalIgnoreCase) == true;

        /// <summary>
        /// Returns a friendly string summary of this grade item.
        /// </summary>
        public override string ToString() =>
            $"{ItemName} ({ItemType}{(Hidden ? ", hidden" : "")})";
    }
}
