namespace MoodleToGoogleClassroomSync {
    partial class TestDispatch {
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
            btnGoogleClassroom = new Button();
            btnMoodle = new Button();
            SuspendLayout();
            // 
            // btnGoogleClassroom
            // 
            btnGoogleClassroom.Location = new Point(12, 12);
            btnGoogleClassroom.Name = "btnGoogleClassroom";
            btnGoogleClassroom.Size = new Size(119, 74);
            btnGoogleClassroom.TabIndex = 0;
            btnGoogleClassroom.Text = "Open GC Test";
            btnGoogleClassroom.UseVisualStyleBackColor = true;
            btnGoogleClassroom.Click += btnGoogleClassroom_Click;
            // 
            // btnMoodle
            // 
            btnMoodle.Location = new Point(669, 12);
            btnMoodle.Name = "btnMoodle";
            btnMoodle.Size = new Size(119, 74);
            btnMoodle.TabIndex = 1;
            btnMoodle.Text = "Open Moodle Test";
            btnMoodle.UseVisualStyleBackColor = true;
            btnMoodle.Click += btnMoodle_Click;
            // 
            // TestDispatch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnMoodle);
            Controls.Add(btnGoogleClassroom);
            Name = "TestDispatch";
            Text = "TestDispatch";
            ResumeLayout(false);
        }

        #endregion

        private Button btnGoogleClassroom;
        private Button btnMoodle;
    }
}