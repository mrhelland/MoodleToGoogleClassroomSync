using System;
using System.Collections.Generic;
using System.Linq;
using GradebookCore.Models;
using MoodleLib.Models;

namespace GradebookCore.Mappers {
    /// <summary>
    /// Converts MoodleLib models into GradebookCore domain models.
    /// Ensures consistent IDs and filters duplicates automatically.
    /// </summary>
    public static class MoodleToGradebookMapper {

        // === ASSIGNMENTS ===

        /// <summary>
        /// Converts a Moodle grade item (module type) into a Gradebook assignment.
        /// </summary>
        public static Assignment ToAssignment(MGradeItem item) {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            return new Assignment {
                Id = 0, // Gradebook will assign unique ID
                Name = item.ItemName ?? "(Unnamed Item)",
                Description = item.Feedback ?? string.Empty,
                SourceSystem = "Moodle",
                SourceSystemId = item.IdString,
                DestinationSystem = string.Empty,
                DestinationSystemId = string.Empty,
                CategoryId = item.CategoryIdString ?? string.Empty,
                MaxPoints = item.GradeMax ?? 0,
                DueDate = null,
                CutoffDate = null
            };
        }

        /// <summary>
        /// Converts multiple Moodle grade items to unique assignments.
        /// </summary>
        public static List<Assignment> ToAssignmentList(MGradeItem[] items) {
            if(items == null || items.Length == 0)
                return new List<Assignment>();

            return items
                .Where(i => i.IsModule) // only actual graded activities
                .GroupBy(i => i.IdString) // remove duplicates
                .Select(g => ToAssignment(g.First()))
                .ToList();
        }

        // === CATEGORIES ===

        /// <summary>
        /// Converts a Moodle category-type grade item into a Gradebook category.
        /// </summary>
        public static Category ToCategory(MGradeItem item) {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            double? weight = null;
            if(!string.IsNullOrWhiteSpace(item.WeightFormatted)) {
                var clean = item.WeightFormatted.Replace("%", "").Trim();
                if(double.TryParse(clean, out double val))
                    weight = val;
            }

            return new Category {
                Id = 0,
                Name = item.ItemName ?? "(Unnamed Category)",
                SourceSystem = "Moodle",
                SourceSystemId = item.IdString,
                DestinationSystem = string.Empty,
                DestinationSystemId = string.Empty,
                WeightPercent = weight,
                ParentCategoryId = item.CategoryIdString ?? string.Empty,
                Assignments = new List<Assignment>()
            };
        }

        /// <summary>
        /// Converts multiple Moodle grade items to unique categories.
        /// </summary>
        public static List<Category> ToCategoryList(MGradeItem[] items) {
            if(items == null || items.Length == 0)
                return new List<Category>();

            return items
                .Where(i => i.IsCategory)
                .GroupBy(i => i.IdString)
                .Select(g => ToCategory(g.First()))
                .ToList();
        }

        // === STUDENTS ===

        /// <summary>
        /// Converts a Moodle student to a Gradebook student.
        /// </summary>
        public static Student ToStudent(MStudent student) {
            if(student == null)
                throw new ArgumentNullException(nameof(student));

            return new Student {
                Id = 0,
                Name = student.FullName ?? $"{student.GivenName} {student.FamilyName}".Trim(),
                SourceSystem = "Moodle",
                SourceSystemId = student.Id,
                DestinationSystem = string.Empty,
                DestinationSystemId = string.Empty,
                Username = student.Username ?? string.Empty,
                Email = student.Email ?? string.Empty,
                GivenName = student.GivenName ?? string.Empty,
                FamilyName = student.FamilyName ?? string.Empty
            };
        }

        /// <summary>
        /// Converts multiple Moodle students to unique Gradebook students.
        /// </summary>
        public static List<Student> ToStudentList(MStudent[] students) {
            if(students == null || students.Length == 0)
                return new List<Student>();

            return students
                .GroupBy(s => s.Id)
                .Select(g => ToStudent(g.First()))
                .ToList();
        }

        // === GRADE CELLS ===

        /// <summary>
        /// Converts a Moodle grade item into a GradeCell record for Gradebook.
        /// </summary>
        public static GradeCell ToGradeCell(MGradeItem item) {
            if(item == null)
                throw new ArgumentNullException(nameof(item));

            return new GradeCell {
                Id = 0,
                Grade = item.GradeEarned ?? item.GradeRaw?.ToString("0.##") ?? string.Empty,
                Submitted = false,
                Graded = item.GradeRaw.HasValue,
                Late = false,
                Feedback = item.Feedback ?? string.Empty,
                TimeSubmitted = null,
                TimeGraded = null
            };
        }

        /// <summary>
        /// Converts multiple Moodle grade items into unique GradeCells.
        /// </summary>
        public static List<GradeCell> ToGradeCellList(MGradeItem[] items) {
            if(items == null || items.Length == 0)
                return new List<GradeCell>();

            return items
                .Where(i => i.IsModule)
                .GroupBy(i => i.IdString)
                .Select(g => ToGradeCell(g.First()))
                .ToList();
        }
    }
}
