namespace MoodleToGoogleClassroomSync {
    partial class TestGC {
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
            btnGetCourses = new Button();
            dtCourses = new DataGridView();
            dtStudents = new DataGridView();
            splitContainer1 = new SplitContainer();
            lblCourse = new Label();
            ((System.ComponentModel.ISupportInitialize)dtCourses).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dtStudents).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // btnGetCourses
            // 
            btnGetCourses.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnGetCourses.Location = new Point(3, 3);
            btnGetCourses.Name = "btnGetCourses";
            btnGetCourses.Size = new Size(462, 40);
            btnGetCourses.TabIndex = 0;
            btnGetCourses.Text = "Get Google Classroom Courses";
            btnGetCourses.UseVisualStyleBackColor = true;
            // 
            // dtCourses
            // 
            dtCourses.AllowUserToAddRows = false;
            dtCourses.AllowUserToDeleteRows = false;
            dtCourses.AllowUserToResizeRows = false;
            dtCourses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dtCourses.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtCourses.Location = new Point(3, 49);
            dtCourses.Name = "dtCourses";
            dtCourses.RowHeadersVisible = false;
            dtCourses.Size = new Size(462, 516);
            dtCourses.TabIndex = 1;
            // 
            // dtStudents
            // 
            dtStudents.AllowUserToAddRows = false;
            dtStudents.AllowUserToDeleteRows = false;
            dtStudents.AllowUserToResizeRows = false;
            dtStudents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dtStudents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dtStudents.Location = new Point(3, 49);
            dtStudents.Name = "dtStudents";
            dtStudents.RowHeadersVisible = false;
            dtStudents.Size = new Size(928, 516);
            dtStudents.TabIndex = 2;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(12, 12);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(dtCourses);
            splitContainer1.Panel1.Controls.Add(btnGetCourses);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(lblCourse);
            splitContainer1.Panel2.Controls.Add(dtStudents);
            splitContainer1.Size = new Size(1406, 568);
            splitContainer1.SplitterDistance = 468;
            splitContainer1.TabIndex = 3;
            // 
            // lblCourse
            // 
            lblCourse.AutoSize = true;
            lblCourse.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCourse.Location = new Point(6, 14);
            lblCourse.Name = "lblCourse";
            lblCourse.Size = new Size(158, 21);
            lblCourse.TabIndex = 3;
            lblCourse.Text = "No Course Selected";
            // 
            // TestGC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1430, 592);
            Controls.Add(splitContainer1);
            Name = "TestGC";
            Text = "Test Google Classroom";
            Load += Test_Load;
            ((System.ComponentModel.ISupportInitialize)dtCourses).EndInit();
            ((System.ComponentModel.ISupportInitialize)dtStudents).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnGetCourses;
        private DataGridView dtCourses;
        private DataGridView dtStudents;
        private SplitContainer splitContainer1;
        private Label lblCourse;
    }
}