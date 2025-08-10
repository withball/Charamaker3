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
            ScriptB = new TextBox();
            messageB = new TextBox();
            pathB = new TextBox();
            saveB = new Button();
            speedUD = new NumericUpDown();
            SRB = new Button();
            DRB = new Button();
            TRB = new Button();
            ReverseB = new Button();
            narabeB = new Button();
            ((System.ComponentModel.ISupportInitialize)speedUD).BeginInit();
            SuspendLayout();
            // 
            // ScriptB
            // 
            ScriptB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ScriptB.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            ScriptB.Location = new Point(8, 7);
            ScriptB.Margin = new Padding(2);
            ScriptB.Multiline = true;
            ScriptB.Name = "ScriptB";
            ScriptB.ScrollBars = ScrollBars.Both;
            ScriptB.Size = new Size(544, 212);
            ScriptB.TabIndex = 0;
            ScriptB.TextChanged += ScriptB_TextChanged;
            ScriptB.KeyDown += ScriptB_KeyDown;
            // 
            // messageB
            // 
            messageB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            messageB.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            messageB.Location = new Point(8, 268);
            messageB.Margin = new Padding(2);
            messageB.Multiline = true;
            messageB.Name = "messageB";
            messageB.ScrollBars = ScrollBars.Both;
            messageB.Size = new Size(466, 159);
            messageB.TabIndex = 2;
            // 
            // pathB
            // 
            pathB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pathB.Location = new Point(8, 223);
            pathB.Margin = new Padding(2);
            pathB.Name = "pathB";
            pathB.Size = new Size(544, 23);
            pathB.TabIndex = 3;
            pathB.KeyDown += pathB_KeyDown;
            // 
            // saveB
            // 
            saveB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveB.Location = new Point(477, 268);
            saveB.Margin = new Padding(2);
            saveB.Name = "saveB";
            saveB.Size = new Size(78, 20);
            saveB.TabIndex = 4;
            saveB.Text = "Save";
            saveB.UseVisualStyleBackColor = true;
            saveB.Click += saveB_Click;
            // 
            // speedUD
            // 
            speedUD.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            speedUD.DecimalPlaces = 1;
            speedUD.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            speedUD.Location = new Point(482, 299);
            speedUD.Margin = new Padding(2);
            speedUD.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            speedUD.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            speedUD.Name = "speedUD";
            speedUD.Size = new Size(70, 23);
            speedUD.TabIndex = 5;
            speedUD.Value = new decimal(new int[] { 1, 0, 0, 0 });
            speedUD.ValueChanged += speedUD_ValueChanged;
            // 
            // SRB
            // 
            SRB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SRB.Location = new Point(477, 353);
            SRB.Margin = new Padding(2);
            SRB.Name = "SRB";
            SRB.Size = new Size(78, 20);
            SRB.TabIndex = 6;
            SRB.Text = "ScaleRst";
            SRB.UseVisualStyleBackColor = true;
            SRB.Click += SRB_Click;
            // 
            // DRB
            // 
            DRB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            DRB.Location = new Point(477, 377);
            DRB.Margin = new Padding(2);
            DRB.Name = "DRB";
            DRB.Size = new Size(78, 20);
            DRB.TabIndex = 7;
            DRB.Text = "DegreeRst";
            DRB.UseVisualStyleBackColor = true;
            DRB.Click += DRB_Click;
            // 
            // TRB
            // 
            TRB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TRB.Location = new Point(477, 401);
            TRB.Margin = new Padding(2);
            TRB.Name = "TRB";
            TRB.Size = new Size(78, 20);
            TRB.TabIndex = 8;
            TRB.Text = "TextureRst";
            TRB.UseVisualStyleBackColor = true;
            TRB.Click += TRB_Click;
            // 
            // ReverseB
            // 
            ReverseB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ReverseB.Location = new Point(477, 322);
            ReverseB.Margin = new Padding(2);
            ReverseB.Name = "ReverseB";
            ReverseB.Size = new Size(78, 20);
            ReverseB.TabIndex = 9;
            ReverseB.Text = "Reverse";
            ReverseB.UseVisualStyleBackColor = true;
            ReverseB.Click += ReverseB_Click;
            // 
            // narabeB
            // 
            narabeB.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            narabeB.Location = new Point(8, 247);
            narabeB.Margin = new Padding(2);
            narabeB.Name = "narabeB";
            narabeB.Size = new Size(78, 20);
            narabeB.TabIndex = 10;
            narabeB.Text = "Narabe";
            narabeB.UseVisualStyleBackColor = true;
            narabeB.Click += narabeB_Click;
            // 
            // MotionMaker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 429);
            Controls.Add(narabeB);
            Controls.Add(ReverseB);
            Controls.Add(TRB);
            Controls.Add(DRB);
            Controls.Add(SRB);
            Controls.Add(speedUD);
            Controls.Add(saveB);
            Controls.Add(pathB);
            Controls.Add(messageB);
            Controls.Add(ScriptB);
            Margin = new Padding(2);
            Name = "MotionMaker";
            Text = "MotionMaker";
            Load += MotionMaker_Load;
            ((System.ComponentModel.ISupportInitialize)speedUD).EndInit();
            ResumeLayout(false);
            PerformLayout();

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
        private Button narabeB;
    }
}