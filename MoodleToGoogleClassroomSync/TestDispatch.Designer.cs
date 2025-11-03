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
            btnGenerateStructure = new Button();
            txtOutput = new TextBox();
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
            btnMoodle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMoodle.Location = new Point(669, 12);
            btnMoodle.Name = "btnMoodle";
            btnMoodle.Size = new Size(119, 74);
            btnMoodle.TabIndex = 1;
            btnMoodle.Text = "Open Moodle Test";
            btnMoodle.UseVisualStyleBackColor = true;
            btnMoodle.Click += btnMoodle_Click;
            // 
            // btnGenerateStructure
            // 
            btnGenerateStructure.Location = new Point(12, 163);
            btnGenerateStructure.Name = "btnGenerateStructure";
            btnGenerateStructure.Size = new Size(119, 74);
            btnGenerateStructure.TabIndex = 2;
            btnGenerateStructure.Text = "Build Project Structure File";
            btnGenerateStructure.UseVisualStyleBackColor = true;
            btnGenerateStructure.Click += btnGenerateStructure_Click;
            // 
            // txtOutput
            // 
            txtOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtOutput.Location = new Point(141, 166);
            txtOutput.Multiline = true;
            txtOutput.Name = "txtOutput";
            txtOutput.ScrollBars = ScrollBars.Both;
            txtOutput.Size = new Size(647, 272);
            txtOutput.TabIndex = 3;
            // 
            // TestDispatch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtOutput);
            Controls.Add(btnGenerateStructure);
            Controls.Add(btnMoodle);
            Controls.Add(btnGoogleClassroom);
            Name = "TestDispatch";
            Text = "TestDispatch";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnGoogleClassroom;
        private Button btnMoodle;
        private Button btnGenerateStructure;
        private TextBox txtOutput;
    }
}