using System.Text.RegularExpressions;
using System.Web;

namespace MoodleLib.Utils {
    /// <summary>
    /// Provides helper methods for sanitizing Moodle HTML output
    /// (e.g. gradeformatted, feedback, weightformatted).
    /// Removes HTML tags and decodes HTML entities to plain text.
    /// </summary>
    public static class HtmlCleaner {
        // Compiled regex to strip all HTML tags quickly.
        private static readonly Regex TagRegex = new("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Removes HTML tags and decodes entities from a string.
        /// </summary>
        /// <param name="input">The raw HTML string (may be null or empty).</param>
        /// <returns>Cleaned plain text, safe for display.</returns>
        public static string Clean(string? input) {
            if(string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove all HTML tags like <i>, <span>, etc.
            string noTags = TagRegex.Replace(input, string.Empty);

            // Decode HTML entities like &nbsp;, &gt;, &lt;, etc.
            string decoded = HttpUtility.HtmlDecode(noTags);

            // Trim and normalize whitespace
            return decoded?.Trim() ?? string.Empty;
        }
    }
}
