namespace HF.Admin
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkIsProd = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnInyect = new System.Windows.Forms.Button();
            this.cboModules = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkIsProd
            // 
            this.chkIsProd.AutoSize = true;
            this.chkIsProd.Location = new System.Drawing.Point(617, 42);
            this.chkIsProd.Name = "chkIsProd";
            this.chkIsProd.Size = new System.Drawing.Size(94, 29);
            this.chkIsProd.TabIndex = 0;
            this.chkIsProd.Text = "Is Prod";
            this.chkIsProd.UseVisualStyleBackColor = true;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(435, 134);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(276, 34);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnInyect
            // 
            this.btnInyect.Location = new System.Drawing.Point(55, 134);
            this.btnInyect.Name = "btnInyect";
            this.btnInyect.Size = new System.Drawing.Size(282, 34);
            this.btnInyect.TabIndex = 2;
            this.btnInyect.Text = "Inyect";
            this.btnInyect.UseVisualStyleBackColor = true;
            this.btnInyect.Click += new System.EventHandler(this.btnInyect_Click);
            // 
            // cboModules
            // 
            this.cboModules.FormattingEnabled = true;
            this.cboModules.Location = new System.Drawing.Point(166, 40);
            this.cboModules.Name = "cboModules";
            this.cboModules.Size = new System.Drawing.Size(404, 33);
            this.cboModules.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(55, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "Modules";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkIsProd);
            this.groupBox1.Controls.Add(this.cboModules);
            this.groupBox1.Controls.Add(this.btnTest);
            this.groupBox1.Controls.Add(this.btnInyect);
            this.groupBox1.Location = new System.Drawing.Point(34, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(752, 201);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 292);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Admin HangFire";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CheckBox chkIsProd;
        private Button btnTest;
        private Button btnInyect;
        private ComboBox cboModules;
        private Label label1;
        private GroupBox groupBox1;
    }
}