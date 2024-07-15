namespace Charamaker
{
    partial class MotionMaker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ScriptB = new System.Windows.Forms.TextBox();
            this.messageB = new System.Windows.Forms.TextBox();
            this.pathB = new System.Windows.Forms.TextBox();
            this.saveB = new System.Windows.Forms.Button();
            this.speedUD = new System.Windows.Forms.NumericUpDown();
            this.SRB = new System.Windows.Forms.Button();
            this.DRB = new System.Windows.Forms.Button();
            this.TRB = new System.Windows.Forms.Button();
            this.ReverseB = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.speedUD)).BeginInit();
            this.SuspendLayout();
            // 
            // ScriptB
            // 
            this.ScriptB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScriptB.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ScriptB.Location = new System.Drawing.Point(12, 12);
            this.ScriptB.Multiline = true;
            this.ScriptB.Name = "ScriptB";
            this.ScriptB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ScriptB.Size = new System.Drawing.Size(776, 312);
            this.ScriptB.TabIndex = 0;
            this.ScriptB.TextChanged += new System.EventHandler(this.ScriptB_TextChanged);
            this.ScriptB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ScriptB_KeyDown);
            // 
            // messageB
            // 
            this.messageB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageB.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.messageB.Location = new System.Drawing.Point(12, 367);
            this.messageB.Multiline = true;
            this.messageB.Name = "messageB";
            this.messageB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.messageB.Size = new System.Drawing.Size(664, 263);
            this.messageB.TabIndex = 2;
            // 
            // pathB
            // 
            this.pathB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathB.Location = new System.Drawing.Point(12, 330);
            this.pathB.Name = "pathB";
            this.pathB.Size = new System.Drawing.Size(776, 31);
            this.pathB.TabIndex = 3;
            this.pathB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pathB_KeyDown);
            // 
            // saveB
            // 
            this.saveB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveB.Location = new System.Drawing.Point(682, 367);
            this.saveB.Name = "saveB";
            this.saveB.Size = new System.Drawing.Size(112, 34);
            this.saveB.TabIndex = 4;
            this.saveB.Text = "Save";
            this.saveB.UseVisualStyleBackColor = true;
            this.saveB.Click += new System.EventHandler(this.saveB_Click);
            // 
            // speedUD
            // 
            this.speedUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.speedUD.DecimalPlaces = 1;
            this.speedUD.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.speedUD.Location = new System.Drawing.Point(688, 419);
            this.speedUD.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.speedUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.speedUD.Name = "speedUD";
            this.speedUD.Size = new System.Drawing.Size(100, 31);
            this.speedUD.TabIndex = 5;
            this.speedUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // SRB
            // 
            this.SRB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SRB.Location = new System.Drawing.Point(682, 509);
            this.SRB.Name = "SRB";
            this.SRB.Size = new System.Drawing.Size(112, 34);
            this.SRB.TabIndex = 6;
            this.SRB.Text = "ScaleRst";
            this.SRB.UseVisualStyleBackColor = true;
            this.SRB.Click += new System.EventHandler(this.SRB_Click);
            // 
            // DRB
            // 
            this.DRB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DRB.Location = new System.Drawing.Point(682, 549);
            this.DRB.Name = "DRB";
            this.DRB.Size = new System.Drawing.Size(112, 34);
            this.DRB.TabIndex = 7;
            this.DRB.Text = "DegreeRst";
            this.DRB.UseVisualStyleBackColor = true;
            this.DRB.Click += new System.EventHandler(this.DRB_Click);
            // 
            // TRB
            // 
            this.TRB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TRB.Location = new System.Drawing.Point(682, 589);
            this.TRB.Name = "TRB";
            this.TRB.Size = new System.Drawing.Size(112, 34);
            this.TRB.TabIndex = 8;
            this.TRB.Text = "TextureRst";
            this.TRB.UseVisualStyleBackColor = true;
            this.TRB.Click += new System.EventHandler(this.TRB_Click);
            // 
            // ReverseB
            // 
            this.ReverseB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ReverseB.Location = new System.Drawing.Point(682, 456);
            this.ReverseB.Name = "ReverseB";
            this.ReverseB.Size = new System.Drawing.Size(112, 34);
            this.ReverseB.TabIndex = 9;
            this.ReverseB.Text = "Reverse";
            this.ReverseB.UseVisualStyleBackColor = true;
            this.ReverseB.Click += new System.EventHandler(this.ReverseB_Click);
            // 
            // MotionMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 635);
            this.Controls.Add(this.ReverseB);
            this.Controls.Add(this.TRB);
            this.Controls.Add(this.DRB);
            this.Controls.Add(this.SRB);
            this.Controls.Add(this.speedUD);
            this.Controls.Add(this.saveB);
            this.Controls.Add(this.pathB);
            this.Controls.Add(this.messageB);
            this.Controls.Add(this.ScriptB);
            this.Name = "MotionMaker";
            this.Text = "MotionMaker";
            this.Load += new System.EventHandler(this.MotionMaker_Load);
            ((System.ComponentModel.ISupportInitialize)(this.speedUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox ScriptB;
        private TextBox messageB;
        private TextBox pathB;
        private Button saveB;
        private NumericUpDown speedUD;
        private Button SRB;
        private Button DRB;
        private Button TRB;
        private Button ReverseB;
    }
}