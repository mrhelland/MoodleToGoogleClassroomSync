using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodleToGoogleClassroomSync.Utils {
    public class DataGridUtilities {


        public static void HideColumns(DataGridView dgv, params string[] columnNames) {
            if(dgv == null || columnNames == null || columnNames.Length == 0)
                return;

            foreach(var colName in columnNames) {
                if(dgv.Columns.Contains(colName))
                    dgv.Columns[colName].Visible = false;
            }
        }



        /// <summary>
        /// Safely sets the display order (DisplayIndex) of a DataGridView column by name.
        /// </summary>
        /// <param name="grid">The DataGridView control.</param>
        /// <param name="columnName">The name (DataPropertyName or column Name) of the column.</param>
        /// <param name="index">The target display index (0 = leftmost).</param>
        public static void SetColumnDisplayOrder(DataGridView grid, string columnName, int index) {
            if(grid == null)
                throw new ArgumentNullException(nameof(grid));

            if(string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));

            // ✅ Ensure the column exists before setting
            if(!grid.Columns.Contains(columnName)) {
                // Optionally, log or ignore silently
                Console.WriteLine($"Column '{columnName}' not found in DataGridView '{grid.Name}'.");
                return;
            }

            // Clamp index to valid range
            int safeIndex = Math.Max(0, Math.Min(index, grid.Columns.Count - 1));

            grid.Columns[columnName].DisplayIndex = safeIndex;
        }

        /// <summary>
        /// Sets display order for multiple columns in order.
        /// Example: SetColumnDisplayOrder(dtCourses, "ViewStudents", "Name", "State")
        /// </summary>
        public static void SetColumnDisplayOrder(DataGridView grid, params string[] columnNames) {
            if(grid == null || columnNames == null)
                return;

            for(int i = 0; i < columnNames.Length; i++) {
                var name = columnNames[i];
                if(grid.Columns.Contains(name))
                    grid.Columns[name].DisplayIndex = i;
            }
        }

        /// <summary>
        /// Sets the width (in pixels) of a specific column in a DataGridView.
        /// </summary>
        /// <param name="grid">The DataGridView control containing the column.</param>
        /// <param name="columnName">The name of the column (either Name or DataPropertyName).</param>
        /// <param name="width">The desired width in pixels (minimum 20).</param>
        public static void SetColumnWidth(DataGridView grid, string columnName, int width) {
            if(grid == null)
                throw new ArgumentNullException(nameof(grid));

            if(string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));

            if(width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");

            // Ensure manual sizing is respected
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Only apply if column exists
            if(!grid.Columns.Contains(columnName)) {
                Console.WriteLine($"⚠️ Column '{columnName}' not found in grid '{grid.Name}'.");
                return;
            }

            // Clamp to minimum width
            grid.Columns[columnName].Width = Math.Max(20, width);
        }
    }
}
