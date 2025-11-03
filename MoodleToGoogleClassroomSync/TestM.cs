using Microsoft.Extensions.Logging;
using MoodleLib.Models;
using MoodleLib.Providers;
using MoodleLib.Services;
using MoodleLib.Utils;
using MoodleToGoogleClassroomSync.Utils; // For DataGridUtilities
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoodleToGoogleClassroomSync {
    public partial class TestM : Form {

        private MCourse currentCourse;
        private List<MCourse> currentCourses;
        private List<MStudent> currentStudents;
        private MApiProvider mApiProvider;
        private MAuthProvider mAuthProvider;

        private readonly MCourseProvider _courseProvider;
        private readonly MCourseService _courseService;
        private readonly MStudentProvider _studentProvider;
        private readonly MStudentService _studentService;

        public TestM() {
            InitializeComponent();

            // === Logging setup ===
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole()
                       .AddDebug()
                       .SetMinimumLevel(LogLevel.Debug)
                       .AddFile(AppConfig.GetLoggingPath());

            });

            // === Moodle API configuration ===
            string baseUrl = AppConfig.GetMoodleBaseUrl();
            string token = AppConfig.GetMoodleToken();

            mAuthProvider = new MAuthProvider(baseUrl, token, loggerFactory.CreateLogger<MAuthProvider>());
            mApiProvider = new MApiProvider(mAuthProvider, loggerFactory.CreateLogger<MApiProvider>());

            // === Providers & Services ===
            _courseProvider = new MCourseProvider(mApiProvider, loggerFactory.CreateLogger<MCourseProvider>());
            _courseService = new MCourseService(_courseProvider, loggerFactory.CreateLogger<MCourseService>());
            _studentProvider = new MStudentProvider(mApiProvider, loggerFactory.CreateLogger<MStudentProvider>());
            _studentService = new MStudentService(_studentProvider, loggerFactory.CreateLogger<MStudentService>());

            // === Wire up UI events ===
            btnGetCourses.Click += async (s, e) => await BtnGetCourses_Click(s, e);
            dtCourses.CellContentClick += DtCourses_CellContentClick;
        }

        private void Test_Load(object sender, EventArgs e) {
        }

        // === Retrieve course list ===
        private async Task BtnGetCourses_Click(object sender, EventArgs e) {
            btnGetCourses.Enabled = false;

            try {
                dtCourses.DataSource = null;

                // Retrieve and sort courses
                currentCourses = await _courseService.GetCoursesSortedAsync();

                var sortableCourses = new SortableBindingList<MCourse>(currentCourses);
                dtCourses.DataSource = sortableCourses;

                // Add a button column for students if not already present
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
                if(dtCourses.Columns.Contains("FullName"))
                    dtCourses.Columns["FullName"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                // Move button to the front
                dtCourses.Columns["ViewStudents"].DisplayIndex = 0;

                // Hide unused columns
                DataGridUtilities.HideColumns(dtCourses, "Summary", "IdNumber", "Format", "Category");

                // Set display order and column widths
                DataGridUtilities.SetColumnDisplayOrder(dtCourses, "ViewStudents", "FullName", "ShortName", "Visible");
                DataGridUtilities.SetColumnWidth(dtCourses, "ViewStudents", 60);
                DataGridUtilities.SetColumnWidth(dtCourses, "FullName", 250);
                DataGridUtilities.SetColumnWidth(dtCourses, "Visible", 70);
            }
            catch(Exception ex) {
                MessageBox.Show($"Error retrieving Moodle courses: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                btnGetCourses.Enabled = true;
            }
        }

        // === Handle "View" button click in dtCourses ===
        private async void DtCourses_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if(e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if(dtCourses.Columns[e.ColumnIndex].Name == "ViewStudents") {
                currentCourse = currentCourses[e.RowIndex];
                this.Text = $"Moodle Test — {currentCourse.FullName}";
                this.lblCourse.Text = currentCourse.FullName;

                await LoadStudentsForCourseAsync(currentCourse);
            }
        }

        //// === Load the students for the selected Moodle course ===
        //private async Task LoadStudentsForCourseAsync(MCourse course) {
        //    try {
        //        dtStudents.DataSource = null;
        //        currentStudents = await _studentService.GetStudentsSortedAsync(course.Id);

        //        if(currentStudents == null || currentStudents.Count == 0) {
        //            MessageBox.Show("No students found for this Moodle course.", "Info",
        //                MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }

        //        var sortableStudents = new SortableBindingList<MStudent>(currentStudents);
        //        dtStudents.DataSource = sortableStudents;
        //        dtStudents.AutoResizeColumns();

        //        if(dtStudents.Columns.Contains("FullName"))
        //            dtStudents.Columns["FullName"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

        //        // Hide less relevant columns
        //        DataGridUtilities.HideColumns(dtStudents, "GivenName", "FamilyName", "CourseId");

        //        // Set display order and widths
        //        DataGridUtilities.SetColumnDisplayOrder(dtStudents, "FullName", "Email", "EnrollmentStatus");
        //        DataGridUtilities.SetColumnWidth(dtStudents, "FullName", 250);
        //        DataGridUtilities.SetColumnWidth(dtStudents, "Email", 300);
        //        DataGridUtilities.SetColumnWidth(dtStudents, "EnrollmentStatus", 100);
        //    }
        //    catch(Exception ex) {
        //        MessageBox.Show($"Error retrieving Moodle students: {ex.Message}", "Error",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        // === Load the students for the selected Moodle course ===
        private async Task LoadStudentsForCourseAsync(MCourse course) {
            try {
                dtStudents.DataSource = null;
                currentStudents = await _studentService.GetStudentsSortedAsync(course.Id);

                if(currentStudents == null || currentStudents.Count == 0) {
                    MessageBox.Show("No students found for this Moodle course.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var sortableStudents = new SortableBindingList<MStudent>(currentStudents);
                dtStudents.DataSource = sortableStudents;
                dtStudents.AutoResizeColumns();

                if(dtStudents.Columns.Contains("FullName"))
                    dtStudents.Columns["FullName"].HeaderCell.SortGlyphDirection = SortOrder.Ascending;

                // Hide less relevant columns
                DataGridUtilities.HideColumns(dtStudents, "GivenName", "FamilyName", "CourseId");

                // Set display order and widths
                DataGridUtilities.SetColumnDisplayOrder(dtStudents, "FullName", "Email", "EnrollmentStatus");
                DataGridUtilities.SetColumnWidth(dtStudents, "FullName", 250);
                DataGridUtilities.SetColumnWidth(dtStudents, "Email", 300);
                DataGridUtilities.SetColumnWidth(dtStudents, "EnrollmentStatus", 100);

                // === Add "View Grades" button column (if not already added) ===
                const string btnColName = "colViewGrades";
                if(!dtStudents.Columns.Contains(btnColName)) {
                    var btnCol = new DataGridViewButtonColumn {
                        Name = btnColName,
                        HeaderText = "",
                        Text = "View Grades",
                        UseColumnTextForButtonValue = true,
                        Width = 110,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        FlatStyle = FlatStyle.Standard
                    };
                    dtStudents.Columns.Insert(0, btnCol);
                }

                // === Hook up event handler once ===
                dtStudents.CellContentClick -= DtStudents_CellContentClick;
                dtStudents.CellContentClick += DtStudents_CellContentClick;
            }
            catch(Exception ex) {
                MessageBox.Show($"Error retrieving Moodle students: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DtStudents_CellContentClick(object? sender, DataGridViewCellEventArgs e) {
            if(e.RowIndex < 0)
                return; // header row
            var grid = sender as DataGridView;
            if(grid == null)
                return;

            // Check if the clicked cell is the "View Grades" button
            if(grid.Columns[e.ColumnIndex].Name == "colViewGrades") {
                var student = grid.Rows[e.RowIndex].DataBoundItem as MStudent;
                if(student == null)
                    return;

                // Create the grade viewer form
                var form = new TestStudentGradesM {
                    Student = student,
                    ApiProvider = mApiProvider,     // assuming your parent form defines these
                    AuthProvider = mAuthProvider
                };
                form.LoadData();
                form.ShowDialog(this);

            }
        }


    }
}
