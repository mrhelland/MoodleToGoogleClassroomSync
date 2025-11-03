using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GradebookCore.Models {
    /// <summary>
    /// Represents an entire course gradebook (students × assignments grid).
    /// Uses object references for GradeCell relationships.
    /// </summary>
    [Serializable]
    public class Gradebook {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SourceSystem { get; set; } = string.Empty;
        public string SourceId { get; set; } = string.Empty;
        public string DestinationSystem { get; set; } = string.Empty;
        public string DestinationId { get; set; } = string.Empty;

        public List<Student> Students { get; set; } = new();
        public List<Assignment> Assignments { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<GradeCell> Entries { get; set; } = new();

        // Static ID generator ensures globally unique integer IDs across this runtime.
        private static int _nextId = 1;
        private static readonly object _idLock = new();

        /// <summary>
        /// Generates a unique integer ID for new entities.
        /// </summary>
        private static int GenerateId() {
            lock(_idLock) {
                return _nextId++;
            }
        }

        /// <summary>
        /// Assigns unique IDs to any items (students, assignments, categories, grade cells)
        /// that do not already have one.
        /// </summary>
        public void EnsureUniqueIds() {
            foreach(var s in Students.Where(s => s.Id == 0))
                s.Id = GenerateId();

            foreach(var a in Assignments.Where(a => a.Id == 0))
                a.Id = GenerateId();

            foreach(var c in Categories.Where(c => c.Id == 0))
                c.Id = GenerateId();

            foreach(var g in Entries.Where(g => g.Id == 0))
                g.Id = GenerateId();
        }

        /// <summary>
        /// Adds or updates a grade cell for a specific student and assignment.
        /// </summary>
        public void AddGrade(GradeCell cell) {
            if(cell.Student == null || cell.Assignment == null)
                throw new ArgumentException("GradeCell must have valid Student and Assignment references.");

            // If an existing grade for this student and assignment exists, replace it.
            var existing = Entries.FirstOrDefault(
                g => g.Student == cell.Student && g.Assignment == cell.Assignment);

            if(existing != null) {
                Entries.Remove(existing);
            }

            if(cell.Id == 0)
                cell.Id = GenerateId();

            Entries.Add(cell);
        }

        /// <summary>
        /// Retrieves a grade cell for the given student and assignment.
        /// </summary>
        public GradeCell? GetGrade(Student student, Assignment assignment) =>
            Entries.FirstOrDefault(c => c.Student == student && c.Assignment == assignment);

        /// <summary>
        /// Returns all grade cells for a specific student.
        /// </summary>
        public IEnumerable<GradeCell> GetGradesForStudent(Student student) =>
            Entries.Where(c => c.Student == student);

        /// <summary>
        /// Returns all grade cells for a specific assignment across all students.
        /// </summary>
        public IEnumerable<GradeCell> GetGradesForAssignment(Assignment assignment) =>
            Entries.Where(c => c.Assignment == assignment);

        /// <summary>
        /// Converts the gradebook into a DataTable for easy export to CSV or Excel.
        /// </summary>
        public DataTable ToDataTable() {
            var dt = new DataTable();
            dt.Columns.Add("Student");

            foreach(var a in Assignments)
                dt.Columns.Add(a.Name);

            foreach(var s in Students) {
                var row = dt.NewRow();
                row["Student"] = s.FullName;

                foreach(var a in Assignments) {
                    var cell = GetGrade(s, a);
                    row[a.Name] = cell?.Grade ?? "-";
                }

                dt.Rows.Add(row);
            }

            return dt;
        }

        /// <summary>
        /// Exports the gradebook to JSON (students, assignments, categories, and grades).
        /// </summary>
        public string ToJson(bool indented = true) {
            var export = new {
                CourseId,
                CourseName,
                Students = Students.Select(s => new {
                    s.Id,
                    s.Name,
                    s.FullName,
                    s.SourceSystemId,
                    s.SourceSystem,
                    s.DestinationSystemId,
                    s.DestinationSystem,
                    Grades = Assignments.Select(a => {
                        var cell = GetGrade(s, a);
                        return new {
                            AssignmentId = a.Id,
                            AssignmentName = a.Name,
                            Grade = cell?.Grade,
                            Submitted = cell?.Submitted ?? false
                        };
                    }).ToList()
                }).ToList(),
                Assignments = Assignments.Select(a => new {
                    a.Id,
                    a.Name,
                    a.CategoryId,
                    a.SourceSystemId,
                    a.SourceSystem,
                    a.DestinationSystemId,
                    a.DestinationSystem
                }).ToList(),
                Categories = Categories.Select(c => new {
                    c.Id,
                    c.Name,
                    c.WeightPercent,
                    c.SourceSystemId,
                    c.SourceSystem,
                    c.DestinationSystemId,
                    c.DestinationSystem
                }).ToList()
            };

            var options = new JsonSerializerOptions {
                WriteIndented = indented,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            return JsonSerializer.Serialize(export, options);
        }

        /// <summary>
        /// Returns a textual summary of the gradebook for debugging/logging.
        /// </summary>
        public void PrintSummary() {
            Console.WriteLine($"Gradebook for Course {CourseName} ({CourseId}):");
            foreach(var s in Students) {
                Console.WriteLine($"  {s.FullName,-20}:");
                foreach(var a in Assignments) {
                    var g = GetGrade(s, a);
                    var grade = g?.Grade ?? "-";
                    Console.WriteLine($"    {a.Name,-25} {grade,-5} {(g?.Submitted == true ? "✓" : "✗")}");
                }
            }
        }
    }
}
