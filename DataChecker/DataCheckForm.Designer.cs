namespace DataChecker
{
    partial class DataCheckForm
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
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            PathBox = new TextBox();
            AllBox = new TextBox();
            PackBox = new TextBox();
            UnpackNameBox = new TextBox();
            KaniPackBox = new TextBox();
            KaniPackEndBox = new TextBox();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 16;
            timer1.Tick += Ticked;
            // 
            // PathBox
            // 
            PathBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PathBox.Location = new Point(12, 415);
            PathBox.Name = "PathBox";
            PathBox.Size = new Size(776, 23);
            PathBox.TabIndex = 0;
            // 
            // AllBox
            // 
            AllBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            AllBox.Location = new Point(12, 48);
            AllBox.Multiline = true;
            AllBox.Name = "AllBox";
            AllBox.ScrollBars = ScrollBars.Both;
            AllBox.Size = new Size(369, 361);
            AllBox.TabIndex = 1;
            // 
            // PackBox
            // 
            PackBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PackBox.Location = new Point(387, 48);
            PackBox.Multiline = true;
            PackBox.Name = "PackBox";
            PackBox.ScrollBars = ScrollBars.Both;
            PackBox.Size = new Size(401, 361);
            PackBox.TabIndex = 2;
            // 
            // UnpackNameBox
            // 
            UnpackNameBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            UnpackNameBox.Location = new Point(387, 12);
            UnpackNameBox.Name = "UnpackNameBox";
            UnpackNameBox.Size = new Size(401, 23);
            UnpackNameBox.TabIndex = 3;
            // 
            // KaniPackBox
            // 
            KaniPackBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            KaniPackBox.Location = new Point(12, 12);
            KaniPackBox.Name = "KaniPackBox";
            KaniPackBox.Size = new Size(180, 23);
            KaniPackBox.TabIndex = 4;
            // 
            // KaniPackEndBox
            // 
            KaniPackEndBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            KaniPackEndBox.Location = new Point(201, 12);
            KaniPackEndBox.Name = "KaniPackEndBox";
            KaniPackEndBox.Size = new Size(180, 23);
            KaniPackEndBox.TabIndex = 5;
            // 
            // DataCheckForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(KaniPackEndBox);
            Controls.Add(KaniPackBox);
            Controls.Add(UnpackNameBox);
            Controls.Add(PackBox);
            Controls.Add(AllBox);
            Controls.Add(PathBox);
            Name = "DataCheckForm";
            Text = "DataCheck";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private TextBox PathBox;
        private TextBox AllBox;
        private TextBox PackBox;
        private TextBox UnpackNameBox;
        private TextBox KaniPackBox;
        private TextBox KaniPackEndBox;
    }
}
