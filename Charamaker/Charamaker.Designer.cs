namespace Test
{
    partial class Charamaker
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.loadB = new System.Windows.Forms.Button();
            this.textB = new System.Windows.Forms.TextBox();
            this.removeB = new System.Windows.Forms.Button();
            this.saveB = new System.Windows.Forms.Button();
            this.zoomUD = new System.Windows.Forms.NumericUpDown();
            this.DSB = new System.Windows.Forms.TextBox();
            this.messageB = new System.Windows.Forms.TextBox();
            this.setBaseB = new System.Windows.Forms.Button();
            this.PointB = new System.Windows.Forms.TrackBar();
            this.motionB = new System.Windows.Forms.Button();
            this.screenshotB = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.zoomUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PointB)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.ticked);
            // 
            // loadB
            // 
            this.loadB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.loadB.Location = new System.Drawing.Point(635, 785);
            this.loadB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loadB.Name = "loadB";
            this.loadB.Size = new System.Drawing.Size(107, 38);
            this.loadB.TabIndex = 0;
            this.loadB.Text = "load";
            this.loadB.UseVisualStyleBackColor = true;
            this.loadB.Click += new System.EventHandler(this.loadB_Click);
            // 
            // textB
            // 
            this.textB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textB.Location = new System.Drawing.Point(17, 785);
            this.textB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textB.Name = "textB";
            this.textB.Size = new System.Drawing.Size(593, 31);
            this.textB.TabIndex = 1;
            // 
            // removeB
            // 
            this.removeB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeB.Location = new System.Drawing.Point(865, 785);
            this.removeB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.removeB.Name = "removeB";
            this.removeB.Size = new System.Drawing.Size(107, 38);
            this.removeB.TabIndex = 2;
            this.removeB.Text = "remove";
            this.removeB.UseVisualStyleBackColor = true;
            this.removeB.Click += new System.EventHandler(this.removeB_Click);
            // 
            // saveB
            // 
            this.saveB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveB.Location = new System.Drawing.Point(750, 785);
            this.saveB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveB.Name = "saveB";
            this.saveB.Size = new System.Drawing.Size(107, 38);
            this.saveB.TabIndex = 3;
            this.saveB.Text = "save";
            this.saveB.UseVisualStyleBackColor = true;
            this.saveB.Click += new System.EventHandler(this.saveB_Click);
            // 
            // zoomUD
            // 
            this.zoomUD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomUD.DecimalPlaces = 1;
            this.zoomUD.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.zoomUD.Location = new System.Drawing.Point(1473, 12);
            this.zoomUD.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.zoomUD.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.zoomUD.Name = "zoomUD";
            this.zoomUD.Size = new System.Drawing.Size(94, 31);
            this.zoomUD.TabIndex = 4;
            this.zoomUD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.zoomUD.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.zoomUD.ValueChanged += new System.EventHandler(this.zoomUD_ValueChanged);
            // 
            // DSB
            // 
            this.DSB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DSB.Location = new System.Drawing.Point(1138, 89);
            this.DSB.Multiline = true;
            this.DSB.Name = "DSB";
            this.DSB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.DSB.Size = new System.Drawing.Size(412, 509);
            this.DSB.TabIndex = 5;
            this.DSB.WordWrap = false;
            this.DSB.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DSB_KeyUp);
            // 
            // messageB
            // 
            this.messageB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageB.Location = new System.Drawing.Point(1138, 652);
            this.messageB.Multiline = true;
            this.messageB.Name = "messageB";
            this.messageB.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.messageB.Size = new System.Drawing.Size(412, 179);
            this.messageB.TabIndex = 6;
            // 
            // setBaseB
            // 
            this.setBaseB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.setBaseB.Location = new System.Drawing.Point(1443, 606);
            this.setBaseB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.setBaseB.Name = "setBaseB";
            this.setBaseB.Size = new System.Drawing.Size(107, 38);
            this.setBaseB.TabIndex = 7;
            this.setBaseB.Text = "setBase";
            this.setBaseB.UseVisualStyleBackColor = true;
            this.setBaseB.Click += new System.EventHandler(this.setBaseB_Click);
            // 
            // PointB
            // 
            this.PointB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PointB.Location = new System.Drawing.Point(1271, 12);
            this.PointB.Maximum = 100;
            this.PointB.Name = "PointB";
            this.PointB.Size = new System.Drawing.Size(181, 69);
            this.PointB.TabIndex = 8;
            this.PointB.Value = 50;
            // 
            // motionB
            // 
            this.motionB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.motionB.Location = new System.Drawing.Point(980, 785);
            this.motionB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.motionB.Name = "motionB";
            this.motionB.Size = new System.Drawing.Size(107, 38);
            this.motionB.TabIndex = 9;
            this.motionB.Text = "motion";
            this.motionB.UseVisualStyleBackColor = true;
            this.motionB.Click += new System.EventHandler(this.motionB_Click);
            // 
            // screenshotB
            // 
            this.screenshotB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.screenshotB.Location = new System.Drawing.Point(1138, 606);
            this.screenshotB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.screenshotB.Name = "screenshotB";
            this.screenshotB.Size = new System.Drawing.Size(107, 38);
            this.screenshotB.TabIndex = 10;
            this.screenshotB.Text = "screenshot";
            this.screenshotB.UseVisualStyleBackColor = true;
            this.screenshotB.Click += new System.EventHandler(this.screenshotB_Click);
            // 
            // Charamaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 843);
            this.Controls.Add(this.screenshotB);
            this.Controls.Add(this.motionB);
            this.Controls.Add(this.PointB);
            this.Controls.Add(this.setBaseB);
            this.Controls.Add(this.messageB);
            this.Controls.Add(this.DSB);
            this.Controls.Add(this.zoomUD);
            this.Controls.Add(this.saveB);
            this.Controls.Add(this.removeB);
            this.Controls.Add(this.textB);
            this.Controls.Add(this.loadB);
            this.Name = "Charamaker";
            this.Text = "Charamaker3Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.Resize += new System.EventHandler(this.Resized);
            ((System.ComponentModel.ISupportInitialize)(this.zoomUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PointB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Button loadB;
        private TextBox textB;
        private Button removeB;
        private Button saveB;
        private NumericUpDown zoomUD;
        private TextBox DSB;
        private TextBox messageB;
        private Button setBaseB;
        private TrackBar PointB;
        private Button motionB;
        private Button screenshotB;
    }
}