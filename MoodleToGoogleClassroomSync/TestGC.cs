using GoogleClassroomLib.Models;
using GoogleClassroomLib.Providers;
using GoogleClassroomLib.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoodleToGoogleClassroomSync.Utils;

namespace MoodleToGoogleClassroomSync {
    public partial class TestGC : Form {

        private Course currentCourse;
        private List<Course> currentCourses;
        private List<Student> currentStudents;


        private readonly CourseProvider _courseProvider;
        private readonly CourseService _courseService;
        private readonly StudentProvider _studentProvider;
        private readonly StudentService _studentService;

        public TestGC() {
            InitializeComponent();

            // === Logging setup ===
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            var authLogger = loggerFactory.CreateLogger<AuthService>();
            var auth = new AuthService(
                AppConfig.GetGoogleCredentialsPath(),
                AppConfig.GetTokenStorePath(),
                authLogger
            );

            // Get authenticated Google Classroom client
            var classroomService = Task.Run(() => auth.GetAuthenticatedServiceAsync()).Result;

            _courseProvider = new CourseProvider(classroomService, loggerFactory.CreateLogger<CourseProvider>());
            _courseService = new CourseService(_courseProvider, loggerFactory.CreateLogger<CourseService>());
            _studentProvider = new StudentProvider(classroomService, loggerFactory.CreateLogger<StudentProvider>());
            _studentService = new StudentService(_studentProvider, loggerFactory.CreateLogger<StudentService>());


            // Wire up event handlers
            btnGetCourses.Click += async (s, e) => await BtnGetCourses_Click(s, e);
            dtCourses.CellContentClick += DtCourses_CellContentClick;

        }

        private void Test_Load(object sender, EventArgs e) {

        }

        // === Load all courses ===
        private async Task BtnGetCourses_Click(object sender, EventArgs e) {
            btnGetCourses.Enabled = false;
            try {
                dtCourses.DataSource = null;
                currentCourses = await _courseService.GetCoursesAsync(true, null, true);

                // Default sort by Name
                currentCourses = currentCourses.OrderBy(c => c.Name).ToList();

                var sortableCourses = new SortableBindingList<Course>(currentCourses);

                // Add courses to DataGridView
                dtCourses.DataSource = sortableCourses;




                // Add a button column to view students if not already added
                if(!dtCourses.Columns.Contains("ViewStudents")) {
                    var btnColumn = new DataGridViewButtonColumn {
                        HeaderText = "Students",
                        Text = "View",
                        Name = "ViewStudents",
                        UseColumnTextForButtonValue = true,
                        Width = 80,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    };
                    dtCourses.Columns.Add(btnColumn);
                }
                // Default sort glyph
                if(dtCourses.Columns.Contains("Name"))
                    dtCourses.Columns["Name"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                // ✅ Move button to the front
                //dtCourses.Columns["ViewStudents"].DisplayIndex = 0;

                // ✅ Hide unwanted columns
                DataGridUtilities.HideColumns(dtCourses, "Description", "OwnerEmail", "DisplayName");
                DataGridUtilities.SetColumnDisplayOrder(dtCourses, "ViewStudents", "Name", "State");
                DataGridUtilities.SetColumnWidth(dtCourses, "ViewStudents", 60);
                DataGridUtilities.SetColumnWidth(dtCourses, "Name", 200);
                DataGridUtilities.SetColumnWidth(dtCourses, "State", 60);
            }
            catch(Exception ex) {
                MessageBox.Show($"Error retrieving courses: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                btnGetCourses.Enabled = true;
            }
        }

        // === Handle click on “View” button for a course ===
        private async void DtCourses_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            // Ignore header clicks
            if(e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // Check if clicked column is the ViewStudents button
            if(dtCourses.Columns[e.ColumnIndex].Name == "ViewStudents") {
                // Get the selected course
                currentCourse = currentCourses[e.RowIndex];
                this.Text = $"Test — {currentCourse.DisplayName}";
                this.lblCourse.Text = currentCourse.DisplayName;

                await LoadStudentsForCourseAsync(currentCourse);
            }
        }

        // === Load the students for the selected course ===
        private async Task LoadStudentsForCourseAsync(Course course) {
            try {
                dtStudents.DataSource = null;
                currentStudents = await _studentService.GetStudentsSortedAsync(course.Id);

                // Default sort by last name
                currentStudents = currentStudents
                    .OrderBy(s => GetLastName(s.Name))
                    .ToList();

                var sortableStudents = new SortableBindingList<Student>(currentStudents);
                dtStudents.DataSource = sortableStudents;
                dtStudents.AutoResizeColumns();


                if(dtStudents.Columns.Contains("Name"))
                    dtStudents.Columns["Name"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                if(currentStudents.Count == 0) {
                    MessageBox.Show("No students found for this course.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                DataGridUtilities.HideColumns(dtStudents, "FamilyName", "GivenName", "DisplayLabel", "CourseId");
                DataGridUtilities.SetColumnDisplayOrder(dtStudents, "Name", "Email", "EnrollmentSTatus");
                DataGridUtilities.SetColumnWidth(dtStudents, "Name", 200);
                DataGridUtilities.SetColumnWidth(dtStudents, "Email", 300);
                DataGridUtilities.SetColumnWidth(dtStudents, "EnrollmentStatus", 100);
            }
            catch(Exception ex) {
                MessageBox.Show($"Error retrieving students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // === Helper: extract last name ===
        private static string GetLastName(string fullName) {
            if(string.IsNullOrWhiteSpace(fullName))
                return string.Empty;
            var parts = fullName.Trim().Split(' ');
            return parts.Length > 1 ? parts[^1] : parts[0];
        }

    }


}
