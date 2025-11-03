using GoogleClassroomLib;
using MoodleLib;
using MoodleToGoogleClassroomSync.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoodleToGoogleClassroomSync {
    public partial class TestDispatch : Form {
        public TestDispatch() {
            InitializeComponent();
        }

        private void btnGoogleClassroom_Click(object sender, EventArgs e) {
            TestGC gcForm = new TestGC();
            gcForm.Show();
        }

        private void btnMoodle_Click(object sender, EventArgs e) {
            TestM mForm = new TestM();
            mForm.Show();
        }

        private void btnGenerateStructure_Click(object sender, EventArgs e) {
            Assembly.Load("MoodleLib");
            Assembly.Load("GoogleClassroomLib");
            Assembly.Load("GradebookCore");

            string[] namespaces = { "MoodleLib", "GoogleClassroomLib", "GradebookCore" };
            string structure = StructureGenerator.GenerateStructure(namespaces);
            txtOutput.Text = structure;
        }


    }
}
