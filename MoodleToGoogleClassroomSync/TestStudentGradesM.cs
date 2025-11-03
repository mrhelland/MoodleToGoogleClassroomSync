using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoodleLib.Models;
using MoodleLib.Providers;
using MoodleLib.Services;
using System.Diagnostics;

namespace MoodleToGoogleClassroomSync {
    public partial class TestStudentGradesM : Form {

        // === Publicly assignable dependencies ===
        public MApiProvider? ApiProvider {
            get; set;
        }
        public MAuthProvider? AuthProvider {
            get; set;
        }

        public MStudent? Student {
            get; set;
        }

        public TestStudentGradesM() {
            Debug.WriteLine("DEBUG: Running TestStudentGradesM()");
            InitializeComponent();
        }

        //private async void TestStudentGradesM_Load(object sender, EventArgs e) {
        //    await BuildDataGridAsync();
        //}

        public async void LoadData() {
            Debug.WriteLine("DEBUG: Running LoadData()");
            await BuildDataGridAsync(); 
        }

        /// <summary>
        /// Loads and displays all MGradeItem records for the current student.
        /// </summary>
        private async Task BuildDataGridAsync() {
            Debug.WriteLine("DEBUG: Running BuildDataGridAsync()");
            if(Student == null) {
                Debug.WriteLine("DEBUG: Student is NULL");
                lblStudent.Text = "No Student Loaded";
                dgvGrades.DataSource = null;
                return;
            }
            Debug.WriteLine($"DEBUG: Student.Id={Student.Id}, CourseId={Student.CourseId}");
            try {
                lblStudent.Text = Student.FullName ?? "(Unnamed Student)";
                dgvGrades.DataSource = null;
                dgvGrades.Rows.Clear();

                // === Validate API dependencies ===
                if(ApiProvider == null) {
                    MessageBox.Show("ApiProvider is not set.", "Dependency Missing",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if(AuthProvider == null) {
                    MessageBox.Show("AuthProvider is not set.", "Dependency Missing",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // === Create providers/services using injected dependencies ===
                var gradeProvider = new MGradeItemProvider(ApiProvider);
                var submissionProvider = new MSubmissionStatusProvider(ApiProvider);
                var submissionService = new MSubmissionStatusService(submissionProvider);
                var gradeService = new MGradeItemService(gradeProvider, submissionService);

                // === Fetch grade items for the student ===
                var items = await gradeService.GetVisibleActivityGradeItemsAsync(
                    Student.CourseId, Student.Id
                );

                Debug.WriteLine($"DEBUG: items.Count = {items.Count}");

                // === Prepare data for display ===
                var displayData = items.Select(i => new {
                    i.ItemName,
                    i.ItemType,
                    i.ItemModule,
                    Grade = i.GradeEarned,
                    Max = i.GradeMax,
                    Min = i.GradeMin,
                    i.Status,
                    i.WeightFormatted,
                    i.Hidden,
                    i.Locked,
                    i.SubmissionState
                }).ToList();

                Debug.WriteLine($"DEBUG: displayData.Count = {displayData?.Count ?? 0}");

                dgvGrades.AutoGenerateColumns = true;
                dgvGrades.DataSource = displayData;

                // === Optional: user-friendly headers ===
                dgvGrades.Columns["ItemName"].HeaderText = "Activity";
                dgvGrades.Columns["ItemModule"].HeaderText = "Module";
                dgvGrades.Columns["Grade"].HeaderText = "Grade";
                dgvGrades.Columns["Max"].HeaderText = "Max";
                dgvGrades.Columns["Min"].HeaderText = "Min";
                dgvGrades.Columns["Status"].HeaderText = "Status";
                dgvGrades.Columns["WeightFormatted"].HeaderText = "Weight";

                dgvGrades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch(Exception ex) {
                MessageBox.Show($"Error loading grades:\n{ex.Message}",
                    "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
