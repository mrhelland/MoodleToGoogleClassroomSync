namespace MoodleToGoogleClassroomSync {
    partial class TestStudentGradesM {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            dgvGrades = new DataGridView();
            lblStudent = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvGrades).BeginInit();
            SuspendLayout();
            // 
            // dgvGrades
            // 
            dgvGrades.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvGrades.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGrades.Location = new Point(12, 48);
            dgvGrades.Name = "dgvGrades";
            dgvGrades.Size = new Size(1148, 544);
            dgvGrades.TabIndex = 0;
            // 
            // lblStudent
            // 
            lblStudent.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblStudent.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStudent.ImageAlign = ContentAlignment.MiddleLeft;
            lblStudent.Location = new Point(12, 9);
            lblStudent.Name = "lblStudent";
            lblStudent.Size = new Size(1148, 36);
            lblStudent.TabIndex = 1;
            lblStudent.Text = "label1";
            lblStudent.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // TestStudentGradesM
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1172, 604);
            Controls.Add(lblStudent);
            Controls.Add(dgvGrades);
            Name = "TestStudentGradesM";
            Text = "TestStudentGradesM";
            ((System.ComponentModel.ISupportInitialize)dgvGrades).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvGrades;
        private Label lblStudent;
    }
}